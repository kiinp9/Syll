using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace syll.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Form_Config_Fe_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Class",
                schema: "core",
                table: "Table",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Style",
                schema: "core",
                table: "Table",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Class",
                schema: "core",
                table: "Row",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Style",
                schema: "core",
                table: "Row",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Class",
                schema: "core",
                table: "Layout",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Style",
                schema: "core",
                table: "Layout",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Class",
                schema: "core",
                table: "Item",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Style",
                schema: "core",
                table: "Item",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Class",
                schema: "core",
                table: "DropDown",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Style",
                schema: "core",
                table: "DropDown",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Class",
                schema: "core",
                table: "Block",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Style",
                schema: "core",
                table: "Block",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Class",
                schema: "core",
                table: "Table");

            migrationBuilder.DropColumn(
                name: "Style",
                schema: "core",
                table: "Table");

            migrationBuilder.DropColumn(
                name: "Class",
                schema: "core",
                table: "Row");

            migrationBuilder.DropColumn(
                name: "Style",
                schema: "core",
                table: "Row");

            migrationBuilder.DropColumn(
                name: "Class",
                schema: "core",
                table: "Layout");

            migrationBuilder.DropColumn(
                name: "Style",
                schema: "core",
                table: "Layout");

            migrationBuilder.DropColumn(
                name: "Class",
                schema: "core",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "Style",
                schema: "core",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "Class",
                schema: "core",
                table: "DropDown");

            migrationBuilder.DropColumn(
                name: "Style",
                schema: "core",
                table: "DropDown");

            migrationBuilder.DropColumn(
                name: "Class",
                schema: "core",
                table: "Block");

            migrationBuilder.DropColumn(
                name: "Style",
                schema: "core",
                table: "Block");
        }
    }
}
