using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessClubManagement.Models;
using FitnessClubManagement.ViewModels;

namespace FitnessClubManagement.Controllers
{
    // STRICT SECURITY: Only users with the "Trainer" role can access this.
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
        public async Task<IActionResult> Index()
        {
            // Securely get the logged-in trainer's name
            var trainerName = User.Identity?.Name;

            // Fetch only the workouts created by this specific trainer
            // .Include(w => w.Bookings) allows us to count how many people have booked
            var myWorkouts = await _context.Workouts
                .Include(w => w.Bookings)
                .Where(w => w.TrainerName == trainerName)
                .OrderBy(w => w.ScheduledTime)
                .ToListAsync();

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
                    // The system automatically assigns the logged-in trainer's name!
                    TrainerName = User.Identity!.Name!
                };

                _context.Workouts.Add(workout);
                await _context.SaveChangesAsync();

                // Teleport back to the dashboard after creating the session
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}