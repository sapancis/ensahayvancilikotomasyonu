using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ciftlik2.Migrations
{
    /// <inheritdoc />
    public partial class SutCikisEklendilk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TedaviUcreti",
                table: "TedaviKayitlari",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SatisFiyati",
                table: "Hayvanlar",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FinansIslemleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tur = table.Column<int>(type: "int", nullable: false),
                    Kategori = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinansIslemleri", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinansIslemleri");

            migrationBuilder.DropColumn(
                name: "TedaviUcreti",
                table: "TedaviKayitlari");

            migrationBuilder.DropColumn(
                name: "SatisFiyati",
                table: "Hayvanlar");
        }
    }
}
