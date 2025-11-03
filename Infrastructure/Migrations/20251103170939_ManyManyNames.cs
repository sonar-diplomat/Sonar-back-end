using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ManyManyNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser_AspNetUsers_AdminsId",
                table: "ChatUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser_Chat_ChatsWhereAdminId",
                table: "ChatUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser1_AspNetUsers_MembersId",
                table: "ChatUser1");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser1_Chat_ChatsWhereMemberId",
                table: "ChatUser1");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatUser1",
                table: "ChatUser1");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatUser",
                table: "ChatUser");

            migrationBuilder.RenameTable(
                name: "ChatUser1",
                newName: "ChatMembers");

            migrationBuilder.RenameTable(
                name: "ChatUser",
                newName: "ChatAdmins");

            migrationBuilder.RenameIndex(
                name: "IX_ChatUser1_MembersId",
                table: "ChatMembers",
                newName: "IX_ChatMembers_MembersId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatUser_ChatsWhereAdminId",
                table: "ChatAdmins",
                newName: "IX_ChatAdmins_ChatsWhereAdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMembers",
                table: "ChatMembers",
                columns: new[] { "ChatsWhereMemberId", "MembersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatAdmins",
                table: "ChatAdmins",
                columns: new[] { "AdminsId", "ChatsWhereAdminId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChatAdmins_AspNetUsers_AdminsId",
                table: "ChatAdmins",
                column: "AdminsId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatAdmins_Chat_ChatsWhereAdminId",
                table: "ChatAdmins",
                column: "ChatsWhereAdminId",
                principalTable: "Chat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMembers_AspNetUsers_MembersId",
                table: "ChatMembers",
                column: "MembersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMembers_Chat_ChatsWhereMemberId",
                table: "ChatMembers",
                column: "ChatsWhereMemberId",
                principalTable: "Chat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatAdmins_AspNetUsers_AdminsId",
                table: "ChatAdmins");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatAdmins_Chat_ChatsWhereAdminId",
                table: "ChatAdmins");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMembers_AspNetUsers_MembersId",
                table: "ChatMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMembers_Chat_ChatsWhereMemberId",
                table: "ChatMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMembers",
                table: "ChatMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatAdmins",
                table: "ChatAdmins");

            migrationBuilder.RenameTable(
                name: "ChatMembers",
                newName: "ChatUser1");

            migrationBuilder.RenameTable(
                name: "ChatAdmins",
                newName: "ChatUser");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMembers_MembersId",
                table: "ChatUser1",
                newName: "IX_ChatUser1_MembersId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatAdmins_ChatsWhereAdminId",
                table: "ChatUser",
                newName: "IX_ChatUser_ChatsWhereAdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatUser1",
                table: "ChatUser1",
                columns: new[] { "ChatsWhereMemberId", "MembersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatUser",
                table: "ChatUser",
                columns: new[] { "AdminsId", "ChatsWhereAdminId" });

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
                name: "FK_ChatUser1_AspNetUsers_MembersId",
                table: "ChatUser1",
                column: "MembersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUser1_Chat_ChatsWhereMemberId",
                table: "ChatUser1",
                column: "ChatsWhereMemberId",
                principalTable: "Chat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
