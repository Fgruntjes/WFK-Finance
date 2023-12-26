using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DataMigrations.src.Migrations
{
    /// <inheritdoc />
    public partial class InstitutionAccountTransactions3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstitutionAccountTransactions_InstitutionConnectionAccounts_AccountId",
                table: "InstitutionAccountTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_InstitutionConnectionAccounts_InstitutionConnections_InstitutionConnectionId",
                table: "InstitutionConnectionAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InstitutionConnectionAccounts",
                table: "InstitutionConnectionAccounts");

            migrationBuilder.RenameTable(
                name: "InstitutionConnectionAccounts",
                newName: "InstitutionAccounts");

            migrationBuilder.RenameIndex(
                name: "IX_InstitutionConnectionAccounts_InstitutionConnectionId",
                table: "InstitutionAccounts",
                newName: "IX_InstitutionAccounts_InstitutionConnectionId");

            migrationBuilder.RenameIndex(
                name: "IX_InstitutionConnectionAccounts_ExternalId_InstitutionConnectionId",
                table: "InstitutionAccounts",
                newName: "IX_InstitutionAccounts_ExternalId_InstitutionConnectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstitutionAccounts",
                table: "InstitutionAccounts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InstitutionAccounts_InstitutionConnections_InstitutionConnectionId",
                table: "InstitutionAccounts",
                column: "InstitutionConnectionId",
                principalTable: "InstitutionConnections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InstitutionAccountTransactions_InstitutionAccounts_AccountId",
                table: "InstitutionAccountTransactions",
                column: "AccountId",
                principalTable: "InstitutionAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstitutionAccounts_InstitutionConnections_InstitutionConnectionId",
                table: "InstitutionAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_InstitutionAccountTransactions_InstitutionAccounts_AccountId",
                table: "InstitutionAccountTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InstitutionAccounts",
                table: "InstitutionAccounts");

            migrationBuilder.RenameTable(
                name: "InstitutionAccounts",
                newName: "InstitutionConnectionAccounts");

            migrationBuilder.RenameIndex(
                name: "IX_InstitutionAccounts_InstitutionConnectionId",
                table: "InstitutionConnectionAccounts",
                newName: "IX_InstitutionConnectionAccounts_InstitutionConnectionId");

            migrationBuilder.RenameIndex(
                name: "IX_InstitutionAccounts_ExternalId_InstitutionConnectionId",
                table: "InstitutionConnectionAccounts",
                newName: "IX_InstitutionConnectionAccounts_ExternalId_InstitutionConnectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InstitutionConnectionAccounts",
                table: "InstitutionConnectionAccounts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InstitutionAccountTransactions_InstitutionConnectionAccounts_AccountId",
                table: "InstitutionAccountTransactions",
                column: "AccountId",
                principalTable: "InstitutionConnectionAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InstitutionConnectionAccounts_InstitutionConnections_InstitutionConnectionId",
                table: "InstitutionConnectionAccounts",
                column: "InstitutionConnectionId",
                principalTable: "InstitutionConnections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
