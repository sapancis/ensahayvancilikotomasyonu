using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ciftlik2.Migrations
{
    /// <inheritdoc />
    public partial class vc5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ayarlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CiftlikAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CiftlikSahibi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StandartGebelikSuresi = table.Column<int>(type: "int", nullable: false),
                    KuruyaAlmaGunu = table.Column<int>(type: "int", nullable: false),
                    KizginlikBaslangicGun = table.Column<int>(type: "int", nullable: false),
                    KizginlikBitisGun = table.Column<int>(type: "int", nullable: false),
                    GebelikTestiGun = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ayarlar", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Ayarlar",
                columns: new[] { "Id", "CiftlikAdi", "CiftlikSahibi", "GebelikTestiGun", "KizginlikBaslangicGun", "KizginlikBitisGun", "KuruyaAlmaGunu", "StandartGebelikSuresi" },
                values: new object[] { 1, "Çiftlik Yönetim Sistemi", null, 40, 18, 23, 220, 280 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ayarlar");
        }
    }
}
