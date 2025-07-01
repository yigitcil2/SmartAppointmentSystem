using System.ComponentModel.DataAnnotations;

namespace SmartAppointment.MVC.ViewModels
{
    public class ConsultantCreateViewModel
    {
        public int ID { get; set; } // Consultant'ın ID'si

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; } // GÜVENLİK UYARISI: Frontend'de hashlemeyin, backend'de yapın!

        [Required]
        [StringLength(200)]
        public string ExpertiseArea { get; set; }

        [Required]
        [Range(0.01, 10000.00, ErrorMessage = "Saatlik ücret 0.01 ile 10000 arasında olmalıdır.")]
        [DataType(DataType.Currency)]
        public decimal HourlyRate { get; set; }
    }
}
