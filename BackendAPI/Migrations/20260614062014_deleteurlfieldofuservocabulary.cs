using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class deleteurlfieldofuservocabulary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FolderVocabularies_UserFlashcardLists_UserFlashcardListId",
                table: "FolderVocabularies");

            migrationBuilder.DropForeignKey(
                name: "FK_FolderVocabularies_UserVocabularies_UserVocabularyId",
                table: "FolderVocabularies");

            migrationBuilder.DropIndex(
                name: "IX_FolderVocabularies_UserFlashcardListId",
                table: "FolderVocabularies");

            migrationBuilder.DropIndex(
                name: "IX_FolderVocabularies_UserVocabularyId",
                table: "FolderVocabularies");

            migrationBuilder.DropColumn(
                name: "AudioUrl",
                table: "UserVocabularies");

            migrationBuilder.DropColumn(
                name: "UserFlashcardListId",
                table: "FolderVocabularies");

            migrationBuilder.DropColumn(
                name: "UserVocabularyId",
                table: "FolderVocabularies");

            migrationBuilder.CreateIndex(
                name: "IX_FolderVocabularies_ListId",
                table: "FolderVocabularies",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderVocabularies_VocabularyId",
                table: "FolderVocabularies",
                column: "VocabularyId");

            migrationBuilder.AddForeignKey(
                name: "FK_FolderVocabularies_UserFlashcardLists_ListId",
                table: "FolderVocabularies",
                column: "ListId",
                principalTable: "UserFlashcardLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FolderVocabularies_UserVocabularies_VocabularyId",
                table: "FolderVocabularies",
                column: "VocabularyId",
                principalTable: "UserVocabularies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FolderVocabularies_UserFlashcardLists_ListId",
                table: "FolderVocabularies");

            migrationBuilder.DropForeignKey(
                name: "FK_FolderVocabularies_UserVocabularies_VocabularyId",
                table: "FolderVocabularies");

            migrationBuilder.DropIndex(
                name: "IX_FolderVocabularies_ListId",
                table: "FolderVocabularies");

            migrationBuilder.DropIndex(
                name: "IX_FolderVocabularies_VocabularyId",
                table: "FolderVocabularies");

            migrationBuilder.AddColumn<string>(
                name: "AudioUrl",
                table: "UserVocabularies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserFlashcardListId",
                table: "FolderVocabularies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserVocabularyId",
                table: "FolderVocabularies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FolderVocabularies_UserFlashcardListId",
                table: "FolderVocabularies",
                column: "UserFlashcardListId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderVocabularies_UserVocabularyId",
                table: "FolderVocabularies",
                column: "UserVocabularyId");

            migrationBuilder.AddForeignKey(
                name: "FK_FolderVocabularies_UserFlashcardLists_UserFlashcardListId",
                table: "FolderVocabularies",
                column: "UserFlashcardListId",
                principalTable: "UserFlashcardLists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FolderVocabularies_UserVocabularies_UserVocabularyId",
                table: "FolderVocabularies",
                column: "UserVocabularyId",
                principalTable: "UserVocabularies",
                principalColumn: "Id");
        }
    }
}
