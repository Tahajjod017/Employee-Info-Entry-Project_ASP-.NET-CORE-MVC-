using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeMvc.Models
{
    public class 
        Educationalinfo
    {
        [Key]
        public int EducationalinfoId { get; set; }
        [ForeignKey("Employeeinfo")]
        public int EmployeeID { get; set; }
        public int AutoId { get; set; }
        [StringLength(100)]
        public string? Institution { get; set; }
        public string? ExamTitle { get; set; }
        [StringLength(50)]
        public string? Result { get; set; }
        [StringLength(50)]
        public int? PassingYear { get; set; }
        //Navigation Property
        public virtual Employeeinfo Employeeinfo { get; set; }

    }
}
