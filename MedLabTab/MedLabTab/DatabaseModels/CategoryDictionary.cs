using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseModels;

[Table("CategoryDictionary")]
public partial class CategoryDictionary
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    public string CategoryName { get; set; } = null!;

    [InverseProperty("CategoryNavigation")]
    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
