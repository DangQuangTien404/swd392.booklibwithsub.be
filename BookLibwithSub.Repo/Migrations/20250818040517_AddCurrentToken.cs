using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLibwithSub.Repo.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentToken",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentToken",
                table: "Users");
        }
    }
}
