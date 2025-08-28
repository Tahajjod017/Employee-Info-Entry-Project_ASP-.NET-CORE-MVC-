using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EmployeeMvc.Models
{
    public class Employeeinfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AutoID { get; set; }
        [Key]
        public string EmployeeID { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(50)]
        public string? Department { get; set; } = string.Empty;
        public string? DevSkills { get; set; } = string.Empty;
        public DateTime? JoiningDate { get; set; } = DateTime.MinValue;
        public string? Address { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        [StringLength(50)]
        public string? Designation { get; set; } = string.Empty;
        public int? GrossSalary { get; set; } = 0;
        [StringLength (20)]
        public string Phone {  get; set; }
        public string? PhotoPath {  get; set; } = string.Empty;
        [NotMapped]
        public IFormFile? photo { get; set; }
        //Navigation for Fogen key
        [JsonIgnore]
        public virtual ICollection<Educationalinfo> Educationalinfos { get; set; } = new List<Educationalinfo>();


    }
}
