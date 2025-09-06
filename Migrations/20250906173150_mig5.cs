using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_System.Migrations
{
    /// <inheritdoc />
    public partial class mig5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_issue_users_assignee_id",
                table: "issue");

            migrationBuilder.DropForeignKey(
                name: "fk_issue_users_author_id",
                table: "issue");

            migrationBuilder.DropPrimaryKey(
                name: "pk_issue",
                table: "issue");

            migrationBuilder.RenameTable(
                name: "issue",
                newName: "issues");

            migrationBuilder.RenameIndex(
                name: "ix_issue_author_id",
                table: "issues",
                newName: "ix_issues_author_id");

            migrationBuilder.RenameIndex(
                name: "ix_issue_assignee_id",
                table: "issues",
                newName: "ix_issues_assignee_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_issues",
                table: "issues",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_issues_users_assignee_id",
                table: "issues",
                column: "assignee_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_issues_users_author_id",
                table: "issues",
                column: "author_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_issues_users_assignee_id",
                table: "issues");

            migrationBuilder.DropForeignKey(
                name: "fk_issues_users_author_id",
                table: "issues");

            migrationBuilder.DropPrimaryKey(
                name: "pk_issues",
                table: "issues");

            migrationBuilder.RenameTable(
                name: "issues",
                newName: "issue");

            migrationBuilder.RenameIndex(
                name: "ix_issues_author_id",
                table: "issue",
                newName: "ix_issue_author_id");

            migrationBuilder.RenameIndex(
                name: "ix_issues_assignee_id",
                table: "issue",
                newName: "ix_issue_assignee_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_issue",
                table: "issue",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_issue_users_assignee_id",
                table: "issue",
                column: "assignee_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_issue_users_author_id",
                table: "issue",
                column: "author_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
