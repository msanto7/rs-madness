using RSMadnessEngine.Api.DTOs.BracketEntry;
using RSMadnessEngine.Data.Entities;

namespace RSMadnessEngine.Api.Services.BracketEntries.Repositories
{
    public interface IBracketEntryRepository
    {
        Task<GetBracketEntryResponse?> GetResponseByUserIdAsync(string userId);
        Task<BracketEntry?> GetByUserIdWithRanksAsync(string userId);
        void Add(BracketEntry bracketEntry);
        Task SaveChangesAsync();
    }
}
