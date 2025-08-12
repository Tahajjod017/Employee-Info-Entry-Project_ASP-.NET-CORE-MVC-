using System.Linq;
using EmployeeMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeMvc.Controllers
{
    public class DepartmentController : Controller

    {
        private ApplicationDBContext dBContext;

        public bool ISPartial { get; private set; }

        public DepartmentController(ApplicationDBContext _dBContext)
        {
            dBContext = _dBContext;
        }
        public async Task<IActionResult> Index(bool isPartial = false)
        {
            var Department = await dBContext.Departments.ToListAsync();
            ViewBag.Departments = Department;
            var NextID = await GetNextDepartmentId();
            ViewBag.nextId = NextID;
            
            if (isPartial)
            {
                ISPartial = true;
                    return PartialView(Department);
            }

            return View(Department);
        }
        public async Task<IActionResult> Getall()
        {
            var data = await dBContext.Departments.OrderByDescending(x=>x.AutoId).ToListAsync();
            return Json(data);
        }
        [HttpPost]
        public async Task<IActionResult> Save(Department department)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false });
            }
            string inputName = department.DepartmentName.Trim().ToLower();
            bool isDuplicate = await dBContext.Departments.AnyAsync(d => d.DepartmentName.Trim().ToLower() == inputName && d.DepartmentId != department.DepartmentId);

            if (isDuplicate)
            {
                TempData["ErrorMessage"] = "Data alrady exists with this name?";
                return RedirectToAction("Index");
            }
            if (department.AutoId == 0)
            {
                dBContext.Departments.Add(department);
                TempData["SuccessMessage"] = "Employee add successfully";
            }
            else
            {
                var existingDepartment = await dBContext.Departments.FindAsync(department.DepartmentId);
                if (existingDepartment != null)
                {
                    existingDepartment.DepartmentName = department.DepartmentName;
                    existingDepartment.AutoId = department.AutoId;

                }
                TempData["SuccessMessage"] = "Employee updated successfully";

            }
            await dBContext.SaveChangesAsync();
            return Json(new { success = true });
            //return RedirectToAction("Index");

        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var department = await dBContext.Departments.FindAsync(id);
            if (department != null)
            {
                dBContext.Departments.Remove(department);
                await dBContext.SaveChangesAsync();
                return Json(new { success = true, message = "Depatment deleted successfully!" });
            }
            return Json(new { success = false, message = "Department not found!" });

            //return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult>GetDepartment(string id)
        {
            var department = await dBContext.Departments.FindAsync(id);
            if ( department != null)
            {
                return Json(department);
            }
            return Json(null);
        }
        public async Task<string> GetNextDepartmentId()
        {
            var allIds = await dBContext.Departments.Select(d => d.DepartmentId).ToListAsync();
            if (allIds.Count == 0)
            {
                return "001"; // If no departments exist, start with DPT001
            }
           int maxId = allIds
                .Select(id => int.TryParse(id, out var num) ? num : 0)
                .Max();
            int nextId = maxId + 1;
            return nextId.ToString("D3"); // Format as DPT followed by zero-padded number
        }
        [HttpPost]
        public async Task<IActionResult> BulkDelete ([FromBody] List<string> ids)
        {
            if (ids == null)
            {
                return NotFound();                
            }
            foreach(var id in ids)
            {
                var department = await dBContext.Departments.FindAsync(id);
                if (department !=null)
                {
                    dBContext.Departments.Remove(department);
                    await dBContext.SaveChangesAsync();
                }  
            }
            
            return Ok();
        }
        [HttpPost]
        public async Task <IActionResult> IsDuplicate (string departmentName, string departmentId)
        { 
         if(string.IsNullOrEmpty(departmentName))
            {
                return Json(false);
            }
            string checkName = departmentName.Trim().ToLower();

            var isDuplicate = await dBContext.Departments.AnyAsync(d => d.DepartmentName.Trim().ToLower() == checkName && d.DepartmentId != departmentId);

            return Json(isDuplicate);
        }






    }
}
