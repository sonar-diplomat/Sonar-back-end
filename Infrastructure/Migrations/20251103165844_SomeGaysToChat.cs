using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SomeGaysToChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser_AspNetUsers_UsersId",
                table: "ChatUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser_Chat_ChatsId",
                table: "ChatUser");

            migrationBuilder.DropTable(
                name: "MessageUser");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "ChatUser",
                newName: "ChatsWhereAdminId");

            migrationBuilder.RenameColumn(
                name: "ChatsId",
                table: "ChatUser",
                newName: "AdminsId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatUser_UsersId",
                table: "ChatUser",
                newName: "IX_ChatUser_ChatsWhereAdminId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Message",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SenderId",
                table: "Message",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "Chat",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChatUser1",
                columns: table => new
                {
                    ChatsWhereMemberId = table.Column<int>(type: "integer", nullable: false),
                    MembersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatUser1", x => new { x.ChatsWhereMemberId, x.MembersId });
                    table.ForeignKey(
                        name: "FK_ChatUser1_AspNetUsers_MembersId",
                        column: x => x.MembersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatUser1_Chat_ChatsWhereMemberId",
                        column: x => x.ChatsWhereMemberId,
                        principalTable: "Chat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Message_SenderId",
                table: "Message",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_CreatorId",
                table: "Chat",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUser1_MembersId",
                table: "ChatUser1",
                column: "MembersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_AspNetUsers_CreatorId",
                table: "Chat",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUser_AspNetUsers_AdminsId",
                table: "ChatUser",
                column: "AdminsId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUser_Chat_ChatsWhereAdminId",
                table: "ChatUser",
                column: "ChatsWhereAdminId",
                principalTable: "Chat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_AspNetUsers_SenderId",
                table: "Message",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_AspNetUsers_CreatorId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser_AspNetUsers_AdminsId",
                table: "ChatUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser_Chat_ChatsWhereAdminId",
                table: "ChatUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_AspNetUsers_SenderId",
                table: "Message");

            migrationBuilder.DropTable(
                name: "ChatUser1");

            migrationBuilder.DropIndex(
                name: "IX_Message_SenderId",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Chat_CreatorId",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Chat");

            migrationBuilder.RenameColumn(
                name: "ChatsWhereAdminId",
                table: "ChatUser",
                newName: "UsersId");

            migrationBuilder.RenameColumn(
                name: "AdminsId",
                table: "ChatUser",
                newName: "ChatsId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatUser_ChatsWhereAdminId",
                table: "ChatUser",
                newName: "IX_ChatUser_UsersId");

            migrationBuilder.CreateTable(
                name: "MessageUser",
                columns: table => new
                {
                    MessagesId = table.Column<int>(type: "integer", nullable: false),
                    usersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageUser", x => new { x.MessagesId, x.usersId });
                    table.ForeignKey(
                        name: "FK_MessageUser_AspNetUsers_usersId",
                        column: x => x.usersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageUser_Message_MessagesId",
                        column: x => x.MessagesId,
                        principalTable: "Message",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageUser_usersId",
                table: "MessageUser",
                column: "usersId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUser_AspNetUsers_UsersId",
                table: "ChatUser",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUser_Chat_ChatsId",
                table: "ChatUser",
                column: "ChatsId",
                principalTable: "Chat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
