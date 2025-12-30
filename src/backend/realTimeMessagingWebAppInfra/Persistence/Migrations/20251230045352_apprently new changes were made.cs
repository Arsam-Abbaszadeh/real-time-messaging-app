using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace realTimeMessagingWebApp.Migrations
{
    /// <inheritdoc />
    public partial class apprentlynewchangesweremade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AttachmentType",
                table: "MessageAttachments",
                newName: "AttachmentObjectKey");

            migrationBuilder.RenameColumn(
                name: "AttachmentKey",
                table: "MessageAttachments",
                newName: "AttachmentMimeType");

            migrationBuilder.AddColumn<string>(
                name: "AttachmentFileExtension",
                table: "MessageAttachments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAtUtc",
                table: "MessageAttachments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentFileExtension",
                table: "MessageAttachments");

            migrationBuilder.DropColumn(
                name: "UploadedAtUtc",
                table: "MessageAttachments");

            migrationBuilder.RenameColumn(
                name: "AttachmentObjectKey",
                table: "MessageAttachments",
                newName: "AttachmentType");

            migrationBuilder.RenameColumn(
                name: "AttachmentMimeType",
                table: "MessageAttachments",
                newName: "AttachmentKey");
        }
    }
}
