using System.ComponentModel.DataAnnotations;

namespace EmployeeMvc.Models
{
    public class Employeeinfo
    {
        [Key]
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public int Department { get; set; }
        public DateTime JoiningDate { get; set; }
        public string Address { get; set; }
        public string? Email { get; set; }
        public int Designation { get; set; }
        public int GrossSalary { get; set; }
        public string Phone {  get; set; }
        public string Photo {  get; set; }

    }
}
