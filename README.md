# rs-madness

March Madness pool app based on `Blank Bracket 2026.xlsx`.

- Live site: https://rs-madness-web.onrender.com
- Frontend: React + TypeScript (Vite)
- Backend: .NET 10 Web API + EF Core
- Database: PostgreSQL 17

## Goal

Build a simple company pool app where users can:

1. Log in
2. Create a ranked bracket entry
3. Track points as games finish
4. View a live leaderboard

## Bracket Rules

1. Rank all 64 teams.
2. Use each number from `1` through `64` exactly once.
3. Higher rank values should go to teams expected to advance further.
4. Every time a team wins, you earn the rank value you assigned to that team.
5. Example: rank `50` and `3` wins = `150` points.
6. Upsets still score using your assigned rank for the winning team.

## Scoring Logic

1. `current_points = SUM(team.rank * team.wins)`
2. `potential_points = SUM(team.in * (6 - team.wins) * team.rank)`
3. `max_possible = current_points + potential_points`

## Tech Stack

### Frontend

- React + TypeScript
- Vite
- Deployed as a Render static site
- Local URL: `http://localhost:5173`

### Backend

- .NET 10 Web API
- Entity Framework Core
- Hosted service for hourly sync + score recalculation
- Deployed as a Render web service
- Local API URL: `http://localhost:5202`
- OpenAPI JSON: `http://localhost:5202/openapi/v1.json`

### Database

- PostgreSQL 17
- Local DB via Docker Compose
- Managed PostgreSQL in Render for production

## External Sports Data

Current source:

- https://github.com/pseudo-r/Public-ESPN-API
- https://github.com/pseudo-r/Public-ESPN-API/blob/main/docs/sports/basketball.md

## Local Development

### 1) Start the database

```powershell
cd RSMadnessEngine
docker compose up -d
```

### 2) Run the API

```powershell
cd RSMadnessEngine
dotnet run --project RSMadnessEngine.Api
```

API runs at `http://localhost:5202`.

### 3) Run the frontend

```powershell
cd RSMadnessWeb
npm run dev
```

Frontend runs at `http://localhost:5173`.

## Entity Framework Workflow

When you change entities:

1. Update entity classes in `RSMadnessEngine.Data/Entities`
2. Register new entities in `AppDbContext`
3. Create and apply migration

```powershell
cd RSMadnessEngine
dotnet ef migrations add <MigrationName> --startup-project RSMadnessEngine.Api --project RSMadnessEngine.Data
dotnet ef database update --startup-project RSMadnessEngine.Api --project RSMadnessEngine.Data
```

## CI/CD

- GitHub Actions workflow: `.github/workflows/ci.yml`
- Runs backend and frontend builds on PRs/pushes to `main`
- Render auto-deploys from `main` after merge

## Notes

- Use local Docker Postgres for development and testing.
- Use Render environment variables for production secrets and connection strings.
