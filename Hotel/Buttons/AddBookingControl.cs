using Hotel.Models;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

public class AddBookingControl : UserControl
{
    private TextBox txtGuestId, txtRoomId;
    private DateTimePicker dtpCheckIn, dtpCheckOut;
    private ComboBox cmbStatus;
    private GroupBox bookingBox;

    public AddBookingControl()
    {
        bookingBox = new GroupBox
        {
            Text = "Створення нового бронювання",
            Dock = DockStyle.None,
            Width = 700,
            Height = 450,
            Font = new Font("Segoe UI", 10F),
            Padding = new Padding(20)
        };

        var layoutPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 6
        };
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        txtGuestId = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5) };
        txtRoomId = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5) };
        dtpCheckIn = new DateTimePicker { Dock = DockStyle.Fill, Margin = new Padding(5) };
        dtpCheckOut = new DateTimePicker { Dock = DockStyle.Fill, Margin = new Padding(5) };
        cmbStatus = new ComboBox { Dock = DockStyle.Fill, Margin = new Padding(5), DropDownStyle = ComboBoxStyle.DropDownList };
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

        var buttonPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill };
        var btnSave = new Button { Text = "Зберегти", Width = 100 };
        var btnCancel = new Button { Text = "Скасувати", Width = 100 };
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

    // Центрування головного GroupBox
    private void CenterControls()
    {
        bookingBox.Left = (this.ClientSize.Width - bookingBox.Width) / 2;
        bookingBox.Top = (this.ClientSize.Height - bookingBox.Height) / 2;
    }

    // Очищення полів форми
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

    // Збереження нового бронювання
    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        // Валідація введених даних
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

    private Label CreateLabel(string text) => new Label { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Margin = new Padding(5) };
}