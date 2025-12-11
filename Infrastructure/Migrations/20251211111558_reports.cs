using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class reports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportReportReasonType");

            migrationBuilder.AddColumn<int>(
                name: "ReportReasonTypeId",
                table: "Report",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReportReasonTypeId1",
                table: "Report",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReportReasonTypeReportableEntityType",
                columns: table => new
                {
                    ApplicableEntityTypesId = table.Column<int>(type: "integer", nullable: false),
                    ApplicableReportReasonTypesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportReasonTypeReportableEntityType", x => new { x.ApplicableEntityTypesId, x.ApplicableReportReasonTypesId });
                    table.ForeignKey(
                        name: "FK_ReportReasonTypeReportableEntityType_ReportReasonType_Appli~",
                        column: x => x.ApplicableReportReasonTypesId,
                        principalTable: "ReportReasonType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportReasonTypeReportableEntityType_ReportableEntityType_A~",
                        column: x => x.ApplicableEntityTypesId,
                        principalTable: "ReportableEntityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ReportReasonType",
                columns: new[] { "Id", "Name", "RecommendedSuspensionDuration" },
                values: new object[,]
                {
                    { 5, "Copyright infringement", new TimeSpan(30, 0, 0, 0, 0) },
                    { 6, "Explicit content without marking", new TimeSpan(7, 0, 0, 0, 0) },
                    { 7, "Spam audio / noise", new TimeSpan(3, 0, 0, 0, 0) },
                    { 8, "Incorrect tags/metadata", new TimeSpan(1, 0, 0, 0, 0) },
                    { 9, "Copyrighted cover without permission", new TimeSpan(30, 0, 0, 0, 0) },
                    { 10, "Inappropriate playlist name", new TimeSpan(7, 0, 0, 0, 0) },
                    { 11, "Abusive behavior", new TimeSpan(14, 0, 0, 0, 0) },
                    { 12, "Fake identity", new TimeSpan(30, 0, 0, 0, 0) },
                    { 13, "Bot account", new TimeSpan(90, 0, 0, 0, 0) },
                    { 14, "Inappropriate bio", new TimeSpan(7, 0, 0, 0, 0) },
                    { 15, "Scam attempts / phishing", new TimeSpan(90, 0, 0, 0, 0) },
                    { 16, "Spam messaging", new TimeSpan(7, 0, 0, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "ReportReasonTypeReportableEntityType",
                columns: new[] { "ApplicableEntityTypesId", "ApplicableReportReasonTypesId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 4 },
                    { 2, 1 },
                    { 2, 3 },
                    { 2, 4 },
                    { 3, 1 },
                    { 3, 3 },
                    { 3, 4 },
                    { 4, 1 },
                    { 4, 4 }
                });

            migrationBuilder.InsertData(
                table: "ReportableEntityType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 5, "Playlist" },
                    { 6, "Artist" }
                });

            migrationBuilder.InsertData(
                table: "ReportReasonTypeReportableEntityType",
                columns: new[] { "ApplicableEntityTypesId", "ApplicableReportReasonTypesId" },
                values: new object[,]
                {
                    { 1, 11 },
                    { 1, 12 },
                    { 1, 13 },
                    { 1, 14 },
                    { 1, 15 },
                    { 1, 16 },
                    { 2, 5 },
                    { 2, 6 },
                    { 2, 7 },
                    { 2, 8 },
                    { 3, 9 },
                    { 5, 1 },
                    { 5, 4 },
                    { 5, 10 },
                    { 6, 1 },
                    { 6, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportReasonTypeId",
                table: "Report",
                column: "ReportReasonTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReportReasonTypeId1",
                table: "Report",
                column: "ReportReasonTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_ReportReasonTypeReportableEntityType_ApplicableReportReason~",
                table: "ReportReasonTypeReportableEntityType",
                column: "ApplicableReportReasonTypesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_ReportReasonType_ReportReasonTypeId",
                table: "Report",
                column: "ReportReasonTypeId",
                principalTable: "ReportReasonType",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_ReportReasonType_ReportReasonTypeId1",
                table: "Report",
                column: "ReportReasonTypeId1",
                principalTable: "ReportReasonType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Report_ReportReasonType_ReportReasonTypeId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_ReportReasonType_ReportReasonTypeId1",
                table: "Report");

            migrationBuilder.DropTable(
                name: "ReportReasonTypeReportableEntityType");

            migrationBuilder.DropIndex(
                name: "IX_Report_ReportReasonTypeId",
                table: "Report");

            migrationBuilder.DropIndex(
                name: "IX_Report_ReportReasonTypeId1",
                table: "Report");

            migrationBuilder.DeleteData(
                table: "ReportReasonType",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ReportReasonType",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ReportReasonType",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ReportReasonType",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ReportReasonType",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ReportReasonType",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ReportReasonType",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ReportReasonType",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ReportReasonType",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ReportReasonType",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ReportReasonType",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ReportReasonType",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ReportableEntityType",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ReportableEntityType",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "ReportReasonTypeId",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "ReportReasonTypeId1",
                table: "Report");

            migrationBuilder.CreateTable(
                name: "ReportReportReasonType",
                columns: table => new
                {
                    ReportReasonTypeId = table.Column<int>(type: "integer", nullable: false),
                    ReportsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportReportReasonType", x => new { x.ReportReasonTypeId, x.ReportsId });
                    table.ForeignKey(
                        name: "FK_ReportReportReasonType_ReportReasonType_ReportReasonTypeId",
                        column: x => x.ReportReasonTypeId,
                        principalTable: "ReportReasonType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportReportReasonType_Report_ReportsId",
                        column: x => x.ReportsId,
                        principalTable: "Report",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportReportReasonType_ReportsId",
                table: "ReportReportReasonType",
                column: "ReportsId");
        }
    }
}
