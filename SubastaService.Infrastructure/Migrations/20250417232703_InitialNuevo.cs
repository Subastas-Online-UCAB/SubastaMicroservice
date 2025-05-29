using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubastaService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialNuevo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_subastaPrueba",
                table: "subastaPrueba");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "subastaPrueba");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "subastaPrueba");

            migrationBuilder.RenameTable(
                name: "subastaPrueba",
                newName: "Subasta");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Subasta",
                newName: "IdUsuario");

            migrationBuilder.AddColumn<Guid>(
                name: "IdSubasta",
                table: "Subasta",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CondicionParticipacion",
                table: "Subasta",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Subasta",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duracion",
                table: "Subasta",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Subasta",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicio",
                table: "Subasta",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "IdProducto",
                table: "Subasta",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "IncrementoMinimo",
                table: "Subasta",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioBase",
                table: "Subasta",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioReserva",
                table: "Subasta",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoSubasta",
                table: "Subasta",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subasta",
                table: "Subasta",
                column: "IdSubasta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Subasta",
                table: "Subasta");

            migrationBuilder.DropColumn(
                name: "IdSubasta",
                table: "Subasta");

            migrationBuilder.DropColumn(
                name: "CondicionParticipacion",
                table: "Subasta");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Subasta");

            migrationBuilder.DropColumn(
                name: "Duracion",
                table: "Subasta");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Subasta");

            migrationBuilder.DropColumn(
                name: "FechaInicio",
                table: "Subasta");

            migrationBuilder.DropColumn(
                name: "IdProducto",
                table: "Subasta");

            migrationBuilder.DropColumn(
                name: "IncrementoMinimo",
                table: "Subasta");

            migrationBuilder.DropColumn(
                name: "PrecioBase",
                table: "Subasta");

            migrationBuilder.DropColumn(
                name: "PrecioReserva",
                table: "Subasta");

            migrationBuilder.DropColumn(
                name: "TipoSubasta",
                table: "Subasta");

            migrationBuilder.RenameTable(
                name: "Subasta",
                newName: "subastaPrueba");

            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "subastaPrueba",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "subastaPrueba",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "subastaPrueba",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subastaPrueba",
                table: "subastaPrueba",
                column: "Id");
        }
    }
}
