using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FitFinder.Infrastructure.Migrations
{
    public partial class AddAuditableEntityToTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "Users",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "LastModifiedByUserId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedUtc",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "Sessions",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                table: "Sessions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "LastModifiedByUserId",
                table: "Sessions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedUtc",
                table: "Sessions",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "Bookings",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedUtc",
                table: "Bookings",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "LastModifiedByUserId",
                table: "Bookings",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedUtc",
                table: "Bookings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModifiedUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "LastModifiedUtc",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CreatedUtc",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "LastModifiedByUserId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "LastModifiedUtc",
                table: "Bookings");
        }
    }
}
