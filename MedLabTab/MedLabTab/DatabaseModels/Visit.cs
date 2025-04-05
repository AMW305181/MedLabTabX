using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseModels;

public partial class Visit
{
    [Key]
    public int id { get; set; }

    public float Cost { get; set; }

    public bool PaymentStatus { get; set; }

    public bool IsActive { get; set; }

    public int PatientId { get; set; }

    public int? TimeSlotId { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("Visits")]
    public virtual User Patient { get; set; } = null!;

    [InverseProperty("Visit")]
    public virtual ICollection<TestHistory> TestHistories { get; set; } = new List<TestHistory>();

    [ForeignKey("TimeSlotId")]
    [InverseProperty("Visits")]
    public virtual Schedule? TimeSlot { get; set; }
}
