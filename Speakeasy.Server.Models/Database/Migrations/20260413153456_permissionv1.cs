using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Speakeasy.Server.Models.Database.Migrations
{
    /// <inheritdoc />
    public partial class permissionv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupRoleClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupRoleClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GroupRoleClaims_GroupRoles_GroupRoleId",
                        column: x => x.GroupRoleId,
                        principalTable: "GroupRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupRolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupRolePermissions_GroupRoles_GroupRoleId",
                        column: x => x.GroupRoleId,
                        principalTable: "GroupRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoleClaims_GroupRoleId",
                table: "GroupRoleClaims",
                column: "GroupRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRoleClaims_UserId",
                table: "GroupRoleClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRolePermissions_GroupRoleId",
                table: "GroupRolePermissions",
                column: "GroupRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupRoleClaims");

            migrationBuilder.DropTable(
                name: "GroupRolePermissions");
        }
    }
}
