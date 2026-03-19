using System;
using System.Collections.Generic;
using System.Text;

namespace RSMadnessEngine.Data.Entities
{
    public class Team
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;

        public int Seed { get; set; }

        public string Region { get; set; } = string.Empty;

        // props
        public TeamStatus? TeamStatus { get; set; }
        public ICollection<BracketEntryTeamRank> BracketEntryTeamRanks { get; set; } = new List<BracketEntryTeamRank>();
    }
}
