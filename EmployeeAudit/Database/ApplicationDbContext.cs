using EmployeeAudit.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAudit.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            modelBuilder.Entity<Employee>().ToTable("employees", "public");
        }
    }
}