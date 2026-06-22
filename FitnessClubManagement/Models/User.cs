using System;
using System.Collections.Generic;

namespace FitnessClubManagement.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public decimal? WeightKgs { get; set; }

    public string? ExperienceLevel { get; set; }

    public int? TargetProtein { get; set; }

    public int? TargetWaterMl { get; set; }

    public string? Discipline { get; set; }
    public string? WeightClass { get; set; }
    public bool IsAvailableForSparring { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<NutritionLog> NutritionLogs { get; set; } = new List<NutritionLog>();

    public virtual ICollection<SparringRequest> SparringRequests { get; set; } = new List<SparringRequest>();
}
