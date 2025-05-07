using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MedLabTab.DatabaseModels;

public partial class MedLabContext : DbContext
{
    public MedLabContext()
    {
    }

    public MedLabContext(DbContextOptions<MedLabContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CategoryDictionary> CategoryDictionaries { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<StatusDictionary> StatusDictionaries { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<TestHistory> TestHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    public virtual DbSet<Visit> Visits { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MedLab;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoryDictionary>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Category__3213E83FD19C0623");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Reports__3213E83FEA2DA7A6");

            entity.HasOne(d => d.Sample).WithMany(p => p.Reports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reports__SampleI__5CD6CB2B");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Schedule__3213E83F755348C9");

            entity.HasOne(d => d.Nurse).WithMany(p => p.Schedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Schedules__Nurse__300424B4");
        });

        modelBuilder.Entity<StatusDictionary>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__StatusDi__3213E83F443501F1");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Tests__3213E83F5F99CC46");

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.Tests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tests__Category__2D27B809");
        });

        modelBuilder.Entity<TestHistory>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__TestHist__3213E83FC3B070DA");

            entity.HasOne(d => d.Analyst).WithMany(p => p.TestHistoryAnalysts).HasConstraintName("FK__TestHisto__Analy__398D8EEE");

            entity.HasOne(d => d.Patient).WithMany(p => p.TestHistoryPatients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TestHisto__Patie__3A81B327");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.TestHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TestHisto__Statu__36B12243");

            entity.HasOne(d => d.Test).WithMany(p => p.TestHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TestHisto__TestI__37A5467C");

            entity.HasOne(d => d.Visit).WithMany(p => p.TestHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TestHisto__Visit__38996AB5");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Users__3213E83F9638323E");

            entity.HasOne(d => d.UserTypeNavigation).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__UserType__2A4B4B5E");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__UserType__3213E83F1294A3FC");
        });

        modelBuilder.Entity<Visit>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Visits__3213E83F6F38653F");

            entity.HasOne(d => d.Patient).WithMany(p => p.Visits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Visits__PatientI__32E0915F");

            entity.HasOne(d => d.TimeSlot).WithMany(p => p.Visits).HasConstraintName("FK__Visits__TimeSlot__33D4B598");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
