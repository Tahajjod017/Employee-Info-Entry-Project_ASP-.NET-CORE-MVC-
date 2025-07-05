using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeMvc.Models
{
    public class Department
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AutoId { get; set; }
        [Key]
        public string DepartmentId { get; set; }
        [StringLength(50)]
        public string DepartmentName { get; set; }
    }
}
