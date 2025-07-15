using System.ComponentModel.DataAnnotations;

namespace SmartAppointment.MVC.ViewModels
{
    public class ConsultantCreateViewModel
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password should be at least 6 characters.")]
        public string Password { get; set; } 

        [Required]
        [StringLength(200)]
        public string ExpertiseArea { get; set; }

        [Required]
        [Range(0.01, 10000.00, ErrorMessage = "Hourly wage must be between 0.01 and 10000.")]
        [DataType(DataType.Currency)]
        public decimal HourlyRate { get; set; }
    }
}
