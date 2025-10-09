using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeMvc.Models;
using System.Data;
using System.Runtime.InteropServices;
using Microsoft.Data.SqlClient;

namespace EmployeeMvc.Controllers
{
    public class RosterController : Controller
    {
        private readonly ApplicationDBContext dbContext;

        public RosterController(ApplicationDBContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public async Task<IActionResult> Index(List<string> SelectedNames, string shift, DateTime? dateFrom, DateTime? dateTo, int page = 1)
         {
            var query = dbContext.HRMATDRosterScheduleEntries.AsQueryable();
            if(SelectedNames != null && SelectedNames.Any())
            {
                //query = query.Where(r => SelectedNames.Contains(r.EmployeeName));
            }
            if (!string.IsNullOrEmpty(shift) && int.TryParse(shift, out int shiftCode))
            {
                query = query.Where(r => r.ShiftCode == shiftCode);
            }

            var data = query
       .OrderBy(e => e.Date)
       .Skip((page - 1) * 10)
       .Take(10)
       .ToList();

            //Load Employees name
            var employess = await dbContext.HRMEmployees
                .Select(e => e.Name)
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync();

            //Load Shift Names
            var shifts = await dbContext.HRMATDShifts
                .Select(s => s.ShiftName)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            ViewBag.Employess = dbContext.HRMEmployees.Select(e => e.Name).Distinct().ToList(); // Note: keeping your typo "Employess"
            ViewBag.Shifts = dbContext.HRMATDShifts.Select(s => s.ShiftName).Distinct().ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAll(int page = 1,
    int pageSize = 10,
    string? search = null,
    string? designation = null,
    string? shifts = null,
    string? dateFrom = null,
    string? dateTo = null,
            string? shiftF = null,
            List<string>? name = null,
            string? sortBy = "AI_ID",
            string? sortOrder = "asc")

        {
            try
            {
                var data = from roster in dbContext.HRMATDRosterScheduleEntries

                           join emp in dbContext.HRMEmployees on roster.EmployeeID equals emp.EmployeeID

                           join shift in dbContext.HRMATDShifts on roster.ShiftCode equals shift.ShiftCode

                           join des in dbContext.HRMDefDesignations on emp.DesignationCode equals des.DesignationCode into desGroup

                           from des in desGroup.DefaultIfEmpty()

                           select new
                           {
                               AI_ID = roster.AI_ID,
                               RosterScheduleCode = roster.RosterScheduleCode,
                               EmployeeID = emp.EmployeeID,
                               EmployeeName = emp.Name,
                               DesignationName = des != null ? des.DesignationName : "",
                               Date = roster.Date,
                               ShiftName = shift.ShiftName,
                               ShiftStartTime = shift.ShiftStartTime.ToString("HH:mm"),
                               ShiftEndTime = shift.ShiftEndTime.ToString("HH:mm"),
                           };
                if (!string.IsNullOrEmpty(search))
{
    data = data.Where(x =>
                x.EmployeeName.Contains(search) ||
                x.EmployeeID.Contains(search) ||
                x.DesignationName.Contains(search) ||
                x.ShiftName.Contains(search) ||
                x.Date.ToString().Contains(search)
    );

}
                //Apply Filters
                if (!string.IsNullOrEmpty(designation))
                {
                    data = data.Where(x => x.DesignationName == designation);
                }
                if (!string.IsNullOrEmpty(shifts))
                {
                    data = data.Where(x => x.ShiftName == shifts);
                }
                if (!string.IsNullOrEmpty(dateFrom) && DateTime.TryParse(dateFrom, out var fromDate))
                {
                    fromDate = fromDate.Date; // Ensure time is 00:00:00
                    data = data.Where(x => x.Date >= fromDate);
                }

                if (!string.IsNullOrEmpty(dateTo) && DateTime.TryParse(dateTo, out var toDate))
                {
                    toDate = toDate.Date.AddDays(1).AddTicks(-1); // End of day
                    data = data.Where(x => x.Date <= toDate);
                }
                if (name != null && name.Any())
                {
                    data = data.Where(x => name.Contains(x.EmployeeName));
                }
                if (!string.IsNullOrEmpty(shiftF))
                {
                    data = data.Where(x => x.ShiftName == (shiftF));
                }

                //Apply Sorting
                
                data = sortBy?.ToLower() switch
                {
                    "employeeid" => sortOrder?.ToLower() == "desc"
                    ? data.OrderByDescending(x => x.EmployeeID)
                    : data.OrderBy(x => x.EmployeeID),

                    "employeename" => sortOrder?.ToLower() == "desc"
                    ? data.OrderByDescending(x => x.EmployeeName)
                    : data.OrderBy(x => x.EmployeeName),

                    "designationname" => sortOrder?.ToLower() == "desc"
                    ? data.OrderByDescending(x => x.DesignationName)
                    : data.OrderBy(x => x.DesignationName),

                    "date" => sortOrder?.ToLower() == "desc"
                    ? data.OrderByDescending(x => x.Date)
                    : data.OrderBy(x => x.Date),

                    "shiftname" => sortOrder?.ToLower() == "desc"
                    ? data.OrderByDescending(x => x.ShiftName)
                    : data.OrderBy(x => x.ShiftName),

                    _ => sortOrder?.ToLower() == "desc"
                    ? data.OrderByDescending(x => x.AI_ID)
                    : data.OrderBy(r => r.AI_ID),

                };




                var totalRecords = await data.CountAsync(); // filtered data count
                var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                var pagedData = await data
                    //.OrderBy(r => r.AI_ID)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                //Apply formating After TolistAsync()

                var processedData = pagedData.Select((x, index) => new
                {
                    SerialNumber = (page - 1) * pageSize + index + 1,
                    x.AI_ID,
                    x.RosterScheduleCode,
                    x.EmployeeID,
                    x.EmployeeName,
                    x.DesignationName,
                    Date = x.Date.ToString("yyyy-MM-dd"),
                    x.ShiftName,
                    x.ShiftStartTime,
                    x.ShiftEndTime
                }).ToList();

                return Json(new
                {
                    success = true,
                    data = processedData,
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


    }
}