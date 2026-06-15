using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FeePolymorphismUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Fees",
                newName: "BaseAmount");

            migrationBuilder.AddColumn<string>(
                name: "FeeDiscriminator",
                table: "Fees",
                type: "TEXT",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Fees",
                type: "TEXT",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PenaltyRate",
                table: "Fees",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeeDiscriminator",
                table: "Fees");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Fees");

            migrationBuilder.DropColumn(
                name: "PenaltyRate",
                table: "Fees");

            migrationBuilder.RenameColumn(
                name: "BaseAmount",
                table: "Fees",
                newName: "Amount");
        }
    }
}
