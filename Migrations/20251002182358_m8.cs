using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_System.Migrations
{
    /// <inheritdoc />
    public partial class m8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_teams_users_user_id",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "ix_teams_user_id",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "teams");

            migrationBuilder.CreateTable(
                name: "TeamUsers",
                columns: table => new
                {
                    team_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_team_users", x => new { x.team_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_team_users_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_team_users_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_team_users_user_id",
                table: "TeamUsers",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamUsers");

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "teams",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_teams_user_id",
                table: "teams",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_teams_users_user_id",
                table: "teams",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
