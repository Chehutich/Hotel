using Hotel.Forms;
using Hotel.Localization;
using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Common;
using Hotel.Core;

namespace Hotel.Buttons
{
    public class AddBookingControl : UserControl
    {
        private GroupBox bookingBox;
        private Font commonFont = new Font("Segoe UI", 11F);
        private Font pickerFont = new Font("Segoe UI", 12F);
        private Font totalFont = new Font("Segoe UI", 14F, FontStyle.Bold);

        private TextBox txtPhoneNumber;
        private Button btnFindGuest;
        private Label lblGuestName;
        private NumericUpDown numGuests;
        private DateTimePicker dtpCheckIn, dtpCheckOut;
        private Label lblNumNights;
        private Button btnFindRooms;
        private ComboBox cmbAvailableRooms;
        private Label lblTotalPrice;
        private Button btnSave;
        private Button btnCancel;

        private Guest? currentGuest = null;
        private List<RoomSearchResult> availableRoomsList = new List<RoomSearchResult>();
        private decimal currentTotalPrice = 0;

        private class RoomSearchResult
        {
            public int RoomId { get; set; }
            public string RoomType { get; set; } = "";
            public decimal PricePerNight { get; set; }
            public string DisplayName => $"{RoomId} - {RoomType} ({PricePerNight:F2} грн/ніч)";
        }


