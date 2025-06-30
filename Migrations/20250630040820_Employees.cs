using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeMvc.Migrations
{
    /// <inheritdoc />
    public partial class Employees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DesingnationShortname",
                table: "Designations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesingnationShortname",
                table: "Designations");
        }
    }
}
