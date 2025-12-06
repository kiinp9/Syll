using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace syll.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Add_ChienDich_DanhBa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChienDichDanhBa",
                schema: "core",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdChienDich = table.Column<int>(type: "int", nullable: false),
                    IdDanhBa = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "getdate()"),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChienDichDanhBa",
                schema: "core");
        }
    }
}
