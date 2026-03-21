# rs-madness

March Madness pool app based on `Blank Bracket 2026.xlsx`.

Live site found at: https://rs-madness-web.onrender.com

## Goal

Build a simple app where users:

1. Log in
2. Create their bracket entry (using the same scoring logic as the spreadsheet)
3. Track score updates as real tournament results come in
4. See a live leaderboard

## Bracket Rules

1. Rank all 64 teams.
2. Use each number from 1 through 64 exactly once (no duplicates).
3. Higher rank values should go to teams expected to advance further.
4. Scoring: every time a team wins, the user earns that team's assigned rank value.
5. Example: rank 50 and 3 wins = 150 points from that team.
6. Upsets still score: if a lower-ranked-by-seed team wins, points are still based only on the user's assigned rank for that team.


## Scoring Formulas

Per user entry, scoring should be:

1. `current_points = SUM(team.rank * team.wins)`
2. `potential_points = SUM(team.in * (6 - team.wins) * team.rank)`
3. `max_possible = current_points + potential_points`


## FE Tech

1. React + TypeScript

    dev url - http://localhost:5173/

    Hosting
    - static site on render
    - https://rs-madness-web.onrender.com

## BE Tech

1. .NET 10 API
    - Entity Framework 
    - Postgresql Running in a local docker image

    - while local api is running, grab json file and import into postman for endpoints
    - import the following link while running the api: http://localhost:5202/openapi/v1.json

    Background Job For Pulling ESPN data and Recalculating the bracket leaderboard
            --> using a hosted service from .NET.Sdk.Worker -- delaying the job every hour
            --> so we get fresh team information...and fresh leaderboard updates every hour

    FREE Sports API I am using for the live game data refresh
        - https://github.com/pseudo-r/Public-ESPN-API
        - https://github.com/pseudo-r/Public-ESPN-API/blob/main/docs/sports/basketball.md

    Hosting
        - web service on render


## Database Tech

    1. Postgresql 17

    Docker DB Flow
        --> docker-compose.yml (holds the info for db name, username, pwd for local)
        --> docker compose up -d (starts the local postgresql db in docker container)

    Entity Framework Flow
        --> add/update entities to the RSMadnessengine.Data/Entities 
        --> add any new entity classes to the AppDbContext.cs
        --> execute the following 2 commands while docker DB image is running
            1. dotnet ef migrations add AddDomainEntities --startup-project RSMadnessEngine.Api --project RSMadnessEngine.Data
            2. dotnet ef database update --startup-project RSMadnessEngine.Api --project RSMadnessEngine.Data
   

## Local Dev Workflow

1. clone repository
2. Terminal 1: docker compose up -d (Postgres — leave running)
3. Terminal 2: dotnet run --project RSMadnessEngine.Api (API on port 5001)
4. Terminal 3: cd frontend && npm run dev (React on port 3000)
