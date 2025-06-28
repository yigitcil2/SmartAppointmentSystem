// SmartAppointment.MVC.Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using SmartAppointment.MVC.Models;

namespace SmartAppointment.MVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Consultant> Consultants { get; set; }
        public DbSet<ConsultantAvailability> ConsultantAvailabilities { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User ve Consultant arasýndaki iliþki
            modelBuilder.Entity<Consultant>()
                .HasOne(c => c.user)
                .WithOne() // User'ýn bu tarafta bir navigation property'si yok
                .HasForeignKey<Consultant>(c => c.userId)
                .OnDelete(DeleteBehavior.NoAction); // <-- Burasý

            // Consultant ve ConsultantAvailability arasýndaki iliþki
            modelBuilder.Entity<ConsultantAvailability>()
                .HasOne(ca => ca.consultant)
                .WithMany(c => c.Availabilities)
                .HasForeignKey(ca => ca.ConsultantID)
                .OnDelete(DeleteBehavior.NoAction); // <-- Burasý

            // Appointment ve User (müþteri) arasýndaki iliþki
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany() // User'ýn Appointment tarafýnda bir navigation property'si yok
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction); // <-- Burasý

            // Appointment ve Consultant arasýndaki iliþki
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.consultant)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ConsultantId)
                .OnDelete(DeleteBehavior.NoAction); // <-- Burasý

            // Appointment ve ConsultantAvailability arasýndaki iliþki (Hata veren yer burasýydý)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.ConsultantAvailability)
                .WithMany() // ConsultantAvailability'nin Appointment tarafýnda bir navigation property'si yok
                .HasForeignKey(a => a.ConsultantAvailabilityID)
                .OnDelete(DeleteBehavior.NoAction); // <-- Burasý


            // Unique kýsýtlamalar (varsa, her zaman önerilir)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}