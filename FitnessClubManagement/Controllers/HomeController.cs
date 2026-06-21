using FitnessClubManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FitnessClubManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly FitnessClubDbContext _context;

        // Injecting the database context
        public HomeController(FitnessClubDbContext context)
        {
            _context = context;
        }

        // GET: /Home/Index (The Landing Page)
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Home/Schedule (The Public Schedule)
        public async Task<IActionResult> Schedule()
        {
            var upcomingWorkouts = await _context.Workouts
                .Include(w => w.Bookings)
                .Where(w => w.ScheduledTime > DateTime.Now)
                .OrderBy(w => w.ScheduledTime)
                .ToListAsync();

            // Ако има логнат потребител, вземаме неговите резервации
            List<int> userBookedWorkoutIds = new List<int>();
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out int userId))
                {
                    // Вземаме само ID-тата на тренировките, за които ТОЗИ потребител има запис
                    userBookedWorkoutIds = await _context.Bookings
                        .Where(b => b.UserId == userId)
                        .Select(b => b.WorkoutId.GetValueOrDefault())
                        .ToListAsync();
                }
            }

            // Подаваме списъка с резервираните ID-та към View-то чрез ViewBag
            ViewBag.UserBookings = userBookedWorkoutIds;

            return View(upcomingWorkouts);
        }
    }
}