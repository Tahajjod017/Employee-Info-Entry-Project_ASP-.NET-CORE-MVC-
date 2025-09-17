using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeMvc.Models;
using System.Text;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace EmployeeMvc.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDBContext dbContext;
        private readonly IWebHostEnvironment webHost;

        public ReportController(ApplicationDBContext _dbContext, IWebHostEnvironment webHost)
        {
            dbContext = _dbContext;
            this.webHost = webHost;
        }

        public async Task<IActionResult> Index()
        {
            // Get departments for dropdown
            ViewBag.Departments = await dbContext.Departments.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetFilteredEmployees(string? department, DateTime? joiningDateFrom, DateTime? joiningDateTo, string? search, int page = 1)
        {
            try
            {
                const int pageSize = 10; // Set page size

                var query = from emp in dbContext.Employeeinfos
                            join des in dbContext.Designations on emp.Designation equals des.DesignationId into desiGroup
                            from des in desiGroup.DefaultIfEmpty()
                            join dep in dbContext.Departments on emp.Department equals dep.DepartmentId into depGroup
                            from dep in depGroup.DefaultIfEmpty()
                            select new
                            {
                                emp.AutoID,
                                emp.EmployeeID,
                                emp.Name,
                                emp.Address,
                                emp.Email,
                                emp.Phone,
                                emp.PhotoPath,
                                emp.GrossSalary,
                                emp.JoiningDate,
                                emp.DevSkills,
                                Designation = des.DesignationName ?? "N/A",
                                Department = dep.DepartmentName ?? "N/A"
                            };

                // Apply filters
                if (!string.IsNullOrEmpty(department))
                {
                    query = query.Where(e => e.Department == department);
                }

                if (joiningDateFrom.HasValue)
                {
                    query = query.Where(e => e.JoiningDate >= joiningDateFrom.Value);
                }

                if (joiningDateTo.HasValue)
                {
                    query = query.Where(e => e.JoiningDate <= joiningDateTo.Value);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(e => e.Name.Contains(search) ||
                                           e.EmployeeID.Contains(search) ||
                                           e.Email.Contains(search) ||
                                           e.Phone.Contains(search));
                }

                // Get total count for pagination
                var totalRecords = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                // Apply pagination
                var employees = await query
                    .OrderBy(e => e.EmployeeID)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Get educational info for each employee
                var employeeIds = employees.Select(e => e.EmployeeID).ToList();
                var educationalInfos = await dbContext.Educationalinfos
                    .Where(edu => employeeIds.Contains(edu.EmployeeID))
                    .ToListAsync();

                // Process the results
                var processedEmployees = employees.Select((emp, index) => new
                {
                    SlNo = ((page - 1) * pageSize) + index + 1, // Correct serial number for pagination
                    emp.EmployeeID,
                    emp.Name,
                    emp.Address,
                    emp.Designation,
                    emp.Department,
                    DevelopmentSkills = GetDevSkillsNames(emp.DevSkills),
                    JoiningDate = emp.JoiningDate?.ToString("dd-MM-yyyy") ?? "N/A",
                    emp.Phone,
                    emp.Email,
                    emp.GrossSalary,
                    emp.PhotoPath,
                    EducationalInfo = GetEducationalInfoString(educationalInfos.Where(edu => edu.EmployeeID == emp.EmployeeID).ToList())
                }).ToList();

                return Json(new
                {
                    success = true,
                    data = processedEmployees,
                    pagination = new
                    {
                        currentPage = page,
                        totalPages = totalPages,
                        totalRecords = totalRecords,
                        pageSize = pageSize,
                        hasPrevious = page > 1,
                        hasNext = page < totalPages
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        private string GetDevSkillsNames(string devSkillIds)
        {
            if (string.IsNullOrEmpty(devSkillIds))
                return "";

            try
            {
                var skillIds = devSkillIds.Split(',')
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Select(id => id.Trim())
                    .ToList();

                if (!skillIds.Any())
                    return "";

                var skillNames = dbContext.Devskills
                    .Where(d => skillIds.Contains(d.DevId))
                    .Select(d => d.DevShortName)
                    .ToList();

                return skillNames.Any() ? string.Join(", ", skillNames) : "";
            }
            catch
            {
                return "";
            }
        }

        private string GetEducationalInfoString(List<Educationalinfo> educationalInfos)
        {
            if (educationalInfos == null || !educationalInfos.Any())
                return "";

            var eduInfo = new StringBuilder();
            foreach (var edu in educationalInfos)
            {
                eduInfo.AppendLine($"{edu.ExamTitle} - {edu.Institution} ({edu.PassingYear}) - {edu.Result}");
            }

            return eduInfo.ToString().Trim();
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(string? department, DateTime? joiningDateFrom, DateTime? joiningDateTo, string? search)
        {
            try
            {
                // Set EPPlus license context
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var employees = await GetEmployeesForExport(department, joiningDateFrom, joiningDateTo, search);

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Employee Report");

                    // Headers
                    worksheet.Cells[1, 1].Value = "Sl No.";
                    worksheet.Cells[1, 2].Value = "Employee ID";
                    worksheet.Cells[1, 3].Value = "Employee Name";
                    worksheet.Cells[1, 4].Value = "Address";
                    worksheet.Cells[1, 5].Value = "Designation";
                    worksheet.Cells[1, 6].Value = "Department";
                    worksheet.Cells[1, 7].Value = "Development Skills";
                    worksheet.Cells[1, 8].Value = "Joining Date";
                    worksheet.Cells[1, 9].Value = "Phone";
                    worksheet.Cells[1, 10].Value = "Email";
                    worksheet.Cells[1, 11].Value = "Gross Salary";
                    worksheet.Cells[1, 12].Value = "Educational Info";

                    // Style headers
                    using (var range = worksheet.Cells[1, 1, 1, 12])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    // Data
                    for (int i = 0; i < employees.Count; i++)
                    {
                        var emp = employees[i];
                        worksheet.Cells[i + 2, 1].Value = i + 1;
                        worksheet.Cells[i + 2, 2].Value = emp.EmployeeID;
                        worksheet.Cells[i + 2, 3].Value = emp.Name;
                        worksheet.Cells[i + 2, 4].Value = emp.Address ?? "";
                        worksheet.Cells[i + 2, 5].Value = emp.Designation;
                        worksheet.Cells[i + 2, 6].Value = emp.Department;
                        worksheet.Cells[i + 2, 7].Value = GetDevSkillsNames(emp.DevSkills);
                        worksheet.Cells[i + 2, 8].Value = emp.JoiningDate?.ToString("dd-MM-yyyy") ?? "";
                        worksheet.Cells[i + 2, 9].Value = emp.Phone ?? "";
                        worksheet.Cells[i + 2, 10].Value = emp.Email ?? "";
                        worksheet.Cells[i + 2, 11].Value = emp.GrossSalary ?? 0;
                        worksheet.Cells[i + 2, 12].Value = emp.EducationalInfo ?? "";
                    }

                    // Auto fit columns
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;

                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"EmployeeReport_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Export failed: " + ex.Message });
            }
        }

        private async Task<List<dynamic>> GetEmployeesForExport(string? department, DateTime? joiningDateFrom, DateTime? joiningDateTo, string? search)
        {
            var query = from emp in dbContext.Employeeinfos
                        join des in dbContext.Designations on emp.Designation equals des.DesignationId into desiGroup
                        from des in desiGroup.DefaultIfEmpty()
                        join dep in dbContext.Departments on emp.Department equals dep.DepartmentId into depGroup
                        from dep in depGroup.DefaultIfEmpty()
                        select new
                        {
                            emp.EmployeeID,
                            emp.Name,
                            emp.Address,
                            emp.Email,
                            emp.Phone,
                            emp.GrossSalary,
                            emp.JoiningDate,
                            emp.DevSkills,
                            Designation = des.DesignationName ?? "N/A",
                            Department = dep.DepartmentName ?? "N/A"
                        };

            // Apply same filters as GetFilteredEmployees
            if (!string.IsNullOrEmpty(department))
                query = query.Where(e => e.Department == department);

            if (joiningDateFrom.HasValue)
                query = query.Where(e => e.JoiningDate >= joiningDateFrom.Value);

            if (joiningDateTo.HasValue)
                query = query.Where(e => e.JoiningDate <= joiningDateTo.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(e => e.Name.Contains(search) || e.EmployeeID.Contains(search));

            var employees = await query.OrderBy(e => e.EmployeeID).ToListAsync();

            // Get educational info
            var employeeIds = employees.Select(e => e.EmployeeID).ToList();
            var educationalInfos = await dbContext.Educationalinfos
                .Where(edu => employeeIds.Contains(edu.EmployeeID))
                .ToListAsync();

            return employees.Select(emp => new
            {
                emp.EmployeeID,
                emp.Name,
                emp.Address,
                emp.Designation,
                emp.Department,
                emp.DevSkills,
                emp.JoiningDate,
                emp.Phone,
                emp.Email,
                emp.GrossSalary,
                EducationalInfo = GetEducationalInfoString(educationalInfos.Where(edu => edu.EmployeeID == emp.EmployeeID).ToList())
            }).Cast<dynamic>().ToList();
        }
    }
}