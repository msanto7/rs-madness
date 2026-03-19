using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RSMadnessEngine.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDomainEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BracketEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BracketEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BracketEntries_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Seed = table.Column<int>(type: "integer", nullable: false),
                    Region = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BracketEntryScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BracketEntryId = table.Column<int>(type: "integer", nullable: false),
                    CurrentPoints = table.Column<int>(type: "integer", nullable: false),
                    PotentialPoints = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BracketEntryScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BracketEntryScores_BracketEntries_BracketEntryId",
                        column: x => x.BracketEntryId,
                        principalTable: "BracketEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BracketEntryTeamRanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BracketEntryId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BracketEntryTeamRanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BracketEntryTeamRanks_BracketEntries_BracketEntryId",
                        column: x => x.BracketEntryId,
                        principalTable: "BracketEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BracketEntryTeamRanks_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    Wins = table.Column<int>(type: "integer", nullable: false),
                    IsAlive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamStatuses_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BracketEntries_UserId",
                table: "BracketEntries",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BracketEntryScores_BracketEntryId",
                table: "BracketEntryScores",
                column: "BracketEntryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BracketEntryTeamRanks_BracketEntryId",
                table: "BracketEntryTeamRanks",
                column: "BracketEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_BracketEntryTeamRanks_TeamId",
                table: "BracketEntryTeamRanks",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamStatuses_TeamId",
                table: "TeamStatuses",
                column: "TeamId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BracketEntryScores");

            migrationBuilder.DropTable(
                name: "BracketEntryTeamRanks");

            migrationBuilder.DropTable(
                name: "TeamStatuses");

            migrationBuilder.DropTable(
                name: "BracketEntries");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
