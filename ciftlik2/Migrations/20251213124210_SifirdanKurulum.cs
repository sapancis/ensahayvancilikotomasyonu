using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ciftlik2.Migrations
{
    /// <inheritdoc />
    public partial class SifirdanKurulum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hayvanlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KupeNumarasi = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cins = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DogumTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Cinsiyet = table.Column<int>(type: "int", nullable: false),
                    LaktasyonSayisi = table.Column<int>(type: "int", nullable: false),
                    Kisirlastirildi = table.Column<bool>(type: "bit", nullable: false),
                    GirisSekli = table.Column<int>(type: "int", nullable: false),
                    BabaKupeNumarasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnaKupeNumarasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlisFiyati = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AlisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GeldigiYer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    CikisNedeni = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hayvanlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SutKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SagilanHayvanSayisi = table.Column<int>(type: "int", nullable: false),
                    Miktar = table.Column<double>(type: "float", nullable: false),
                    Vakit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SutKayitlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YemAlimlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YemTuru = table.Column<int>(type: "int", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Miktar = table.Column<double>(type: "float", nullable: false),
                    ToplamFiyat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AlisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YemAlimlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AsiKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AsiIsmi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AsiTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HayvanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsiKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsiKayitlari_Hayvanlar_HayvanId",
                        column: x => x.HayvanId,
                        principalTable: "Hayvanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TedaviKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Teshis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KullanilanIlac = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SutArinmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceteyiYazanVeteriner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DenetleyenVeteriner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HayvanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TedaviKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TedaviKayitlari_Hayvanlar_HayvanId",
                        column: x => x.HayvanId,
                        principalTable: "Hayvanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestIsmi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TestTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Sonuc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HayvanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestKayitlari_Hayvanlar_HayvanId",
                        column: x => x.HayvanId,
                        principalTable: "Hayvanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tohumlanmalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisiHayvanId = table.Column<int>(type: "int", nullable: false),
                    ErkekHayvanId = table.Column<int>(type: "int", nullable: true),
                    TohumlanmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TohumlanmaTuru = table.Column<int>(type: "int", nullable: false),
                    GebelikTestiSonucu = table.Column<int>(type: "int", nullable: true),
                    GebelikTestDurumu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tohumlanmalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tohumlanmalar_Hayvanlar_DisiHayvanId",
                        column: x => x.DisiHayvanId,
                        principalTable: "Hayvanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tohumlanmalar_Hayvanlar_ErkekHayvanId",
                        column: x => x.ErkekHayvanId,
                        principalTable: "Hayvanlar",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsiKayitlari_HayvanId",
                table: "AsiKayitlari",
                column: "HayvanId");

            migrationBuilder.CreateIndex(
                name: "IX_Hayvanlar_KupeNumarasi",
                table: "Hayvanlar",
                column: "KupeNumarasi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TedaviKayitlari_HayvanId",
                table: "TedaviKayitlari",
                column: "HayvanId");

            migrationBuilder.CreateIndex(
                name: "IX_TestKayitlari_HayvanId",
                table: "TestKayitlari",
                column: "HayvanId");

            migrationBuilder.CreateIndex(
                name: "IX_Tohumlanmalar_DisiHayvanId",
                table: "Tohumlanmalar",
                column: "DisiHayvanId");

            migrationBuilder.CreateIndex(
                name: "IX_Tohumlanmalar_ErkekHayvanId",
                table: "Tohumlanmalar",
                column: "ErkekHayvanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsiKayitlari");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "SutKayitlari");

            migrationBuilder.DropTable(
                name: "TedaviKayitlari");

            migrationBuilder.DropTable(
                name: "TestKayitlari");

            migrationBuilder.DropTable(
                name: "Tohumlanmalar");

            migrationBuilder.DropTable(
                name: "YemAlimlari");

            migrationBuilder.DropTable(
                name: "Hayvanlar");
        }
    }
}
