using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable


namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Genres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFriendRequests");

            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                table: "Track",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                table: "Album",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genre", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MoodTag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoodTag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlbumMoodTag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlbumId = table.Column<int>(type: "integer", nullable: false),
                    MoodTagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlbumMoodTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlbumMoodTag_Album_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Album",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlbumMoodTag_MoodTag_MoodTagId",
                        column: x => x.MoodTagId,
                        principalTable: "MoodTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackMoodTag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrackId = table.Column<int>(type: "integer", nullable: false),
                    MoodTagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackMoodTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackMoodTag_MoodTag_MoodTagId",
                        column: x => x.MoodTagId,
                        principalTable: "MoodTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackMoodTag_Track_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Genre",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Rock" },
                    { 2, "Pop" },
                    { 3, "Hip-Hop" },
                    { 4, "Electronic" },
                    { 5, "Jazz" },
                    { 6, "Classical" },
                    { 7, "Country" },
                    { 8, "R&B" },
                    { 9, "Metal" },
                    { 10, "Folk" }
                });

            migrationBuilder.InsertData(
                table: "MoodTag",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Happy" },
                    { 2, "Sad" },
                    { 3, "Energetic" },
                    { 4, "Relaxed" },
                    { 5, "Melancholic" },
                    { 6, "Upbeat" },
                    { 7, "Calm" },
                    { 8, "Aggressive" },
                    { 9, "Romantic" },
                    { 10, "Nostalgic" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Track_GenreId",
                table: "Track",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Album_GenreId",
                table: "Album",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumMoodTag_AlbumId",
                table: "AlbumMoodTag",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_AlbumMoodTag_MoodTagId",
                table: "AlbumMoodTag",
                column: "MoodTagId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackMoodTag_MoodTagId",
                table: "TrackMoodTag",
                column: "MoodTagId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackMoodTag_TrackId",
                table: "TrackMoodTag",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Album_Genre_GenreId",
                table: "Album",
                column: "GenreId",
                principalTable: "Genre",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Track_Genre_GenreId",
                table: "Track",
                column: "GenreId",
                principalTable: "Genre",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Album_Genre_GenreId",
                table: "Album");

            migrationBuilder.DropForeignKey(
                name: "FK_Track_Genre_GenreId",
                table: "Track");

            migrationBuilder.DropTable(
                name: "AlbumMoodTag");

            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropTable(
                name: "TrackMoodTag");

            migrationBuilder.DropTable(
                name: "MoodTag");

            migrationBuilder.DropIndex(
                name: "IX_Track_GenreId",
                table: "Track");

            migrationBuilder.DropIndex(
                name: "IX_Album_GenreId",
                table: "Album");

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "Track");

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "Album");

            migrationBuilder.CreateTable(
                name: "UserFriendRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FromUserId = table.Column<int>(type: "integer", nullable: false),
                    ToUserId = table.Column<int>(type: "integer", nullable: false),
                    IsAccepted = table.Column<bool>(type: "boolean", nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFriendRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFriendRequests_AspNetUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserFriendRequests_AspNetUsers_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFriendRequests_FromUserId",
                table: "UserFriendRequests",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFriendRequests_ToUserId",
                table: "UserFriendRequests",
                column: "ToUserId");
        }
    }
}
