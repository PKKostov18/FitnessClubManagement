using System;
using System.Collections.Generic;

namespace FitnessClubManagement.Models;

public partial class NutritionLog
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public DateOnly LogDate { get; set; }

    public int ProteinConsumed { get; set; }

    public int WaterConsumedMl { get; set; }

    public virtual User? User { get; set; }
}
