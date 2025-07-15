using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartAppointment.MVC.Data;
using SmartAppointment.MVC.Models;
using SmartAppointment.MVC.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartAppointment.MVC.Controllers
{
    [Authorize(Roles = "User")]
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;
        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> CreateAppointment()
        {
            var consultants = await _context.Consultants.Include(c => c.user).ToListAsync();
            var availabilities = await _context.ConsultantAvailabilities
                .Where(a => !a.IsBooked)
                .ToListAsync();

            var avaliabilityList = availabilities.Select(a => new
            {
                a.ID,
                DisplayText = $"{a.DayOfWeek} | {a.StartTime} - {a.EndTime}"
            });

            ViewBag.Consultants = consultants;
            ViewBag.Availabilities = availabilities;

            return View(new AppointmentCreateViewModel());
        }
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment(AppointmentCreateViewModel model)
        {
           if(!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields.";
                return RedirectToAction(nameof(CreateAppointment));
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var availability = await _context.ConsultantAvailabilities
        .FirstOrDefaultAsync(a => a.ID == model.ConsultantAvailabilityID && !a.IsBooked);

            if (availability == null)
            {
                TempData["Error"] = "Selected time slot is no longer available.";
                return RedirectToAction(nameof(CreateAppointment));
            }
            var appointment = new Appointment
            {
                UserId = userId,
                ConsultantId = model.ConsultantId,
                ConsultantAvailabilityID = model.ConsultantAvailabilityID,
                AppointmentDate = DateTime.Now, 
                Notes = model.Notes
            };
            _context.Appointments.Add(appointment);
            availability.IsBooked = true;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Appointment booked successfully!";
            return RedirectToAction("MyAppointments");
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyAppointments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var appointments = await _context.Appointments
                .Include(a => a.Consultant)
                .ThenInclude(c => c.user)
                .Include(a => a.ConsultantAvailability)
                .Where(a => a.UserId == userId)
                .ToListAsync();

            return View(appointments);
        }
        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAppoitnment(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var appointment = await _context.Appointments
                 .Include(a => a.ConsultantAvailability)
                 .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (appointment == null)
            {
                TempData["Error"] = "Appointment not found.";
                return RedirectToAction(nameof(MyAppointments));
            }
            appointment.ConsultantAvailability.IsBooked = false;
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Appointment cancelled.";
            return RedirectToAction(nameof(MyAppointments));

        }

    }
}
