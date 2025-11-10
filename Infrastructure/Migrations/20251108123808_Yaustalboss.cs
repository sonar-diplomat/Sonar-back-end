using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Yaustalboss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AlbumArtists",
                table: "AlbumArtists");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AlbumArtists",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "ArtistId",
                table: "AlbumArtists",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AlbumArtists",
                table: "AlbumArtists",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumArtists_AlbumId",
                table: "AlbumArtists",
                column: "AlbumId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AlbumArtists",
                table: "AlbumArtists");

            migrationBuilder.DropIndex(
                name: "IX_AlbumArtists_AlbumId",
                table: "AlbumArtists");

            migrationBuilder.AlterColumn<int>(
                name: "ArtistId",
                table: "AlbumArtists",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "AlbumArtists",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AlbumArtists",
                table: "AlbumArtists",
                columns: new[] { "AlbumId", "ArtistId" });
        }
    }
}
