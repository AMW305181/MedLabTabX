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

    public int PatientId { get; set; }

    public int NurseId { get; set; }

    public int AnalystId { get; set; }

    public DateOnly CreationDate { get; set; }

    public TimeOnly CreationTime { get; set; }

    public string Results { get; set; } = null!;

    [ForeignKey("AnalystId")]
    [InverseProperty("ReportAnalysts")]
    public virtual User Analyst { get; set; } = null!;

    [ForeignKey("NurseId")]
    [InverseProperty("ReportNurses")]
    public virtual User Nurse { get; set; } = null!;

    [ForeignKey("PatientId")]
    [InverseProperty("ReportPatients")]
    public virtual User Patient { get; set; } = null!;

    [ForeignKey("SampleId")]
    [InverseProperty("Reports")]
    public virtual TestHistory Sample { get; set; } = null!;
}
