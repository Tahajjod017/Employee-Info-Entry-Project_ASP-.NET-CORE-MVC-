using System.ComponentModel.DataAnnotations;

namespace EmployeeMvc.Models
{
    public class Designation
    {
        public int DesignationId { get; set; }
        [StringLength(50)]
        public string DesignationName { get; set; }
    }
}
