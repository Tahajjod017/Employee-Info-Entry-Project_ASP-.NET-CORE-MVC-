using System.ComponentModel.DataAnnotations;

namespace EmployeeMvc.Models
{
    public class Designation
    {
        [Key]
        [StringLength(3)]
        public string DesignationId { get; set; }
        [StringLength(50)]
        public string DesignationName { get; set; }
        [StringLength(20)]
        public string? DesingnationShortname  { get; set; }
    }
}
