using System.Buffers;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using EmployeeMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EmployeeMvc.Controllers
{
    public class EmployeeInfoController1 : Controller
    {
        private ApplicationDBContext dbContext; private readonly IWebHostEnvironment webHost;
        public EmployeeInfoController1(ApplicationDBContext _dbContext, IWebHostEnvironment webHost)
        {
            dbContext = _dbContext;
            this.webHost = webHost;
        }
        private bool photopath;
        private const int PageSize = 10;

        public object? PhotoPath { get; private set; }
        public object? Photo { get; set; }
        public string EmployeeID { get; private set; }

        public async Task<IActionResult> Index(int page = 1, string search = "")
        // Get Department and Designation for dropdown

        {
            ViewBag.Departments = await dbContext.Departments.ToListAsync();
            ViewBag.Designation = await dbContext.Designations.ToListAsync();

            // Query empliyess with search functionality
            var query1 = from emp in dbContext.Employeeinfos
                         join des in dbContext.Designations on emp.Designation equals des.DesignationId into desiGroup
                         from des in desiGroup.DefaultIfEmpty()
                         join dep in dbContext.Departments on emp.Department equals dep.DepartmentId into depGroup
                         from dep in depGroup.DefaultIfEmpty()

                         select new
                         {
                             emp.AutoID,
                             emp.EmployeeID,
                             emp.Name,
                             emp.Email,
                             emp.Phone,
                             emp.PhotoPath,
                             emp.GrossSalary,
                             emp.JoiningDate,
                             Designation = des.DesignationName,
                             Department = dep.DepartmentName
                         };

            var query = query1.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Name.Contains(search)
                || e.Email.Contains(search)
                || e.Phone.Contains(search));
            }
            int PageSize = 10; // Set the page size
            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            var employee = await query
                //.Skip((page - 1) * PageSize)
                //.Take(PageSize)
                .ToListAsync();

            //ViewBag.viewpage = totalPages;
            ViewBag.viewpage = totalPages;
            ViewBag.viewrecords = totalRecords;
            ViewBag.viewsize = PageSize;
            ViewBag.viewsearch = search;
            ViewBag.Hasprevious = page < 1;
            ViewBag.Hasforward = page < totalPages;
            ViewBag.emp = employee;
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Save(Employeeinfo employee, string[] selectedDevSkills, List<Educationalinfo> EducationRecords)
        {

            // Add debug logging
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToList();

                return Json(new { success = false, message = "Validation failed", errors = errors });
            }

            try
            {
                bool isDuplicate = await IsDuplicate(employee.Name, employee.EmployeeID, employee.Phone);
                if (isDuplicate)
                {
                    return Json(new { success = false, message = "Data already exists with this name or phone number." });
                }

                if (employee.AutoID == 0)
                {
                    // New Employee
                    if (employee.photo != null)
                    {
                        string? uniqueFileName = GetUploadFileName(employee);
                        employee.PhotoPath = uniqueFileName;
                    }

                    var emp = new Employeeinfo();
                    emp.EmployeeID = employee.EmployeeID;
                    emp.Name = employee.Name;
                    emp.Phone = employee.Phone;
                    emp.Department = employee.Department ?? string.Empty;
                    emp.Designation = employee.Designation ?? string.Empty;
                    emp.Address = employee.Address ?? string.Empty;
                    emp.Email = employee.Email ?? string.Empty;
                    emp.PhotoPath = employee.PhotoPath ?? string.Empty;
                    emp.JoiningDate = employee.JoiningDate ?? DateTime.MinValue;
                    emp.GrossSalary = employee.GrossSalary ?? 0;
                    emp.DevSkills = selectedDevSkills != null ? string.Join(",", selectedDevSkills) : string.Empty;

                    dbContext.Employeeinfos.Add(emp);
                    await dbContext.SaveChangesAsync(); // Save to get AutoId

                    // Save Education Records
                    Console.WriteLine("Education Records Count: " + (EducationRecords?.Count ?? 0));
                    if (EducationRecords != null)
                    {
                        foreach (var edu in EducationRecords)
                        {
                            Console.WriteLine($"Exam: {edu.ExamTitle}, Institution: {edu.Institution}, Year: {edu.PassingYear}");
                        }
                    }

                    if (EducationRecords != null && EducationRecords.Count > 0)
                    {
                        foreach (var educationRecord in EducationRecords)
                        {
                            if (!string.IsNullOrWhiteSpace(educationRecord.ExamTitle) &&
                                !string.IsNullOrWhiteSpace(educationRecord.Institution))
                            {
                                var eduInfo = new Educationalinfo
                                {
                                    EmployeeID = emp.EmployeeID,
                                    ExamTitle = educationRecord.ExamTitle,
                                    Institution = educationRecord.Institution,
                                    Result = educationRecord.Result,
                                    PassingYear = educationRecord.PassingYear
                                };
                                dbContext.Educationalinfos.Add(eduInfo);
                            }
                        }
                        await dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    // Update existing employee
                    var emp = await dbContext.Employeeinfos.FindAsync(employee.AutoID);
                    if (emp != null)
                    {
                        if (employee.photo != null)
                        {
                            if (!string.IsNullOrEmpty(emp.PhotoPath))
                            {
                                var photoPath = Path.Combine(webHost.WebRootPath, "Image", Path.GetFileName(emp.PhotoPath));
                                try
                                {
                                    System.IO.File.Delete(photoPath);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error deleting file: {ex.Message}");
                                }
                            }
                            string? uniqueFileName = GetUploadFileName(employee);
                            emp.PhotoPath = uniqueFileName;
                        }

                        emp.EmployeeID = employee.EmployeeID;
                        emp.Name = employee.Name;
                        emp.Phone = employee.Phone;
                        emp.Department = employee.Department ?? string.Empty;
                        emp.Designation = employee.Designation ?? string.Empty;
                        emp.Address = employee.Address ?? string.Empty;
                        emp.Email = employee.Email ?? string.Empty;
                        emp.JoiningDate = employee.JoiningDate ?? DateTime.MinValue;
                        emp.GrossSalary = employee.GrossSalary;
                        emp.DevSkills = selectedDevSkills != null ? string.Join(",", selectedDevSkills) : string.Empty;

                        // Update Education Records
                        var existingEducationRecords = dbContext.Educationalinfos.Where(e => e.EmployeeID == emp.EmployeeID);
                        dbContext.Educationalinfos.RemoveRange(existingEducationRecords);

                        // Add updated education records
                        if (EducationRecords != null && EducationRecords.Count > 0)
                        {
                            foreach (var educationRecord in EducationRecords)
                            {
                                if (!string.IsNullOrWhiteSpace(educationRecord.ExamTitle) &&
                                    !string.IsNullOrWhiteSpace(educationRecord.Institution))
                                {
                                    var eduInfo = new Educationalinfo
                                    {
                                        EmployeeID = emp.EmployeeID,
                                        ExamTitle = educationRecord.ExamTitle,
                                        Institution = educationRecord.Institution,
                                        Result = educationRecord.Result,
                                        PassingYear = educationRecord.PassingYear
                                    };
                                    dbContext.Educationalinfos.Add(eduInfo);
                                }
                            }
                        }
                        await dbContext.SaveChangesAsync();
                    }
                }

                return Json(new { success = true, message = "Employee saved successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving employee: " + ex.Message });
            }
        }

        private string? GetUploadFileName(Employeeinfo emp)
        {
            string? uniqueFileName = null;
            if (emp.photo != null)
            {
                var uploadsFolder = Path.Combine(webHost.WebRootPath, "Image");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + emp.photo.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    emp.photo.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var employee = await dbContext.Employeeinfos.FindAsync(id);
            if (employee != null)
            {
                if (!string.IsNullOrEmpty(employee.PhotoPath))
                {
                    var photoPath = Path.Combine(webHost.WebRootPath, employee.PhotoPath.TrimStart('/'));
                    if (System.IO.File.Exists(photoPath))
                    {
                        try
                        {
                            System.IO.File.Delete(photoPath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting file: {ex.Message}");
                        }
                    }
                }
                dbContext.Employeeinfos.Remove(employee);
                await dbContext.SaveChangesAsync();
                return Json(new { success = true, message = "Employee deleted successfully." });
            }
            return Json(new { success = false, message = "Employee not found!" });
        }

        [HttpPost]
        public async Task<IActionResult> BulkDelete([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return NotFound();

            foreach (var id in ids)
            {
                var employee = await dbContext.Employeeinfos.FindAsync(id);
                if (employee != null)
                {
                    // Delete photo if exists
                    if (!string.IsNullOrEmpty(employee.PhotoPath))
                    {
                        var photoPath = Path.Combine(webHost.WebRootPath, "Image", Path.GetFileName(employee.PhotoPath));
                        {
                            try
                            {
                                System.IO.File.Delete(photoPath);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error deleting file: {ex.Message}");
                            }
                        }
                    }
                    dbContext.Employeeinfos.Remove(employee);
                }
            }
            await dbContext.SaveChangesAsync();
            return Ok(new { success = true, message = "Employees deleted successfully." });
        }

        public async Task<IActionResult> Clear(int id)
        {
            //await dbContext.SaveChangesAsync();
            return (RedirectToAction("Index"));

        }

        public IActionResult AddPartial()
        {
            return PartialView("_AddPartial", new Designation());

        }
        public async Task<string> GetEmployeeNextID()
        {
            var allIds = await dbContext.Employeeinfos.Select(e => e.EmployeeID).ToListAsync();

            if (allIds.Count == 0)
            {
                return "001";
            }
            int maxId = allIds.Select(id => int.TryParse(id, out var num) ? num : 0).Max();

            int nextId = maxId + 1;
            return nextId.ToString("D3");
        }
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var employee = await dbContext.Employeeinfos
                .Include(e => e.Educationalinfos) // Include educational records
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

                if (employee != null)
                {
                    // Create a response object that includes educational records
                    var response = new
                    {
                        employeeID = employee.EmployeeID,
                        autoID = employee.AutoID,
                        name = employee.Name,
                        designation = employee.Designation,
                        department = employee.Department,
                        grossSalary = employee.GrossSalary,
                        joiningDate = employee.JoiningDate,
                        address = employee.Address,
                        phone = employee.Phone,
                        email = employee.Email,
                        photoPath = employee.PhotoPath,
                        // Add educational records
                        educationalRecords = employee.Educationalinfos?.Select(edu => new Educationalinfo
                        {
                            EducationalinfoID = edu.EducationalinfoID,
                            //AutoId = edu.AutoId,
                            ExamTitle = edu.ExamTitle,
                            Institution = edu.Institution,
                            Result = edu.Result,
                            PassingYear = edu.PassingYear
                        }).ToList() ?? new List<Educationalinfo>()
                    };

                    return Json(response);
                }
                return Json(null);
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        [HttpGet]
        public async Task<bool> IsDuplicate(string Name, string EmployeeID, string Phone)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Phone))
            {
                return (false);
            }
            string checkName = Name.Trim().ToLower();
            string checkPhone = Phone.Trim().ToLower();
            string checkEmployeeID = EmployeeID.Trim().ToLower();

            var isDuplicate = await dbContext.Employeeinfos.
                AnyAsync(e => e.Name.Trim().ToLower() == checkName &&
                e.EmployeeID.Trim().ToLower() != EmployeeID &&
                e.Phone.Trim().ToLower() == Phone);

            if (isDuplicate)
            {
                return true;
            }

            return false;
        }
        public async Task<JsonResult> GetallDev(string? search)
        {
            var query = dbContext.Devskills.AsQueryable();


            var data = await query.OrderByDescending(x => x.AutoId).ToListAsync();
            return Json(data);
        }

        public async Task<JsonResult> Getall(string? search, int page = 1)
        {
            var query1 = from emp in dbContext.Employeeinfos
                         join des in dbContext.Designations on emp.Designation equals des.DesignationId into desiGroup
                         from des in desiGroup.DefaultIfEmpty()
                         join dep in dbContext.Departments on emp.Department equals dep.DepartmentId into depGroup
                         from dep in depGroup.DefaultIfEmpty()

                         select new
                         {
                             emp.AutoID,
                             emp.EmployeeID,
                             emp.Name,
                             emp.Email,
                             emp.Phone,
                             emp.PhotoPath,
                             emp.GrossSalary,
                             emp.JoiningDate,
                             Designation = des.DesignationName,
                             Department = dep.DepartmentName
                         };

            var query = query1.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Name.Contains(search)
                || e.Email.Contains(search)
                || e.Phone.Contains(search));

            }
            var data = await query.OrderByDescending(x => x.AutoID).ToListAsync();
            return Json(data);
        }
        [HttpPost]
        public async Task<IActionResult> GetPaginated()
        {
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            var start = HttpContext.Request.Form["start"].FirstOrDefault();
            var length = HttpContext.Request.Form["length"].FirstOrDefault();
            var searchValue = HttpContext.Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

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
                            emp.Email,
                            emp.Phone,
                            emp.PhotoPath,
                            emp.GrossSalary,
                            emp.JoiningDate,
                            emp.DevSkills,
                            Designation = des.DesignationName,
                            Department = dep.DepartmentName
                        };

            // Filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(e =>
                    e.Name.Contains(searchValue) ||
                    e.Email.Contains(searchValue) ||
                    e.Phone.Contains(searchValue));
            }

            // Count total after filtering
            int totalRecords = await query.CountAsync();

            // Apply pagination
            var data = await query
                .OrderByDescending(x => x.AutoID)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            //Pocess DevSkills to get readable names
            var processedData = data.Select(item => new
            {
                item.AutoID,
                item.EmployeeID,
                item.Name,
                item.Email,
                item.Phone,
                item.PhotoPath,
                item.GrossSalary,
                item.JoiningDate,
                item.Designation,
                item.Department,
                DevSkills = GetDevSkillsNames(item.DevSkills)

            }).ToList();

            //Return in DataTables format

            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = processedData
            });
        }

        // Helper method to convert DevSkills IDs to readable names
        private string GetDevSkillsNames(string devSkillIds)
        {
            if (string.IsNullOrEmpty(devSkillIds))
                return "";

            try
            {
                // Split comma-separated IDs and convert to integers
                var skillIds = devSkillIds.Split(',')
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    //.Select(id => int.Parse(id.Trim()))
                    .ToList();

                if (!skillIds.Any())
                    return "";

                // Get skill names from Devskills table based on your table structure
                var skillNames = dbContext.Devskills
                    .Where(d => skillIds.Contains(d.DevId))
                    .Select(d => d.DevName) // Use DevName for full names or DevShortName for short names
                    .ToList();

                return string.Join(", ", skillNames);
            }
            catch (Exception)
            {
                return ""; // Return empty string if parsing fails
            }
        }

    }
    
}

    
        