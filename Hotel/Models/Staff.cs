using System;
using System.Collections.Generic;

namespace Hotel.Models;

public partial class Staff
{
    public int IdStaff { get; set; }

    public string StaffFirstName { get; set; } = null!;

    public string StaffLastName { get; set; } = null!;

    public string JobTitle { get; set; } = null!;

    public string StaffPhoneNumber { get; set; } = null!;
}
