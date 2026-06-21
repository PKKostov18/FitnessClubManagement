using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FitnessClubManagement.Models;
using FitnessClubManagement.ViewModels;

namespace FitnessClubManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly FitnessClubDbContext _context;

        public AccountController(FitnessClubDbContext context)
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

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the email is already taken
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    Role = "User" // Default role for new registrations
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            return View(model);
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = HashPassword(model.Password);
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == hashedPassword);

                if (user != null)
                {
                    // Create claims (the user's digital ID card)
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // 1. Define explicit session properties (strictly non-persistent)
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = false, // Tells the browser NOT to save this cookie to the hard drive
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30), // Session automatically expires after 30 minutes of inactivity
                        AllowRefresh = true
                    };

                    // 2. Sign in the user securely WITH the authProperties
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties); // <-- This is the crucial part that was missing!

                    // --- SMART ROLE-BASED REDIRECT ---
                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (user.Role == "Trainer")
                    {
                        return RedirectToAction("Index", "Trainer");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                // Invalid credentials
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            return View(model);
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}