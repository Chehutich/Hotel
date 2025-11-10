using Hotel.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using Hotel.Localization;

namespace Hotel
{
    public class AddBookingControl : UserControl
    {
        private TextBox txtGuestId, txtRoomId;
        private DateTimePicker dtpCheckIn, dtpCheckOut;
        private ComboBox cmbStatus;
        private GroupBox bookingBox;
        private Font commonFont = new Font("Segoe UI", 11F);
        private Font pickerFont = new Font("Segoe UI", 12F);
        private Dictionary<string, string> bookingStatusOptions;

        public AddBookingControl()
        {
            bookingStatusOptions = new Dictionary<string, string>
            {
                { "підтверджено", Strings.Status_Booking_Confirmed },
                { "скасовано", Strings.Status_Booking_Cancelled }
            };

            bookingBox = new GroupBox
            {
                Text = Strings.AddBookingTitle,
                Dock = DockStyle.None,
                Width = 800,
                Height = 480,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Padding = new Padding(25),
                ForeColor = ThemeManager.TextColor // (ЗМІНЕНО)
            };

            var layoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6
            };
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // (ЗМІНЕНО) Кольори полів вводу
            txtGuestId = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
            txtRoomId = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };

            // (ЗМІНЕНО) Кольори календаря
            dtpCheckIn = new DateTimePicker { Dock = DockStyle.Fill, Margin = new Padding(5), Font = pickerFont, Format = DateTimePickerFormat.Long };
            dtpCheckIn.CalendarMonthBackground = ThemeManager.InputBackground;
            dtpCheckIn.CalendarForeColor = ThemeManager.InputForeColor;
            dtpCheckIn.CalendarTitleBackColor = ThemeManager.ButtonBackground;
            dtpCheckIn.CalendarTitleForeColor = ThemeManager.ButtonForeColor;
            dtpCheckIn.CalendarTrailingForeColor = Color.Gray;

            dtpCheckOut = new DateTimePicker { Dock = DockStyle.Fill, Margin = new Padding(5), Font = pickerFont, Format = DateTimePickerFormat.Long };
            dtpCheckOut.CalendarMonthBackground = ThemeManager.InputBackground;
            dtpCheckOut.CalendarForeColor = ThemeManager.InputForeColor;
            dtpCheckOut.CalendarTitleBackColor = ThemeManager.ButtonBackground;
            dtpCheckOut.CalendarTitleForeColor = ThemeManager.ButtonForeColor;
            dtpCheckOut.CalendarTrailingForeColor = Color.Gray;

            // (ЗМІНЕНО) Кольори ComboBox
            cmbStatus = new ComboBox { Dock = DockStyle.Fill, Margin = new Padding(5), DropDownStyle = ComboBoxStyle.DropDownList, Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };

            cmbStatus.DataSource = new BindingSource(bookingStatusOptions, null);
            cmbStatus.DisplayMember = "Value";
            cmbStatus.ValueMember = "Key";

            layoutPanel.Controls.Add(CreateLabel(Strings.LabelGuestID), 0, 0);
            layoutPanel.Controls.Add(txtGuestId, 1, 0);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelRoomID), 0, 1);
            layoutPanel.Controls.Add(txtRoomId, 1, 1);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelCheckIn), 0, 2);
            layoutPanel.Controls.Add(dtpCheckIn, 1, 2);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelCheckOut), 0, 3);
            layoutPanel.Controls.Add(dtpCheckOut, 1, 3);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelBookingStatus), 0, 4);
            layoutPanel.Controls.Add(cmbStatus, 1, 4);

            var buttonPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill, Padding = new Padding(0, 15, 0, 0) };

            // (ЗМІНЕНО) Кольори кнопок
            var btnSave = new Button { Text = Strings.ButtonSave, Width = 130, Height = 40, Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };
            var btnCancel = new Button { Text = Strings.ButtonCancel, Width = 130, Height = 40, Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };

            buttonPanel.Controls.Add(btnSave);
            buttonPanel.Controls.Add(btnCancel);
            layoutPanel.Controls.Add(buttonPanel, 1, 5);

            bookingBox.Controls.Add(layoutPanel);
            this.Controls.Add(bookingBox);

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            this.Load += (sender, e) => CenterControls();
            this.Resize += (sender, e) => CenterControls();
        }

        private void CenterControls()
        {
            bookingBox.Left = (this.ClientSize.Width - bookingBox.Width) / 2;
            bookingBox.Top = (this.ClientSize.Height - bookingBox.Height) / 2;
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtGuestId.Clear();
            txtRoomId.Clear();
            dtpCheckIn.Value = DateTime.Now;
            dtpCheckOut.Value = DateTime.Now;
            cmbStatus.SelectedIndex = -1;
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtGuestId.Text, out int guestId) || !int.TryParse(txtRoomId.Text, out int roomId)) { MessageBox.Show(Strings.ValidationIdError, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (dtpCheckOut.Value <= dtpCheckIn.Value) { MessageBox.Show(Strings.ValidationDateError, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (cmbStatus.SelectedValue == null) { MessageBox.Show(Strings.ValidationStatusError, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try
            {
                using (var context = new HotelDbContext())
                {
                    var newReservation = new Reservation
                    {
                        IdGuest = guestId,
                        IdRoom = roomId,
                        CheckInDate = DateOnly.FromDateTime(dtpCheckIn.Value),
                        CheckOutDate = DateOnly.FromDateTime(dtpCheckOut.Value),
                        BookingStatus = cmbStatus.SelectedValue.ToString()!
                    };
                    context.Reservations.Add(newReservation);
                    await context.SaveChangesAsync();
                    MessageBox.Show("Бронювання успішно створено!", Strings.AddBookingTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження бронювання: {ex.InnerException?.Message ?? ex.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // (ЗМІНЕНО) Колір тексту Label
        private Label CreateLabel(string text) => new Label { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Margin = new Padding(5), Font = commonFont, ForeColor = ThemeManager.TextColor };
    }
}