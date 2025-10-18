using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Hotel.Models;

public partial class HotelDbContext : DbContext
{
    public HotelDbContext()
    {
    }

    public HotelDbContext(DbContextOptions<HotelDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AvailableRoomsPerType> AvailableRoomsPerTypes { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DiscountUsage> DiscountUsages { get; set; }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<GuestsWithChildrenInformation> GuestsWithChildrenInformations { get; set; }

    public virtual DbSet<HotelRoom> HotelRooms { get; set; }

    public virtual DbSet<HotelType> HotelTypes { get; set; }

    public virtual DbSet<MonthlyBooking> MonthlyBookings { get; set; }

    public virtual DbSet<PresenceOfChild> PresenceOfChildren { get; set; }

    public virtual DbSet<RegularGuestsInformation> RegularGuestsInformations { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AvailableRoomsPerType>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("available_rooms_per_type");

            entity.Property(e => e.AvailableRooms)
                .HasColumnType("bigint(21)")
                .HasColumnName("available_rooms");
            entity.Property(e => e.RoomType)
                .HasMaxLength(50)
                .HasColumnName("room_type");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.IdDiscount).HasName("PRIMARY");

            entity.ToTable("discounts");

            entity.Property(e => e.IdDiscount)
                .HasColumnType("int(11)")
                .HasColumnName("id_discount");
            entity.Property(e => e.DiscountConditions)
                .HasMaxLength(255)
                .HasColumnName("discount_conditions");
            entity.Property(e => e.DiscountDescription)
                .HasColumnType("text")
                .HasColumnName("discount_description");
            entity.Property(e => e.DiscountPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("discount_percentage");
        });

        modelBuilder.Entity<DiscountUsage>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("discount_usage");

            entity.Property(e => e.DiscountDescription)
                .HasColumnType("text")
                .HasColumnName("discount_description");
            entity.Property(e => e.TotalBookings)
                .HasColumnType("bigint(21)")
                .HasColumnName("total_bookings");
        });

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasKey(e => e.IdGuest).HasName("PRIMARY");

            entity.ToTable("guests");

            entity.HasIndex(e => e.PassportSeries, "passport_series_UNIQUE").IsUnique();

            entity.Property(e => e.IdGuest)
                .HasColumnType("int(11)")
                .HasColumnName("id_guest");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.GuestFirstName)
                .HasMaxLength(50)
                .HasColumnName("guest_first_name");
            entity.Property(e => e.GuestLastName)
                .HasMaxLength(50)
                .HasColumnName("guest_last_name");
            entity.Property(e => e.IsRegularGuest).HasColumnName("is_regular_guest");
            entity.Property(e => e.PassportSeries)
                .HasMaxLength(20)
                .HasColumnName("passport_series");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<GuestsWithChildrenInformation>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("guests_with_children_information");

            entity.Property(e => e.AgeOfChild)
                .HasColumnType("int(11)")
                .HasColumnName("age_of_child");
            entity.Property(e => e.ChildrenPresence).HasColumnName("children_presence");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.GuestFirstName)
                .HasMaxLength(50)
                .HasColumnName("guest_first_name");
            entity.Property(e => e.GuestLastName)
                .HasMaxLength(50)
                .HasColumnName("guest_last_name");
            entity.Property(e => e.IdGuest)
                .HasColumnType("int(11)")
                .HasColumnName("id_guest");
            entity.Property(e => e.IsRegularGuest).HasColumnName("is_regular_guest");
            entity.Property(e => e.NumberOfChild)
                .HasColumnType("tinyint(4)")
                .HasColumnName("number_of_child");
            entity.Property(e => e.PassportSeries)
                .HasMaxLength(20)
                .HasColumnName("passport_series");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<HotelRoom>(entity =>
        {
            entity.HasKey(e => e.IdRooms).HasName("PRIMARY");

            entity.ToTable("hotel_rooms");

            entity.Property(e => e.IdRooms)
                .HasColumnType("int(11)")
                .HasColumnName("id_rooms");
            entity.Property(e => e.RoomStatus)
                .HasMaxLength(20)
                .HasColumnName("room_status");
            entity.Property(e => e.RoomType)
                .HasMaxLength(50)
                .HasColumnName("room_type");
        });

        modelBuilder.Entity<HotelType>(entity =>
        {
            entity.HasKey(e => e.RoomType).HasName("PRIMARY");

            entity.ToTable("hotel_types");

            entity.Property(e => e.RoomType)
                .HasColumnType("int(11)")
                .HasColumnName("room_type");
            entity.Property(e => e.PricePerNight)
                .HasPrecision(10, 2)
                .HasColumnName("price_per_night");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<MonthlyBooking>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("monthly_bookings");

            entity.Property(e => e.BookingMonth)
                .HasMaxLength(5)
                .HasColumnName("booking_month");
            entity.Property(e => e.TotalBookings)
                .HasColumnType("bigint(21)")
                .HasColumnName("total_bookings");
        });

        modelBuilder.Entity<PresenceOfChild>(entity =>
        {
            entity.HasKey(e => e.IdGuest).HasName("PRIMARY");

            entity.ToTable("presence_of_children");

            entity.Property(e => e.IdGuest)
                .ValueGeneratedNever()
                .HasColumnType("int(11)")
                .HasColumnName("id_guest");
            entity.Property(e => e.AgeOfChild)
                .HasColumnType("int(11)")
                .HasColumnName("age_of_child");
            entity.Property(e => e.ChildrenPresence).HasColumnName("children_presence");
            entity.Property(e => e.NumberOfChild)
                .HasColumnType("tinyint(4)")
                .HasColumnName("number_of_child");

            entity.HasOne(d => d.IdGuestNavigation).WithOne(p => p.PresenceOfChild)
                .HasForeignKey<PresenceOfChild>(d => d.IdGuest)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("presence_of_children_ibfk_1");
        });

        modelBuilder.Entity<RegularGuestsInformation>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("regular_guests_information");

            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.GuestFirstName)
                .HasMaxLength(50)
                .HasColumnName("guest_first_name");
            entity.Property(e => e.GuestLastName)
                .HasMaxLength(50)
                .HasColumnName("guest_last_name");
            entity.Property(e => e.IdGuest)
                .HasColumnType("int(11)")
                .HasColumnName("id_guest");
            entity.Property(e => e.IsRegularGuest).HasColumnName("is_regular_guest");
            entity.Property(e => e.PassportSeries)
                .HasMaxLength(20)
                .HasColumnName("passport_series");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.IdBooking).HasName("PRIMARY");

            entity.ToTable("reservation");

            entity.HasIndex(e => e.IdDiscount, "id_discount");

            entity.HasIndex(e => e.IdGuest, "id_guest");

            entity.HasIndex(e => e.IdRoom, "id_room");

            entity.Property(e => e.IdBooking)
                .HasColumnType("int(11)")
                .HasColumnName("id_booking");
            entity.Property(e => e.BookingStatus)
                .HasMaxLength(20)
                .HasColumnName("booking_status");
            entity.Property(e => e.CheckInDate).HasColumnName("check_in_date");
            entity.Property(e => e.CheckOutDate).HasColumnName("check_out_date");
            entity.Property(e => e.IdDiscount)
                .HasColumnType("int(11)")
                .HasColumnName("id_discount");
            entity.Property(e => e.IdGuest)
                .HasColumnType("int(11)")
                .HasColumnName("id_guest");
            entity.Property(e => e.IdRoom)
                .HasColumnType("int(11)")
                .HasColumnName("id_room");

            entity.HasOne(d => d.IdDiscountNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.IdDiscount)
                .HasConstraintName("reservation_ibfk_3");

            entity.HasOne(d => d.IdGuestNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.IdGuest)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reservation_ibfk_1");

            entity.HasOne(d => d.IdRoomNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.IdRoom)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reservation_ibfk_2");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.IdStaff).HasName("PRIMARY");

            entity.ToTable("staff");

            entity.Property(e => e.IdStaff)
                .HasColumnType("int(11)")
                .HasColumnName("id_staff");
            entity.Property(e => e.JobTitle)
                .HasMaxLength(50)
                .HasColumnName("job_title");
            entity.Property(e => e.StaffFirstName)
                .HasMaxLength(50)
                .HasColumnName("staff_first_name");
            entity.Property(e => e.StaffLastName)
                .HasMaxLength(50)
                .HasColumnName("staff_last_name");
            entity.Property(e => e.StaffPhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("staff_phone_number");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
