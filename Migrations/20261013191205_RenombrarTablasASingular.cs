using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeaMall.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarTablasASingular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inasistencias_Usuarios_IdUsuario",
                table: "Inasistencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Publicaciones_IdPublicacion",
                table: "Notificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Usuarios_IdUsuario",
                table: "Notificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Publicaciones_Usuarios_IdUsuario",
                table: "Publicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicacionesLocales_Locales_IdLocal",
                table: "PublicacionesLocales");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicacionesLocales_Publicaciones_IdPublicacion",
                table: "PublicacionesLocales");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Locales_IdLocal",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PublicacionesLocales",
                table: "PublicacionesLocales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Publicaciones",
                table: "Publicaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notificaciones",
                table: "Notificaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Locales",
                table: "Locales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inasistencias",
                table: "Inasistencias");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "usuario");

            migrationBuilder.RenameTable(
                name: "PublicacionesLocales",
                newName: "publicacionlocal");

            migrationBuilder.RenameTable(
                name: "Publicaciones",
                newName: "publicacion");

            migrationBuilder.RenameTable(
                name: "Notificaciones",
                newName: "notificacion");

            migrationBuilder.RenameTable(
                name: "Locales",
                newName: "local");

            migrationBuilder.RenameTable(
                name: "Inasistencias",
                newName: "inasistencia");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_IdLocal",
                table: "usuario",
                newName: "IX_usuario_IdLocal");

            migrationBuilder.RenameIndex(
                name: "IX_PublicacionesLocales_IdLocal",
                table: "publicacionlocal",
                newName: "IX_publicacionlocal_IdLocal");

            migrationBuilder.RenameIndex(
                name: "IX_Publicaciones_IdUsuario",
                table: "publicacion",
                newName: "IX_publicacion_IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Notificaciones_IdUsuario",
                table: "notificacion",
                newName: "IX_notificacion_IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_Notificaciones_IdPublicacion",
                table: "notificacion",
                newName: "IX_notificacion_IdPublicacion");

            migrationBuilder.RenameIndex(
                name: "IX_Inasistencias_IdUsuario",
                table: "inasistencia",
                newName: "IX_inasistencia_IdUsuario");

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuario",
                table: "usuario",
                column: "IdUsuario");

            migrationBuilder.AddPrimaryKey(
                name: "PK_publicacionlocal",
                table: "publicacionlocal",
                columns: new[] { "IdPublicacion", "IdLocal" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_publicacion",
                table: "publicacion",
                column: "IdPublicacion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_notificacion",
                table: "notificacion",
                column: "IdNotificacion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_local",
                table: "local",
                column: "IdLocal");

            migrationBuilder.AddPrimaryKey(
                name: "PK_inasistencia",
                table: "inasistencia",
                column: "IdInasistencia");

            migrationBuilder.AddForeignKey(
                name: "FK_inasistencia_usuario_IdUsuario",
                table: "inasistencia",
                column: "IdUsuario",
                principalTable: "usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_notificacion_publicacion_IdPublicacion",
                table: "notificacion",
                column: "IdPublicacion",
                principalTable: "publicacion",
                principalColumn: "IdPublicacion",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_notificacion_usuario_IdUsuario",
                table: "notificacion",
                column: "IdUsuario",
                principalTable: "usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_publicacion_usuario_IdUsuario",
                table: "publicacion",
                column: "IdUsuario",
                principalTable: "usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_publicacionlocal_local_IdLocal",
                table: "publicacionlocal",
                column: "IdLocal",
                principalTable: "local",
                principalColumn: "IdLocal",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_publicacionlocal_publicacion_IdPublicacion",
                table: "publicacionlocal",
                column: "IdPublicacion",
                principalTable: "publicacion",
                principalColumn: "IdPublicacion",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_usuario_local_IdLocal",
                table: "usuario",
                column: "IdLocal",
                principalTable: "local",
                principalColumn: "IdLocal",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inasistencia_usuario_IdUsuario",
                table: "inasistencia");

            migrationBuilder.DropForeignKey(
                name: "FK_notificacion_publicacion_IdPublicacion",
                table: "notificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_notificacion_usuario_IdUsuario",
                table: "notificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_publicacion_usuario_IdUsuario",
                table: "publicacion");

            migrationBuilder.DropForeignKey(
                name: "FK_publicacionlocal_local_IdLocal",
                table: "publicacionlocal");

            migrationBuilder.DropForeignKey(
                name: "FK_publicacionlocal_publicacion_IdPublicacion",
                table: "publicacionlocal");

            migrationBuilder.DropForeignKey(
                name: "FK_usuario_local_IdLocal",
                table: "usuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usuario",
                table: "usuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_publicacionlocal",
                table: "publicacionlocal");

            migrationBuilder.DropPrimaryKey(
                name: "PK_publicacion",
                table: "publicacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_notificacion",
                table: "notificacion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_local",
                table: "local");

            migrationBuilder.DropPrimaryKey(
                name: "PK_inasistencia",
                table: "inasistencia");

            migrationBuilder.RenameTable(
                name: "usuario",
                newName: "Usuarios");

            migrationBuilder.RenameTable(
                name: "publicacionlocal",
                newName: "PublicacionesLocales");

            migrationBuilder.RenameTable(
                name: "publicacion",
                newName: "Publicaciones");

            migrationBuilder.RenameTable(
                name: "notificacion",
                newName: "Notificaciones");

            migrationBuilder.RenameTable(
                name: "local",
                newName: "Locales");

            migrationBuilder.RenameTable(
                name: "inasistencia",
                newName: "Inasistencias");

            migrationBuilder.RenameIndex(
                name: "IX_usuario_IdLocal",
                table: "Usuarios",
                newName: "IX_Usuarios_IdLocal");

            migrationBuilder.RenameIndex(
                name: "IX_publicacionlocal_IdLocal",
                table: "PublicacionesLocales",
                newName: "IX_PublicacionesLocales_IdLocal");

            migrationBuilder.RenameIndex(
                name: "IX_publicacion_IdUsuario",
                table: "Publicaciones",
                newName: "IX_Publicaciones_IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_notificacion_IdUsuario",
                table: "Notificaciones",
                newName: "IX_Notificaciones_IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_notificacion_IdPublicacion",
                table: "Notificaciones",
                newName: "IX_Notificaciones_IdPublicacion");

            migrationBuilder.RenameIndex(
                name: "IX_inasistencia_IdUsuario",
                table: "Inasistencias",
                newName: "IX_Inasistencias_IdUsuario");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "IdUsuario");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PublicacionesLocales",
                table: "PublicacionesLocales",
                columns: new[] { "IdPublicacion", "IdLocal" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Publicaciones",
                table: "Publicaciones",
                column: "IdPublicacion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notificaciones",
                table: "Notificaciones",
                column: "IdNotificacion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Locales",
                table: "Locales",
                column: "IdLocal");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inasistencias",
                table: "Inasistencias",
                column: "IdInasistencia");

            migrationBuilder.AddForeignKey(
                name: "FK_Inasistencias_Usuarios_IdUsuario",
                table: "Inasistencias",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Publicaciones_IdPublicacion",
                table: "Notificaciones",
                column: "IdPublicacion",
                principalTable: "Publicaciones",
                principalColumn: "IdPublicacion",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Usuarios_IdUsuario",
                table: "Notificaciones",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Publicaciones_Usuarios_IdUsuario",
                table: "Publicaciones",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicacionesLocales_Locales_IdLocal",
                table: "PublicacionesLocales",
                column: "IdLocal",
                principalTable: "Locales",
                principalColumn: "IdLocal",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicacionesLocales_Publicaciones_IdPublicacion",
                table: "PublicacionesLocales",
                column: "IdPublicacion",
                principalTable: "Publicaciones",
                principalColumn: "IdPublicacion",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Locales_IdLocal",
                table: "Usuarios",
                column: "IdLocal",
                principalTable: "Locales",
                principalColumn: "IdLocal",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
