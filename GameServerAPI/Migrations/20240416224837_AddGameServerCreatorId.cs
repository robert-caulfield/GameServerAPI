using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddGameServerCreatorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorUserId",
                table: "GameServers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Server", "SERVER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3", null, "Player", "PLAYER" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3059a4e1-29ed-44e3-a086-ffb486c174c4", "AQAAAAIAAYagAAAAEAFqOBazBRHGnLrfHwI2oQ80mCuuAV6q7HjQiavNbwZCtKD/ZJUI7K6FEisFMXrIPw==", "dcd27cbc-fd49-4297-8978-b215100df99e" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "2", 0, "8a5226c5-3934-4c7a-bd87-cb6ca2d9f9ea", "server@example.com", false, false, null, "SERVER@EXAMPLE.COM", "SERVER", "AQAAAAIAAYagAAAAEF6FN5U8nMuh8lWZk79n3o7gwVYoMopw5T2hRnqTXUTyQNVipybfr6VmBZPcRG9Hpg==", null, false, "fc2a0d7f-7739-491f-9b2e-4fb815411047", false, "server" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "2", "2" });

            migrationBuilder.CreateIndex(
                name: "IX_GameServers_CreatorUserId",
                table: "GameServers",
                column: "CreatorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameServers_AspNetUsers_CreatorUserId",
                table: "GameServers",
                column: "CreatorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameServers_AspNetUsers_CreatorUserId",
                table: "GameServers");

            migrationBuilder.DropIndex(
                name: "IX_GameServers_CreatorUserId",
                table: "GameServers");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "2" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "GameServers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Player", "PLAYER" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "41f21be2-863c-4e20-8521-a7466d5be70f", "AQAAAAIAAYagAAAAEN347acCVhUeEbFwjNJKdWY9+d3fIaGFan9DeBX5rE7N7a1OPMieA96WdJ3V5tMJCQ==", "988f3a75-0532-48e2-8dc2-78a802d29969" });
        }
    }
}
