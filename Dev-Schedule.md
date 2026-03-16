# 30-Day Delivery Plan

    ## Week 1 (Mar 16–Mar 22): Spec Freeze + Foundations
        - Finalize requirements and acceptance criteria.
        - Finalize DB schema and API contract.
        - Set up environments (dev/staging/prod), CI/CD, hosting skeleton.
        - Implement auth + profile.
        - Exit criteria: user can register/login and retrieve profile in staging.
    ## Week 2 (Mar 23–Mar 29): Entry Workflow
        - Seed 64 teams and season data.
        - Build ranking UI and validations (1..64 unique, sum 2080).
        - Save draft and submit entry.
        - Add lock-time enforcement.
        - Exit criteria: complete entry flow works end-to-end.
    ## Week 3 (Mar 30–Apr 5): Scoring + Leaderboard
        - Admin result update flow.
        - Score engine (current_points, potential_points, leaderboard rank).
        - Leaderboard UI with tie-break behavior.
        - Exit criteria: after admin updates, scores and rank update correctly.
    ## Week 4 (Apr 6–Apr 12): Hardening + UAT
        - Security pass (rate limits, auth checks, input validation).
        - QA test pass against all rules.
        - Performance and responsive QA.
        - Pilot with small coworker group.
        - Exit criteria: zero P1 bugs, pilot signoff.
    ## Final Buffer (Apr 13–Apr 16): Launch
        - Production data reset/seed.
        - Final smoke tests.
        - Go-live and monitoring.