using Hotel.Forms;
using Hotel.Localization;
using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Hotel.Buttons
{
    public class ListBookingsControl : UserControl
    {
        private DataGridView dgv;
        private TextBox txtSearch;
        private ComboBox cmbSort;
        private GroupBox bookingBox;
        private Font commonFont = new Font("Segoe UI", 10F);
        private Dictionary<string, string> bookingSortOptions;

        private Button btnCheckIn;
        private Button btnCheckOut;

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
                ForeColor = ThemeManager.TextColor
            };

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
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

            txtSearch = new TextBox { Width = 200, Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };

            // (НОВЕ) Живий пошук
            txtSearch.TextChanged += (s, e) => LoadBookings(txtSearch.Text, cmbSort.SelectedValue as string);

            cmbSort = new ComboBox { Width = 200, Margin = new Padding(3), DropDownStyle = ComboBoxStyle.DropDownList, Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
            var btnSearch = new Button { Text = Strings.ButtonSearch, Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };
            var btnReset = new Button { Text = Strings.ButtonReset, Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };

            cmbSort.DataSource = new BindingSource(bookingSortOptions, null);
            cmbSort.DisplayMember = "Value";
            cmbSort.ValueMember = "Key";

            filterPanel.Controls.Add(new Label { Text = Strings.LabelSearch, AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Font = commonFont, Margin = new Padding(3, 0, 0, 0), ForeColor = ThemeManager.TextColor });
            filterPanel.Controls.Add(txtSearch);
            filterPanel.Controls.Add(new Label { Text = Strings.LabelSort, AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(10, 0, 0, 0), Font = commonFont, ForeColor = ThemeManager.TextColor });
            filterPanel.Controls.Add(cmbSort);
            filterPanel.Controls.Add(btnSearch);
            filterPanel.Controls.Add(btnReset);

            var actionsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 10)
            };

            btnCheckIn = new Button { Text = Strings.Booking_CheckIn, Size = new Size(120, 40), Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor, Enabled = false };
            btnCheckOut = new Button { Text = Strings.Booking_CheckOut, Size = new Size(120, 40), Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor, Enabled = false };

            actionsPanel.Controls.Add(btnCheckIn);
            actionsPanel.Controls.Add(btnCheckOut);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = ThemeManager.GridBackground,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.GridHeaderBackground;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ThemeManager.GridHeaderForeColor;
            dgv.DefaultCellStyle.BackColor = ThemeManager.InputBackground;
            dgv.DefaultCellStyle.ForeColor = ThemeManager.InputForeColor;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersDefaultCellStyle.BackColor = ThemeManager.GridHeaderBackground;
            dgv.RowTemplate.Height = 30;

            mainLayout.Controls.Add(filterPanel, 0, 0);
            mainLayout.Controls.Add(actionsPanel, 0, 1);
            mainLayout.Controls.Add(dgv, 0, 2);

            bookingBox.Controls.Add(mainLayout);
            this.Controls.Add(bookingBox);

            this.Load += ListBookingsControl_Load;
            btnSearch.Click += BtnSearch_Click;
            btnReset.Click += BtnReset_Click;
            dgv.SelectionChanged += Dgv_SelectionChanged;
            btnCheckIn.Click += BtnCheckIn_Click;
            btnCheckOut.Click += BtnCheckOut_Click;
            this.Resize += (sender, e) => CenterControls();
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            btnCheckIn.Enabled = false;
            btnCheckOut.Enabled = false;

            if (dgv.SelectedRows.Count == 0) return;

            var selectedRow = dgv.SelectedRows[0];
            var status = selectedRow.Cells["Status"].Value?.ToString();
            var checkInDate = (DateOnly)selectedRow.Cells["CheckIn"].Value;

            if (status == "підтверджено" && checkInDate <= DateOnly.FromDateTime(DateTime.Now))
            {
                btnCheckIn.Enabled = true;
            }

            if (status == "Проживає")
            {
                btnCheckOut.Enabled = true;
            }
        }

        private async void BtnCheckIn_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            var selectedRow = dgv.SelectedRows[0];
            int bookingId = (int)selectedRow.Cells["BookingId"].Value;
            int roomId = (int)selectedRow.Cells["RoomId"].Value;

            var confirm = MessageBox.Show($"Заселити гостя у кімнату {roomId}?", Strings.Booking_CheckIn, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using (var context = new HotelDbContext())
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        var reservation = await context.Reservations.FindAsync(bookingId);
                        if (reservation != null)
                        {
                            reservation.BookingStatus = "Проживає";
                        }

                        var room = await context.HotelRooms.FindAsync(roomId);
                        if (room != null)
                        {
                            room.RoomStatus = "Зайнята";
                        }

                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                }

                await LoadBookings(txtSearch.Text, cmbSort.SelectedValue as string);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка заселення: {ex.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnCheckOut_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            var selectedRow = dgv.SelectedRows[0];
            int bookingId = (int)selectedRow.Cells["BookingId"].Value;
            int roomId = (int)selectedRow.Cells["RoomId"].Value;

            var confirm = MessageBox.Show($"Виселити гостя з кімнати {roomId}?", Strings.Booking_CheckOut, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using (var context = new HotelDbContext())
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        var reservation = await context.Reservations.FindAsync(bookingId);
                        if (reservation != null)
                        {
                            reservation.BookingStatus = "Завершено";
                        }

                        var room = await context.HotelRooms.FindAsync(roomId);
                        if (room != null)
                        {
                            room.RoomStatus = "на прибиранні";
                        }

                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                }

                await LoadBookings(txtSearch.Text, cmbSort.SelectedValue as string);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка виселення: {ex.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private async Task LoadBookings(string? searchTerm = null, string? sortBy = null)
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
                            NumberOfGuests = r.NumberOfGuests,
                            CheckIn = r.CheckInDate,
                            CheckOut = r.CheckOutDate,
                            Status = r.BookingStatus
                        })
                        .ToListAsync();

                    dgv.DataSource = bookings;

                    dgv.Columns["BookingId"].HeaderText = Strings.Col_BookingID;
                    dgv.Columns["GuestName"].HeaderText = Strings.Col_GuestName;
                    dgv.Columns["RoomId"].HeaderText = Strings.Col_RoomID;
                    dgv.Columns["NumberOfGuests"].HeaderText = Strings.Col_NumGuests;
                    dgv.Columns["CheckIn"].HeaderText = Strings.Col_CheckIn;
                    dgv.Columns["CheckOut"].HeaderText = Strings.Col_CheckOut;
                    dgv.Columns["Status"].HeaderText = Strings.Col_Status;

                    if (dgv.Columns["NumberOfGuests"] != null)
                    {
                        dgv.Columns["NumberOfGuests"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        dgv.Columns["NumberOfGuests"].FillWeight = 50;
                    }
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