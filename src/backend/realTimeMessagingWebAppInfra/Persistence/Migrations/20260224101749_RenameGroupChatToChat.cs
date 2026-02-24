using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace realTimeMessagingWebApp.Migrations
{
    /// <inheritdoc />
    public partial class RenameGroupChatToChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys that reference the old table/column names
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_GroupChats_GroupChatId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupChatConnectors_GroupChats_GroupChatId",
                table: "GroupChatConnectors");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupChatConnectors_Users_UserId",
                table: "GroupChatConnectors");

            // Rename tables
            migrationBuilder.RenameTable(
                name: "GroupChats",
                newName: "Chats");

            migrationBuilder.RenameTable(
                name: "GroupChatConnectors",
                newName: "ChatConnectors");

            // Rename columns in Chats (formerly GroupChats)
            migrationBuilder.RenameColumn(
                name: "GroupChatId",
                table: "Chats",
                newName: "ChatId");

            migrationBuilder.RenameColumn(
                name: "GroupChatName",
                table: "Chats",
                newName: "ChatName");

            migrationBuilder.RenameColumn(
                name: "GroupChatCreatorId",
                table: "Chats",
                newName: "ChatCreatorId");

            migrationBuilder.RenameColumn(
                name: "GroupChatAdminId",
                table: "Chats",
                newName: "ChatAdminId");

            // Rename ChatType column to ChatKind
            migrationBuilder.RenameColumn(
                name: "ChatType",
                table: "Chats",
                newName: "ChatKind");

            // Rename columns in ChatConnectors (formerly GroupChatConnectors)
            migrationBuilder.RenameColumn(
                name: "GroupChatConnectorId",
                table: "ChatConnectors",
                newName: "ChatConnectorId");

            migrationBuilder.RenameColumn(
                name: "GroupChatId",
                table: "ChatConnectors",
                newName: "ChatId");

            // Rename column in Messages
            migrationBuilder.RenameColumn(
                name: "GroupChatId",
                table: "Messages",
                newName: "ChatId");

            // Rename indexes
            migrationBuilder.RenameIndex(
                name: "PK_GroupChats",
                table: "Chats",
                newName: "PK_Chats");

            migrationBuilder.RenameIndex(
                name: "PK_GroupChatConnectors",
                table: "ChatConnectors",
                newName: "PK_ChatConnectors");

            migrationBuilder.RenameIndex(
                name: "IX_GroupChatConnectors_GroupChatId",
                table: "ChatConnectors",
                newName: "IX_ChatConnectors_ChatId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupChatConnectors_UserId",
                table: "ChatConnectors",
                newName: "IX_ChatConnectors_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_GroupChatId",
                table: "Messages",
                newName: "IX_Messages_ChatId");

            // Re-add foreign keys with new names
            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatConnectors_Chats_ChatId",
                table: "ChatConnectors",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatConnectors_Users_UserId",
                table: "ChatConnectors",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatConnectors_Chats_ChatId",
                table: "ChatConnectors");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatConnectors_Users_UserId",
                table: "ChatConnectors");

            // Rename columns in Messages
            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "Messages",
                newName: "GroupChatId");

            // Rename columns in ChatConnectors back
            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "ChatConnectors",
                newName: "GroupChatId");

            migrationBuilder.RenameColumn(
                name: "ChatConnectorId",
                table: "ChatConnectors",
                newName: "GroupChatConnectorId");

            // Rename columns in Chats back
            migrationBuilder.RenameColumn(
                name: "ChatKind",
                table: "Chats",
                newName: "ChatType");

            migrationBuilder.RenameColumn(
                name: "ChatAdminId",
                table: "Chats",
                newName: "GroupChatAdminId");

            migrationBuilder.RenameColumn(
                name: "ChatCreatorId",
                table: "Chats",
                newName: "GroupChatCreatorId");

            migrationBuilder.RenameColumn(
                name: "ChatName",
                table: "Chats",
                newName: "GroupChatName");

            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "Chats",
                newName: "GroupChatId");

            // Rename indexes back
            migrationBuilder.RenameIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                newName: "IX_Messages_GroupChatId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatConnectors_UserId",
                table: "ChatConnectors",
                newName: "IX_GroupChatConnectors_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatConnectors_ChatId",
                table: "ChatConnectors",
                newName: "IX_GroupChatConnectors_GroupChatId");

            migrationBuilder.RenameIndex(
                name: "PK_ChatConnectors",
                table: "ChatConnectors",
                newName: "PK_GroupChatConnectors");

            migrationBuilder.RenameIndex(
                name: "PK_Chats",
                table: "Chats",
                newName: "PK_GroupChats");

            // Rename tables back
            migrationBuilder.RenameTable(
                name: "ChatConnectors",
                newName: "GroupChatConnectors");

            migrationBuilder.RenameTable(
                name: "Chats",
                newName: "GroupChats");

            // Re-add foreign keys with old names
            migrationBuilder.AddForeignKey(
                name: "FK_Messages_GroupChats_GroupChatId",
                table: "Messages",
                column: "GroupChatId",
                principalTable: "GroupChats",
                principalColumn: "GroupChatId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupChatConnectors_GroupChats_GroupChatId",
                table: "GroupChatConnectors",
                column: "GroupChatId",
                principalTable: "GroupChats",
                principalColumn: "GroupChatId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupChatConnectors_Users_UserId",
                table: "GroupChatConnectors",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
