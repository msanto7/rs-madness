# 1-Month Build Plan: .NET 10 + React TS + Managed Postgres + Hourly Sync

## Summary

Build a monorepo with a single `.NET 10` API (including an internal hourly background sync job), a separate `React + TypeScript` static frontend (Vite), and managed PostgreSQL on Render.
Scoring and ranking rules are locked to the spreadsheet rules, with manual admin override always available if API data is missing/wrong.

### General Feature List

1. drag and drop ordering of teams
2. pulling live data from sports API to update each persons stats and rank
3. page to honor the excel file
4. fix the submit save flow for the ranking page (submit fails if no save first)
5. fix the ranking page ordering...order by seed and reverse the rank numbers
6. need to maybe pull the teamID from the espn id...right now I am just copying the "shortDisplayName" field from the api response...and making sure our team names match in the db so I can just look them up by name
    - this is also going to cause an issue already because there are 2 damn Miami teams
    - so need to store the api's team ID...then do the look based on that instead
    - the seeder and the name lookup in the data service need to change
7. need to reset the team ids in the db -- my current bracket entry wont match the teamids of the bracket entry from before??
    - might not matter though because I am doing the team name lookup right now...will check later




## List of things to Improve -- TODO

1. JWT token is in localStorage...not cookie
2. Validate that the teams ids from the ranking save are actually matching whats in the DB
3. Save and Submit user feedback so they know it saved on the ranking page

## Implementation Plan (Decision-Complete)

### Week 1 (Mar 16–Mar 22): Foundation + Scaffolding

1. Create repository structure:
- `backend/` for .NET solution
- `frontend/` for Vite React TS app
- `infra/` for Docker/local infra + deploy manifests
2. Backend scaffold:
- `RsMadness.Api` (`net10.0`) as single deployable service
- ASP.NET Identity + JWT auth
- EF Core + PostgreSQL
- health endpoint and OpenAPI
3. Frontend scaffold:
- Vite React TS app
- routing shell + auth state shell
- drag-and-drop library wiring placeholder for ranking page
4. Infra scaffold:
- Docker Compose for local Postgres
- `.env.example` files for API and frontend
- Render deployment config placeholders (API + static site + managed DB)
5. Exit criteria:
- API runs locally against Docker Postgres
- Frontend runs locally and calls API health endpoint
- register/login/me flow works in local env

### Week 2 (Mar 23–Mar 29): Ranking Entry Flow

1. Add core domain/data:
- teams (64 slots including play-in pair slots)
- entries and rank assignments
- lock timestamp handling
2. Ranking UI:
- drag-and-drop rank assignment for all 64 teams
- validation: all teams ranked, integers 1..64, unique, sum=2080
3. Entry APIs:
- save draft ranks
- submit/lock entry
- get current user entry
4. Exit criteria:
- user can complete and submit valid ranking entry
- invalid entries blocked with clear validation errors
- post-lock edits are rejected by API

### Week 3 (Mar 30–Apr 5): Scoring + Leaderboard + Hourly Sync

1. Scoring engine:
- `current_points = SUM(rank * wins)`
- `potential_points = SUM(in * (6 - wins) * rank)`
- leaderboard sorting: current desc, potential desc, submitted_at asc
2. Hourly sync inside API:
- hosted background service running every hour
- provider adapter interface + API-SPORTS free-tier implementation
- idempotent writes to games/team status
- trigger score recalculation after each successful sync
3. Reliability/fallback:
- sync run logging
- admin `Sync Now`
- admin manual edit of game outcomes/team status
4. Locked scoring rule:
- play-in games do not earn points; scoring starts from ranked 64-team bracket
5. Exit criteria:
- hourly sync updates wins/losses and leaderboard automatically
- admin can correct data and force recalculation

### Week 4 (Apr 6–Apr 12): Hardening + Deploy + UAT

1. Security and resilience:
- auth hardening, CORS policy, rate limits on auth/sync endpoints
- retry + backoff for external API calls
- structured logging + error handling
2. Deployment:
- Render static site (frontend)
- Render web service (API, always-on tier)
- managed Postgres
- environment variable setup + migration startup strategy
3. QA/UAT:
- full rule validation pass
- scoring accuracy checks against spreadsheet scenarios
- responsive UX pass for ranking + leaderboard
4. Exit criteria:
- staging signoff with pilot users
- no P1 bugs, launch checklist complete

### Buffer (Apr 13–Apr 16): Launch

1. Final data seed/cleanup
2. Smoke tests in production
3. Go-live + monitoring

## Public Interfaces / Types to Add Early

1. API endpoints (v1):
- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/auth/me`
- `GET /api/teams`
- `GET /api/entry/me`
- `PUT /api/entry/me/ranks`
- `POST /api/entry/me/submit`
- `GET /api/leaderboard`
- `POST /api/admin/sync/run`
- `PATCH /api/admin/games/{id}` (manual override)
2. Backend abstractions:
- `IGameDataProvider` (external score feed adapter)
- `IScoringService` (recalculate entry scores)
- `ISyncService` (hourly sync orchestration)
3. Core data tables:
- `users`, `pools`, `pool_members`
- `teams`, `entries`, `entry_team_ranks`
- `games`, `team_status`, `entry_scores`
- `sync_runs`, `provider_game_map`

## Test Plan (must pass before launch)

1. Unit tests:
- rank validation (unique 1..64, sum 2080)
- scoring formulas and tie-break ordering
- play-in no-points rule
2. Integration tests:
- auth register/login/me
- entry submit and lock behavior
- admin override updates recalculated leaderboard
3. Sync tests:
- hourly sync idempotency (re-running same payload doesn’t double count)
- provider failure + retry behavior
- `Sync Now` endpoint behavior
4. UI tests:
- drag-and-drop rank assignment correctness
- validation messages
- leaderboard rendering and ordering

## Assumptions (Locked Defaults)

1. Hosting target: Render-first
2. Backend shape: single API service (background job in same service)
3. Auth: ASP.NET Identity + JWT
4. Frontend scaffold: Vite React TS
5. External data: API-SPORTS free tier, hourly polling
6. Reliability fallback: manual admin override always available
7. Local dev DB: Docker Compose Postgres
