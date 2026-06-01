using System;
using System.Collections.Generic;

namespace FitnessClubManagement.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? WorkoutId { get; set; }

    public DateTime? BookingDate { get; set; }

    public virtual User? User { get; set; }

    public virtual Workout? Workout { get; set; }
}
