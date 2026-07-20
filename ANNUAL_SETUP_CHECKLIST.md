# Annual Tournament Setup Checklist

The app has no admin UI or seed-refresh mechanism — every item below is a manual
code/config/DB change made once a year before the new bracket opens. Nothing here
is automated; if you skip a step the app will keep running against stale data
without erroring.

## 1. Team data (Data project)

- [ ] Edit `RSMadnessEngine/RSMadnessEngine.Data/Seed/TeamSeeder.cs` with the new
      season's 64 teams (Name, Seed, Region).
- [ ] Clear the DB before restarting the API — `TeamSeeder` only seeds when the
      `Teams` table is empty, so it silently no-ops otherwise:
  ```sql
  delete from "Teams";
  ALTER SEQUENCE "Teams_Id_seq" RESTART WITH 1;
  ```
  (`TeamStatuses` cascades from `Teams`, so it clears with it.) See
  `sql-scratch/dev-db-query-scratch.sql` for the pattern used previously.
- [ ] Restart the API so `TeamSeeder.SeedTeamsAsync` (called from `Program.cs`)
      reseeds the fresh team list.

## 2. Submission deadline (API config)

- [ ] Update `Tournament:SubmissionDeadlineUtc` in
      `RSMadnessEngine/RSMadnessEngine.Api/appsettings.json`.
- [ ] Update the same key in
      `RSMadnessEngine/RSMadnessEngine.Api/appsettings.Development.json`.
- [ ] Double check it's not already set for a future year that doesn't match the
      season you're seeding (it was found already set to `2027-03-20T17:00:00Z`
      once — verify before assuming it's current).

## 3. Live sync date range (API)

- [ ] Update the hardcoded ESPN date filter in
      `RSMadnessEngine/RSMadnessEngine.Api/Services/NcaaDataProvider.cs`
      (`dates=20260319-20260415` today) to the new tournament's date range. If
      this is missed, `TournamentSyncBackgroundJob` will run hourly and silently
      return zero games — no error, no results.

## 4. Frontend year references (Web)

- [ ] Update the year badge in `RSMadnessWeb/src/components/Layout.tsx`
      (`<span className="top-nav__badge">2026</span>`).
- [ ] Replace `RSMadnessWeb/public/Blank Bracket 2026.xlsx` with the new season's
      blank bracket file, named for the new year.
- [ ] Update the `href`/`download` filename in
      `RSMadnessWeb/src/pages/BlankBracketPage.tsx` to match the new filename.

## 5. Sanity checks (rarely change, but verify)

- [ ] `ScoringService.cs` hardcodes `6` as max wins per team (6 rounds for a
      64-team single-elim bracket). Only relevant if the tournament format
      itself changes.
- [ ] `BracketEntryService.cs` validates each submitted bracket has exactly 64
      ranked teams summing to `2080` (1+2+...+64). Also tied to the 64-team
      format — only revisit if that changes.

## Not currently supported

- No admin UI or admin API endpoints exist for teams, tournament setup, or
  scoring rules — all of the above requires direct file edits + DB access.
- No separate "bracket structure" (rounds/matchups) exists to seed — the data
  model is a full 64-team ranking pool, not a single-elimination tree, so
  `TeamSeeder.cs` (Name/Seed/Region) is the entire structural setup.
