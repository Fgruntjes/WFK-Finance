using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DataMigrations.src.Migrations
{
    /// <inheritdoc />
    public partial class InstitutionAccountTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Organisations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ImportStatus",
                table: "InstitutionConnectionAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastImport",
                table: "InstitutionConnectionAccounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastImportError",
                table: "InstitutionConnectionAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_Slug",
                table: "Organisations",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organisations_Slug",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "ImportStatus",
                table: "InstitutionConnectionAccounts");

            migrationBuilder.DropColumn(
                name: "LastImport",
                table: "InstitutionConnectionAccounts");

            migrationBuilder.DropColumn(
                name: "LastImportError",
                table: "InstitutionConnectionAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Organisations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
