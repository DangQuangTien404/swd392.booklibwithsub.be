using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLibwithSub.Repo.Migrations
{
    /// <inheritdoc />
    public partial class BookLoanItemsCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanItems_Books_BookID",
                table: "LoanItems");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanItems_Books_BookID",
                table: "LoanItems",
                column: "BookID",
                principalTable: "Books",
                principalColumn: "BookID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanItems_Books_BookID",
                table: "LoanItems");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanItems_Books_BookID",
                table: "LoanItems",
                column: "BookID",
                principalTable: "Books",
                principalColumn: "BookID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
