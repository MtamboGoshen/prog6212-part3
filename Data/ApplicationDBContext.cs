using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaim.Models;

namespace ContractMonthlyClaim.Data
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        public DbSet<Claim> Claims { get; set; }

        // ADD THIS METHOD TO FIX THE DECIMAL WARNINGS
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This is important for Identity

            // Configure the precision for your decimal properties
            modelBuilder.Entity<Claim>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.HourlyRate).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.HoursWorked).HasColumnType("decimal(18, 2)");
            });
        }
    }
}