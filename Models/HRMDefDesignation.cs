using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeMvc.Models
{
    [Table("HRM_Def_Designation")]
    public class HRMDefDesignation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AI_ID { get; set; }
        [StringLength(50)]
        public string DesignationCode { get; set; }
        [StringLength(100)]
        public string? DesignationName { get; set; }
        [StringLength(50)]
        public string? DesignationShortName { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? ModifyDate { get; set; }

    }
}
