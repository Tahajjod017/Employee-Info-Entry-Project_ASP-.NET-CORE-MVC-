using EmployeeMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeMvc.Controllers
{
    public class DepartmentController : Controller

    {
        private ApplicationDBContext dBContext;
        public DepartmentController(ApplicationDBContext _dBContext)
        {
            dBContext = _dBContext;
        }
        public async Task<IActionResult> Index()
        {
            var Department = await dBContext.Departments.ToListAsync();
            ViewBag.Departments = Department;
            var NextID = await dBContext.Departments.MaxAsync(d => d.AutoId);
            return View(Department);
        }
        public async Task<IActionResult> Getall()
        {
            var data = await dBContext.Departments.ToListAsync();
            return Json(data);
        }
        [HttpPost]
        public async Task<IActionResult> Save(Department department)
        {
            if (!ModelState.IsValid)
                RedirectToAction("Index");
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
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Delete(string id)
        {
            var department = await dBContext.Departments.FindAsync(id);
            if (department != null)
            {
                dBContext.Departments.Remove(department);
                await dBContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }



    }
}
