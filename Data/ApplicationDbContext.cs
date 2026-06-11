using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VehicleShield.Models;

namespace VehicleShield.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Estimate> Estimates { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // One-to-one between ApplicationUser and Customer
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Customer)
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Customer relationships
            builder.Entity<Customer>()
                .HasMany(c => c.Vehicles)
                .WithOne(v => v.Customer)
                .HasForeignKey(v => v.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Customer>()
                .HasMany(c => c.Policies)
                .WithOne(p => p.Customer)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Customer>()
                .HasMany(c => c.Estimates)
                .WithOne(e => e.Customer)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Customer>()
                .HasMany(c => c.Billings)
                .WithOne(b => b.Customer)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicle relationships
            builder.Entity<Vehicle>()
                .HasMany(v => v.Policies)
                .WithOne(p => p.Vehicle)
                .HasForeignKey(p => p.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Policy relationships
            builder.Entity<Policy>()
                .HasMany(p => p.Billings)
                .WithOne(b => b.Policy)
                .HasForeignKey(b => b.PolicyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Policy>()
                .HasMany(p => p.Claims)
                .WithOne(c => c.Policy)
                .HasForeignKey(c => c.PolicyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
