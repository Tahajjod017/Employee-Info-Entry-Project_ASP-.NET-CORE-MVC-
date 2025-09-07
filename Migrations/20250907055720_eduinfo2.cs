using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeMvc.Migrations
{
    /// <inheritdoc />
    public partial class eduinfo2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educationalinfos_Employeeinfos_EmployeeID",
                table: "Educationalinfos");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeID",
                table: "Educationalinfos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Educationalinfos_Employeeinfos_EmployeeID",
                table: "Educationalinfos",
                column: "EmployeeID",
                principalTable: "Employeeinfos",
                principalColumn: "EmployeeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educationalinfos_Employeeinfos_EmployeeID",
                table: "Educationalinfos");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeID",
                table: "Educationalinfos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Educationalinfos_Employeeinfos_EmployeeID",
                table: "Educationalinfos",
                column: "EmployeeID",
                principalTable: "Employeeinfos",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
