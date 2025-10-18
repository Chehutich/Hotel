using System;
using System.Collections.Generic;

namespace Hotel.Models;

public partial class HotelRoom
{
    public int IdRooms { get; set; }

    public string RoomType { get; set; } = null!;

    public string RoomStatus { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
