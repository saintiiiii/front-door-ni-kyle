using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIXIT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingWorkReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompletionNotes",
                table: "Bookings",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartsUsed",
                table: "Bookings",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletionNotes",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PartsUsed",
                table: "Bookings");
        }
    }
}
