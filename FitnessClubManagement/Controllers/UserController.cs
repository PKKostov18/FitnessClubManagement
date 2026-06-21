using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FitnessClubManagement.Models;

namespace FitnessClubManagement.Controllers
{
    // STRICT SECURITY: Must be logged in to access anything in this controller
    [Authorize]
    public class UserController : Controller
    {
        private readonly FitnessClubDbContext _context;

        public UserController(FitnessClubDbContext context)
        {
            _context = context;
        }

        // Helper method to securely get the logged-in user's ID
        private int GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdStr!);
        }

        // GET: /User/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            int userId = GetCurrentUserId();

            // Fetch only the future bookings belonging to this specific user
            var myBookings = await _context.Bookings
                .Include(b => b.Workout)
                .Where(b => b.UserId == userId && b.Workout!.ScheduledTime > DateTime.Now)
                .OrderBy(b => b.Workout!.ScheduledTime)
                .ToListAsync();

            return View(myBookings);
        }

        // POST: /User/BookSession
        [HttpPost]
        public async Task<IActionResult> BookSession(int workoutId)
        {
            int userId = GetCurrentUserId();

            // 1. Check if the user already booked this session
            bool alreadyBooked = await _context.Bookings.AnyAsync(b => b.WorkoutId == workoutId && b.UserId == userId);
            if (alreadyBooked)
            {
                TempData["ErrorMessage"] = "You have already secured a spot for this session.";
                return RedirectToAction("Schedule", "Home");
            }

            // 2. Fetch the workout and check capacity
            var workout = await _context.Workouts.Include(w => w.Bookings).FirstOrDefaultAsync(w => w.Id == workoutId);
            if (workout == null) return NotFound();

            if (workout.Bookings.Count >= workout.Capacity)
            {
                TempData["ErrorMessage"] = "This session has reached maximum capacity.";
                return RedirectToAction("Schedule", "Home");
            }

            // 3. Create the booking
            var booking = new Booking
            {
                UserId = userId,
                WorkoutId = workoutId,
                BookingDate = DateTime.Now
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Spot secured successfully! See you on the mats.";
            return RedirectToAction("Dashboard");
        }

        // POST: /User/CancelBooking
        [HttpPost]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            int userId = GetCurrentUserId();
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Your booking was canceled.";
            }

            return RedirectToAction("Dashboard");
        }
    }
}