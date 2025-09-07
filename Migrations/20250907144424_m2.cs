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
            migrationBuilder.DropColumn(
                name: "key_id",
                table: "issues");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "key_id",
                table: "issues",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
