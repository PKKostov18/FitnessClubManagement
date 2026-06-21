using System.ComponentModel.DataAnnotations;

namespace FitnessClubManagement.ViewModels
{
    public class AddWorkoutViewModel
    {
        [Required(ErrorMessage = "Workout title is required.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Scheduled time is required.")]
        public DateTime ScheduledTime { get; set; } = DateTime.Today.AddDays(1).AddHours(12);

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 50, ErrorMessage = "Capacity must be between 1 and 50 fighters.")]
        public int Capacity { get; set; }
    }
}