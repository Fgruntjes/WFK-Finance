using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DataMigrations.src.Migrations
{
    /// <inheritdoc />
    public partial class TransactionCategorySortOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "TransactionCategory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "TransactionCategory");
        }
    }
}
