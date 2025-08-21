using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeeMvc.Migrations
{
    /// <inheritdoc />
    public partial class All : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DepartmentName",
                table: "Departments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentId",
                table: "Departments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: "001");

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: "002");

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: "003");

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: "004");

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: "005");

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: "006");

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: "007");

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: "008");

            migrationBuilder.DeleteData(
                table: "Designations",
                keyColumn: "DesignationId",
                keyValue: "001");

            migrationBuilder.DeleteData(
                table: "Designations",
                keyColumn: "DesignationId",
                keyValue: "002");

            migrationBuilder.DeleteData(
                table: "Designations",
                keyColumn: "DesignationId",
                keyValue: "003");

            migrationBuilder.DeleteData(
                table: "Designations",
                keyColumn: "DesignationId",
                keyValue: "004");

            migrationBuilder.DeleteData(
                table: "Designations",
                keyColumn: "DesignationId",
                keyValue: "005");

            migrationBuilder.DeleteData(
                table: "Designations",
                keyColumn: "DesignationId",
                keyValue: "006");

            migrationBuilder.DeleteData(
                table: "Designations",
                keyColumn: "DesignationId",
                keyValue: "007");

            migrationBuilder.DeleteData(
                table: "Designations",
                keyColumn: "DesignationId",
                keyValue: "008");

            migrationBuilder.DeleteData(
                table: "Designations",
                keyColumn: "DesignationId",
                keyValue: "009");

            migrationBuilder.DeleteData(
                table: "Designations",
                keyColumn: "DesignationId",
                keyValue: "010");

            migrationBuilder.DeleteData(
                table: "Designations",
                keyColumn: "DesignationId",
                keyValue: "011");

            migrationBuilder.DeleteData(
                table: "Designations",
                keyColumn: "DesignationId",
                keyValue: "012");

            migrationBuilder.DeleteData(
                table: "Devskills",
                keyColumn: "DevId",
                keyValue: "001");

            migrationBuilder.DeleteData(
                table: "Devskills",
                keyColumn: "DevId",
                keyValue: "002");

            migrationBuilder.DeleteData(
                table: "Devskills",
                keyColumn: "DevId",
                keyValue: "003");

            migrationBuilder.DeleteData(
                table: "Devskills",
                keyColumn: "DevId",
                keyValue: "004");

            migrationBuilder.DeleteData(
                table: "Devskills",
                keyColumn: "DevId",
                keyValue: "005");

            migrationBuilder.DeleteData(
                table: "Devskills",
                keyColumn: "DevId",
                keyValue: "006");

            migrationBuilder.DeleteData(
                table: "Devskills",
                keyColumn: "DevId",
                keyValue: "007");

            migrationBuilder.DeleteData(
                table: "Devskills",
                keyColumn: "DevId",
                keyValue: "008");

            migrationBuilder.DeleteData(
                table: "Devskills",
                keyColumn: "DevId",
                keyValue: "009");

            migrationBuilder.DeleteData(
                table: "Devskills",
                keyColumn: "DevId",
                keyValue: "010");

            migrationBuilder.DeleteData(
                table: "Devskills",
                keyColumn: "DevId",
                keyValue: "011");

            migrationBuilder.DeleteData(
                table: "Devskills",
                keyColumn: "DevId",
                keyValue: "012");

            migrationBuilder.DeleteData(
                table: "Devskills",
                keyColumn: "DevId",
                keyValue: "013");

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentName",
                table: "Departments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentId",
                table: "Departments",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);
        }
    }
}
