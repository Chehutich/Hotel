using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Hotel
{
    public class ListBookingsControl : UserControl
    {
        private DataGridView dgv;
        private TextBox txtSearch;
        private ComboBox cmbSort;
        private GroupBox bookingBox;
        private Font commonFont = new Font("Segoe UI", 10F);
        private Dictionary<string, string> bookingSortOptions;

        public ListBookingsControl()
        {
            bookingSortOptions = new Dictionary<string, string>
            {
                { "Date_DESC", Strings.Sort_Booking_Date_DESC },
                { "Date_ASC", Strings.Sort_Booking_Date_ASC },
                { "ID_ASC", Strings.Sort_Booking_ID }
            };

            bookingBox = new GroupBox
            {
                Text = Strings.ListBookingsTitle,
                Dock = DockStyle.None,
                Width = 1100,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Padding = new Padding(15),
                ForeColor = ThemeManager.TextColor // (ЗМІНЕНО)
            };

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 10)
            };

            // (ЗМІНЕНО) Кольори полів вводу
            txtSearch = new TextBox { Width = 200, Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
            cmbSort = new ComboBox { Width = 200, Margin = new Padding(3), DropDownStyle = ComboBoxStyle.DropDownList, Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };

            // (ЗМІНЕНО) Кольори кнопок
            var btnSearch = new Button { Text = Strings.ButtonSearch, Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };
            var btnReset = new Button { Text = Strings.ButtonReset, Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };

            cmbSort.DataSource = new BindingSource(bookingSortOptions, null);
            cmbSort.DisplayMember = "Value";
            cmbSort.ValueMember = "Key";

            // (ЗМІНЕНО) Колір тексту Label
            filterPanel.Controls.Add(new Label { Text = Strings.LabelSearch, AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Font = commonFont, Margin = new Padding(3, 0, 0, 0), ForeColor = ThemeManager.TextColor });
            filterPanel.Controls.Add(txtSearch);
            // (ЗМІНЕНО) Колір тексту Label
            filterPanel.Controls.Add(new Label { Text = Strings.LabelSort, AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(10, 0, 0, 0), Font = commonFont, ForeColor = ThemeManager.TextColor });
            filterPanel.Controls.Add(cmbSort);
            filterPanel.Controls.Add(btnSearch);
            filterPanel.Controls.Add(btnReset);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = ThemeManager.GridBackground, // (ЗМІНЕНО)
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F)
            };

            // (ЗМІНЕНО) Кольори сітки
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.GridHeaderBackground;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ThemeManager.GridHeaderForeColor;
            dgv.DefaultCellStyle.BackColor = ThemeManager.InputBackground;
            dgv.DefaultCellStyle.ForeColor = ThemeManager.InputForeColor;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersDefaultCellStyle.BackColor = ThemeManager.GridHeaderBackground;

            dgv.RowTemplate.Height = 30;

            mainLayout.Controls.Add(filterPanel, 0, 0);
            mainLayout.Controls.Add(dgv, 0, 1);

            bookingBox.Controls.Add(mainLayout);
            this.Controls.Add(bookingBox);

            this.Load += ListBookingsControl_Load;
            btnSearch.Click += BtnSearch_Click;
            btnReset.Click += BtnReset_Click;
            this.Resize += (sender, e) => CenterControls();
        }

        private void CenterControls()
        {
            bookingBox.Height = this.ClientSize.Height - 10;
            bookingBox.Left = (this.ClientSize.Width - bookingBox.Width) / 2;
            bookingBox.Top = 5;
        }

        private void ListBookingsControl_Load(object? sender, EventArgs e)
        {
            LoadBookings();
            CenterControls();
        }

        private async void LoadBookings(string? searchTerm = null, string? sortBy = null)
        {
            try
            {
                using (var context = new HotelDbContext())
                {
                    var query = context.Reservations
                                       .Include(r => r.IdGuestNavigation)
                                       .AsQueryable();

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        query = query.Where(r =>
                            r.IdGuestNavigation.GuestLastName.Contains(searchTerm) ||
                            r.IdRoom.ToString().Contains(searchTerm) ||
                            r.BookingStatus.Contains(searchTerm)
                        );
                    }

                    switch (sortBy)
                    {
                        case "Date_DESC": query = query.OrderByDescending(r => r.CheckInDate); break;
                        case "Date_ASC": query = query.OrderBy(r => r.CheckInDate); break;
                        case "ID_ASC": query = query.OrderBy(r => r.IdBooking); break;
                        default: query = query.OrderByDescending(r => r.CheckInDate); break;
                    }

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

                    dgv.Columns["BookingId"].HeaderText = Strings.Col_BookingID;
                    dgv.Columns["GuestName"].HeaderText = Strings.Col_GuestName;
                    dgv.Columns["RoomId"].HeaderText = Strings.Col_RoomID;
                    dgv.Columns["CheckIn"].HeaderText = Strings.Col_CheckIn;
                    dgv.Columns["CheckOut"].HeaderText = Strings.Col_CheckOut;
                    dgv.Columns["Status"].HeaderText = Strings.Col_Status;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження бронювань: {ex.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            LoadBookings(txtSearch.Text, cmbSort.SelectedValue as string);
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            txtSearch.Clear();
            cmbSort.SelectedIndex = -1;
            LoadBookings();
        }
    }
}