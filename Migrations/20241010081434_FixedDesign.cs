using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class FixedDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Attribute",
                table: "ProductAttributes",
                newName: "Value");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "ProductAttributes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Key",
                table: "ProductAttributes");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "ProductAttributes",
                newName: "Attribute");
        }
    }
}
