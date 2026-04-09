using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Speakeasy.Server.Models.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageReactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageReaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomEmojiId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmojiChar = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageReaction_CustomEmojis_CustomEmojiId",
                        column: x => x.CustomEmojiId,
                        principalTable: "CustomEmojis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MessageReaction_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageReactionUser",
                columns: table => new
                {
                    MessageReactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReactorsId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReactionUser", x => new { x.MessageReactionId, x.ReactorsId });
                    table.ForeignKey(
                        name: "FK_MessageReactionUser_AspNetUsers_ReactorsId",
                        column: x => x.ReactorsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageReactionUser_MessageReaction_MessageReactionId",
                        column: x => x.MessageReactionId,
                        principalTable: "MessageReaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageReaction_CustomEmojiId",
                table: "MessageReaction",
                column: "CustomEmojiId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReaction_MessageId",
                table: "MessageReaction",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactionUser_ReactorsId",
                table: "MessageReactionUser",
                column: "ReactorsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageReactionUser");

            migrationBuilder.DropTable(
                name: "MessageReaction");
        }
    }
}
