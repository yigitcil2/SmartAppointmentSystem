using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAppointment.MVC.Data;
using SmartAppointment.MVC.Models;
using System.Security.Claims;

namespace SmartAppointment.MVC.Controllers
{
    [Authorize(Roles = "Consultant")]
    public class ConsultantController : Controller
    {
        private readonly AppDbContext _context;
        public ConsultantController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var consultants = await _context.Consultants.Include(c=>c.user).Where(c=>c.user.Role == "Consultant").ToListAsync();
            return View();
        }
       
        [Authorize(Roles = "Consultant")]
        public  async Task<IActionResult> MyAppointments()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;    //userId of the user that logged in the system.
            int userId = int.Parse(userIdString);

            var consultant = await _context.Consultants.FirstOrDefaultAsync(c => c.userId == userId);
            if (consultant == null)
            {
                return NotFound(); 
            }
            var appointments = await _context.Appointments
                .Where(a => a.ConsultantId == consultant.ID)
                .Include(a => a.User)
                .ToListAsync();
            return View(appointments);
        }
        [Authorize(Roles = "Consultant")]
        public async Task<IActionResult> ManageAvailability()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); 
            }
            int userIdInt = int.Parse(userId);
            var consultant = await _context.Consultants
                .Include(a => a.user)
                .FirstOrDefaultAsync(c => c.userId == userIdInt);
            //We also load the User object associated with Include.
            //With FirstOrDefaultAsync we get the first advisor whose user ID matches (otherwise it returns null).
            if(consultant == null)
            {
                return NotFound("Consultant not found.");
            }
            var availabilities = await _context.ConsultantAvailabilities
                .Where(a => a.ConsultantID == consultant.ID)
                .ToListAsync();

            return View(availabilities);    
        }
        [HttpGet]
        [Authorize(Roles = "Consultant")]
        public IActionResult CreateAvailability()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Consultant")]

        public async Task<IActionResult> CreateAvailability(ConsultantAvailability  model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            int userIdInt = int.Parse(userId);

            var consultant = await _context.Consultants
                .FirstOrDefaultAsync(c => c.userId == userIdInt);
            if (consultant == null) return NotFound();

            model.ConsultantID = consultant.ID;
            _context.ConsultantAvailabilities.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Availability added successfully!";
            return RedirectToAction(nameof(ManageAvailability));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Consultant")]
        public async Task<IActionResult> DeleteAvailability(int id)
        {
            // Finding the user logged in
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userId)) return Unauthorized();
            int userIdInt = int.Parse(userId);

            // Finding the consultant represents user
            var consultant = await _context.Consultants
                .FirstOrDefaultAsync (c => c.userId == userIdInt);
            if (consultant == null) return NotFound();

            var availability = await _context.ConsultantAvailabilities
                .FirstOrDefaultAsync(a => a.ID == id && a.ConsultantID == consultant.ID);
            if (availability == null)
                return NotFound("Availability not found or does not belong to you.");

            _context.ConsultantAvailabilities.Remove(availability);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Availability deleted.";
            return RedirectToAction(nameof(ManageAvailability));
        }

        [HttpGet]
        [Authorize(Roles = "Consultant")]

        public async Task<IActionResult> EditAvailability(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            int userIdInt = int.Parse(userId);

            var consultant = await _context.Consultants
                .FirstOrDefaultAsync(c => c.userId == userIdInt);
            if (consultant == null) return NotFound();

            var availability = await _context.ConsultantAvailabilities
                .FirstOrDefaultAsync(a => a.ID == id && a.ConsultantID == consultant.ID);

            if (availability == null) return NotFound();

            return View(availability);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> EditAvailability(int id, ConsultantAvailability model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            int userIdInt = int.Parse(userId);

            var consultant = await _context.Consultants
                .FirstOrDefaultAsync(c => c.userId == userIdInt);
            if (consultant == null) return NotFound();

            var availability = await _context.ConsultantAvailabilities
                .FirstOrDefaultAsync(a => a.ID == id && a.ConsultantID == consultant.ID);
            if (availability == null) return NotFound();

            availability.StartDate = model.StartDate;
            availability.EndDate = model.EndDate;
            availability.DayOfWeek = model.DayOfWeek;
            availability.StartTime = model.StartTime;
            availability.EndTime = model.EndTime;
            availability.IsRecurring = model.IsRecurring;

            try
            {
                _context.ConsultantAvailabilities.Update(availability);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Availability updated successfully!";
                return RedirectToAction(nameof(ManageAvailability));
            }
            catch (DbUpdateConcurrencyException)
            {
                return View(model); 
            }
        }

    }
}
