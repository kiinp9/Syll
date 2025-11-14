using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace syll.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_FormTruongData_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTruongCustom",
                schema: "core",
                table: "FormTruongData");

            migrationBuilder.AddColumn<int>(
                name: "BlockTruongNhanBan",
                schema: "core",
                table: "FormTruongData",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockTruongNhanBan",
                schema: "core",
                table: "FormTruongData");

            migrationBuilder.AddColumn<bool>(
                name: "IsTruongCustom",
                schema: "core",
                table: "FormTruongData",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
