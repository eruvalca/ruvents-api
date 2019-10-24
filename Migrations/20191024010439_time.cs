using Microsoft.EntityFrameworkCore.Migrations;

namespace ruvents_api.Migrations
{
    public partial class time : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EndTime",
                table: "Ruvents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartTime",
                table: "Ruvents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Ruvents");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Ruvents");
        }
    }
}
