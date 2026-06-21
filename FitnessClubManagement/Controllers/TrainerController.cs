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
        public async Task<IActionResult> Index(string filter = "active")
        {
            var trainerName = User.Identity?.Name;
            var now = DateTime.Now;

            // Базова заявка за тренировките на текущия треньор
            var query = _context.Workouts
                .Include(w => w.Bookings)
                .Where(w => w.TrainerName == trainerName);

            // Прилагане на филтър в зависимост от избора
            if (filter == "active")
            {
                query = query.Where(w => w.ScheduledTime >= now);
            }
            else if (filter == "expired")
            {
                query = query.Where(w => w.ScheduledTime < now);
            }

            // Сортиране: Първо активните (бъдещи) тренировки, след това по хронологичен ред
            var myWorkouts = await query
                .OrderByDescending(w => w.ScheduledTime >= now) // True (активни) отиват най-отгоре
                .ThenBy(w => w.ScheduledTime)                  // След това сортираме по време
                .ToListAsync();

            // Запазваме текущия филтър във ViewBag, за да го визуализираме в изгледа
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

        // GET: /Trainer/Roster/5
        public async Task<IActionResult> Roster(int id)
        {
            var trainerName = User.Identity?.Name;

            // Fetch the workout, include the bookings, and THEN include the User for each booking
            var workout = await _context.Workouts
                .Include(w => w.Bookings)
                    .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(w => w.Id == id && w.TrainerName == trainerName);

            // Security check: Ensure the workout exists and belongs to this specific trainer
            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }
    }
}