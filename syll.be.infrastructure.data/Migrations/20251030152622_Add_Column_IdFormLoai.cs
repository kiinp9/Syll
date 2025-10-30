using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace syll.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Column_IdFormLoai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdFormLoai",
                schema: "core",
                table: "FormTruongData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdFormLoai",
                schema: "core",
                table: "FormDauMuc",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdFormLoai",
                schema: "core",
                table: "FormTruongData");

            migrationBuilder.DropColumn(
                name: "IdFormLoai",
                schema: "core",
                table: "FormDauMuc");
        }
    }
}
