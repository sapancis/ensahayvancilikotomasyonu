using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ciftlik2.Migrations
{
    /// <inheritdoc />
    public partial class vc5as : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuzagiSuttenKesmeGunu",
                table: "Ayarlar",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Ayarlar",
                keyColumn: "Id",
                keyValue: 1,
                column: "BuzagiSuttenKesmeGunu",
                value: 70);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuzagiSuttenKesmeGunu",
                table: "Ayarlar");
        }
    }
}
