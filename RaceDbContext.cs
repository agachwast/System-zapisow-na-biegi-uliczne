using Microsoft.EntityFrameworkCore;
using System;

namespace RaceRegistration.Models
{
    public class RaceDbContext : DbContext
    {
        public RaceDbContext(DbContextOptions<RaceDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Registration> Registrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracja modelu User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Konfiguracja modelu Registration
            modelBuilder.Entity<Registration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AgeGroup).IsRequired();
                entity.Property(e => e.Gender).IsRequired().HasMaxLength(1);
                entity.Property(e => e.Distance).IsRequired().HasMaxLength(10);
                entity.Property(e => e.RegistrationDate).HasDefaultValueSql("GETDATE()");
                
                // Relacja z użytkownikiem
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
} 