using System;
using System.Collections.Generic;

namespace Hotel.Models;

public partial class Guest
{
    public int IdGuest { get; set; }

    public string GuestFirstName { get; set; } = null!;

    public string GuestLastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public bool? IsRegularGuest { get; set; }

    public string? PassportSeries { get; set; }

    public virtual PresenceOfChild? PresenceOfChild { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
