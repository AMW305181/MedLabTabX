using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseModels;

public partial class User
{
    [Key]
    public int id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Login { get; set; } = null!;

    public int UserType { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    public bool IsActive { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string Surname { get; set; } = null!;

    [StringLength(11)]
    [Unicode(false)]
    public string PESEL { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    [InverseProperty("Nurse")]
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    [InverseProperty("Analyst")]
    public virtual ICollection<TestHistory> TestHistoryAnalysts { get; set; } = new List<TestHistory>();

    [InverseProperty("Patient")]
    public virtual ICollection<TestHistory> TestHistoryPatients { get; set; } = new List<TestHistory>();

    [ForeignKey("UserType")]
    [InverseProperty("Users")]
    public virtual UserType UserTypeNavigation { get; set; } = null!;

    [InverseProperty("Patient")]
    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();

    public string StatusText => IsActive ? "Dezaktywuj" : "Aktywuj";
    public Brush StatusColor => IsActive ? Brushes.IndianRed : Brushes.LimeGreen;
}
