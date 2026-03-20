using Microsoft.EntityFrameworkCore;
using RSMadnessEngine.Data.Entities;

namespace RSMadnessEngine.Data.Seed;

public static class TeamSeeder
{
    public static async Task SeedTeamsAsync(AppDbContext db)
    {
        if (await db.Teams.AnyAsync())
            return;

        var teams = new List<Team>
        {
            // East
            new() { Name = "Duke", Seed = 1, Region = "East" },
            new() { Name = "Siena", Seed = 16, Region = "East" },
            new() { Name = "Ohio State", Seed = 8, Region = "East" },
            new() { Name = "TCU", Seed = 9, Region = "East" },
            new() { Name = "St John's", Seed = 5, Region = "East" },
            new() { Name = "Northern Iowa", Seed = 12, Region = "East" },
            new() { Name = "Kansas", Seed = 4, Region = "East" },
            new() { Name = "Cal Baptist", Seed = 13, Region = "East" },
            new() { Name = "Louisville", Seed = 6, Region = "East" },
            new() { Name = "South Florida", Seed = 11, Region = "East" },
            new() { Name = "Michigan St", Seed = 3, Region = "East" },
            new() { Name = "N Dakota St", Seed = 14, Region = "East" },
            new() { Name = "UCLA", Seed = 7, Region = "East" },
            new() { Name = "UCF", Seed = 10, Region = "East" },
            new() { Name = "UConn", Seed = 2, Region = "East" },
            new() { Name = "Furman", Seed = 15, Region = "East" },

            // South
            new() { Name = "Florida", Seed = 1, Region = "South" },
            new() { Name = "Prairie View", Seed = 16, Region = "South" },
            new() { Name = "Clemson", Seed = 8, Region = "South" },
            new() { Name = "Iowa", Seed = 9, Region = "South" },
            new() { Name = "Vanderbilt", Seed = 5, Region = "South" },
            new() { Name = "McNeese", Seed = 12, Region = "South" },
            new() { Name = "Nebraska", Seed = 4, Region = "South" },
            new() { Name = "Troy", Seed = 13, Region = "South" },
            new() { Name = "North Carolina", Seed = 6, Region = "South" },
            new() { Name = "VCU", Seed = 11, Region = "South" },
            new() { Name = "Illinois", Seed = 3, Region = "South" },
            new() { Name = "Penn", Seed = 14, Region = "South" },
            new() { Name = "Saint Mary's", Seed = 7, Region = "South" },
            new() { Name = "Texas A&M", Seed = 10, Region = "South" },
            new() { Name = "Houston", Seed = 2, Region = "South" },
            new() { Name = "Idaho", Seed = 15, Region = "South" },

            // West
            new() { Name = "Arizona", Seed = 1, Region = "West" },
            new() { Name = "Long Island", Seed = 16, Region = "West" },
            new() { Name = "Villanova", Seed = 8, Region = "West" },
            new() { Name = "Utah State", Seed = 9, Region = "West" },
            new() { Name = "Wisconsin", Seed = 5, Region = "West" },
            new() { Name = "High Point", Seed = 12, Region = "West" },
            new() { Name = "Arkansas", Seed = 4, Region = "West" },
            new() { Name = "Hawai'i", Seed = 13, Region = "West" },
            new() { Name = "BYU", Seed = 6, Region = "West" },
            new() { Name = "Texas", Seed = 11, Region = "West" },
            new() { Name = "Gonzaga", Seed = 3, Region = "West" },
            new() { Name = "Kennesaw St", Seed = 14, Region = "West" },
            new() { Name = "Miami", Seed = 7, Region = "West" },
            new() { Name = "Missouri", Seed = 10, Region = "West" },
            new() { Name = "Purdue", Seed = 2, Region = "West" },
            new() { Name = "Queens", Seed = 15, Region = "West" },

            // Midwest
            new() { Name = "Michigan", Seed = 1, Region = "Midwest" },
            new() { Name = "Howard", Seed = 16, Region = "Midwest" },
            new() { Name = "Georgia", Seed = 8, Region = "Midwest" },
            new() { Name = "Saint Louis", Seed = 9, Region = "Midwest" },
            new() { Name = "Texas Tech", Seed = 5, Region = "Midwest" },
            new() { Name = "Akron", Seed = 12, Region = "Midwest" },
            new() { Name = "Alabama", Seed = 4, Region = "Midwest" },
            new() { Name = "Hofstra", Seed = 13, Region = "Midwest" },
            new() { Name = "Tennessee", Seed = 6, Region = "Midwest" },
            new() { Name = "Miami OH", Seed = 11, Region = "Midwest" },
            new() { Name = "Virginia", Seed = 3, Region = "Midwest" },
            new() { Name = "Wright St", Seed = 14, Region = "Midwest" },
            new() { Name = "Kentucky", Seed = 7, Region = "Midwest" },
            new() { Name = "Santa Clara", Seed = 10, Region = "Midwest" },
            new() { Name = "Iowa State", Seed = 2, Region = "Midwest" },
            new() { Name = "Tennessee St", Seed = 15, Region = "Midwest" },
        };

        db.Teams.AddRange(teams);
        await db.SaveChangesAsync();
    }
}