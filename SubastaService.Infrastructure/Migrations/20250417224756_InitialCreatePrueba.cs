using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubastaService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreatePrueba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_subasta",
                table: "subasta");

            migrationBuilder.RenameTable(
                name: "subasta",
                newName: "subastaPrueba");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subastaPrueba",
                table: "subastaPrueba",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_subastaPrueba",
                table: "subastaPrueba");

            migrationBuilder.RenameTable(
                name: "subastaPrueba",
                newName: "subasta");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subasta",
                table: "subasta",
                column: "Id");
        }
    }
}
