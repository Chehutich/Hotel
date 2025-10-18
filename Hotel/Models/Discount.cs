using System;
using System.Collections.Generic;

namespace Hotel.Models;

public partial class Discount
{
    public int IdDiscount { get; set; }

    public string DiscountDescription { get; set; } = null!;

    public decimal DiscountPercentage { get; set; }

    public string? DiscountConditions { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
