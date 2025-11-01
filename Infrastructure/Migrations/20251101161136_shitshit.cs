using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class shitshit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RootFolderId",
                table: "Library",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Library_RootFolderId",
                table: "Library",
                column: "RootFolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Library_Folder_RootFolderId",
                table: "Library",
                column: "RootFolderId",
                principalTable: "Folder",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Library_Folder_RootFolderId",
                table: "Library");

            migrationBuilder.DropIndex(
                name: "IX_Library_RootFolderId",
                table: "Library");

            migrationBuilder.DropColumn(
                name: "RootFolderId",
                table: "Library");
        }
    }
}
