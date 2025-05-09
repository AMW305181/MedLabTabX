using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseModels;

public partial class Report
{
    [Key]
    public int id { get; set; }

    public int SampleId { get; set; }

    public DateOnly LastUpdateDate { get; set; }

    public TimeOnly LastUpdateTime { get; set; }

    public string Results { get; set; } = null!;

    [ForeignKey("SampleId")]
    [InverseProperty("Reports")]
    public virtual TestHistory Sample { get; set; } = null!;
}
