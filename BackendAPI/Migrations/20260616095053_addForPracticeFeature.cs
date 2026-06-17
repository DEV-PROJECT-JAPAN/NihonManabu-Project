using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class addForPracticeFeature : Migration
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

            migrationBuilder.CreateTable(
                name: "FolderVocabularies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VocabularyId = table.Column<int>(type: "int", nullable: false),
                    ListId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderVocabularies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolderVocabularies_UserFlashcardLists_ListId",
                        column: x => x.ListId,
                        principalTable: "UserFlashcardLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FolderVocabularies_UserVocabularies_VocabularyId",
                        column: x => x.VocabularyId,
                        principalTable: "UserVocabularies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FolderVocabularies_ListId",
                table: "FolderVocabularies",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderVocabularies_VocabularyId",
                table: "FolderVocabularies",
                column: "VocabularyId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FolderVocabularies");

            migrationBuilder.DropTable(
                name: "Subscriptions");

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
