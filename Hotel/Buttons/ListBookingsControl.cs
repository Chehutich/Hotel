using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class ListBookingsControl : UserControl
{
    private DataGridView dgv;
    private TextBox txtSearch;
    private ComboBox cmbSort;

    public ListBookingsControl()
    {
        var bookingBox = new GroupBox
        {
            Text = "Список бронювань",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            Padding = new Padding(15)
        };

        // 1. Головна панель (Фільтри зверху, Таблиця знизу)
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        // 2. Панель фільтрів
        var filterPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true
        };

        txtSearch = new TextBox { Width = 200, Margin = new Padding(3) };
        cmbSort = new ComboBox { Width = 180, Margin = new Padding(3), DropDownStyle = ComboBoxStyle.DropDownList };
        var btnSearch = new Button { Text = "Пошук", Width = 100, Margin = new Padding(3) };
        var btnReset = new Button { Text = "Скинути", Width = 100, Margin = new Padding(3) };

        // Додаємо опції сортування
        cmbSort.Items.AddRange(new string[] {
            "За датою заїзду (новіші)",
            "За датою заїзду (старіші)",
            "За ID бронювання"
        });

        filterPanel.Controls.Add(new Label { Text = "Пошук:", AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft });
        filterPanel.Controls.Add(txtSearch);
        filterPanel.Controls.Add(new Label { Text = "Сортувати:", AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(10, 0, 0, 0) });
        filterPanel.Controls.Add(cmbSort);
        filterPanel.Controls.Add(btnSearch);
        filterPanel.Controls.Add(btnReset);

        // 3. Таблиця (DataGridView)
        dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None
        };

        mainLayout.Controls.Add(filterPanel, 0, 0);
        mainLayout.Controls.Add(dgv, 0, 1);

        bookingBox.Controls.Add(mainLayout);
        this.Controls.Add(bookingBox);

        this.Load += ListBookingsControl_Load;
        btnSearch.Click += BtnSearch_Click;
        btnReset.Click += BtnReset_Click;
    }

    private void ListBookingsControl_Load(object? sender, EventArgs e)
    {
        // Завантажуємо дані при першому відкритті
        LoadBookings();
    }

    private async void LoadBookings(string? searchTerm = null, string? sortBy = null)
    {
        try
        {
            using (var context = new HotelDbContext())
            {
                // Починаємо з базового запиту, одразу під'єднуючи Гостя
                var query = context.Reservations
                                   .Include(r => r.IdGuestNavigation)
                                   .AsQueryable();

                // 1. Фільтрація (Пошук)
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(r =>
                        // Шукаємо за прізвищем гостя
                        r.IdGuestNavigation.GuestLastName.Contains(searchTerm) ||
                        // Шукаємо за ID кімнати (потрібно перетворити int на string)
                        r.IdRoom.ToString().Contains(searchTerm) ||
                        // Шукаємо за статусом
                        r.BookingStatus.Contains(searchTerm)
                    );
                }

                // 2. Сортування
                switch (sortBy)
                {
                    case "За датою заїзду (новіші)":
                        query = query.OrderByDescending(r => r.CheckInDate);
                        break;
                    case "За датою заїзду (старіші)":
                        query = query.OrderBy(r => r.CheckInDate);
                        break;
                    case "За ID бронювання":
                        query = query.OrderBy(r => r.IdBooking);
                        break;
                    default:
                        query = query.OrderByDescending(r => r.CheckInDate); // Сортування за замовчуванням
                        break;
                }

                // 3. Виконання запиту та вибір потрібних стовпців
                var bookings = await query
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

                // 4. Налаштування стовпців
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

    private void BtnSearch_Click(object? sender, EventArgs e)
    {
        // Викликаємо завантаження з поточними значеннями фільтрів
        LoadBookings(txtSearch.Text, cmbSort.SelectedItem as string);
    }

    private void BtnReset_Click(object? sender, EventArgs e)
    {
        // Очищуємо поля і завантажуємо повний список
        txtSearch.Clear();
        cmbSort.SelectedIndex = -1;
        LoadBookings();
    }
}