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
    private GroupBox availabilityBox;

    public CheckAvailabilityControl()
    {
        availabilityBox = new GroupBox
        {
            Text = "Перевірка доступних номерів",
            Dock = DockStyle.None,
            Width = 900,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
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

        this.Load += CheckAvailabilityControl_Load;
        btnSearch.Click += BtnSearch_Click;
        this.Resize += (sender, e) => CenterControls();
    }

    // Центрування головного GroupBox
    private void CenterControls()
    {
        availabilityBox.Height = this.ClientSize.Height;
        availabilityBox.Left = (this.ClientSize.Width - availabilityBox.Width) / 2;
    }

    // Завантаження початкового списку доступних кімнат
    private async void CheckAvailabilityControl_Load(object? sender, EventArgs e)
    {
        await LoadAvailableRooms();
        CenterControls();
    }

    // Обробка пошуку за вказаним діапазоном дат
    private async void BtnSearch_Click(object? sender, EventArgs e)
    {
        await LoadAvailableRooms(DateOnly.FromDateTime(dtpCheckIn.Value), DateOnly.FromDateTime(dtpCheckOut.Value));
    }

    // Завантаження доступних кімнат, опціонально фільтруючи за датою
    private async Task LoadAvailableRooms(DateOnly? checkIn = null, DateOnly? checkOut = null)
    {
        try
        {
            using (var context = new HotelDbContext())
            {
                var availableRoomsQuery = context.HotelRooms.Where(hr => hr.RoomStatus == "доступна");

                // Фільтрація заброньованих кімнат, якщо вказано дати
                if (checkIn.HasValue && checkOut.HasValue)
                {
                    var bookedRoomIds = await context.Reservations
                        .Where(r => r.BookingStatus == "підтверджено" &&
                                    checkIn.Value < r.CheckOutDate &&
                                    checkOut.Value > r.CheckInDate)
                        .Select(r => r.IdRoom)
                        .Distinct()
                        .ToListAsync();

                    availableRoomsQuery = availableRoomsQuery.Where(hr => !bookedRoomIds.Contains(hr.IdRooms));
                }

                // Вибірка та відображення даних
                var roomsToShow = await availableRoomsQuery
                    .Select(hr => new {
                        RoomId = hr.IdRooms,
                        RoomType = hr.RoomType,
                        Status = hr.RoomStatus
                    })
                    .OrderBy(r => r.RoomId)
                    .ToListAsync();

                dgv.DataSource = roomsToShow;

                // Налаштування стовпців
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