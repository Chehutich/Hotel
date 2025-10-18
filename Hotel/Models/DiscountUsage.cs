using System;
using System.Collections.Generic;

namespace Hotel.Models;

public partial class DiscountUsage
{
    public string DiscountDescription { get; set; } = null!;

    public long TotalBookings { get; set; }
}
