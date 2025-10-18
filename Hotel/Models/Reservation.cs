using System;
using System.Collections.Generic;

namespace Hotel.Models;

public partial class Reservation
{
    public int IdBooking { get; set; }

    public int IdGuest { get; set; }

    public int IdRoom { get; set; }

    public DateOnly CheckInDate { get; set; }

    public DateOnly CheckOutDate { get; set; }

    public string BookingStatus { get; set; } = null!;

    public int? IdDiscount { get; set; }

    public virtual Discount? IdDiscountNavigation { get; set; }

    public virtual Guest IdGuestNavigation { get; set; } = null!;

    public virtual HotelRoom IdRoomNavigation { get; set; } = null!;
}