        public AddBookingControl()
        {
            bookingBox = new GroupBox
            {
                Text = Strings.AddBookingTitle,
                Dock = DockStyle.None,
                Width = 900,
                Height = 650,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Padding = new Padding(25),
                ForeColor = ThemeManager.TextColor
            };

            var layoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 8,
                BackColor = ThemeManager.ContentBackground
            };

            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));

            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));

            txtPhoneNumber = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
            btnFindGuest = new Button { Text = Strings.Booking_FindGuest, Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };
            lblGuestName = new Label { Text = $"({Strings.Booking_GuestInfo})", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(5), ForeColor = ThemeManager.TextColor, Font = new Font(commonFont, FontStyle.Italic) };
            numGuests = new NumericUpDown { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont, Minimum = 1, Maximum = 10, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };

            dtpCheckIn = new DateTimePicker { Dock = DockStyle.Fill, Margin = new Padding(5), Font = pickerFont, Format = DateTimePickerFormat.Long };
            SetupDatePickerTheme(dtpCheckIn);
            dtpCheckOut = new DateTimePicker { Dock = DockStyle.Fill, Margin = new Padding(5), Font = pickerFont, Format = DateTimePickerFormat.Long };
            SetupDatePickerTheme(dtpCheckOut);
            lblNumNights = new Label { Text = string.Format(Strings.Booking_NumNights, 0), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(5), ForeColor = ThemeManager.TextColor, Font = new Font(commonFont, FontStyle.Bold) };

            btnFindRooms = new Button { Text = Strings.Booking_FindRooms, Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };
            cmbAvailableRooms = new ComboBox { Dock = DockStyle.Fill, Margin = new Padding(5), DropDownStyle = ComboBoxStyle.DropDownList, Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };

            lblTotalPrice = new Label { Text = $"{Strings.Booking_TotalPrice} 0.00 грн", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Margin = new Padding(5), Font = totalFont, ForeColor = ThemeManager.TextColor };

            btnSave = new Button { Text = Strings.ButtonSave, Width = 130, Height = 40, Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };
            btnCancel = new Button { Text = Strings.ButtonCancel, Width = 130, Height = 40, Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };
            var buttonPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill, Padding = new Padding(0, 15, 0, 0) };
            buttonPanel.Controls.Add(btnSave);
            buttonPanel.Controls.Add(btnCancel);

            layoutPanel.Controls.Add(CreateLabel(Strings.LabelPhoneNumber), 0, 0);
            layoutPanel.Controls.Add(txtPhoneNumber, 1, 0);
            layoutPanel.Controls.Add(btnFindGuest, 2, 0);

            layoutPanel.Controls.Add(CreateLabel(Strings.Booking_GuestName), 0, 1);
            layoutPanel.Controls.Add(lblGuestName, 1, 1);
            layoutPanel.SetColumnSpan(lblGuestName, 2);

            layoutPanel.Controls.Add(CreateLabel(Strings.Booking_NumGuests), 0, 2);
            layoutPanel.Controls.Add(numGuests, 1, 2);

            layoutPanel.Controls.Add(CreateLabel(Strings.LabelCheckIn), 0, 3);
            layoutPanel.Controls.Add(dtpCheckIn, 1, 3);
            layoutPanel.SetColumnSpan(dtpCheckIn, 2);

            layoutPanel.Controls.Add(CreateLabel(Strings.LabelCheckOut), 0, 4);
            layoutPanel.Controls.Add(dtpCheckOut, 1, 4);
            layoutPanel.Controls.Add(lblNumNights, 2, 4);

            layoutPanel.Controls.Add(btnFindRooms, 1, 5);
            layoutPanel.SetColumnSpan(btnFindRooms, 2);

            layoutPanel.Controls.Add(CreateLabel(Strings.Booking_AvailableRooms), 0, 6);
            layoutPanel.Controls.Add(cmbAvailableRooms, 1, 6);
            layoutPanel.SetColumnSpan(cmbAvailableRooms, 2);

            layoutPanel.Controls.Add(lblTotalPrice, 0, 7);
            layoutPanel.SetColumnSpan(lblTotalPrice, 2);
            layoutPanel.Controls.Add(buttonPanel, 2, 7);

            bookingBox.Controls.Add(layoutPanel);
            this.Controls.Add(bookingBox);

            btnFindGuest.Click += BtnFindGuest_Click;
            dtpCheckIn.ValueChanged += DatesChanged_ValueChanged;
            dtpCheckOut.ValueChanged += DatesChanged_ValueChanged;
            numGuests.ValueChanged += (s, e) => ResetRoomSearch();
            btnFindRooms.Click += BtnFindRooms_Click;
            cmbAvailableRooms.SelectedIndexChanged += CmbAvailableRooms_SelectedIndexChanged;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            this.Load += (sender, e) => CenterControls();
            this.Resize += (sender, e) => CenterControls();

            UpdateNightCount();
            ResetRoomSearch();
        }

        // (ЗМІНЕНО) Логіка пошуку гостя
        private async void BtnFindGuest_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text)) return;

            try
            {
                using (var context = new HotelDbContext())
                {
                    currentGuest = await context.Guests
                        .FirstOrDefaultAsync(g => g.PhoneNumber == txtPhoneNumber.Text);
                }

                if (currentGuest != null)
                {
                    lblGuestName.Text = $"{currentGuest.GuestFirstName} {currentGuest.GuestLastName} (ID: {currentGuest.IdGuest})";
                    lblGuestName.Font = new Font(commonFont, FontStyle.Bold);
                }
                else
                {
                    currentGuest = null;
                    lblGuestName.Text = $"({Strings.Booking_GuestNotFound})";
                    lblGuestName.Font = new Font(commonFont, FontStyle.Italic);

                    // (НОВЕ) Запитуємо, чи додати гостя
                    var result = MessageBox.Show(
                        Strings.Booking_AskCreateGuest_Msg, // "Гостя не знайдено. Додати?"
                        Strings.Booking_AskCreateGuest_Title,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Знаходимо Form1
                        var form1 = this.FindForm() as Form1;
                        if (form1 != null)
                        {
                            // Відкриваємо AddGuestControl, передаючи номер телефону
                            var addGuestControl = new AddGuestControl(txtPhoneNumber.Text);
                            form1.ShowControl(addGuestControl);
                        }
                    }
                }
                ResetRoomSearch();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка пошуку гостя: {ex.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DatesChanged_ValueChanged(object? sender, EventArgs e)
        {
            if (dtpCheckOut.Value <= dtpCheckIn.Value)
            {
                dtpCheckOut.Value = dtpCheckIn.Value.AddDays(1);
            }
            UpdateNightCount();
            ResetRoomSearch();
        }

        private void UpdateNightCount()
        {
            int nights = (dtpCheckOut.Value.Date - dtpCheckIn.Value.Date).Days;
            lblNumNights.Text = string.Format(Strings.Booking_NumNights, nights);
        }

        private async void BtnFindRooms_Click(object? sender, EventArgs e)
        {
            var checkIn = DateOnly.FromDateTime(dtpCheckIn.Value);
            var checkOut = DateOnly.FromDateTime(dtpCheckOut.Value);
            var guestCount = (int)numGuests.Value;

            try
            {
                using (var context = new HotelDbContext())
                {
                    var bookedRoomIds = await context.Reservations
                        .Where(r => r.BookingStatus == "підтверджено" &&
                                    checkIn < r.CheckOutDate &&
                                    checkOut > r.CheckInDate)
                        .Select(r => r.IdRoom)
                        .Distinct()
                        .ToListAsync();

                    var suitableRoomTypeNames = await context.HotelTypes
                        .Where(ht => ht.MaxCapacity >= guestCount)
                        .Select(ht => ht.TypeName)
                        .ToListAsync();

                    var query = from hr in context.HotelRooms.AsNoTracking()
                                join ht in context.HotelTypes.AsNoTracking() on hr.RoomType equals ht.TypeName
                                where hr.RoomStatus == "доступна" &&
                                      !bookedRoomIds.Contains(hr.IdRooms) &&
                                      suitableRoomTypeNames.Contains(hr.RoomType)
                                select new RoomSearchResult
                                {
                                    RoomId = hr.IdRooms,
                                    RoomType = hr.RoomType,
                                    PricePerNight = ht.PricePerNight
                                };

                    availableRoomsList = await query.OrderBy(r => r.PricePerNight).ThenBy(r => r.RoomId).ToListAsync();
                }

                if (availableRoomsList.Any())
                {
                    cmbAvailableRooms.DataSource = availableRoomsList;
                    cmbAvailableRooms.DisplayMember = "DisplayName";
                    cmbAvailableRooms.ValueMember = "RoomId";
                    cmbAvailableRooms.Enabled = true;
                }
                else
                {
                    MessageBox.Show(Strings.Booking_NoRoomsFound, Strings.Booking_FindRooms, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetRoomSearch();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка пошуку кімнат: {ex.Message}\n{ex.InnerException?.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetRoomSearch()
        {
            cmbAvailableRooms.DataSource = null;
            cmbAvailableRooms.Items.Clear();
            cmbAvailableRooms.Items.Add(Strings.Booking_SelectRoom);
            cmbAvailableRooms.SelectedIndex = 0;
            cmbAvailableRooms.Enabled = false;
            availableRoomsList.Clear();
            UpdateTotalPrice();
        }

        private void CmbAvailableRooms_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateTotalPrice();
        }

        private async void UpdateTotalPrice()
        {
            if (currentGuest == null || cmbAvailableRooms.SelectedValue == null || !(cmbAvailableRooms.SelectedValue is int))
            {
                currentTotalPrice = 0;
                lblTotalPrice.Text = $"{Strings.Booking_TotalPrice} 0.00 грн";
                return;
            }

            int roomId = (int)cmbAvailableRooms.SelectedValue;

            try
            {
                using (var context = new HotelDbContext())
                using (var connection = context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "calculate_total_price";
                        command.CommandType = CommandType.StoredProcedure;

                        var lastNameParam = new MySqlParameter("p_name_guest", currentGuest.GuestLastName);
                        var roomIdParam = new MySqlParameter("p_id_room", roomId);
                        var checkInParam = new MySqlParameter("p_check_in_date", dtpCheckIn.Value.Date);
                        var checkOutParam = new MySqlParameter("p_check_out_date", dtpCheckOut.Value.Date);

                        var totalPriceParam = new MySqlParameter("p_total_price", MySqlDbType.Decimal)
                        {
                            Direction = ParameterDirection.Output
                        };

                        command.Parameters.Add(lastNameParam);
                        command.Parameters.Add(roomIdParam);
                        command.Parameters.Add(checkInParam);
                        command.Parameters.Add(checkOutParam);
                        command.Parameters.Add(totalPriceParam);

                        await command.ExecuteNonQueryAsync();

                        if (totalPriceParam.Value != DBNull.Value)
                        {
                            currentTotalPrice = Convert.ToDecimal(totalPriceParam.Value);
                            lblTotalPrice.Text = $"{Strings.Booking_TotalPrice} {currentTotalPrice:F2} грн";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка розрахунку ціни: {ex.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (currentGuest == null)
            {
                MessageBox.Show("Будь ласка, знайдіть та виберіть гостя.", Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            if (cmbAvailableRooms.SelectedValue == null || !(cmbAvailableRooms.SelectedValue is int))
            {
                MessageBox.Show("Будь ласка, знайдіть та виберіть кімнату.", Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }
            if (currentTotalPrice <= 0)
            {
                MessageBox.Show("Не вдалося розрахувати вартість. Перевірте дані.", Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
            }

            try
            {
                using (var context = new HotelDbContext())
                {
                    var newReservation = new Reservation
                    {
                        IdGuest = currentGuest.IdGuest,
                        IdRoom = (int)cmbAvailableRooms.SelectedValue,
                        NumberOfGuests = (int)numGuests.Value,
                        CheckInDate = DateOnly.FromDateTime(dtpCheckIn.Value),
                        CheckOutDate = DateOnly.FromDateTime(dtpCheckOut.Value),
                        BookingStatus = "підтверджено",
                        TotalPrice = currentTotalPrice,
                        IdDiscount = null
                    };

                    context.Reservations.Add(newReservation);
                    await context.SaveChangesAsync();

                    var guestNameParam = new MySqlParameter("p_name_guest", currentGuest.GuestLastName);
                    await context.Database.ExecuteSqlRawAsync("CALL update_regular_guest_status(@p_name_guest)", guestNameParam);

                    MessageBox.Show("Бронювання успішно створено!", Strings.AddBookingTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження бронювання: {ex.InnerException?.Message ?? ex.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // --- Допоміжні методи ---

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            currentGuest = null;
            txtPhoneNumber.Clear();
            lblGuestName.Text = $"({Strings.Booking_GuestInfo})";
            lblGuestName.Font = new Font(commonFont, FontStyle.Italic);
            numGuests.Value = 1;
            dtpCheckIn.Value = DateTime.Now;
            dtpCheckOut.Value = DateTime.Now.AddDays(1);
            ResetRoomSearch();
        }

        private Label CreateLabel(string text) => new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
            Margin = new Padding(5),
            Font = commonFont,
            ForeColor = ThemeManager.TextColor
        };

        private void SetupDatePickerTheme(DateTimePicker picker)
        {
            picker.CalendarMonthBackground = ThemeManager.InputBackground;
            picker.CalendarForeColor = ThemeManager.InputForeColor;
            picker.CalendarTitleBackColor = ThemeManager.ButtonBackground;
            picker.CalendarTitleForeColor = ThemeManager.ButtonForeColor;
            picker.CalendarTrailingForeColor = Color.Gray;
        }

        private void CenterControls()
        {
            bookingBox.Left = (this.ClientSize.Width - bookingBox.Width) / 2;
            bookingBox.Top = (this.ClientSize.Height - bookingBox.Height) / 2;
        }
    }
}