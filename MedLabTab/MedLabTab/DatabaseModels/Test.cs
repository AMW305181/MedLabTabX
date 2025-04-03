using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseModels;

[Index("id", Name = "UQ__Tests__3213E83E8F042D8E", IsUnique = true)]
public partial class Test
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    public string TestName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public float Price { get; set; }

    public int Category { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("Category")]
    [InverseProperty("Tests")]
    public virtual CategoryDictionary CategoryNavigation { get; set; } = null!;

    [InverseProperty("Test")]
    public virtual ICollection<TestHistory> TestHistories { get; set; } = new List<TestHistory>();
}
