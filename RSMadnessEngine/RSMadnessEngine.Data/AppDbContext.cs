using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RSMadnessEngine.Data.Entities;
using System.Security.Cryptography.X509Certificates;

namespace RSMadnessEngine.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Team> Teams => Set<Team>();
        public DbSet<TeamStatus> TeamStatuses => Set<TeamStatus>();
        public DbSet<BracketEntry> BracketEntries => Set<BracketEntry>();
        public DbSet<BracketEntryTeamRank> BracketEntryTeamRanks => Set<BracketEntryTeamRank>();
        public DbSet<BracketEntryScore> BracketEntryScores => Set<BracketEntryScore>();
    }
}
