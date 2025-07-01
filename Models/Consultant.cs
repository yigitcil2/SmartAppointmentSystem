namespace SmartAppointment.MVC.Models
{
    public class Consultant
    {
        public int ID { get; set; }
        public int userId { get; set; } //Foreign Key
        public User user { get; set; }
        public string ExpertiseArea { get; set; }
        public decimal HourlyRate { get; set; }
        public ICollection<ConsultantAvailability> Availabilities { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
