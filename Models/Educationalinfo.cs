using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EmployeeMvc.Models
{
    public class 
        Educationalinfo
    {
        [Key]
        public int EducationalinfoID { get; set; }
        [ForeignKey("Employeeinfo")]
        public string EmployeeID { get; set; }

        public int AutoId { get; set; }
        [StringLength(100)]
        public string? ExamTitle { get; set; }
        public string? Institution { get; set; }
       
        public decimal? Result { get; set; }
        public int? PassingYear { get; set; }
        //Navigation Property
        [JsonIgnore]
        public virtual Employeeinfo? Employeeinfo { get; set; }

    }
}
