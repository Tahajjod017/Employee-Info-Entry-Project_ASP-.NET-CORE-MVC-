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
        public string Department { get; set; }
        public DateTime JoiningDate { get; set; }
        public string Address { get; set; }
        public string? Email { get; set; }
        [StringLength(50)]
        public string Designation { get; set; }
        public int GrossSalary { get; set; }
        [StringLength (20)]
        public string? Phone {  get; set; }
        public string? PhotoPath {  get; set; }
        [NotMapped]
        public IFormFile? photo { get; set; }

    }
}
