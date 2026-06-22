using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FitnessClubManagement.Models;

namespace FitnessClubManagement.Controllers
{
    // STRICT SECURITY
    [Authorize]
    public class UserController : Controller
    {
        private readonly FitnessClubDbContext _context;

        public UserController(FitnessClubDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdStr!);
        }

        // GET: /User/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            int userId = GetCurrentUserId();

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

            bool alreadyBooked = await _context.Bookings.AnyAsync(b => b.WorkoutId == workoutId && b.UserId == userId);
            if (alreadyBooked)
            {
                TempData["ErrorMessage"] = "You have already secured a spot for this session.";
                return RedirectToAction("Schedule", "Home");
            }

            var workout = await _context.Workouts.Include(w => w.Bookings).FirstOrDefaultAsync(w => w.Id == workoutId);
            if (workout == null) return NotFound();

            if (workout.Bookings.Count >= workout.Capacity)
            {
                TempData["ErrorMessage"] = "This session has reached maximum capacity.";
                return RedirectToAction("Schedule", "Home");
            }

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

        // GET: /User/Sparring
        public async Task<IActionResult> Sparring()
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var currentUser = await _context.Users.FindAsync(currentUserId);

            var availableFighters = await _context.Users
                .Where(u => u.Role == "User" && u.Id != currentUserId && u.IsAvailableForSparring)
                .ToListAsync();

            var unreadCounts = await _context.ChatMessages
                .Where(m => m.ReceiverId == currentUserId && !m.IsRead)
                .GroupBy(m => m.SenderId)
                .Select(g => new { SenderId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.SenderId, x => x.Count);

            ViewBag.CurrentUser = currentUser;
            ViewBag.UnreadCounts = unreadCounts;

            return View(availableFighters);
        }

        // POST: /User/UpdateSparringProfile
        [HttpPost]
        public async Task<IActionResult> UpdateSparringProfile(string discipline, string weightClass, string experience, bool isAvailable)
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var user = await _context.Users.FindAsync(currentUserId);

            if (user != null)
            {
                user.Discipline = discipline;
                user.WeightClass = weightClass;
                user.ExperienceLevel = experience;
                user.IsAvailableForSparring = isAvailable;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Combat protocol updated. Matchmaking parameters saved and synchronized.";
            }

            return RedirectToAction("Sparring");
        }

        // GET: /User/Chat
        public async Task<IActionResult> Chat(int receiverId)
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var receiver = await _context.Users.FindAsync(receiverId);
            if (receiver == null) return RedirectToAction("Sparring");

            var messages = await _context.ChatMessages
                .Include(m => m.Sender)
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == receiverId) ||
                            (m.SenderId == receiverId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            var unreadMessages = messages.Where(m => m.ReceiverId == currentUserId && m.SenderId == receiverId && !m.IsRead).ToList();
            if (unreadMessages.Any())
            {
                unreadMessages.ForEach(m => m.IsRead = true);
                await _context.SaveChangesAsync();
            }

            ViewBag.Receiver = receiver;
            ViewBag.CurrentUserId = currentUserId;

            return View(messages);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadMessageCount()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Json(0);

            int currentUserId = int.Parse(userIdStr);
            var count = await _context.ChatMessages.CountAsync(m => m.ReceiverId == currentUserId && !m.IsRead);

            return Json(count);
        }

        // POST: /User/SendMessage
        [HttpPost]
        public async Task<IActionResult> SendMessage(int receiverId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return RedirectToAction("Chat", new { receiverId });

            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var message = new ChatMessage
            {
                SenderId = currentUserId,
                ReceiverId = receiverId,
                Content = content.Trim(),
                Timestamp = DateTime.Now
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Chat", new { receiverId });
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