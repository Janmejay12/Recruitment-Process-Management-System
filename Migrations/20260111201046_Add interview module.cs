using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recruitment_System.Migrations
{
    /// <inheritdoc />
    public partial class Addinterviewmodule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterviewProcesses",
                columns: table => new
                {
                    InterviewProcessId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewId = table.Column<int>(type: "int", nullable: false),
                    TotalRounds = table.Column<int>(type: "int", nullable: false),
                    CurrentRound = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "NotStarted"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewProcesses", x => x.InterviewProcessId);
                    table.ForeignKey(
                        name: "FK_InterviewProcesses_CandidateJobReviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "CandidateJobReviews",
                        principalColumn: "ReviewId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewRounds",
                columns: table => new
                {
                    InterviewRoundId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewProcessId = table.Column<int>(type: "int", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    RoundType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Mode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MeetingLinkOrLocation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewRounds", x => x.InterviewRoundId);
                    table.ForeignKey(
                        name: "FK_InterviewRounds_InterviewProcesses_InterviewProcessId",
                        column: x => x.InterviewProcessId,
                        principalTable: "InterviewProcesses",
                        principalColumn: "InterviewProcessId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewFeedbacks",
                columns: table => new
                {
                    InterviewFeedbackId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewRoundId = table.Column<int>(type: "int", nullable: false),
                    InterviewerUserId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Recommendation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewFeedbacks", x => x.InterviewFeedbackId);
                    table.ForeignKey(
                        name: "FK_InterviewFeedbacks_InterviewRounds_InterviewRoundId",
                        column: x => x.InterviewRoundId,
                        principalTable: "InterviewRounds",
                        principalColumn: "InterviewRoundId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewFeedbacks_Users_InterviewerUserId",
                        column: x => x.InterviewerUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewPanelMembers",
                columns: table => new
                {
                    InterviewPanelMemberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewRoundId = table.Column<int>(type: "int", nullable: false),
                    InterviewerUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewPanelMembers", x => x.InterviewPanelMemberId);
                    table.ForeignKey(
                        name: "FK_InterviewPanelMembers_InterviewRounds_InterviewRoundId",
                        column: x => x.InterviewRoundId,
                        principalTable: "InterviewRounds",
                        principalColumn: "InterviewRoundId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewPanelMembers_Users_InterviewerUserId",
                        column: x => x.InterviewerUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewFeedbacks_InterviewerUserId",
                table: "InterviewFeedbacks",
                column: "InterviewerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewFeedbacks_InterviewRoundId_InterviewerUserId",
                table: "InterviewFeedbacks",
                columns: new[] { "InterviewRoundId", "InterviewerUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewPanelMembers_InterviewerUserId",
                table: "InterviewPanelMembers",
                column: "InterviewerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewPanelMembers_InterviewRoundId_InterviewerUserId",
                table: "InterviewPanelMembers",
                columns: new[] { "InterviewRoundId", "InterviewerUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewProcesses_ReviewId",
                table: "InterviewProcesses",
                column: "ReviewId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewRounds_InterviewProcessId_RoundNumber",
                table: "InterviewRounds",
                columns: new[] { "InterviewProcessId", "RoundNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterviewFeedbacks");

            migrationBuilder.DropTable(
                name: "InterviewPanelMembers");

            migrationBuilder.DropTable(
                name: "InterviewRounds");

            migrationBuilder.DropTable(
                name: "InterviewProcesses");
        }
    }
}
