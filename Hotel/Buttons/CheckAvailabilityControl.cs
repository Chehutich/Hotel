using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Hotel.Localization;

namespace Hotel
{
    public class CheckAvailabilityControl : UserControl
    {
        private DataGridView dgv;
        private DateTimePicker dtpCheckIn;
        private DateTimePicker dtpCheckOut;
        private GroupBox availabilityBox;
        private Font commonFont = new Font("Segoe UI", 10F);
        private Font pickerFont = new Font("Segoe UI", 12F);

        public CheckAvailabilityControl()
        {
            availabilityBox = new GroupBox
            {
                Text = Strings.CheckAvailabilityTitle,
                Dock = DockStyle.None,
                Width = 1100,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Padding = new Padding(15),
                ForeColor = ThemeManager.TextColor // (ЗМІНЕНО)
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
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 10)
            };

            // (ЗМІНЕНО) Кольори календаря
            dtpCheckIn = new DateTimePicker { Width = 160, Font = pickerFont, Margin = new Padding(3) };
            dtpCheckIn.CalendarMonthBackground = ThemeManager.InputBackground;
            dtpCheckIn.CalendarForeColor = ThemeManager.InputForeColor;
            dtpCheckIn.CalendarTitleBackColor = ThemeManager.ButtonBackground;
            dtpCheckIn.CalendarTitleForeColor = ThemeManager.ButtonForeColor;
            dtpCheckIn.CalendarTrailingForeColor = Color.Gray;

            dtpCheckOut = new DateTimePicker { Width = 160, Font = pickerFont, Margin = new Padding(3) };
            dtpCheckOut.CalendarMonthBackground = ThemeManager.InputBackground;
            dtpCheckOut.CalendarForeColor = ThemeManager.InputForeColor;
            dtpCheckOut.CalendarTitleBackColor = ThemeManager.ButtonBackground;
            dtpCheckOut.CalendarTitleForeColor = ThemeManager.ButtonForeColor;
            dtpCheckOut.CalendarTrailingForeColor = Color.Gray;

            // (ЗМІНЕНО) Кольори кнопок
            var btnSearch = new Button { Text = Strings.ButtonSearch, Size = new Size(100, 35), Font = commonFont, Margin = new Padding(10, 0, 0, 0), BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };

            // (ЗМІНЕНО) Колір тексту Label
            filterPanel.Controls.Add(new Label { Text = Strings.LabelFrom, AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(5, 8, 5, 0), Font = commonFont, ForeColor = ThemeManager.TextColor });
            filterPanel.Controls.Add(dtpCheckIn);
            // (ЗМІНЕНО) Колір тексту Label
            filterPanel.Controls.Add(new Label { Text = Strings.LabelTo, AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(10, 8, 5, 0), Font = commonFont, ForeColor = ThemeManager.TextColor });
            filterPanel.Controls.Add(dtpCheckOut);
            filterPanel.Controls.Add(btnSearch);

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
            dgv.EnableHeadersVisualStyles = false; // Важливо для застосування стилів заголовків
            dgv.RowHeadersDefaultCellStyle.BackColor = ThemeManager.GridHeaderBackground;


            dgv.RowTemplate.Height = 30;

            mainLayoutPanel.Controls.Add(filterPanel, 0, 0);
            mainLayoutPanel.Controls.Add(dgv, 0, 1);
            availabilityBox.Controls.Add(mainLayoutPanel);
            this.Controls.Add(availabilityBox);

            this.Load += CheckAvailabilityControl_Load;
            btnSearch.Click += BtnSearch_Click;
            this.Resize += (sender, e) => CenterControls();
        }

        private void CenterControls()
        {
            availabilityBox.Height = this.ClientSize.Height - 10;
            availabilityBox.Left = (this.ClientSize.Width - availabilityBox.Width) / 2;
            availabilityBox.Top = 5;
        }

        private async void CheckAvailabilityControl_Load(object? sender, EventArgs e)
        {
            await LoadAvailableRooms();
            CenterControls();
        }

        private async void BtnSearch_Click(object? sender, EventArgs e)
        {
            if (dtpCheckOut.Value <= dtpCheckIn.Value)
            {
                MessageBox.Show(Strings.ValidationDateError, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            await LoadAvailableRooms(DateOnly.FromDateTime(dtpCheckIn.Value), DateOnly.FromDateTime(dtpCheckOut.Value));
        }

        private async Task LoadAvailableRooms(DateOnly? checkIn = null, DateOnly? checkOut = null)
        {
            try
            {
                using (var context = new HotelDbContext())
                {
                    var availableRoomsQuery = context.HotelRooms.Where(hr => hr.RoomStatus == "доступна");

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

                    var roomsToShow = await availableRoomsQuery
                        .Select(hr => new {
                            RoomId = hr.IdRooms,
                            RoomType = hr.RoomType,
                            Status = hr.RoomStatus
                        })
                        .OrderBy(r => r.RoomId)
                        .ToListAsync();

                    dgv.DataSource = roomsToShow;

                    dgv.Columns["RoomId"].HeaderText = Strings.Col_RoomID;
                    dgv.Columns["RoomType"].HeaderText = Strings.Col_RoomType;
                    dgv.Columns["Status"].HeaderText = Strings.Col_Status;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження кімнат: {ex.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}