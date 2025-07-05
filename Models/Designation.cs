using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeMvc.Models
{
    public class Designation
    {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AutoId { get; set; }
        [Key]
        [StringLength(3)]
        public string DesignationId { get; set; }
        [StringLength(50)]
        public string DesignationName { get; set; }
        [StringLength(20)]
        public string? DesingnationShortname  { get; set; }
    }
}
