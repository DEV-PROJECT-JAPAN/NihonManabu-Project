using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class addfoldervocabularies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserVocabularies_UserFlashcardLists_ListId",
                table: "UserVocabularies");

            migrationBuilder.DropIndex(
                name: "IX_UserVocabularies_ListId",
                table: "UserVocabularies");

            migrationBuilder.DropColumn(
                name: "ListId",
                table: "UserVocabularies");

            migrationBuilder.CreateTable(
                name: "FolderVocabularies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VocabularyId = table.Column<int>(type: "int", nullable: false),
                    ListId = table.Column<int>(type: "int", nullable: false),
                    UserFlashcardListId = table.Column<int>(type: "int", nullable: true),
                    UserVocabularyId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderVocabularies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolderVocabularies_UserFlashcardLists_UserFlashcardListId",
                        column: x => x.UserFlashcardListId,
                        principalTable: "UserFlashcardLists",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FolderVocabularies_UserVocabularies_UserVocabularyId",
                        column: x => x.UserVocabularyId,
                        principalTable: "UserVocabularies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FolderVocabularies_UserFlashcardListId",
                table: "FolderVocabularies",
                column: "UserFlashcardListId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderVocabularies_UserVocabularyId",
                table: "FolderVocabularies",
                column: "UserVocabularyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FolderVocabularies");

            migrationBuilder.AddColumn<int>(
                name: "ListId",
                table: "UserVocabularies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserVocabularies_ListId",
                table: "UserVocabularies",
                column: "ListId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserVocabularies_UserFlashcardLists_ListId",
                table: "UserVocabularies",
                column: "ListId",
                principalTable: "UserFlashcardLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
