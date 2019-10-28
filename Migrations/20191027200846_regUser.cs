using Microsoft.EntityFrameworkCore.Migrations;

namespace ruvents_api.Migrations
{
    public partial class regUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Ruvents_RuventId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Ruvents_RuventId1",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RuventId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RuventId1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RuventId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RuventId1",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RuventId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RuventId1",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RuventId",
                table: "Users",
                column: "RuventId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RuventId1",
                table: "Users",
                column: "RuventId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Ruvents_RuventId",
                table: "Users",
                column: "RuventId",
                principalTable: "Ruvents",
                principalColumn: "RuventId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Ruvents_RuventId1",
                table: "Users",
                column: "RuventId1",
                principalTable: "Ruvents",
                principalColumn: "RuventId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
