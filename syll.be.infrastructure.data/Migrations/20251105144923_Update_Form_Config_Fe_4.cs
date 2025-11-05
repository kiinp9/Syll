using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace syll.be.infrastructure.data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Form_Config_Fe_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                schema: "core",
                table: "FormData",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                schema: "core",
                table: "FormData",
                newName: "ModifiedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                schema: "core",
                table: "FormData",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                schema: "core",
                table: "FormData",
                newName: "UpdatedBy");
        }
    }
}
