using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace syll.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_ChienDich_FormLoai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShow",
                schema: "core",
                table: "FormLoai");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "core",
                table: "FormLoai");

            migrationBuilder.AddColumn<bool>(
                name: "IsShow",
                schema: "core",
                table: "ChienDichFormLoai",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "core",
                table: "ChienDichFormLoai",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShow",
                schema: "core",
                table: "ChienDichFormLoai");

            migrationBuilder.DropColumn(
                name: "Order",
                schema: "core",
                table: "ChienDichFormLoai");

            migrationBuilder.AddColumn<bool>(
                name: "IsShow",
                schema: "core",
                table: "FormLoai",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                schema: "core",
                table: "FormLoai",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
