using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCityWebApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CertificatePrivateKey",
                table: "custSpaceSetting",
                type: "varchar(2048)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1024)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CertificatePrivateKey",
                table: "custSpaceSetting",
                type: "varchar(1024)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(2048)");
        }
    }
}
