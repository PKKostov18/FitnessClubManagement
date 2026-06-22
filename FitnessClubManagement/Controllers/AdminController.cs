using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using FitnessClubManagement.Models;
using FitnessClubManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FitnessClubManagement.Controllers
{
    // STRICT SECURITY
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly FitnessClubDbContext _context;

        public AdminController(FitnessClubDbContext context)
        {
            _context = context;
        }

        // SHA256 Password Hashing Algorithm
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        // GET: /Admin/Index
        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var allUsers = await _context.Users.ToListAsync();

            ViewBag.TotalFighters = allUsers.Count(u => u.Role != "Admin" && u.Role != "Trainer");
            ViewBag.ActiveTrainers = allUsers.Count(u => u.Role == "Trainer");
            ViewBag.DeployedSessions = await _context.Workouts.CountAsync(w => w.ScheduledTime >= now);
            ViewBag.GlobalRosterLoad = await _context.Bookings.CountAsync(b => b.Workout!.ScheduledTime >= now);

            return View(allUsers);
        }

        // GET: /Admin/AddTrainer
        public IActionResult AddTrainer()
        {
            return View();
        }

        // POST: /Admin/AddTrainer
        [HttpPost]
        public async Task<IActionResult> AddTrainer(AddTrainerViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "This email is already in use by another user.");
                    return View(model);
                }

                var trainer = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    Role = "Trainer"
                };

                _context.Users.Add(trainer);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Trainer {model.Username} successfully deployed to the platform.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // POST: /Admin/ChangeRole
        [HttpPost]
        public async Task<IActionResult> ChangeRole(int userId, string newRole)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user != null && user.Role != "Admin")
            {
                user.Role = newRole;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Role for {user.Username} successfully updated to {newRole.ToUpper()}.";
            }
            return RedirectToAction("Index");
        }

        // POST: /Admin/DeleteUser
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user != null && user.Role != "Admin")
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "System account explicitly terminated and purged from database layers.";
            }
            return RedirectToAction("Index");
        }
    }
}