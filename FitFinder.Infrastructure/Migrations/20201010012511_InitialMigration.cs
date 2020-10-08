using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace FitFinder.Infrastructure.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingStatus",
                columns: table => new
                {
                    BookingStatusId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingStatus", x => x.BookingStatusId);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserRoleId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.UserRoleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByUserId = table.Column<long>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastModifiedByUserId = table.Column<long>(nullable: true),
                    LastModifiedUtc = table.Column<DateTime>(nullable: true),
                    GoogleId = table.Column<string>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    ProfilePictureUri = table.Column<string>(nullable: true),
                    UserRoleId = table.Column<int>(nullable: false, defaultValue: 2)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_UserRole_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRole",
                        principalColumn: "UserRoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByUserId = table.Column<long>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastModifiedByUserId = table.Column<long>(nullable: true),
                    LastModifiedUtc = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    SessionDateTime = table.Column<DateTime>(nullable: false),
                    Location = table.Column<Point>(nullable: false),
                    LocationString = table.Column<string>(nullable: true),
                    IsOnline = table.Column<bool>(nullable: false),
                    IsInPerson = table.Column<bool>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    TrainerUserId = table.Column<long>(nullable: false),
                    BookingId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_TrainerUserId",
                        column: x => x.TrainerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByUserId = table.Column<long>(nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    LastModifiedByUserId = table.Column<long>(nullable: true),
                    LastModifiedUtc = table.Column<DateTime>(nullable: true),
                    BookingStatusId = table.Column<int>(nullable: false, defaultValue: 0),
                    ClientUserId = table.Column<long>(nullable: false),
                    SessionId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_BookingStatus_BookingStatusId",
                        column: x => x.BookingStatusId,
                        principalTable: "BookingStatus",
                        principalColumn: "BookingStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_ClientUserId",
                        column: x => x.ClientUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BookingStatus",
                columns: new[] { "BookingStatusId", "Name" },
                values: new object[,]
                {
                    { 0, "Pending" },
                    { 1, "Confirmed" },
                    { 2, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "UserRoleId", "Name" },
                values: new object[,]
                {
                    { 0, "Admin" },
                    { 1, "Trainer" },
                    { 2, "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingStatusId",
                table: "Bookings",
                column: "BookingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ClientUserId",
                table: "Bookings",
                column: "ClientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SessionId",
                table: "Bookings",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TrainerUserId",
                table: "Sessions",
                column: "TrainerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GoogleId",
                table: "Users",
                column: "GoogleId",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserRoleId",
                table: "Users",
                column: "UserRoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "BookingStatus");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserRole");
        }
    }
}
