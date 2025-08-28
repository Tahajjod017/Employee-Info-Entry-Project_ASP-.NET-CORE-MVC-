using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeeMvc.Migrations
{
    /// <inheritdoc />
    public partial class I1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AutoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentShortName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "Designations",
                columns: table => new
                {
                    DesignationId = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AutoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesignationName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DesingnationShortname = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designations", x => x.DesignationId);
                });

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

            migrationBuilder.CreateTable(
                name: "Employeeinfos",
                columns: table => new
                {
                    EmployeeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AutoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DevSkills = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JoiningDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GrossSalary = table.Column<int>(type: "int", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employeeinfos", x => x.EmployeeID);
                });

            migrationBuilder.CreateTable(
                name: "Educationalinfos",
                columns: table => new
                {
                    EducationalinfoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AutoId = table.Column<int>(type: "int", nullable: false),
                    ExamTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Institution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Result = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PassingYear = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educationalinfos", x => x.EducationalinfoID);
                    table.ForeignKey(
                        name: "FK_Educationalinfos_Employeeinfos_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employeeinfos",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "DepartmentName", "DepartmentShortName" },
                values: new object[,]
                {
                    { "001", "Software Department", null },
                    { "002", "Quality Department", null },
                    { "003", "Human Resources", null },
                    { "004", "Finance", null },
                    { "005", "Marketing", null },
                    { "006", "Sales", null },
                    { "007", "Administration", null },
                    { "008", "Accounts", null }
                });

            migrationBuilder.InsertData(
                table: "Designations",
                columns: new[] { "DesignationId", "DesignationName", "DesingnationShortname" },
                values: new object[,]
                {
                    { "001", "Software Developer", null },
                    { "002", "Manager", null },
                    { "003", "Team Leader", null },
                    { "004", "Senior Developer", null },
                    { "005", "Junior Developer", null },
                    { "006", "Business Analyst", null },
                    { "007", "Chief Technology Officer", null },
                    { "008", "Quality Assurance", null },
                    { "009", "Administration Officer", null },
                    { "010", "Executive Marketing", null },
                    { "011", "Senior Sales ", null },
                    { "012", "Business Development Manager", null }
                });

            migrationBuilder.InsertData(
                table: "Devskills",
                columns: new[] { "DevId", "DevName", "DevShortName" },
                values: new object[,]
                {
                    { "001", "C# Programming", "C#" },
                    { "002", "JavaScript", "JS" },
                    { "003", "ASP .NET CORE", "CORE" },
                    { "004", "SQL Database", "SQL" },
                    { "005", "SQL Server Management Studio", "SQL Server" },
                    { "006", ".NET Framework", ".NET" },
                    { "007", "ASP.NET MVC", "MVC" },
                    { "008", "Web API Development", "API" },
                    { "009", "HTML/CSS", "HTML" },
                    { "010", "Angular Framework", "Angular" },
                    { "011", "React Library", "React" },
                    { "012", "Python Programming", "Python" },
                    { "013", "C++ Programming", "C++" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Educationalinfos_EmployeeID",
                table: "Educationalinfos",
                column: "EmployeeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Designations");

            migrationBuilder.DropTable(
                name: "Devskills");

            migrationBuilder.DropTable(
                name: "Educationalinfos");

            migrationBuilder.DropTable(
                name: "Employeeinfos");
        }
    }
}
