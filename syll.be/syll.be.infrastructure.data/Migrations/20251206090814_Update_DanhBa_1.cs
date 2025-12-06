using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace syll.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_DanhBa_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChienDichDanhBa",
                schema: "core");

            migrationBuilder.DropColumn(
                name: "LoaiDanhBa",
                schema: "core",
                table: "DanhBa");

            migrationBuilder.CreateTable(
                name: "ChienDichToChuc",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdChienDich = table.Column<int>(type: "int", nullable: false),
                    IdToChuc = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChienDichToChuc", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChienDichToChuc",
                schema: "core",
                table: "ChienDichToChuc",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChienDichToChuc",
                schema: "core");

            migrationBuilder.AddColumn<int>(
                name: "LoaiDanhBa",
                schema: "core",
                table: "DanhBa",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChienDichDanhBa",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdChienDich = table.Column<int>(type: "int", nullable: false),
                    IdDanhBa = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChienDichDanhBa", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChienDichDanhBa",
                schema: "core",
                table: "ChienDichDanhBa",
                column: "Id");
        }
    }
}
