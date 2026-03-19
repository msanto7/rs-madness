using System;
using System.Collections.Generic;
using System.Text;

namespace RSMadnessEngine.Data.Entities
{
    public class BracketEntryScore
    {
        public int Id { get; set; }

        public int BracketEntryId { get; set; }

        public int CurrentPoints { get; set; }

        public int PotentialPoints { get; set; }

        // props
        public BracketEntry BracketEntry { get; set; } = null!;
    }
}
