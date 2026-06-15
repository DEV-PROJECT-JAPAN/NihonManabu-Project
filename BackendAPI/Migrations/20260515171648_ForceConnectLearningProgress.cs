using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class ForceConnectLearningProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_UserId",
                table: "LearningProgresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningProgresses_VocabularyId",
                table: "LearningProgresses",
                column: "VocabularyId");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_Users_UserId",
                table: "LearningProgresses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_Vocabularies_VocabularyId",
                table: "LearningProgresses",
                column: "VocabularyId",
                principalTable: "Vocabularies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_Users_UserId",
                table: "LearningProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_Vocabularies_VocabularyId",
                table: "LearningProgresses");

            migrationBuilder.DropIndex(
                name: "IX_LearningProgresses_UserId",
                table: "LearningProgresses");

            migrationBuilder.DropIndex(
                name: "IX_LearningProgresses_VocabularyId",
                table: "LearningProgresses");
        }
    }
}
