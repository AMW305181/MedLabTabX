using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseModels;

[Table("TestHistory")]
public partial class TestHistory
{
    [Key]
    public int id { get; set; }

    public int VisitId { get; set; }

    public int TestId { get; set; }

    public int PatientId { get; set; }

    public int Status { get; set; }

    public int? AnalystId { get; set; }

    [ForeignKey("AnalystId")]
    [InverseProperty("TestHistoryAnalysts")]
    public virtual User? Analyst { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("TestHistoryPatients")]
    public virtual User Patient { get; set; } = null!;

    [ForeignKey("Status")]
    [InverseProperty("TestHistories")]
    public virtual StatusDictionary StatusNavigation { get; set; } = null!;

    [ForeignKey("TestId")]
    [InverseProperty("TestHistories")]
    public virtual Test Test { get; set; } = null!;

    [ForeignKey("VisitId")]
    [InverseProperty("TestHistories")]
    public virtual Visit Visit { get; set; } = null!;

    public string DisplayPatient => Patient?.Name + " " + Patient?.Surname;
    public string DisplayAnalyst => Analyst != null ? $"{Analyst.Name} {Analyst.Surname}" : "Brak analityka";
    public string DisplayNurse => Visit?.TimeSlot?.Nurse?.Name + " " + Visit?.TimeSlot?.Nurse?.Surname;
    public string DisplayTest => Test?.TestName;
    public string DisplayDate => Visit?.TimeSlot?.Date.ToString("dd.MM.yyyy") ?? "Brak daty";
    public string DisplayTime => Visit?.TimeSlot?.Time.ToString(@"hh\:mm") ?? "Brak godziny";
}
