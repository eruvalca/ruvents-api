using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ruvents_api.Migrations
{
    public partial class newModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Ruvents");

            migrationBuilder.DropColumn(
                name: "EndTimeHour",
                table: "Ruvents");

            migrationBuilder.DropColumn(
                name: "EndTimeMinute",
                table: "Ruvents");

            migrationBuilder.DropColumn(
                name: "StartTimeHour",
                table: "Ruvents");

            migrationBuilder.DropColumn(
                name: "StartTimeMinute",
                table: "Ruvents");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Ruvents",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Ruvents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Ruvents");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Ruvents");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Ruvents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndTimeHour",
                table: "Ruvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndTimeMinute",
                table: "Ruvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartTimeHour",
                table: "Ruvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartTimeMinute",
                table: "Ruvents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
