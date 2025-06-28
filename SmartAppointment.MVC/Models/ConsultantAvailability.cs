namespace SmartAppointment.MVC.Models
{
    public class ConsultantAvailability
    {
        public int ID { get; set; }
        public int ConsultantID { get; set; }
        public Consultant consultant { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsBooked { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsRecurring { get; set; }
    }
}
