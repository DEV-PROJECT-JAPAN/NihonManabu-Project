using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class RebuildUserAndFlashcardSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFlashcards");

            migrationBuilder.CreateTable(
                name: "UserFlashcardLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFlashcardLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFlashcardLists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_UserFlashcardLists_UserId",
                table: "UserFlashcardLists",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFlashcardItems");

            migrationBuilder.DropTable(
                name: "UserFlashcardLists");

            migrationBuilder.CreateTable(
                name: "UserFlashcards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Kana = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Kanji = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Meaning = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFlashcards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFlashcards_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFlashcards_UserId",
                table: "UserFlashcards",
                column: "UserId");
        }
    }
}
