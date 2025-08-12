using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeMvc.Models
{
    public class Devskill
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int AutoId { get; set; }
        [Key]
        [StringLength(3)]
        public string DevId { get; set; }
        [StringLength(50)]
        public string DevName { get; set; }
        [StringLength(20)]
        public string? DevShortName { get; set; }
    }
}
