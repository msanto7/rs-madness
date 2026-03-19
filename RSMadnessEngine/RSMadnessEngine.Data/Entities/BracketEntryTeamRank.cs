using System;
using System.Collections.Generic;
using System.Text;

namespace RSMadnessEngine.Data.Entities
{
    public class BracketEntryTeamRank
    {
        public int Id { get; set; }

        public int BracketEntryId { get; set; }

        public int TeamId { get; set; }

        public int Rank { get; set; }

        // props
        public BracketEntry BracketEntry { get; set; } = null!;
        public Team Team { get; set; } = null!;
    }
}
