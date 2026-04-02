using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace realTimeMessagingWebApp.Migrations
{
    /// <inheritdoc />
    public partial class addisValidsqldefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "isValid",
                table: "RefreshTokens",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "isValid",
                table: "RefreshTokens",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: null); // even though migrations may suggest otherwise, the databse says so
        }
    }
}
