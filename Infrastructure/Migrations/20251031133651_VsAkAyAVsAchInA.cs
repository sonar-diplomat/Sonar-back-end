using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VsAkAyAVsAchInA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlaylistId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PlaylistId",
                table: "AspNetUsers",
                column: "PlaylistId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Playlist_PlaylistId",
                table: "AspNetUsers",
                column: "PlaylistId",
                principalTable: "Playlist",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Playlist_PlaylistId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PlaylistId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PlaylistId",
                table: "AspNetUsers");
        }
    }
}
