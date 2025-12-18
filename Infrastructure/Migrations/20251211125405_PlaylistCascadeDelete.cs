using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable


namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PlaylistCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Playlist_AspNetUsers_CreatorId",
                table: "Playlist");

            migrationBuilder.InsertData(
                table: "File",
                columns: new[] { "Id", "Discriminator", "ItemName", "PostId", "Url" },
                values: new object[,]
                {
                    { 2, "ImageFile", "Default playlist", null, "" },
                    { 3, "ImageFile", "Default track", null, "" },
                    { 4, "ImageFile", "Default playlist negative", null, "" },
                    { 5, "ImageFile", "Default track negative", null, "" }
                });


            migrationBuilder.AddForeignKey(
                name: "FK_Playlist_AspNetUsers_CreatorId",
                table: "Playlist",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Playlist_AspNetUsers_CreatorId",
                table: "Playlist");

            migrationBuilder.AddForeignKey(
                name: "FK_Playlist_AspNetUsers_CreatorId",
                table: "Playlist",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
