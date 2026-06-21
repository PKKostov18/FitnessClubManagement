using System.ComponentModel.DataAnnotations;

namespace FitnessClubManagement.ViewModels
{
    public class AddTrainerViewModel
    {
        [Required(ErrorMessage = "Trainer name is required.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Temporary password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}