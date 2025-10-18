using System;
using System.Collections.Generic;

namespace Hotel.Models;

public partial class AvailableRoomsPerType
{
    public string RoomType { get; set; } = null!;

    public long AvailableRooms { get; set; }
}
