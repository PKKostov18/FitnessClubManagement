using System;
using System.Collections.Generic;

namespace FitnessClubManagement.Models;

public partial class Workout
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string TrainerName { get; set; } = null!;

    public DateTime ScheduledTime { get; set; }

    public int Capacity { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
