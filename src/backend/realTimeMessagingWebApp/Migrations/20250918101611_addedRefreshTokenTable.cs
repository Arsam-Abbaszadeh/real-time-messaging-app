using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace realTimeMessagingWebApp.Migrations
{
    /// <inheritdoc />
    public partial class addedRefreshTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupChatAdmin",
                table: "GroupChats");

            migrationBuilder.RenameColumn(
                name: "GroupChatCreator",
                table: "GroupChats",
                newName: "GroupChatCreatorId");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupChatAdminId",
                table: "GroupChats",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpirationUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    isValid = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "GroupChatAdminId",
                table: "GroupChats");

            migrationBuilder.RenameColumn(
                name: "GroupChatCreatorId",
                table: "GroupChats",
                newName: "GroupChatCreator");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupChatAdmin",
                table: "GroupChats",
                type: "uuid",
                nullable: true);
        }
    }
}
