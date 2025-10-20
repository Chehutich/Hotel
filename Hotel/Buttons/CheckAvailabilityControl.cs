using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class CheckAvailabilityControl : UserControl
{
    private DataGridView dgv;
    private DateTimePicker dtpCheckIn;
    private DateTimePicker dtpCheckOut;

    public CheckAvailabilityControl()
    {
        var availabilityBox = new GroupBox
        {
            Text = "Перевірка доступних номерів",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            Padding = new Padding(15)
        };

        var mainLayoutPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1
        };
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var filterPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true
        };

        dtpCheckIn = new DateTimePicker { Width = 120 };
        dtpCheckOut = new DateTimePicker { Width = 120 };
        var btnSearch = new Button { Text = "Пошук", Width = 100 };

        filterPanel.Controls.Add(new Label { Text = "З:", AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(5, 0, 5, 0) });
        filterPanel.Controls.Add(dtpCheckIn);
        filterPanel.Controls.Add(new Label { Text = "По:", AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(10, 0, 5, 0) });
        filterPanel.Controls.Add(dtpCheckOut);
        filterPanel.Controls.Add(btnSearch);

        dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None
        };

        mainLayoutPanel.Controls.Add(filterPanel, 0, 0);
        mainLayoutPanel.Controls.Add(dgv, 0, 1);
        availabilityBox.Controls.Add(mainLayoutPanel);
        this.Controls.Add(availabilityBox);

        // Add event handlers
        this.Load += CheckAvailabilityControl_Load;
        btnSearch.Click += BtnSearch_Click;
    }

    private async void CheckAvailabilityControl_Load(object? sender, EventArgs e)
    {
        // On initial load, just show all rooms with "available" status
        await LoadAvailableRooms();
    }

    private async void BtnSearch_Click(object? sender, EventArgs e)
    {
        // On button click, show rooms available for the specific date range
        await LoadAvailableRooms(DateOnly.FromDateTime(dtpCheckIn.Value), DateOnly.FromDateTime(dtpCheckOut.Value));
    }

    private async Task LoadAvailableRooms(DateOnly? checkIn = null, DateOnly? checkOut = null)
    {
        try
        {
            using (var context = new HotelDbContext())
            {
                // Start with a base query for rooms that are generally available
                var availableRoomsQuery = context.HotelRooms.Where(hr => hr.RoomStatus == "доступна");

                // If a date range is provided, filter out rooms that are booked in that period
                if (checkIn.HasValue && checkOut.HasValue)
                {
                    // Find IDs of rooms that are booked during the selected date range
                    var bookedRoomIds = await context.Reservations
                        .Where(r => r.BookingStatus == "підтверджено" &&
                                    checkIn.Value < r.CheckOutDate &&
                                    checkOut.Value > r.CheckInDate) // This logic checks for any overlap
                        .Select(r => r.IdRoom)
                        .Distinct()
                        .ToListAsync();

                    // Exclude the booked rooms from our main query
                    availableRoomsQuery = availableRoomsQuery.Where(hr => !bookedRoomIds.Contains(hr.IdRooms));
                }

                // Execute the final query and select data for display
                var roomsToShow = await availableRoomsQuery
                    .Select(hr => new {
                        RoomId = hr.IdRooms,
                        RoomType = hr.RoomType,
                        Status = hr.RoomStatus
                    })
                    .ToListAsync();

                dgv.DataSource = roomsToShow;

                // Configure column headers
                dgv.Columns["RoomId"].HeaderText = "Номер кімнати";
                dgv.Columns["RoomType"].HeaderText = "Тип кімнати";
                dgv.Columns["Status"].HeaderText = "Статус";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка завантаження кімнат: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}