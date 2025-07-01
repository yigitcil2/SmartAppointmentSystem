using System.Security.Cryptography.Pkcs;

namespace SmartAppointment.MVC.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public Consultant consultant { get; set; }
        public int ConsultantId { get; set; }

        public int ConsultantAvailabilityID { get; set; }
        public ConsultantAvailability ConsultantAvailability { get; set; }

    }
}
