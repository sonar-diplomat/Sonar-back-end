using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _4853457 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueueTrack_Queue_QueuesId",
                table: "QueueTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_QueueTrack_Track_TracksId",
                table: "QueueTrack");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QueueTrack",
                table: "QueueTrack");

            migrationBuilder.RenameColumn(
                name: "TracksId",
                table: "QueueTrack",
                newName: "TrackId");

            migrationBuilder.RenameColumn(
                name: "QueuesId",
                table: "QueueTrack",
                newName: "QueueId");

            migrationBuilder.RenameIndex(
                name: "IX_QueueTrack_TracksId",
                table: "QueueTrack",
                newName: "IX_QueueTrack_TrackId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "QueueTrack",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<bool>(
                name: "IsManuallyAdded",
                table: "QueueTrack",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "QueueTrack",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_QueueTrack",
                table: "QueueTrack",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_QueueTrack_QueueId",
                table: "QueueTrack",
                column: "QueueId");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueTrack_Queue_QueueId",
                table: "QueueTrack",
                column: "QueueId",
                principalTable: "Queue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QueueTrack_Track_TrackId",
                table: "QueueTrack",
                column: "TrackId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueueTrack_Queue_QueueId",
                table: "QueueTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_QueueTrack_Track_TrackId",
                table: "QueueTrack");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QueueTrack",
                table: "QueueTrack");

            migrationBuilder.DropIndex(
                name: "IX_QueueTrack_QueueId",
                table: "QueueTrack");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "QueueTrack");

            migrationBuilder.DropColumn(
                name: "IsManuallyAdded",
                table: "QueueTrack");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "QueueTrack");

            migrationBuilder.RenameColumn(
                name: "TrackId",
                table: "QueueTrack",
                newName: "TracksId");

            migrationBuilder.RenameColumn(
                name: "QueueId",
                table: "QueueTrack",
                newName: "QueuesId");

            migrationBuilder.RenameIndex(
                name: "IX_QueueTrack_TrackId",
                table: "QueueTrack",
                newName: "IX_QueueTrack_TracksId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QueueTrack",
                table: "QueueTrack",
                columns: new[] { "QueuesId", "TracksId" });

            migrationBuilder.AddForeignKey(
                name: "FK_QueueTrack_Queue_QueuesId",
                table: "QueueTrack",
                column: "QueuesId",
                principalTable: "Queue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QueueTrack_Track_TracksId",
                table: "QueueTrack",
                column: "TracksId",
                principalTable: "Track",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
