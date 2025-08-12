using Microsoft.EntityFrameworkCore;

namespace EmployeeMvc.Models
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        public DbSet<Employeeinfo> Employeeinfos { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Devskill> Devskills { get; set; }
    }
}
