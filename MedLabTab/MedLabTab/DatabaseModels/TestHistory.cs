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

    [InverseProperty("Sample")]
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    [ForeignKey("Status")]
    [InverseProperty("TestHistories")]
    public virtual StatusDictionary StatusNavigation { get; set; } = null!;

    [ForeignKey("TestId")]
    [InverseProperty("TestHistories")]
    public virtual Test Test { get; set; } = null!;

    [ForeignKey("VisitId")]
    [InverseProperty("TestHistories")]
    public virtual Visit Visit { get; set; } = null!;
}
