using EmployeeMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeMvc.Controllers
{
    public class DesignationController : Controller
    {
        private ApplicationDBContext dbContext;
        public DesignationController(ApplicationDBContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public IActionResult Index()
        {
            var Designation = dbContext.Designations.ToListAsync();
            return View();
        }
    }
}
