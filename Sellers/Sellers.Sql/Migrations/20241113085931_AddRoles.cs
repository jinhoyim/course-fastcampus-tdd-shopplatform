using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sellers.Migrations
{
    /// <inheritdoc />
    public partial class AddRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    UserSequence = table.Column<long>(type: "bigint", nullable: false),
                    ShopId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => new { x.UserSequence, x.ShopId, x.RoleName });
                    table.ForeignKey(
                        name: "FK_Roles_Users_UserSequence",
                        column: x => x.UserSequence,
                        principalTable: "Users",
                        principalColumn: "Sequence",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
