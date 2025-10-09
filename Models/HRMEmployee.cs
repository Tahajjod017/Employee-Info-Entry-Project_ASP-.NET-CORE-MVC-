using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeMvc.Models
{
    [Table("HRM_Employee")]
    public class HRMEmployee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AI_ID { get; set; }

        [StringLength(50)]
        public string EmployeeID { get; set;  }

        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(50)]
        public string DesignationCode { get; set; }

        //public object EmployeeName { get; internal set; }
    }
}
