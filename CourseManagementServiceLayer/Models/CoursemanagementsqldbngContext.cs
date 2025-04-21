using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementServiceLayer.Models;

public partial class CoursemanagementsqldbngContext : DbContext
{
    public CoursemanagementsqldbngContext()
    {
    }

    public CoursemanagementsqldbngContext(DbContextOptions<CoursemanagementsqldbngContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CourseTable> CourseTables { get; set; }

    public virtual DbSet<EnrollmentTable> EnrollmentTables { get; set; }

    public virtual DbSet<LearnerTable> LearnerTables { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CourseTable>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__CourseTa__C92D71A753BA238F");

            entity.ToTable("CourseTable");

            entity.Property(e => e.CourseCost).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CourseName).HasMaxLength(100);

        });

        modelBuilder.Entity<EnrollmentTable>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Enrollme__7F68771BA317F2BB");

            entity.ToTable("EnrollmentTable");

            entity.Property(e => e.EnrollmentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Course).WithMany(p => p.EnrollmentTables)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__Cours__628FA481");

            entity.HasOne(d => d.Learner).WithMany(p => p.EnrollmentTables)
                .HasForeignKey(d => d.LearnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Enrollmen__Learn__619B8048");
        });

        modelBuilder.Entity<LearnerTable>(entity =>
        {
            entity.HasKey(e => e.LearnerId).HasName("PK__LearnerT__67ABFCDAEA6C1F64");

            entity.ToTable("LearnerTable");

            entity.Property(e => e.LearnerContact).HasMaxLength(20);
            entity.Property(e => e.LearnerEmail).HasMaxLength(100);
            entity.Property(e => e.LearnerName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
