
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAppointment.MVC.Data;
using SmartAppointment.MVC.Models;

namespace SmartAppointment.MVC.Controllers
{
    public class RegisterController : Controller
    {
        private readonly AppDbContext _context;

        public RegisterController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User account = new User();
                account.Email = model.Email;
                account.Name = model.Name;
                account.PasswordHash = model.Password;
                
                try
                {
                    _context.Users.Add(account);
                    _context.SaveChanges();

                    ModelState.Clear();
                    ViewBag.Message = $"{account.Name} registered successfully";
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Please enter unique Email or Password");
                    return View(model);
                }

            }
            return View(model);
        }

    }
}