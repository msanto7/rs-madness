using RSMadnessEngine.Api.DTOs.BracketEntry;
using RSMadnessEngine.Api.Errors;
using RSMadnessEngine.Api.Services.BracketEntries.Repositories;
using RSMadnessEngine.Data.Entities;

namespace RSMadnessEngine.Api.Services.BracketEntries
{
    public class BracketEntryService : IBracketEntryService
    {
        private readonly IBracketEntryRepository _bracketEntryRepository;
        private readonly IScoringService _scoringService;

        public BracketEntryService(IBracketEntryRepository bracketEntryRepository, IScoringService scoringService)
        {
            _bracketEntryRepository = bracketEntryRepository;
            _scoringService = scoringService;
        }

        /// <summary>
        /// Gets the bracket entry for the requested user.
        /// </summary>
        public async Task<GetBracketEntryResponse> GetMyBracketEntryAsync(string userId)
        {
            var response = await _bracketEntryRepository.GetResponseByUserIdAsync(userId);
            return response ?? throw new ApiNotFoundException("bracket-entry-not-found", "Bracket entry not found.");
        }

        /// <summary>
        /// Creates or updates a bracket entry for the requested user.
        /// </summary>
        public async Task<GetBracketEntryResponse> SaveRanksAsync(string userId, SaveRanksRequest request)
        {
            // make sure bracket ranking is valid
            var errors = ValidateRanks(request.Ranks);
            if (errors.Any())
            {
                throw new ApiValidationException("invalid-bracket-ranks", "Bracket entry is invalid.", errors);
            }

            var bracketEntry = await _bracketEntryRepository.GetByUserIdWithRanksAsync(userId);

            // make sure we haven't locked in the bracket yet
            if (bracketEntry != null && bracketEntry.SubmittedAt != null)
            {
                throw new ApiConflictException("bracket-entry-locked", "Bracket entry has already been submitted and cannot be modified.");
            }

            // add a new entry if the user has not made one yet
            if (bracketEntry == null)
            {
                bracketEntry = new BracketEntry
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _bracketEntryRepository.Add(bracketEntry);
            }

            // overwrite the existing bracket state
            bracketEntry.EntryTeamRanks.Clear();
            foreach (var rank in request.Ranks)
            {
                bracketEntry.EntryTeamRanks.Add(new BracketEntryTeamRank
                {
                    TeamId = rank.TeamId,
                    Rank = rank.Rank
                });
            }

            await _bracketEntryRepository.SaveChangesAsync();

            // return a full new copy of the bracket entry state
            var response = await _bracketEntryRepository.GetResponseByUserIdAsync(userId);
            return response ?? throw new ApiNotFoundException("bracket-entry-not-found", "Bracket entry not found.");
        }

        /// <summary>
        /// Locks the bracket entry for the logged-in user.
        /// </summary>
        public async Task<GetBracketEntryResponse> SubmitAsync(string userId)
        {
            var bracketEntry = await _bracketEntryRepository.GetByUserIdWithRanksAsync(userId);
            if (bracketEntry == null)
            {
                throw new ApiNotFoundException("bracket-entry-not-found", "No bracket entry found. Save your rankings first.");
            }

            if (bracketEntry.SubmittedAt != null)
            {
                throw new ApiConflictException("bracket-entry-locked", "Bracket entry has already been submitted and cannot be modified.");
            }

            var ranks = bracketEntry.EntryTeamRanks
                .OrderBy(r => r.Rank)
                .Select(r => new RankAssignment
                {
                    TeamId = r.TeamId,
                    Rank = r.Rank
                }).ToList();

            var errors = ValidateRanks(ranks);
            if (errors.Any())
            {
                throw new ApiValidationException("invalid-bracket-ranks", "Bracket entry is invalid.", errors);
            }

            bracketEntry.SubmittedAt = DateTime.UtcNow;
            await _bracketEntryRepository.SaveChangesAsync();

            // calculate score for the current bracket
            await _scoringService.CalculatecoreAsync(bracketEntry);

            // return fresh response
            var response = await _bracketEntryRepository.GetResponseByUserIdAsync(userId);
            return response ?? throw new ApiNotFoundException("bracket-entry-not-found", "Submitted bracket could not be loaded.");
        }

        /// <summary>
        /// Validates bracket ranking rules before saving or submitting.
        /// </summary>
        private static List<string> ValidateRanks(List<RankAssignment> ranks)
        {
            var errors = new List<string>();

            // validate rank count
            if (ranks.Count != 64)
            {
                errors.Add("Exactly 64 ranks must be assigned.");
                return errors;
            }

            // validate rank values
            var rankValues = ranks.Select(r => r.Rank).ToList();

            if (rankValues.Any(r => r < 1 || r > 64))
            {
                errors.Add("Ranks must be between 1 and 64.");
            }

            if (rankValues.Distinct().Count() != 64)
            {
                errors.Add("Ranks must be unique.");
            }

            if (rankValues.Sum() != 2080)
            {
                errors.Add("Ranks must sum to 2080");
            }

            // validate teams
            var teamIds = ranks.Select(r => r.TeamId).ToList();
            if (teamIds.Distinct().Count() != 64)
            {
                errors.Add("Duplicate team Ids found.");
            }

            // TODO -- validate team ids exist in the teams table

            return errors;
        }
    }
}
