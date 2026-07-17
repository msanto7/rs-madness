using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RSMadnessEngine.Data.Entities;

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
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>()
                .HasIndex(rt => rt.TokenHash)
                .IsUnique();

            builder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
