using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeMvc.Models
{
    [Table("HRM_ATD_Shift")]
    public class HRMATDShift
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShiftCode { get; set; }
        [StringLength(50)]
        public string ShiftName { get; set; }
        [StringLength(250)]
        public string ShiftShortName { get; set; }
        public DateTime ShiftStartTime { get; set; }
        public DateTime ShiftEndTime { get; set; }
        public DateTime LateTime { get; set; }
        public DateTime AbsentTime { get; set; }
        public DateTime WEF { get; set; }
        [StringLength(500)]
        public string? Remarks { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
