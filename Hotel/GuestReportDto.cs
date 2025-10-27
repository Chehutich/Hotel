using System;

namespace Hotel.Models // Або Hotel.DTOs
{
    public class GuestReportDto
    {
        public int IdGuest { get; set; }
        public string GuestFirstName { get; set; } = string.Empty;
        public string GuestLastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? DateOfBirth { get; set; } 
        public bool? IsRegularGuest { get; set; }
        public string? PassportSeries { get; set; }

        public GuestReportDto() { }
    }
}