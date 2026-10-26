using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIXIT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicianJobDeclines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TechnicianJobDeclines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianJobDeclines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechnicianJobDeclines_AspNetUsers_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TechnicianJobDeclines_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianJobDeclines_BookingId_TechnicianId",
                table: "TechnicianJobDeclines",
                columns: new[] { "BookingId", "TechnicianId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianJobDeclines_TechnicianId",
                table: "TechnicianJobDeclines",
                column: "TechnicianId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicianJobDeclines");
        }
    }
}
