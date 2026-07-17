# Authentication & Session Management

How login sessions work: HttpOnly cookies carrying a short-lived access token
and a rotating, server-tracked refresh token. This replaced an earlier model
that stored a raw JWT in `localStorage`.

## Why this exists

The old model returned a JWT in the login/register response body, and the
frontend stored it in `localStorage`, attaching it as an `Authorization`
header on every request. That had two production-grade problems:

1. **XSS blast radius.** Anything with `localStorage` access can read the
   token. A single XSS bug anywhere in the app (our code or a dependency)
   let an attacker read the token directly and reuse it — no need to
   intercept traffic, and the credential stayed useful for as long as it was
   valid (up to 60 minutes).
2. **No revocation.** A JWT is self-contained and stateless; the server has
   no record of having issued it and can't invalidate it early. "Logout"
   only removed the local copy — the token itself remained valid anywhere
   else it existed until it naturally expired.

## The pieces

Two tokens, two different jobs:

- **Access token** — a signed JWT (`Jwt:Key`), carries `sub`/`email`
  claims, validated by the standard ASP.NET JWT bearer middleware. Lifetime:
  **15 minutes** (`Jwt:AccessExpireMinutes`). This is what actually
  authorizes API calls.
- **Refresh token** — not a JWT, just 64 random bytes
  (`RandomNumberGenerator.GetBytes(64)`, base64-encoded). Its only job is
  proving "I'm the same session that logged in" so the server will issue a
  new access token. Lifetime: up to **14 days**
  (`Jwt:AbsoluteSessionExpireDays`), but every use is checked against the
  database — unlike the access token, it isn't self-validating.

Both are set as `HttpOnly` cookies (`rs_access_token`, `rs_refresh_token`)
instead of being returned in the JSON body or stored in `localStorage`.
`HttpOnly` means `document.cookie` cannot see them — client-side JS
(including an XSS payload) can still make requests *as* the logged-in user
(cookies ride along automatically), but it can no longer **exfiltrate** the
credential for later or offline reuse. That's the core security upgrade:
XSS goes from "steals the session forever" to "can abuse the session only
while it's live in that tab."

The refresh token is **tracked server-side**, in a `RefreshTokens` table
(`RSMadnessEngine.Data/Entities/RefreshToken.cs`) — one row per issued
refresh token:

| Column | Purpose |
|---|---|
| `UserId` | owner |
| `TokenHash` | SHA-256 hash of the token — the raw value is never persisted |
| `SessionCreatedAt` | when this login session started (carried through rotations, never reset) |
| `LastUsedAt` | last time it was successfully refreshed (updated on every rotation) |
| `ExpiresAt` | hard absolute expiry (`SessionCreatedAt` + 14 days) |
| `RevokedAt` | set on logout or rotation; null means still active |
| `ReplacedByTokenHash` | hash of the token that replaced this one on rotation, for audit trail |

Because the server holds this record, it can decide *at request time*
whether a given refresh token is still good — something a stateless JWT
alone can never do.

## Request flows

### Login / Register

`AuthController.Login` / `Register` → `AuthService.LoginAsync` /
`RegisterAsync` → `BuildAuthSessionResponseAsync`:

1. Credentials validated via ASP.NET Identity (`CheckPasswordAsync`).
2. `TokenService` mints an access JWT (15 min) and a random refresh token.
3. The refresh token is hashed and a new `RefreshToken` row is inserted with
   `ExpiresAt = now + 14 days`.
4. The controller sets both cookies. The JSON response body only ever
   contains `{ displayName, email }` (`AuthResponse`) — raw tokens never
   appear in a response body, only in `Set-Cookie` headers
   (`AuthSessionResponse`, the internal DTO that does carry the raw tokens,
   never leaves the controller).

### A normal authenticated request

- The browser attaches both cookies automatically.
- `Program.cs`'s `JwtBearerEvents.OnMessageReceived` pulls the JWT out of
  `rs_access_token` and hands it to standard bearer validation
  (issuer/audience/signature/lifetime) — same trust mechanism as before,
  just sourced from a cookie instead of an `Authorization` header.
- Valid, unexpired JWT → request proceeds. No DB hit needed; that's the
  point of a short-lived, self-contained access token.

### Access token expires — silent refresh

1. The next API call gets a 401 (JWT `exp` has passed).
2. `apiClient`'s response interceptor (`RSMadnessWeb/src/api/client.ts`)
   catches the 401, confirms it's not itself an auth endpoint, and calls
   `POST /auth/refresh` — the `rs_refresh_token` cookie rides along
   automatically.
3. `AuthController.Refresh` reads the cookie and calls
   `AuthService.RefreshAsync(refreshToken)`.
