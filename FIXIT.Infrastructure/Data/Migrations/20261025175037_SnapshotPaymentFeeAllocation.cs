using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIXIT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SnapshotPaymentFeeAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PlatformFeeAmount",
                table: "Payments",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PlatformFeePercent",
                table: "Payments",
                type: "decimal(9,2)",
                precision: 9,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TechnicianEarnings",
                table: "Payments",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql("""
                UPDATE p
                SET PlatformFeePercent = COALESCE(s.NumericValue, 15.00),
                    PlatformFeeAmount = ROUND(p.Amount * COALESCE(s.NumericValue, 15.00) / 100.00, 2),
                    TechnicianEarnings = p.Amount - ROUND(p.Amount * COALESCE(s.NumericValue, 15.00) / 100.00, 2)
                FROM Payments AS p
                LEFT JOIN PlatformSettings AS s ON s.[Key] = 'PlatformFeePercent'
                WHERE p.Status = 2;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlatformFeeAmount",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PlatformFeePercent",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TechnicianEarnings",
                table: "Payments");
        }
    }
}
