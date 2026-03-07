using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace realTimeMessagingWebApp.Migrations
{
    /// <inheritdoc />
    public partial class updatedatamodelafterawhile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatId",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId_SequenceNumber",
                table: "Messages",
                columns: new[] { "ChatId", "SequenceNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatId_SequenceNumber",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");
        }
    }
}
