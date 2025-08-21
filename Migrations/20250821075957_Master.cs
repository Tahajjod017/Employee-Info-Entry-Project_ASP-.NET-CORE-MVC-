using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeMvc.Migrations
{
    /// <inheritdoc />
    public partial class Master : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Educationalinfos",
                columns: table => new
                {
                    EducationalinfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    AutoId = table.Column<int>(type: "int", nullable: false),
                    Institution = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExamTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Result = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PassingYear = table.Column<int>(type: "int", maxLength: 50, nullable: true),
                    EmployeeinfoAutoID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educationalinfos", x => x.EducationalinfoId);
                    table.ForeignKey(
                        name: "FK_Educationalinfos_Employeeinfos_EmployeeinfoAutoID",
                        column: x => x.EmployeeinfoAutoID,
                        principalTable: "Employeeinfos",
                        principalColumn: "AutoID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Educationalinfos_EmployeeinfoAutoID",
                table: "Educationalinfos",
                column: "EmployeeinfoAutoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Educationalinfos");
        }
    }
}
