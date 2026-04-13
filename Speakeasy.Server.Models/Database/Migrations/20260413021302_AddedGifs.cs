using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Speakeasy.Server.Models.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedGifs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gifs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    StoredFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gifs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gifs_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Gifs_Files_StoredFileId",
                        column: x => x.StoredFileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Gifs_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gifs_AuthorId",
                table: "Gifs",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Gifs_GroupId",
                table: "Gifs",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Gifs_StoredFileId",
                table: "Gifs",
                column: "StoredFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gifs");
        }
    }
}
