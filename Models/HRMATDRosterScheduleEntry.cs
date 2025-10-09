using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmployeeMvc.Models
{

    [Table("HRM_ATD_RosterScheduleEntry")]
    public class HRMATDRosterScheduleEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal AI_ID { get; set; }

        [StringLength(50)]
        public string RosterScheduleCode { get; set; }

        [StringLength(10)]
        public string EmployeeID { get; set; }
        public DateTime Date { get; set; }
        
        public int ShiftCode { get; set; }

        public string Remarks { get; set; }

        public DateTime? EntryDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        //public string EmployeeName { get; internal set; }
    }
}
