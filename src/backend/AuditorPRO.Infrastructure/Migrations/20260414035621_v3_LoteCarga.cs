using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditorPRO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v3_LoteCarga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Nuevas columnas en Simulaciones ───────────────────────────────
            // Objetivo, TipoSimulacion, ResumenResultados, TotalCriticos ya
            // existen en producción (aplicadas por migración previa).
            // Solo faltan FechaReferenciaDatos y LotesUsadosJson.
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaReferenciaDatos",
                table: "Simulaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LotesUsadosJson",
                table: "Simulaciones",
                type: "nvarchar(max)",
                nullable: true);

            // ── Tabla LotesCarga ──────────────────────────────────────────────
            // SnapshotsEntraID y RegistrosEntraID ya existen en producción.
            migrationBuilder.CreateTable(
                name: "LotesCarga",
                columns: table => new
                {
                    Id            = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoCarga     = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaCarga    = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SociedadCodigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SociedadNombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NombreArchivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalRegistros = table.Column<int>(type: "int", nullable: false),
                    Insertados    = table.Column<int>(type: "int", nullable: false),
                    Actualizados  = table.Column<int>(type: "int", nullable: false),
                    Errores       = table.Column<int>(type: "int", nullable: false),
                    CargadoPor    = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsVigente     = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotesCarga", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LotesCarga_TipoCarga_EsVigente",
                table: "LotesCarga",
                columns: new[] { "TipoCarga", "EsVigente" });

            migrationBuilder.CreateIndex(
                name: "IX_LotesCarga_TipoCarga_SociedadCodigo_FechaCarga",
                table: "LotesCarga",
                columns: new[] { "TipoCarga", "SociedadCodigo", "FechaCarga" });

            // ── FK FuentesDatosSimulacion → Simulaciones ──────────────────────
            // La tabla ya existe pero sin FK (creada sin constraint en migración anterior).
            migrationBuilder.AddForeignKey(
                name: "FK_FuentesDatosSimulacion_Simulaciones_SimulacionId",
                table: "FuentesDatosSimulacion",
                column: "SimulacionId",
                principalTable: "Simulaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuentesDatosSimulacion_Simulaciones_SimulacionId",
                table: "FuentesDatosSimulacion");

            migrationBuilder.DropTable(name: "LotesCarga");

            migrationBuilder.DropColumn(name: "FechaReferenciaDatos", table: "Simulaciones");
            migrationBuilder.DropColumn(name: "LotesUsadosJson",      table: "Simulaciones");
        }
    }
}
