using System.Linq;
using EmployeeMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeMvc.Controllers
{
    public class DepartmentController : Controller
    {
        private ApplicationDBContext dbContext;
        public DepartmentController(ApplicationDBContext _dbContext)
        {
            dbContext = _dbContext;
        }
        private bool ISPartial = false;

        public async Task<IActionResult> Index(bool isPartial = false)
        {
            var NextID = await GetNextDepartmentId();
            ViewBag.NextID = NextID;
            var Department = await dbContext.Departments.ToListAsync();

            if (isPartial)
            {
                ISPartial = true;
                return PartialView(Department);
            }
            return View(Department);
        }

        public async Task<JsonResult> Getall(string? search)
        {
            var query = dbContext.Departments.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.DepartmentName.Contains(search) ||
                                         x.DepartmentId.Contains(search) ||
                                         x.DepartmentShortName.Contains(search)
                );
            }

            var data = await query.OrderBy(x => x.AutoId).ToListAsync();
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> Save(Department department)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false });

            string inputName = department.DepartmentName.Trim().ToLower();
            var isDuplicate = IsDuplicate(inputName, department.DepartmentId);

            if (isDuplicate)
            {
                return RedirectToAction("Index");
            }

            if (department.AutoId == 0)
            {
                dbContext.Departments.Add(department);
                TempData["SuccessMessage"] = "Department added successfully!";
            }
            else
            {
                var existingDepartment = await dbContext.Departments.FindAsync(department.DepartmentId);
                if (existingDepartment != null)
                {
                    existingDepartment.DepartmentName = department.DepartmentName;
                    existingDepartment.DepartmentShortName = department.DepartmentShortName;
                }
                TempData["SuccessMessage"] = "Data Updated Successfully!";
            }
            await dbContext.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var department = await dbContext.Departments.FindAsync(id);
            if (department != null)
            {
                dbContext.Departments.Remove(department);
                await dbContext.SaveChangesAsync();
                return Json(new { success = true, message = "Department deleted successfully!" });
            }
            return Json(new { success = false, message = "Department not found!" });
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartment(string id)
        {
            var department = await dbContext.Departments.FindAsync(id);
            if (department != null)
            {
                return Json(department);
            }
            return Json(null);
        }

        public async Task<string> GetNextDepartmentId()
        {
            var allIds = await dbContext.Departments.Select(d => d.DepartmentId).ToListAsync();

            if (allIds.Count == 0)
            {
                return "001";
            }
            int maxId = allIds
                .Select(id => int.TryParse(id, out var num) ? num : 0)
                .Max();

            int nextId = maxId + 1;
            return nextId.ToString("D3");
        }

        [HttpPost]
        public async Task<IActionResult> BulkDelete([FromBody] List<string> ids)
        {
            if (ids == null)
                return NotFound();

            foreach (var id in ids)
            {
                var department = await dbContext.Departments.FindAsync(id);
                if (department != null)
                {
                    dbContext.Departments.Remove(department);
                }
            }
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public bool IsDuplicate(string departmentName, string departmentId)
        {
            if (string.IsNullOrEmpty(departmentName))
            {
                return false;
            }
            string checkName = departmentName.Trim().ToLower();

            var isDuplicate = dbContext.Departments.Any(d => d.DepartmentName.Trim().ToLower() == checkName /*&& d.DepartmentId != departmentId*/);
            return isDuplicate;
        }
    }
}