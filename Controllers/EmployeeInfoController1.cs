using System.Buffers;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
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
        public async Task<IActionResult> Save(Employeeinfo employee, string[] selectedDevSkills)
        {
            if (ModelState.IsValid)
            {
                bool isDuplicate = await IsDuplicate(employee.Name, employee.EmployeeID, employee.Phone);

                if (isDuplicate)
                {
                    TempData["ErrorMessage"] = "Data already exists with this name or phone number.";
                    return RedirectToAction("Index");
                }

                if (employee.AutoID == 0)
                {
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
                    emp.DevSkills = string.Join(",", selectedDevSkills);


                    dbContext.Employeeinfos.Add(emp);
                }

                //dbContext.Employeeinfos.Add(employee);
                else
                {
                    var emp = await dbContext.Employeeinfos.FindAsync(employee.AutoID);

                    if (emp != null)
                    {
                        if (employee.photo != null)
                        {
                            if (!string.IsNullOrEmpty(emp.PhotoPath))
                            {
                                var photoPath = Path.Combine(webHost.WebRootPath, "Image", Path.GetFileName(emp.PhotoPath));
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
                        emp.GrossSalary = string.IsNullOrWhiteSpace(employee.GrossSalary?.ToString()) ? null : employee.GrossSalary;
                    }
                }
                await dbContext.SaveChangesAsync();
            }
            return Json(new { success = true });
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
            var employee = await dbContext.Employeeinfos
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee != null)
            {
                return Json(employee);
            }
            return Json(null);
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

            // Return in DataTables format
            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }
    }
}

    
        