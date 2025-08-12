using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeMvc.Migrations
{
    /// <inheritdoc />
    public partial class DevEmp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DevSkills",
                table: "Employeeinfos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DevSkills",
                table: "Employeeinfos");
        }
    }
}
