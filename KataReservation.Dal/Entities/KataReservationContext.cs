using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KataReservation.Dal.Entities;

public partial class KataReservationContext : DbContext
{
    public KataReservationContext()
    {
    }

    public KataReservationContext(DbContextOptions<KataReservationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BookingEntity> Bookings { get; set; }

    public virtual DbSet<PersonEntity> People { get; set; }

    public virtual DbSet<RoomEntity> Rooms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookingEntity>(entity =>
        {
            entity.ToTable("Booking");

            entity.HasOne(d => d.Person).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Person");

            entity.HasOne(d => d.Room).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Room");
        });

        modelBuilder.Entity<PersonEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_User_1");

            entity.ToTable("Person");

            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RoomEntity>(entity =>
        {
            entity.ToTable("Room");

            entity.Property(e => e.RoomName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
