using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseModels;

[Index("id", Name = "UQ__UserType__3213E83E5334E92F", IsUnique = true)]
public partial class UserType
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    public string TypeName { get; set; } = null!;

    [InverseProperty("UserTypeNavigation")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
