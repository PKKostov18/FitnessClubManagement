using FitnessClubManagement.Models;
using System.Security.Cryptography;
using System.Text;

namespace FitnessClubManagement.Data
{
    public static class DataSeeder
    {
        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public static void Initialize(FitnessClubDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any(u => u.Role == "Admin"))
            {
                return;
            }

            var adminUser = new User
            {
                Username = "SystemAdmin",
                Email = "admin@nexus.fit",
                PasswordHash = HashPassword("Admin123!"),
                Role = "Admin"
            };

            context.Users.Add(adminUser);
            context.SaveChanges();
        }
    }
}