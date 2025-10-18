using System;
using System.Collections.Generic;

namespace Hotel.Models;

public partial class MonthlyBooking
{
    public string? BookingMonth { get; set; }

    public long TotalBookings { get; set; }
}