4. `RefreshAsync` hashes the incoming token, looks up the matching row, and
   checks it's active (not revoked, not past `ExpiresAt`) and the session
   hasn't idle/absolute-expired (see below). If good: mints a **new** access
   token and a **new** refresh token, inserts a new `RefreshToken` row, and
   **revokes the old one** (`RevokedAt = now`), recording
   `ReplacedByTokenHash` so the rotation chain is auditable.
5. New cookies are set on the response; the interceptor retries the
   original request transparently. From the user's perspective nothing
   happened — they just kept using the app.

Concurrent requests are deduplicated: if several requests 401 at the same
moment (e.g. a page firing off several API calls in parallel right as the
access token expires), they all await one shared in-flight
`refreshAccessToken()` promise in `client.ts` instead of each independently
racing to rotate the same refresh token. Without that, the second concurrent
refresh call would find the token already revoked by the first and force a
spurious logout.

### Two independent expiry clocks

`AuthService.IsSessionExpired` checks both:

- **Idle timeout — 60 min (`Jwt:IdleSessionTimeoutMinutes`).**
  `LastUsedAt + 60min <= now`. This is a *sliding* window: every successful
  refresh updates `LastUsedAt`, so staying active at least once an hour
  keeps renewing it. Go quiet for an hour and the next refresh attempt fails
  outright, even if the absolute window hasn't elapsed.
- **Absolute expiry — 14 days (`Jwt:AbsoluteSessionExpireDays`).**
  `SessionCreatedAt + 14days <= now`. `SessionCreatedAt` is carried forward
  unchanged through every rotation, so this is a hard ceiling no amount of
  activity can extend. Bounds the worst case: even a continuously-active
  session can't live forever.

The frontend mirrors the idle side with its own 60-minute inactivity timer
in `AuthProvider` (`RSMadnessWeb/src/auth/AuthProvider.tsx`), listening for
`pointerdown` / `keydown` / `scroll` / `touchstart`, and proactively logs
the user out client-side. This is a UX nicety, not a security boundary — a
client-side timer is trivially bypassable; the server-side check is what
actually matters.

### Logout

`AuthController.Logout` reads `rs_refresh_token`, calls
`AuthService.LogoutAsync`, which finds the matching row and sets
`RevokedAt = now`. Cookies are cleared (re-sent with a past `Expires`).

Importantly, even if a copy of that refresh token existed elsewhere
(browser history, a stale device, a leaked log), it is now **actually
invalid** — the server rejects it on the next `IsActive` check. This is the
revocation capability a stateless-JWT-only model can't offer.

## Cookie configuration

Set via `AuthController.BuildCookieOptions`, driven by config so dev and
prod behave differently without code changes:

- `HttpOnly = true` always.
- `Secure` — `AuthCookies:Secure` (defaults to `!IsDevelopment()` if unset),
  `false` in `appsettings.Development.json` so cookies work over plain
  HTTP on localhost.
- `SameSite` — `AuthCookies:SameSite`, `Lax` by default.
- `Path = "/"`.

### Why `SameSite=Lax` is enough for CSRF here

Cookies are now the auth transport, so CSRF is a real question. CORS is
locked to an explicit allowlist (`Cors:AllowedOrigins`) with
`AllowCredentials`, and all state-changing auth endpoints
(login/logout/refresh) are POST. Browsers don't attach `SameSite=Lax`
cookies to cross-site POST/fetch requests — only to top-level GET
navigations — so a malicious third-party page can't silently trigger
authenticated POSTs against the API using a victim's cookies.

## Config reference

`appsettings.json` (`Jwt` / `AuthCookies` sections):

| Key | Default | Meaning |
|---|---|---|
| `Jwt:AccessExpireMinutes` | 15 | access token / access cookie lifetime |
| `Jwt:IdleSessionTimeoutMinutes` | 60 | sliding idle timeout for the refresh token |
| `Jwt:AbsoluteSessionExpireDays` | 14 | hard cap on a session's total lifetime |
| `AuthCookies:Secure` | `!IsDevelopment()` | `Secure` flag on both cookies |
| `AuthCookies:SameSite` | `Lax` | `SameSite` flag on both cookies |

## Known gaps / follow-ups

Not blockers, but worth tracking:

- **No reuse-detection on rotation.** `ReplacedByTokenHash` is recorded but
  nothing reads it. Presenting an already-revoked (already-rotated) refresh
  token is a strong signal of token theft/replay, and currently gets the
  same generic `invalid-refresh-token` response as an ordinary expired
  token. Ideally this would revoke the entire token family for that user.
- **No cleanup job.** `RefreshTokens` rows are never pruned — revoked and
  expired rows accumulate forever. Fine at current scale; will need a
  retention job eventually.
- **No "log out everywhere."** `IRefreshTokenRepository` has no
  revoke-all-for-user method yet — needed for the theft-detection response
  above, and useful standalone (e.g. on password change).
