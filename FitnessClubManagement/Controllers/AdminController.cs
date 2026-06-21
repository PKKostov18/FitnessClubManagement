using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using FitnessClubManagement.Models;
using FitnessClubManagement.ViewModels;

namespace FitnessClubManagement.Controllers
{
    // STRICT SECURITY: Only users with the "Admin" role can access any action in this controller.
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
        // Displays the main command center and a list of all users
        public IActionResult Index()
        {
            // Fetch all users from the database to display in the dashboard
            var allUsers = _context.Users.ToList();
            return View(allUsers);
        }

        // GET: /Admin/AddTrainer
        // Loads the form to add a new trainer
        public IActionResult AddTrainer()
        {
            return View();
        }

        // POST: /Admin/AddTrainer
        // Processes the submitted form
        [HttpPost]
        public async Task<IActionResult> AddTrainer(AddTrainerViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the email is already taken
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "This email is already in use by another user.");
                    return View(model);
                }

                // Create the new Trainer entity
                var trainer = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    Role = "Trainer" // Hardcoding the role specifically for this action
                };

                _context.Users.Add(trainer);
                await _context.SaveChangesAsync();

                // Redirect back to the dashboard after successful creation
                return RedirectToAction("Index");
            }

            // If validation fails, return the form with error messages
            return View(model);
        }
    }
}