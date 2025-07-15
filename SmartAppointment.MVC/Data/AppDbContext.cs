
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
            
            modelBuilder.Entity<Consultant>()
                .HasOne(c => c.user)
                .WithOne() 
                .HasForeignKey<Consultant>(c => c.userId)
                .OnDelete(DeleteBehavior.NoAction); 

            
            modelBuilder.Entity<ConsultantAvailability>()
                .HasOne(ca => ca.consultant)
                .WithMany(c => c.Availabilities)
                .HasForeignKey(ca => ca.ConsultantID)
                .OnDelete(DeleteBehavior.NoAction); 

            
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany() 
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction); 

            
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Consultant)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ConsultantId)
                .OnDelete(DeleteBehavior.NoAction); 

            
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.ConsultantAvailability)
                .WithMany() 
                .HasForeignKey(a => a.ConsultantAvailabilityID)
                .OnDelete(DeleteBehavior.NoAction); 


            
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}