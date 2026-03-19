using System;
using System.Collections.Generic;
using System.Text;

namespace RSMadnessEngine.Data.Entities
{
    public class BracketEntry
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public DateTime? SubmittedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // props
        public AppUser User { get; set; } = null!;
        public ICollection<BracketEntryTeamRank> EntryTeamRanks { get; set; } = new List<BracketEntryTeamRank>();
        public BracketEntryScore? Score { get; set; }
    }
}
