using System.Security.Cryptography.Pkcs;

namespace SmartAppointment.MVC.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public Consultant Consultant { get; set; }
        public int ConsultantId { get; set; }
        public TimeSpan ConsultantWorkingHourStart { get; set; }
        public TimeSpan ConsultantWorkingHourEnd { get; set; }
        public string Notes { get; set; }
        public int ConsultantAvailabilityID { get; set; }
        public ConsultantAvailability ConsultantAvailability { get; set; }

    }
}
