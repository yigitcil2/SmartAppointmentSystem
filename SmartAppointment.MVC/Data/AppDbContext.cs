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
            // User ve Consultant aras�ndaki ili�ki
            modelBuilder.Entity<Consultant>()
                .HasOne(c => c.user)
                .WithOne() // User'�n bu tarafta bir navigation property'si yok
                .HasForeignKey<Consultant>(c => c.userId)
                .OnDelete(DeleteBehavior.NoAction); // <-- Buras�

            // Consultant ve ConsultantAvailability aras�ndaki ili�ki
            modelBuilder.Entity<ConsultantAvailability>()
                .HasOne(ca => ca.consultant)
                .WithMany(c => c.Availabilities)
                .HasForeignKey(ca => ca.ConsultantID)
                .OnDelete(DeleteBehavior.NoAction); // <-- Buras�

            // Appointment ve User (m��teri) aras�ndaki ili�ki
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany() // User'�n Appointment taraf�nda bir navigation property'si yok
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction); // <-- Buras�

            // Appointment ve Consultant aras�ndaki ili�ki
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.consultant)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ConsultantId)
                .OnDelete(DeleteBehavior.NoAction); // <-- Buras�

            // Appointment ve ConsultantAvailability aras�ndaki ili�ki (Hata veren yer buras�yd�)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.ConsultantAvailability)
                .WithMany() // ConsultantAvailability'nin Appointment taraf�nda bir navigation property'si yok
                .HasForeignKey(a => a.ConsultantAvailabilityID)
                .OnDelete(DeleteBehavior.NoAction); // <-- Buras�


            // Unique k�s�tlamalar (varsa, her zaman �nerilir)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}