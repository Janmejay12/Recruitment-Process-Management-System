using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recruitment_System.Migrations
{
    /// <inheritdoc />
    public partial class Entitiesforscreening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CandidateJobReviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    CurrentStage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Screening"),
                    AssignedReviewerId = table.Column<int>(type: "int", nullable: true),
                    AssignedInterviewerId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateJobReviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_CandidateJobReviews_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "CandidateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidateJobReviews_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateJobReviews_Users_AssignedInterviewerId",
                        column: x => x.AssignedInterviewerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateJobReviews_Users_AssignedReviewerId",
                        column: x => x.AssignedReviewerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CandidateReviewComments",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewId = table.Column<int>(type: "int", nullable: false),
                    CommentText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CommentedByUserId = table.Column<int>(type: "int", nullable: false),
                    RoleAtTime = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateReviewComments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_CandidateReviewComments_CandidateJobReviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "CandidateJobReviews",
                        principalColumn: "ReviewId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidateReviewComments_Users_CommentedByUserId",
                        column: x => x.CommentedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CandidateSkillEvaluations",
                columns: table => new
                {
                    EvaluationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    YearsExperience = table.Column<int>(type: "int", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateSkillEvaluations", x => x.EvaluationId);
                    table.ForeignKey(
                        name: "FK_CandidateSkillEvaluations_CandidateJobReviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "CandidateJobReviews",
                        principalColumn: "ReviewId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidateSkillEvaluations_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "SkillId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateSkillEvaluations_Users_VerifiedByUserId",
                        column: x => x.VerifiedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateJobReviews_AssignedInterviewerId",
                table: "CandidateJobReviews",
                column: "AssignedInterviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateJobReviews_AssignedReviewerId",
                table: "CandidateJobReviews",
                column: "AssignedReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateJobReviews_CandidateId",
                table: "CandidateJobReviews",
                column: "CandidateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateJobReviews_JobId",
                table: "CandidateJobReviews",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateReviewComments_CommentedByUserId",
                table: "CandidateReviewComments",
                column: "CommentedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateReviewComments_ReviewId",
                table: "CandidateReviewComments",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateSkillEvaluations_ReviewId_SkillId",
                table: "CandidateSkillEvaluations",
                columns: new[] { "ReviewId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateSkillEvaluations_SkillId",
                table: "CandidateSkillEvaluations",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateSkillEvaluations_VerifiedByUserId",
                table: "CandidateSkillEvaluations",
                column: "VerifiedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateReviewComments");

            migrationBuilder.DropTable(
                name: "CandidateSkillEvaluations");

            migrationBuilder.DropTable(
                name: "CandidateJobReviews");
        }
    }
}
