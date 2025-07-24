using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeMvc.Models
{
    public class Employeeinfo
    {
        [Key]
        public int AutoID { get; set; } 
        public string EmployeeID { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(50)]
        public string? Department { get; set; } = string.Empty;
        public DateTime? JoiningDate { get; set; } = DateTime.MinValue;
        public string? Address { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        [StringLength(50)]
        public string? Designation { get; set; } = string.Empty;
        public int? GrossSalary { get; set; } = 0;
        [StringLength (20)]
        public string Phone {  get; set; }
        public string? PhotoPath {  get; set; } = string.Empty;
        [NotMapped]
        public IFormFile? photo { get; set; }

    }
}
