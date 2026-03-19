using System;
using System.Collections.Generic;
using System.Text;

namespace RSMadnessEngine.Data.Entities
{
    public class TeamStatus
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        public int Wins { get; set; }

        public bool IsAlive { get; set; } = true;

        // props
        public Team Team { get; set; } = null!;
    }
}
