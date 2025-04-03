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

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<StatusDictionary> StatusDictionaries { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<TestHistory> TestHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    public virtual DbSet<Visit> Visits { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MedLabTab;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoryDictionary>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Category__3213E83F47E55F77");

            entity.Property(e => e.id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Schedule__3213E83F963D2FBA");

            entity.Property(e => e.id).ValueGeneratedNever();

            entity.HasOne(d => d.Nurse).WithMany(p => p.Schedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Schedules__Nurse__35BCFE0A");
        });

        modelBuilder.Entity<StatusDictionary>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__StatusDi__3213E83FC46BFD82");

            entity.Property(e => e.id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Tests__3213E83F093B8882");

            entity.Property(e => e.id).ValueGeneratedNever();

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.Tests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tests__Category__31EC6D26");
        });

        modelBuilder.Entity<TestHistory>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__TestHist__3213E83FF2EC6FEB");

            entity.Property(e => e.id).ValueGeneratedNever();

            entity.HasOne(d => d.Analyst).WithMany(p => p.TestHistoryAnalysts).HasConstraintName("FK__TestHisto__Analy__4CA06362");

            entity.HasOne(d => d.Patient).WithMany(p => p.TestHistoryPatients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TestHisto__Patie__4D94879B");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.TestHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TestHisto__Statu__49C3F6B7");

            entity.HasOne(d => d.Test).WithMany(p => p.TestHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TestHisto__TestI__4AB81AF0");

            entity.HasOne(d => d.Visit).WithMany(p => p.TestHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TestHisto__Visit__4BAC3F29");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Users__3213E83F9E76BFCC");

            entity.Property(e => e.id).ValueGeneratedNever();

            entity.HasOne(d => d.UserTypeNavigation).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__UserType__2E1BDC42");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__UserType__3213E83FD76E05DE");

            entity.Property(e => e.id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Visit>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Visits__3213E83F444D6A1D");

            entity.Property(e => e.id).ValueGeneratedNever();

            entity.HasOne(d => d.Patient).WithMany(p => p.Visits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Visits__PatientI__3E52440B");

            entity.HasOne(d => d.TimeSlot).WithMany(p => p.Visits).HasConstraintName("FK__Visits__TimeSlot__3F466844");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
