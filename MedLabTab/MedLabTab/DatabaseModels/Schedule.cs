using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseModels;

[Index("id", Name = "UQ__Schedule__3213E83E703C9DB0", IsUnique = true)]
public partial class Schedule
{
    [Key]
    public int id { get; set; }

    public int NurseId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    [ForeignKey("NurseId")]
    [InverseProperty("Schedules")]
    public virtual User Nurse { get; set; } = null!;

    [InverseProperty("TimeSlot")]
    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
