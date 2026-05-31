using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUserVocabularyAndRemoveItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFlashcardItems");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Answers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserVocabularies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListId = table.Column<int>(type: "int", nullable: false),
                    Kanji = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Hiragana = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Meaning = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Romaji = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExampleSentence = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsMastered = table.Column<bool>(type: "bit", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    LastReviewed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVocabularies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserVocabularies_UserFlashcardLists_ListId",
                        column: x => x.ListId,
                        principalTable: "UserFlashcardLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserVocabularies_ListId",
                table: "UserVocabularies",
                column: "ListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserVocabularies");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Answers");

            migrationBuilder.CreateTable(
                name: "UserFlashcardItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListId = table.Column<int>(type: "int", nullable: false),
                    VocabularyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFlashcardItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFlashcardItems_UserFlashcardLists_ListId",
                        column: x => x.ListId,
                        principalTable: "UserFlashcardLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFlashcardItems_Vocabularies_VocabularyId",
                        column: x => x.VocabularyId,
                        principalTable: "Vocabularies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFlashcardItems_ListId",
                table: "UserFlashcardItems",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFlashcardItems_VocabularyId",
                table: "UserFlashcardItems",
                column: "VocabularyId");
        }
    }
}
