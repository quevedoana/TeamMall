using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeaMall.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCambiosPublicacionesNotificaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAccesoPublicacion",
                table: "notificacion",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaAccesoPublicacion",
                table: "notificacion");
        }
    }
}