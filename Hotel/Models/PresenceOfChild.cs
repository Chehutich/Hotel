using System;
using System.Collections.Generic;

namespace Hotel.Models;

public partial class PresenceOfChild
{
    public int IdGuest { get; set; }

    public bool? ChildrenPresence { get; set; }

    public sbyte? NumberOfChild { get; set; }

    public int? AgeOfChild { get; set; }

    public virtual Guest IdGuestNavigation { get; set; } = null!;
}
