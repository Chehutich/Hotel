using Hotel.Models;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Hotel
{
    public class AddBookingControl : UserControl
    {
        private TextBox txtGuestId, txtRoomId;
        private DateTimePicker dtpCheckIn, dtpCheckOut;
        private ComboBox cmbStatus;
        private GroupBox bookingBox;
        private Font commonFont = new Font("Segoe UI", 11F);

        public AddBookingControl()
        {
            bookingBox = new GroupBox
            {
                Text = "Створення нового бронювання",
                Dock = DockStyle.None,
                Width = 800, // ЗБІЛЬШЕНО
                Height = 480, // ЗБІЛЬШЕНО
                Font = new Font("Segoe UI", 12F, FontStyle.Bold), // ЗБІЛЬШЕНО
                Padding = new Padding(25)
            };

            var layoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6
            };
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F)); // ЗБІЛЬШЕНО
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            txtGuestId = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont };
            txtRoomId = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont };
            dtpCheckIn = new DateTimePicker { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont, Format = DateTimePickerFormat.Long };
            dtpCheckOut = new DateTimePicker { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont, Format = DateTimePickerFormat.Long };
            cmbStatus = new ComboBox { Dock = DockStyle.Fill, Margin = new Padding(5), DropDownStyle = ComboBoxStyle.DropDownList, Font = commonFont };
            cmbStatus.Items.AddRange(new string[] { "підтверджено", "скасовано" });

            layoutPanel.Controls.Add(CreateLabel("Номер гостя:"), 0, 0);
            layoutPanel.Controls.Add(txtGuestId, 1, 0);
            layoutPanel.Controls.Add(CreateLabel("Номер кімнати:"), 0, 1);
            layoutPanel.Controls.Add(txtRoomId, 1, 1);
            layoutPanel.Controls.Add(CreateLabel("Дата реєстрації:"), 0, 2);
            layoutPanel.Controls.Add(dtpCheckIn, 1, 2);
            layoutPanel.Controls.Add(CreateLabel("Дата виїзду:"), 0, 3);
            layoutPanel.Controls.Add(dtpCheckOut, 1, 3);
            layoutPanel.Controls.Add(CreateLabel("Статус бронювання:"), 0, 4);
            layoutPanel.Controls.Add(cmbStatus, 1, 4);

            var buttonPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill, Padding = new Padding(0, 15, 0, 0) };
            var btnSave = new Button { Text = "Зберегти", Width = 130, Height = 40, Font = commonFont }; // ЗБІЛЬШЕНО
            var btnCancel = new Button { Text = "Скасувати", Width = 130, Height = 40, Font = commonFont }; // ЗБІЛЬШЕНО
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
            if (!int.TryParse(txtGuestId.Text, out int guestId) || !int.TryParse(txtRoomId.Text, out int roomId)) { MessageBox.Show("ID гостя та кімнати повинні бути числами.", "Помилка вводу", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (dtpCheckOut.Value <= dtpCheckIn.Value) { MessageBox.Show("Дата виїзду повинна бути пізніше дати заїзду.", "Помилка вводу", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (cmbStatus.SelectedItem == null) { MessageBox.Show("Будь ласка, виберіть статус бронювання.", "Помилка вводу", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

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
                        BookingStatus = cmbStatus.SelectedItem.ToString()!
                    };
                    context.Reservations.Add(newReservation);
                    await context.SaveChangesAsync();
                    MessageBox.Show("Бронювання успішно створено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження бронювання: {ex.InnerException?.Message ?? ex.Message}", "Помилка бази даних", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Label CreateLabel(string text) => new Label { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Margin = new Padding(5), Font = commonFont };
    }
}
