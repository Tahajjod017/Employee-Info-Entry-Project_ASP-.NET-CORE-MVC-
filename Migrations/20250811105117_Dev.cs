using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeMvc.Migrations
{
    /// <inheritdoc />
    public partial class Dev : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devskills",
                columns: table => new
                {
                    DevId = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AutoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DevName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DevShortName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devskills", x => x.DevId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devskills");
        }
    }
}
