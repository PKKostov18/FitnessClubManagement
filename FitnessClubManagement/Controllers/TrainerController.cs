using FitnessClubManagement.Models;
using FitnessClubManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FitnessClubManagement.Controllers
{
    // STRICT SECURITY
    [Authorize(Roles = "Trainer")]
    public class TrainerController : Controller
    {
        private readonly FitnessClubDbContext _context;

        // Injecting the database context
        public TrainerController(FitnessClubDbContext context)
        {
            _context = context;
        }

        // GET: /Trainer/Index
        public async Task<IActionResult> Index(string filter = "active")
        {
            var trainerName = User.Identity?.Name;
            var now = DateTime.Now;
            var query = _context.Workouts
                .Include(w => w.Bookings)
                .Where(w => w.TrainerName == trainerName);

            if (filter == "active")
            {
                query = query.Where(w => w.ScheduledTime >= now);
            }
            else if (filter == "expired")
            {
                query = query.Where(w => w.ScheduledTime < now);
            }

            var myWorkouts = await query
                .OrderByDescending(w => w.ScheduledTime >= now)
                .ThenBy(w => w.ScheduledTime)
                .ToListAsync();

            ViewBag.CurrentFilter = filter;

            return View(myWorkouts);
        }

        // GET: /Trainer/AddWorkout
        public IActionResult AddWorkout()
        {
            return View(new AddWorkoutViewModel());
        }

        // POST: /Trainer/AddWorkout
        [HttpPost]
        public async Task<IActionResult> AddWorkout(AddWorkoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                var workout = new Workout
                {
                    Title = model.Title,
                    Capacity = model.Capacity,
                    ScheduledTime = model.ScheduledTime,
                    TrainerName = User.Identity!.Name!
                };

                _context.Workouts.Add(workout);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: /Trainer/Roster/5
        public async Task<IActionResult> Roster(int id)
        {
            var trainerName = User.Identity?.Name;

            var workout = await _context.Workouts
                .Include(w => w.Bookings)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(w => w.Id == id && w.TrainerName == trainerName);

            // Security check
            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }

        // GET: /[Controller]/Profile
        public async Task<IActionResult> Profile()
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var user = await _context.Users.FindAsync(currentUserId);

            if (user == null) return NotFound();

            return View(user);
        }

        // POST: /[Controller]/UpdateProfile
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string username, string email, decimal? weightKgs)
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var user = await _context.Users.FindAsync(currentUserId);

            if (user != null)
            {
                user.Username = username;
                user.Email = email;
                user.WeightKgs = weightKgs;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Operative parameters successfully updated and synchronized.";
            }

            return RedirectToAction("Profile");
        }
    }
}