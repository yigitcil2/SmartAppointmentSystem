using System.Security.Cryptography.X509Certificates;

namespace SmartAppointment.MVC.Models;

public class User
{
    public int ID { get; set; }
    public String Name { get; set; }
    public String Email { get; set; }
    public string PasswordHash { get; set; }
    public String Role { get; set; } = "User";
}

