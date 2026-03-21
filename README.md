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


## Scoring Formulas

Per user entry, scoring should be:

1. `current_points = SUM(team.rank * team.wins)`
2. `potential_points = SUM(team.in * (6 - team.wins) * team.rank)`
3. `max_possible = current_points + potential_points`


## FE Tech

1. Frontend: React + TypeScript (responsive web app)
2. Backend: ASP.NET Core Web API (.NET 8+)
3. Database: PostgreSQL
4. Auth: email/password (ASP.NET Core Identity)
5. Hosting: simple managed platform (Render is a good fit)


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

    FREE Sports API I am using for the live game data refresh
        - https://github.com/pseudo-r/Public-ESPN-API
        - https://github.com/pseudo-r/Public-ESPN-API/blob/main/docs/sports/basketball.md

## Dev Workflow
How the dev workflow feels day-to-day
Terminal 1: docker compose up -d (Postgres — leave running)
Terminal 2: dotnet run --project RSMadnessEngine.Api (API on port 5001)
Terminal 3: cd frontend && npm run dev (React on port 3000)
Edit React code → browser updates instantly (Vite HMR)
Edit C# code → restart dotnet (or use dotnet watch)
