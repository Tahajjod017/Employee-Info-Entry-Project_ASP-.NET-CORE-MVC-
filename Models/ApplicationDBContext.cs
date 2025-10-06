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
        public DbSet<Educationalinfo> Educationalinfos { get; set; }
        //public DbSet<HRM_ATD_RosterScheduleEntry> HRM_ATD_RosterScheduleEntries { get; set; }
        public DbSet<HRMATDRosterScheduleEntry> HRMATDRosterScheduleEntries { get; set; }
        public DbSet<HRMDefDesignation> HRMDefDesignations { get; set; }
        public DbSet<HRMEmployee> HRMEmployees { get; set; }
        public DbSet<HRMATDShift> HRMATDShifts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Department seed data
            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = "001", DepartmentName = "Software Department" },
                new Department { DepartmentId = "002", DepartmentName = "Quality Department" },
                new Department { DepartmentId = "003", DepartmentName = "Human Resources" },
                new Department { DepartmentId = "004", DepartmentName = "Finance" },
                new Department { DepartmentId = "005", DepartmentName = "Marketing" },
                new Department { DepartmentId = "006", DepartmentName = "Sales" },
                new Department { DepartmentId = "007", DepartmentName = "Administration" },
                new Department { DepartmentId = "008", DepartmentName = "Accounts" }
          

                ); 

            // Designation seed data
            modelBuilder.Entity<Designation>().HasData(
                new Designation { DesignationId = "001", DesignationName = "Software Developer" },
                new Designation { DesignationId = "002", DesignationName = "Manager" },
                new Designation { DesignationId = "003", DesignationName = "Team Leader" },
                new Designation { DesignationId = "004", DesignationName = "Senior Developer" },
                new Designation { DesignationId = "005", DesignationName = "Junior Developer" },
                new Designation { DesignationId = "006", DesignationName = "Business Analyst" },
                new Designation { DesignationId = "007", DesignationName = "Chief Technology Officer" },
                new Designation { DesignationId = "008", DesignationName = "Quality Assurance" },
                new Designation { DesignationId = "009", DesignationName = "Administration Officer" },
                new Designation { DesignationId = "010", DesignationName = "Executive Marketing" },
                new Designation { DesignationId = "011", DesignationName = "Senior Sales " },
                new Designation { DesignationId = "012", DesignationName = "Business Development Manager" }
            );

            // Development Skills seed data
            modelBuilder.Entity<Devskill>().HasData(
                new Devskill { DevId = "001", DevName = "C# Programming", DevShortName = "C#" },
                new Devskill { DevId = "002", DevName = "JavaScript", DevShortName = "JS" },
                new Devskill { DevId = "003", DevName = "ASP .NET CORE", DevShortName = "CORE" },
                new Devskill { DevId = "004", DevName = "SQL Database", DevShortName = "SQL" },
                new Devskill { DevId = "005", DevName = "SQL Server Management Studio", DevShortName = "SQL Server" },
                new Devskill { DevId = "006", DevName = ".NET Framework", DevShortName = ".NET" },
                new Devskill { DevId = "007", DevName = "ASP.NET MVC", DevShortName = "MVC" },
                new Devskill { DevId = "008", DevName = "Web API Development", DevShortName = "API" },
                new Devskill { DevId = "009", DevName = "HTML/CSS", DevShortName = "HTML" },
                new Devskill { DevId = "010", DevName = "Angular Framework", DevShortName = "Angular" },
                new Devskill { DevId = "011", DevName = "React Library", DevShortName = "React" },
                new Devskill { DevId = "012", DevName = "Python Programming", DevShortName = "Python" },
                new Devskill { DevId = "013", DevName = "C++ Programming", DevShortName = "C++" }
            );

            // Configure relationships if needed
            // Example: If you have foreign key relationships

            // Configure string lengths and constraints
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DepartmentId);
                entity.Property(e => e.DepartmentId).HasMaxLength(10);
                entity.Property(e => e.DepartmentName).HasMaxLength(100);
            });

            modelBuilder.Entity<Devskill>(entity =>
            {
                entity.HasKey(e => e.DevId);
                entity.Property(e => e.DevId).HasMaxLength(3);
                entity.Property(e => e.DevName).HasMaxLength(50);
                entity.Property(e => e.DevShortName).HasMaxLength(20);
            });
        }
    }
}
