using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_System.Migrations
{
    /// <inheritdoc />
    public partial class m7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_team_users_user_id",
                table: "team");

            migrationBuilder.DropPrimaryKey(
                name: "pk_team",
                table: "team");

            migrationBuilder.RenameTable(
                name: "team",
                newName: "teams");

            migrationBuilder.RenameIndex(
                name: "ix_team_user_id",
                table: "teams",
                newName: "ix_teams_user_id");

            migrationBuilder.AddColumn<int>(
                name: "team_id",
                table: "issues",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_teams",
                table: "teams",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_issues_team_id",
                table: "issues",
                column: "team_id");

            migrationBuilder.AddForeignKey(
                name: "fk_issues_teams_team_id",
                table: "issues",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_teams_users_user_id",
                table: "teams",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_issues_teams_team_id",
                table: "issues");

            migrationBuilder.DropForeignKey(
                name: "fk_teams_users_user_id",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "ix_issues_team_id",
                table: "issues");

            migrationBuilder.DropPrimaryKey(
                name: "pk_teams",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "team_id",
                table: "issues");

            migrationBuilder.RenameTable(
                name: "teams",
                newName: "team");

            migrationBuilder.RenameIndex(
                name: "ix_teams_user_id",
                table: "team",
                newName: "ix_team_user_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_team",
                table: "team",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_team_users_user_id",
                table: "team",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
