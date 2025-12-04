using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recruitment_System.Migrations
{
    /// <inheritdoc />
    public partial class updatedcandidate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Candidates_UserId",
                table: "Candidates");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Candidates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Candidates",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_CreatedBy",
                table: "Candidates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_UserId",
                table: "Candidates",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_Users_CreatedBy",
                table: "Candidates",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_Users_CreatedBy",
                table: "Candidates");

            migrationBuilder.DropIndex(
                name: "IX_Candidates_CreatedBy",
                table: "Candidates");

            migrationBuilder.DropIndex(
                name: "IX_Candidates_UserId",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Candidates");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Candidates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_UserId",
                table: "Candidates",
                column: "UserId",
                unique: true);
        }
    }
}
