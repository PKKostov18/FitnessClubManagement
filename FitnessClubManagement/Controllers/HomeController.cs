using FitnessClubManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FitnessClubManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly FitnessClubDbContext _context;
        public HomeController(FitnessClubDbContext context)
        {
            _context = context;
        }

        // GET: /Home/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Home/Schedule
        public async Task<IActionResult> Schedule()
        {
            var upcomingWorkouts = await _context.Workouts
                .Include(w => w.Bookings)
                .Where(w => w.ScheduledTime > DateTime.Now)
                .OrderBy(w => w.ScheduledTime)
                .ToListAsync();

            List<int> userBookedWorkoutIds = new List<int>();
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out int userId))
                {
                    userBookedWorkoutIds = await _context.Bookings
                        .Where(b => b.UserId == userId)
                        .Select(b => b.WorkoutId.GetValueOrDefault())
                        .ToListAsync();
                }
            }

            ViewBag.UserBookings = userBookedWorkoutIds;

            return View(upcomingWorkouts);
        }
    }
}