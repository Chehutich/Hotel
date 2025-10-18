using System;
using System.Collections.Generic;

namespace Hotel.Models;

public partial class HotelType
{
    public int RoomType { get; set; }

    public string TypeName { get; set; } = null!;

    public decimal PricePerNight { get; set; }
}
