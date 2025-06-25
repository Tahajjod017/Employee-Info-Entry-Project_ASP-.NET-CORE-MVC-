using EmployeeMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace EmployeeMvc.Controllers
{
    public class EmployeeInfoController1 : Controller
    {
        private readonly ApplicationDBContext _dbContext;
        private const int PageSize = 10;
        public EmployeeInfoController1(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index(int page = 1, string search = "")
        // Get Department and Designation for dropdown

        {
            ViewBag.Departments = await _dbContext.Departments.ToListAsync();
            ViewBag.Designation = await _dbContext.Designations.ToListAsync();
            //Query empliyess with search functionality
            var query = _dbContext.Employeeinfos.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Name.Contains(search)
                || e.Email.Contains(search)
                || e.Phone.Contains(search));
            }
            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            var employee = await query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.viewpage = totalPages;
            ViewBag.viewrecords = totalRecords;
            ViewBag.viewsize = PageSize;
            ViewBag.viewsearch = search;
            ViewBag.Hasprevious = page < 1;
            ViewBag.Hasforward = page < totalPages;

            return View(employee);

        }
        [HttpPost]
        public async Task<IActionResult> Save(Employeeinfo employee)
        {
            if (ModelState.IsValid)
            {
                if (employee.EmployeeId == 0)
                    _dbContext.Employeeinfos.Add(employee);
                else
                {
                    _dbContext.Employeeinfos.Update(employee);
                }
                await _dbContext.SaveChangesAsync();
            }
            return View(RedirectToAction("Index"));

        }
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _dbContext.Employeeinfos.FindAsync(id);
            if (employee == null)
            {
                return NotFound();

            }
            return Json(employee);

        }
        public async Task<IActionResult> Cleare(int id)
        {
            return View(RedirectToAction("Index"));
        }

    }
}