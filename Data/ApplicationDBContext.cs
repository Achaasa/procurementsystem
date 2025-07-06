using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using procurementsystem.Entities;
namespace procurementsystem.Data
{
    public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : DbContext(options)
    {

        public DbSet<ProcurementItem> ProcurementItems { get; set; }
        public DbSet<ProcurementHistory> ProcurementHistories { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure CreatedBy relationship (User -> ProcurementItem)
            modelBuilder.Entity<ProcurementItem>()
                .HasOne(p => p.CreatedBy)  // CreatedBy relationship
                .WithMany(u => u.CreatedProcurementItems)  // User has many ProcurementItems (CreatedBy)
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);  // Don't delete associated user if procurement item is deleted

            // Configure UpdatedBy relationship (User -> ProcurementItem)
            modelBuilder.Entity<ProcurementItem>()
                .HasOne(p => p.UpdatedBy)  // UpdatedBy relationship
                .WithMany(u => u.UpdatedProcurementItems)  // User has many ProcurementItems (UpdatedBy)
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.SetNull);  // Set UpdatedBy to null when user is deleted

            // Configure ProcurementHistory relationships
            modelBuilder.Entity<ProcurementHistory>()
                .HasOne(ph => ph.ProcurementItem)  // Link to ProcurementItem
                .WithMany()  // ProcurementItem has no navigation property for history
                .HasForeignKey(ph => ph.ProcurementItemId)
                .OnDelete(DeleteBehavior.Cascade);  // If ProcurementItem is deleted, delete associated history entries

            modelBuilder.Entity<ProcurementHistory>()
                .HasOne(ph => ph.UpdatedBy)  // UpdatedBy relationship in ProcurementHistory
                .WithMany(u => u.UpdatedProcurementHistories)  // User has many ProcurementHistories (UpdatedBy)
                .HasForeignKey(ph => ph.UpdatedById)
                .OnDelete(DeleteBehavior.SetNull);  // Set UpdatedBy to null when user is deleted

            // Additional configurations (optional)
            // For example, you can set unique constraints, indexes, etc.
        }

    }
}