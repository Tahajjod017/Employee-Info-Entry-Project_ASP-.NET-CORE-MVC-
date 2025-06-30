using System.ComponentModel.DataAnnotations;

namespace EmployeeMvc.Models
{
    public class Department
    {
        public string DepartmentId { get; set; }
        [StringLength(50)]
        public string DepartmentName { get; set; }
    }
}
