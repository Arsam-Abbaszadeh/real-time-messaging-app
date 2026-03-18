using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace realTimeMessagingWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingChatUserForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Chats_ChatAdminId",
                table: "Chats",
                column: "ChatAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ChatCreatorId",
                table: "Chats",
                column: "ChatCreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_ChatAdminId",
                table: "Chats",
                column: "ChatAdminId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_ChatCreatorId",
                table: "Chats",
                column: "ChatCreatorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_ChatAdminId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_ChatCreatorId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_ChatAdminId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_ChatCreatorId",
                table: "Chats");
        }
    }
}
