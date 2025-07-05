using EmployeeMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EmployeeMvc.Controllers
{
    public class DesignationController : Controller
    {
        private ApplicationDBContext dbContext;
        public DesignationController(ApplicationDBContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public async Task<IActionResult>Index()
        {
            var NextID = await GetDesignationNextID();
            ViewBag.NextID = NextID;
            var Designation = await dbContext.Designations.ToListAsync();
            return View(Designation);
        }

        public async Task<JsonResult> Getall()
        {
            var data = await dbContext.Designations.ToListAsync();
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult>Save(Designation employee)
        {
            if (ModelState.IsValid)
            {
                if (employee.AutoId == 0)
                    
                    dbContext.Designations.Add(employee);
                else
                {
                    var existingEmployee = await dbContext.Designations.FindAsync(employee.DesignationId);
                    if (existingEmployee != null)
                    {
                        existingEmployee.DesignationName = employee.DesignationName;
                        existingEmployee.DesingnationShortname = employee.DesingnationShortname;
                      
                    }

                }
                await dbContext.SaveChangesAsync();
                
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult>Delete(string id)
        {
            var designation = await dbContext.Designations.FindAsync(id);
                if(designation != null)
            {
                dbContext.Designations.Remove(designation);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index"); 
        }
        [HttpGet]
        public async Task<IActionResult> GetDesignation(string id)
        {
             var designation= await dbContext.Designations.FindAsync(id);
            if (designation != null)
            {
                return Json(designation);
            }
            return Json(null);
        }
        private async Task<string> GetDesignationNextID()
        {
            var allIds = await dbContext.Designations.Select(d => d.DesignationId).ToListAsync();

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
                var designation = await dbContext.Designations.FindAsync(id);
                if (designation != null)
                {
                    dbContext.Designations.Remove(designation);
                }
            }
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }

}
