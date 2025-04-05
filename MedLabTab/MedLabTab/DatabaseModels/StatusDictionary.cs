using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseModels;

[Table("StatusDictionary")]
public partial class StatusDictionary
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    public string StatusName { get; set; } = null!;

    [InverseProperty("StatusNavigation")]
    public virtual ICollection<TestHistory> TestHistories { get; set; } = new List<TestHistory>();
}
