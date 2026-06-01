using System;
using System.Collections.Generic;

namespace FitnessClubManagement.Models;

public partial class SparringRequest
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Description { get; set; }

    public DateTime PreferredDate { get; set; }

    public bool IsActive { get; set; }

    public virtual User? User { get; set; }
}
