using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FitnessClubManagement.Models;

public partial class FitnessClubDbContext : DbContext
{
    public FitnessClubDbContext()
    {
    }

    public FitnessClubDbContext(DbContextOptions<FitnessClubDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<NutritionLog> NutritionLogs { get; set; }

    public virtual DbSet<SparringRequest> SparringRequests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Workout> Workouts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=FitnessClubDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bookings__3214EC07702A7FD2");

            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Bookings__UserId__5070F446");

            entity.HasOne(d => d.Workout).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Bookings__Workou__5165187F");
        });

        modelBuilder.Entity<NutritionLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Nutritio__3214EC07D8CA078C");

            entity.Property(e => e.LogDate).HasDefaultValueSql("(CONVERT([date],getdate()))");

            entity.HasOne(d => d.User).WithMany(p => p.NutritionLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Nutrition__UserI__5535A963");
        });

        modelBuilder.Entity<SparringRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sparring__3214EC072C396F03");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PreferredDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.SparringRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__SparringR__UserI__5AEE82B9");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0738EC26ED");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534276DE8DE").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.ExperienceLevel).HasMaxLength(30);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("User");
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.WeightKgs).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<Workout>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Workouts__3214EC0763A0B223");

            entity.Property(e => e.ScheduledTime).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.TrainerName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
