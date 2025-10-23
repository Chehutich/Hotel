using Hotel.Models;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Linq;

public class UpdateRoomStatusControl : UserControl
{
    private TextBox txtRoomId;
    private ComboBox cmbStatus;
    private GroupBox statusBox;

    public UpdateRoomStatusControl()
    {
        statusBox = new GroupBox
        {
            Text = "Оновлення статусу кімнати",
            Dock = DockStyle.None,
            Width = 700,
            Height = 250,
            Font = new Font("Segoe UI", 10F),
            Padding = new Padding(20)
        };

        var layoutPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3
        };
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        txtRoomId = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5) };
        cmbStatus = new ComboBox { Dock = DockStyle.Fill, Margin = new Padding(5), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbStatus.Items.AddRange(new string[] { "доступна", "на прибиранні", "на ремонті" });

        layoutPanel.Controls.Add(CreateLabel("Номер кімнати:"), 0, 0);
        layoutPanel.Controls.Add(txtRoomId, 1, 0);
        layoutPanel.Controls.Add(CreateLabel("Новий статус:"), 0, 1);
        layoutPanel.Controls.Add(cmbStatus, 1, 1);

        var buttonPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill };
        var btnSave = new Button { Text = "Зберегти", Width = 100 };
        var btnCancel = new Button { Text = "Скасувати", Width = 100 };
        buttonPanel.Controls.Add(btnSave);
        buttonPanel.Controls.Add(btnCancel);
        layoutPanel.Controls.Add(buttonPanel, 1, 2);

        statusBox.Controls.Add(layoutPanel);
        this.Controls.Add(statusBox);

        btnSave.Click += BtnSave_Click;
        btnCancel.Click += BtnCancel_Click;

        this.Load += (sender, e) => CenterControls();
        this.Resize += (sender, e) => CenterControls();
    }

    // Центрування головного GroupBox
    private void CenterControls()
    {
        statusBox.Left = (this.ClientSize.Width - statusBox.Width) / 2;
        statusBox.Top = (this.ClientSize.Height - statusBox.Height) / 2;
    }

    // Очищення полів форми
    private void ClearForm()
    {
        txtRoomId.Clear();
        cmbStatus.SelectedIndex = -1;
    }

    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        ClearForm();
    }

    // Оновлення статусу кімнати в базі даних
    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        // Валідація
        if (!int.TryParse(txtRoomId.Text, out int roomId)) { MessageBox.Show("ID кімнати має бути числом.", "Помилка вводу", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (cmbStatus.SelectedItem == null) { MessageBox.Show("Будь ласка, виберіть новий статус.", "Помилка вводу", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

        try
        {
            using (var context = new HotelDbContext())
            {
                var roomToUpdate = await context.HotelRooms.FindAsync(roomId);
                if (roomToUpdate == null) { MessageBox.Show($"Кімнату з ID {roomId} не знайдено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                roomToUpdate.RoomStatus = cmbStatus.SelectedItem.ToString()!;
                await context.SaveChangesAsync();

                MessageBox.Show($"Статус кімнати {roomId} успішно оновлено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка оновлення статусу: {ex.Message}", "Помилка бази даних", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Label CreateLabel(string text) => new Label { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Margin = new Padding(5) };
}