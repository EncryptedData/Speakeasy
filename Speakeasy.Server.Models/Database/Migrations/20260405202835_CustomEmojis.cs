using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Speakeasy.Server.Models.Migrations
{
    /// <inheritdoc />
    public partial class CustomEmojis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomEmojis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    ImageId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomEmojis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomEmojis_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomEmojis_Files_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomEmojis_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomEmojis_AuthorId",
                table: "CustomEmojis",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomEmojis_GroupId",
                table: "CustomEmojis",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomEmojis_ImageId",
                table: "CustomEmojis",
                column: "ImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomEmojis");
        }
    }
}
