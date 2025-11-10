using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class stadonegrov : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumArtists_Artist_ArtistId",
                table: "AlbumArtists");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumArtists_Artist_ArtistId",
                table: "AlbumArtists",
                column: "ArtistId",
                principalTable: "Artist",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlbumArtists_Artist_ArtistId",
                table: "AlbumArtists");

            migrationBuilder.AddForeignKey(
                name: "FK_AlbumArtists_Artist_ArtistId",
                table: "AlbumArtists",
                column: "ArtistId",
                principalTable: "Artist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
