using Hotel.Models;

namespace Hotel.Core
{
    public static class HotelAppContext
    {
        public static string MasterConnectionString { get; set; } = string.Empty;
        public static Staff? CurrentUser { get; set; }
    }
}