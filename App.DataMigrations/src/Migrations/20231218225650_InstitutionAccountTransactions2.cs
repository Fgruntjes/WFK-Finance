using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DataMigrations.src.Migrations
{
    /// <inheritdoc />
    public partial class InstitutionAccountTransactions2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InstitutionAccountTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UnstructuredInformation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CounterPartyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CounterPartyAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstitutionAccountTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstitutionAccountTransactions_InstitutionConnectionAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "InstitutionConnectionAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstitutionAccountTransactions_AccountId",
                table: "InstitutionAccountTransactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InstitutionAccountTransactions_ExternalId_AccountId",
                table: "InstitutionAccountTransactions",
                columns: new[] { "ExternalId", "AccountId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstitutionAccountTransactions");
        }
    }
}
