using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlWork7.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookLoans_Users_EmployeeId",
                table: "BookLoans");

            migrationBuilder.DropIndex(
                name: "IX_BookLoans_EmployeeId",
                table: "BookLoans");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "BookLoans");

            migrationBuilder.CreateIndex(
                name: "IX_BookLoans_UserId",
                table: "BookLoans",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookLoans_Users_UserId",
                table: "BookLoans",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookLoans_Users_UserId",
                table: "BookLoans");

            migrationBuilder.DropIndex(
                name: "IX_BookLoans_UserId",
                table: "BookLoans");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "BookLoans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BookLoans_EmployeeId",
                table: "BookLoans",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookLoans_Users_EmployeeId",
                table: "BookLoans",
                column: "EmployeeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
