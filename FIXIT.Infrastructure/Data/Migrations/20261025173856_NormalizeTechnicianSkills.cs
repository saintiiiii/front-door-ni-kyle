using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIXIT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeTechnicianSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TechnicianSkills",
                columns: table => new
                {
                    TechnicianId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicianSkills", x => new { x.TechnicianId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_TechnicianSkills_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TechnicianSkills_TechnicianProfiles_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "TechnicianProfiles",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechnicianSkills_ServiceId",
                table: "TechnicianSkills",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechnicianSkills");
        }
    }
}
