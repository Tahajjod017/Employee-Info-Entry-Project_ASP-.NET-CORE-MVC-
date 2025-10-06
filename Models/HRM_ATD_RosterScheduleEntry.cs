
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EmployeeMvc.Models
{
    public class HRM_ATD_RosterScheduleEntry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal AI_ID { get; set; }
       
        [StringLength(10)]
        public string EmployeeID { get; set; }
        public DateTime Date { get; set; }

        [StringLength(50)]
        public string Remarks { get; set; }

        public DateTime? EntryDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
