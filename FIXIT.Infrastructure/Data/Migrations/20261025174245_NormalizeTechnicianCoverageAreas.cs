using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIXIT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeTechnicianCoverageAreas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TechnicianCoverageAreas",
                columns: table => new
                {
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServiceAreaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianCoverageAreas", x => new { x.TechnicianId, x.ServiceAreaId });
                    table.ForeignKey(
                        name: "FK_TechnicianCoverageAreas_ServiceAreas_ServiceAreaId",
                        column: x => x.ServiceAreaId,
                        principalTable: "ServiceAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TechnicianCoverageAreas_TechnicianProfiles_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "TechnicianProfiles",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianCoverageAreas_ServiceAreaId",
                table: "TechnicianCoverageAreas",
                column: "ServiceAreaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicianCoverageAreas");
        }
    }
}
