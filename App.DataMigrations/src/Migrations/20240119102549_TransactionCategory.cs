using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DataMigrations.src.Migrations
{
    /// <inheritdoc />
    public partial class TransactionCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "InstitutionAccountTransactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TransactionCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganisationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Group = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionCategory_TransactionCategory_ParentId",
                        column: x => x.ParentId,
                        principalTable: "TransactionCategory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstitutionAccountTransactions_CategoryId",
                table: "InstitutionAccountTransactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCategory_ParentId",
                table: "TransactionCategory",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_InstitutionAccountTransactions_TransactionCategory_CategoryId",
                table: "InstitutionAccountTransactions",
                column: "CategoryId",
                principalTable: "TransactionCategory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstitutionAccountTransactions_TransactionCategory_CategoryId",
                table: "InstitutionAccountTransactions");

            migrationBuilder.DropTable(
                name: "TransactionCategory");

            migrationBuilder.DropIndex(
                name: "IX_InstitutionAccountTransactions_CategoryId",
                table: "InstitutionAccountTransactions");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "InstitutionAccountTransactions");
        }
    }
}
