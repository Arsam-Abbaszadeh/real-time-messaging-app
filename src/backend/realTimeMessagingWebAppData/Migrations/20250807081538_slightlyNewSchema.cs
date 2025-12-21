using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace realTimeMessagingWebApp.Migrations
{
    /// <inheritdoc />
    public partial class slightlyNewSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userName",
                table: "Users",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "passwordHash",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.AddColumn<DateTime>(
                name: "SignUpDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignUpDate",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "userName");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "passwordHash");
        }
    }
}
