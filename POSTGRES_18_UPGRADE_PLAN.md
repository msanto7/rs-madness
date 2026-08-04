# PostgreSQL 18 Upgrade Plan

Handoff doc for upgrading local Docker Postgres and the Render production DB from
17 to 18. Nothing here has been executed yet — this is the plan only.

## Current state (as of writing)

| Component | Now | File |
|---|---|---|
| Local dev DB | `postgres:17` via Docker Compose | `RSMadnessEngine/docker-compose.yml` |
| Prod DB | Render managed Postgres (version set in Render dashboard, not in-repo) | — |
| Driver | `Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.1 / `Npgsql` 10.0.2 | `RSMadnessEngine/RSMadnessEngine.Data/RSMadnessEngine.Data.csproj` |
| Docs | "PostgreSQL 17" mentioned twice | `README.md` (lines 8, 54) |

**Compatibility confirmed:** Npgsql 10 already supports PG18 (it has explicit PG18
feature translations, e.g. `Guid.CreateVersion7()` → `uuidv7()`), and Render now
offers PostgreSQL 18 with a supported in-place upgrade path. No blockers.

Sources:
- https://www.npgsql.org/doc/compatibility.html
- https://render.com/changelog/postgresql-18-is-now-available-for-render-postgres-databases
- https://render.com/docs/postgresql-upgrading

## Plan

### 1. Bump the driver first (do this regardless of DB version)

- `Npgsql.EntityFrameworkCore.PostgreSQL` → latest 10.0.x (10.0.3 available) in
  `RSMadnessEngine/RSMadnessEngine.Data/RSMadnessEngine.Data.csproj` (line 20).
  Minor/patch bump, low risk, pulls in PG18-specific fixes.

### 2. Upgrade local Docker Postgres

Named volume `pgdata` holds PG17's on-disk format — just changing the image tag
to `postgres:18` will fail to start ("database files are incompatible with
server"). Since this is disposable dev data (seeded from `TeamSeeder` +
migrations), pick one:

- **Fast path (recommended for dev):** stop the stack, `docker compose down -v`
  to drop the `pgdata` volume, bump image to `postgres:18` in
  `RSMadnessEngine/docker-compose.yml` (line 3), `docker compose up -d`, then
  `dotnet ef database update` to recreate schema, restart API to reseed teams.
- **Preserve-data path:** `pg_dump` from the running PG17 container → bring up a
  fresh PG18 container on a new volume → `psql`/`pg_restore` the dump in, then
  repoint compose to the new volume.
- Run the app fully against local PG18 (migrations, seeding, a full request
  cycle) before touching Render — this is the real-world compatibility test.

### 3. Upgrade Render production

Render supports a direct in-place upgrade to PG18, keeping the same
credentials/connection string.

1. **Prerequisite check:** confirm the DB instance is on a plan with
   point-in-time recovery (PITR). Legacy instance types without PITR must
   migrate to a flexible plan first — check this in the Render dashboard before
   anything else.
2. **Test the upgrade on a clone first:** Database → Info page → click current
   version → "Clone this database" → wait for clone to finish replicating
   (restores from ~10 min prior) → click "Upgrade to PostgreSQL 18" on the
   clone → watch logs (up to ~1 hour) → verify app can connect and run against
   it.
3. **Upgrade production:** once the clone test is clean, go back to the real
   database's Info page → click current version → "Upgrade to PostgreSQL 18".
   **Expect downtime** — the DB is unavailable during the upgrade. Do this in a
   low-traffic window.
4. **No automatic rollback** if the upgrade fails — the DB just stays on its
   current version, so there's no data-loss risk from a failed attempt, but
   plan the timing assuming you might need to retry.
5. Delete the test clone once you're done with it (Render bills for it while it
   exists).

### 4. Post-upgrade validation

- Hit the live API against the upgraded prod DB: login, bracket submission,
  leaderboard, the hourly sync job.
- Check Render service logs for any Npgsql/EF connection warnings right after
  cutover.

### 5. Documentation updates

- `README.md` line 8: `Database: PostgreSQL 17` → 18
- `README.md` line 54: `- PostgreSQL 17` → 18
- `RSMadnessEngine/docker-compose.yml` line 3: `image: postgres:17` →
  `postgres:18`

## Suggested order

Driver bump → local Docker upgrade + full app test → doc updates → Render
clone-test → Render production upgrade. Doing local first means any real
breakage surfaces on disposable data, not production.

## Status

- [ ] Driver bumped
- [ ] Local Docker upgraded and app tested
- [ ] Docs updated
- [ ] Render clone tested
- [ ] Render production upgraded
- [ ] Post-upgrade validation done
