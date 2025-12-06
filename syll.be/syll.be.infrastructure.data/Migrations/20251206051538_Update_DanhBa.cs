using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace syll.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_DanhBa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoaiDanhBa",
                schema: "core",
                table: "DanhBa",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiDanhBa",
                schema: "core",
                table: "DanhBa");
        }
    }
}
