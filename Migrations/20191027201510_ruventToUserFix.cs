using Microsoft.EntityFrameworkCore.Migrations;

namespace ruvents_api.Migrations
{
    public partial class ruventToUserFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RuventToUser",
                columns: table => new
                {
                    RuventToUserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsAttending = table.Column<bool>(nullable: false),
                    RuventId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuventToUser", x => x.RuventToUserId);
                    table.ForeignKey(
                        name: "FK_RuventToUser_Ruvents_RuventId",
                        column: x => x.RuventId,
                        principalTable: "Ruvents",
                        principalColumn: "RuventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RuventToUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RuventToUser_RuventId",
                table: "RuventToUser",
                column: "RuventId");

            migrationBuilder.CreateIndex(
                name: "IX_RuventToUser_UserId",
                table: "RuventToUser",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RuventToUser");
        }
    }
}
