using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubastaService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InicialSubasta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Subasta",
                table: "Subasta");

            migrationBuilder.RenameTable(
                name: "Subasta",
                newName: "Subastas");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subastas",
                table: "Subastas",
                column: "IdSubasta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Subastas",
                table: "Subastas");

            migrationBuilder.RenameTable(
                name: "Subastas",
                newName: "Subasta");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subasta",
                table: "Subasta",
                column: "IdSubasta");
        }
    }
}
