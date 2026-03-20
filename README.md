# rs-madness

March Madness pool app based on `Blank Bracket 2026.xlsx`.

## Goal

Build a simple app where users:

1. Log in
2. Create their bracket entry (using the same scoring logic as the spreadsheet)
3. Track score updates as real tournament results come in
4. See a live leaderboard

## Official Pool Rules

These are the source-of-truth rules to implement in the app:

1. Rank all 64 teams.
2. Use each number from 1 through 64 exactly once (no duplicates).
3. Higher rank values should go to teams expected to advance further.
4. Scoring: every time a team wins, the user earns that team's assigned rank value.
5. Example: rank 50 and 3 wins = 150 points from that team.
6. Upsets still score: if a lower-ranked-by-seed team wins, points are still based only on the user's assigned rank for that team.
7. The field is 64 teams. Play-in pairs are treated as one slot when ranking.
8. In the original spreadsheet, users entered name in `F1` and ranks in column `F`.
9. Spreadsheet quality check: total rank sum must equal `2080`.

## What This Means For App Logic

1. Each entry contains exactly 64 rank assignments.
2. Entry validation must enforce:
- all 64 teams are ranked
- ranks are integers
- min rank = 1
- max rank = 64
- no rank repeated
- sum of ranks = 2080
3. The app should block submission until all checks pass.
4. After lock time, entries become read-only.

## Spreadsheet Logic (Decoded)

The workbook has one sheet (`Blank`) and 64 teams across 4 regions:

- East: rows 3-18
- South: rows 21-36
- West: rows 39-54
- Midwest: rows 57-72

Important columns:

- `A`: Seed
- `B`: Team
- `C`: Wins (actual wins so far)
- `D`: In (1 if still alive, 0 if eliminated)
- `F`: Rank (user-assigned value per team)
- `G`: Potential points (`In * (6 - Wins) * Rank`)

Key formulas:

- Team potential: `Gx = Dx * (6 - Cx) * Fx`
- Total wins: `C75 = SUM(C3:C74)`
- Teams still alive: `D75 = SUM(D3:D74)`
- Total rank assigned: `F75 = SUM(F3:F74,-F19,-F37,-F55,-F73)`
- Total potential: `G75 = SUM(G3:G74,-G19,-G37,-G55,-G73)`

Notes:

- `F1` is intended for the user name.
- Subtotal rows are 19, 37, 55, 73.

## App Interpretation

Per user entry, scoring should be:

1. `current_points = SUM(team.rank * team.wins)`
2. `potential_points = SUM(team.in * (6 - team.wins) * team.rank)`
3. Optional `max_possible = current_points + potential_points`

Recommended leaderboard sort:

1. `current_points` descending
2. `potential_points` descending (tie-breaker)
3. `entry_submitted_at` ascending (final tie-breaker)

## Recommended Architecture

1. Frontend: React + TypeScript (responsive web app)
2. Backend: ASP.NET Core Web API (.NET 8+)
3. Database: PostgreSQL
4. Auth: email/password (ASP.NET Core Identity)
5. Hosting: simple managed platform (Render is a good fit)

## External Results Sync (Required)

Goal: keep leaderboard up to date automatically without high API cost.

Recommended approach:

1. Use a lower-cost sports API provider for automated game/status pulls.
2. Keep a provider adapter layer so we can switch providers later without rewriting scoring logic.
3. Keep an admin manual override screen as a reliability fallback.

MVP sync cadence:

1. Daily sync each morning (refresh schedule/status baseline).
2. Frequent sync during active game windows (for near-live updates).
3. Overnight reconciliation sync (catch late stat corrections).

MVP reliability requirements:

1. Idempotent upserts for games and team status.
2. Retry logic with exponential backoff for transient API failures.
3. Sync run logging (`started_at`, `completed_at`, `status`, `error`).
4. `last_synced_at` shown in admin and leaderboard views.
5. Manual `Sync Now` action for admins.
6. Manual game result editing if provider data is delayed/incorrect.

MVP scoring trigger:

1. After every successful sync or admin result edit:
- recalculate `team_status` (`wins`, `in`)
- recalculate all `entry_scores`
- refresh leaderboard order

## MVP Scope

- Email/password login
- Basic user profile
- Fill out one ranked entry
- See personal score and potential
- View leaderboard

- Lock entries at tip-off
- Update game winners/scores
- Trigger score recalculation

## Suggested Data Model

- `users`
- `pools`
- `pool_members`
- `teams` (64 tournament teams with seed + region)
- `entries` (one per user per pool)
- `entry_team_ranks` (`entry_id`, `team_id`, `rank`)
- `games` (official game results)
- `team_status` (`team_id`, `wins`, `in`)
- `sync_runs` (`provider`, `started_at`, `completed_at`, `status`, `error`)
- `provider_game_map` (`provider`, `external_game_id`, `game_id`)
- `entry_scores` (`current_points`, `potential_points`, `max_possible`)

## Build Phases

1. Scaffold frontend, backend, database, and auth
2. Seed 64 teams (including play-in pair slots)
3. Build ranking screen with validation checks
4. Build scoring engine and leaderboard
5. Build external results sync jobs + sync monitoring
6. Build admin result update/override flow
7. Deploy and run company pilot

## Next Step

Implement phase 1 now:

- initialize React + .NET solution structure
- create PostgreSQL schema and migrations
- add auth endpoints and basic profile
- seed teams from workbook



## FE
- React
- dev url - http://localhost:5173/

## BE
- .NET 10 C#
- Entity Framework 
- Postgresql Running in a local docker image

- while local api is running, grab json file and import into postman for endpoints
- import the following link while running the api: http://localhost:5202/openapi/v1.json


    Docker DB Flow
        --> docker-compose.yml (holds the info for db name, username, pwd for local)
        --> 

    Entity Framework Flow
        --> add/update entities to the RSMadnessengine.Data/Entities 
        --> add any new entity classes to the AppDbContext.cs
        --> execute the following 2 commands while docker DB image is running
            1. dotnet ef migrations add AddDomainEntities --startup-project RSMadnessEngine.Api --project RSMadnessEngine.Data
            2. dotnet ef database update --startup-project RSMadnessEngine.Api --project RSMadnessEngine.Data

    Background Job For Pulling ESPN data and Recalculating the bracket leaderboard
        --> using a hosted service from .NET.Sdk.Worker -- delaying the job every hour
        --> so we get fresh team information...and fresh leaderboard updates every hour

## Dev Workflow
How the dev workflow feels day-to-day
Terminal 1: docker compose up -d (Postgres — leave running)
Terminal 2: dotnet run --project RSMadnessEngine.Api (API on port 5001)
Terminal 3: cd frontend && npm run dev (React on port 3000)
Edit React code → browser updates instantly (Vite HMR)
Edit C# code → restart dotnet (or use dotnet watch)
