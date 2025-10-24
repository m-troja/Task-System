using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_System.Migrations
{
    /// <inheritdoc />
    public partial class m2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "disabled", "email", "first_name", "last_name", "password", "refresh_token", "salt" },
                values: new object[] { -1, true, "system.user@tasksystem.com", "System User", "System User", "Password", null, new byte[] { 87, 32, 87, 61, 195, 168, 195, 148, 85, 195, 140, 45, 194, 167, 195, 130, 78, 195, 175, 94, 195, 142, 88 } });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: -1);
        }
    }
}
