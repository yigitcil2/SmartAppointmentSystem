using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SmartAppointment.MVC.Data;
using SmartAppointment.MVC.Models;
using SmartAppointment.MVC.ViewModels;

namespace SmartAppointment.MVC.Controllers
{
    public class AdminController : Controller
    {

        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        //Listing consultants
        public async Task<IActionResult> ManageConsultants()
        {
            var consultants = await _context.Consultants.Include(c => c.user).ToListAsync();
            return View(consultants);
        }
        [HttpGet]
        public IActionResult CreateConsultant()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConsultant(ConsultantCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            model.Password = hashedPassword;

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = model.Password,
                Role = "Consultant"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var consultant = new Consultant
            {
                userId = user.ID,
                ExpertiseArea = model.ExpertiseArea,
                HourlyRate = model.HourlyRate,

            };
            _context.Consultants.Add(consultant);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Consultant succesfully added.";
            return RedirectToAction(nameof(ManageConsultants));
        }
        [HttpGet]
        public async Task<IActionResult> EditConsultant(int id)
        {
            var consultant = await _context.Consultants.Include(c => c.user).FirstOrDefaultAsync(c => c.ID == id);
            if (consultant == null)
            {
                return NotFound();
            }
            var model = new ConsultantCreateViewModel
            {
                Name = consultant.user.Name,
                Email = consultant.user.Email,
                ExpertiseArea = consultant.ExpertiseArea,
                HourlyRate = consultant.HourlyRate
            };
            return View(model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConsultant(int id, ConsultantCreateViewModel model)
        {
            if(id != model.ID)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var consultant = await _context.Consultants.Include(c => c.user).FirstOrDefaultAsync(c => c.ID == id);
            if (consultant == null)
            {
                return NotFound();
            }
            consultant.user.Name = model.Name;
            consultant.user.Email = model.Email;
            // Password Updating
            consultant.ExpertiseArea = model.ExpertiseArea;            
            consultant.HourlyRate = model.HourlyRate;

            try
            {
                _context.Update(consultant);
                _context.Update(consultant.user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Consultant succesfully updated.";
                return RedirectToAction(nameof(ManageConsultants));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConsultantExists(consultant.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        private bool ConsultantExists(int id)
        {
            return _context.Consultants.Any(e => e.ID == id);
        }
        [HttpPost, ActionName("DeleteConsultant")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConsultantConfirmed(int id)
        {
            var consultant = await _context.Consultants.Include(c => c.user).FirstOrDefaultAsync(c => c.ID == id);
            if (consultant == null)
            {
                return NotFound();
            }
           
            _context.Consultants.Remove(consultant);
            _context.Users.Remove(consultant.user);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Consultant successfully deleted.";
            return RedirectToAction(nameof(ManageConsultants));
        }

    }
    }

