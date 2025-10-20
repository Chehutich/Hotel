using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class ListBookingsControl : UserControl
{
    private DataGridView dgv;

    public ListBookingsControl()
    {
        var bookingBox = new GroupBox
        {
            Text = "Список бронювань",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            Padding = new Padding(15)
        };

        dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None
        };

        bookingBox.Controls.Add(dgv);
        this.Controls.Add(bookingBox);

        this.Load += ListBookingsControl_Load;
    }

    private async void ListBookingsControl_Load(object? sender, EventArgs e)
    {
        try
        {
            using (var context = new HotelDbContext())
            {
                // Here, we query the Reservations table.
                // .Include(r => r.IdGuestNavigation) tells EF to also load the related Guest object for each reservation.
                // .Select() allows us to create a custom, cleaner object to display, combining properties from both tables.
                var bookings = await context.Reservations
                    .Include(r => r.IdGuestNavigation)
                    .Select(r => new
                    {
                        BookingId = r.IdBooking,
                        GuestName = r.IdGuestNavigation.GuestFirstName + " " + r.IdGuestNavigation.GuestLastName,
                        RoomId = r.IdRoom,
                        CheckIn = r.CheckInDate,
                        CheckOut = r.CheckOutDate,
                        Status = r.BookingStatus
                    })
                    .ToListAsync();

                dgv.DataSource = bookings;

                // Set user-friendly names for the columns
                dgv.Columns["BookingId"].HeaderText = "ID Бронювання";
                dgv.Columns["GuestName"].HeaderText = "Ім'я гостя";
                dgv.Columns["RoomId"].HeaderText = "Номер кімнати";
                dgv.Columns["CheckIn"].HeaderText = "Дата заїзду";
                dgv.Columns["CheckOut"].HeaderText = "Дата виїзду";
                dgv.Columns["Status"].HeaderText = "Статус";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка завантаження бронювань: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}