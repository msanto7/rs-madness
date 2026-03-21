select * from public.AspNetUsers


select * from "BracketEntries"


update "BracketEntries"
set "SubmittedAt" = null
where "Id" = 1

delete from "BracketEntries"
where "Id" = 1

insert into "BracketEntryScores" ("Id", "BracketEntryId", "CurrentPoints", "PotentialPoints")
values
        (1, 1, 3, 50),
        (2, 2, 50, 10),
        (3, 3, 5, 3),
        (4, 4, 5, 10)


delete from "Teams"



-- reset sequence id

ALTER SEQUENCE "Teams_Id_seq" RESTART WITH 1;



-- query the teams that have been knocked out to check with the actual bracket data
-- first round eliminations

select *
from "Teams" t
        inner join "TeamStatuses" ts ON ts."TeamId" = t."Id"
where "IsAlive" = false





