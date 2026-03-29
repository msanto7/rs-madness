using RSMadnessEngine.Api.DTOs.BracketEntry;

namespace RSMadnessEngine.Api.Services.BracketEntries
{
    public interface IBracketEntryService
    {
        Task<GetBracketEntryResponse?> GetMyBracketEntryAsync(string userId);
        Task<GetBracketEntryResponse> SaveRanksAsync(string userId, SaveRanksRequest request);
        Task<GetBracketEntryResponse> SubmitAsync(string userId);
    }
}
