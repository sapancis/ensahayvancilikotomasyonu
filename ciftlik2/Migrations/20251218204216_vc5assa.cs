using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ciftlik2.Migrations
{
    /// <inheritdoc />
    public partial class vc5assa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DogumOncesiBildirimGunu",
                table: "Ayarlar",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Ayarlar",
                keyColumn: "Id",
                keyValue: 1,
                column: "DogumOncesiBildirimGunu",
                value: 15);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DogumOncesiBildirimGunu",
                table: "Ayarlar");
        }
    }
}
