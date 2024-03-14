using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceBotBase.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RenameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBalanceEntity",
                table: "UserBalanceEntity");

            migrationBuilder.RenameTable(
                name: "UserBalanceEntity",
                newName: "UserBalance");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBalance",
                table: "UserBalance",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBalance",
                table: "UserBalance");

            migrationBuilder.RenameTable(
                name: "UserBalance",
                newName: "UserBalanceEntity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBalanceEntity",
                table: "UserBalanceEntity",
                column: "UserId");
        }
    }
}
