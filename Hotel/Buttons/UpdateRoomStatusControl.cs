using Hotel.Models;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Linq;
using System.Collections.Generic;
using Hotel.Localization;

namespace Hotel
{
    public class UpdateRoomStatusControl : UserControl
    {
        private TextBox txtRoomId;
        private ComboBox cmbStatus;
        private GroupBox statusBox;
        private Font commonFont = new Font("Segoe UI", 11F);
        private Dictionary<string, string> roomStatusOptions;

        public UpdateRoomStatusControl()
        {
            roomStatusOptions = new Dictionary<string, string>
            {
                { "доступна", Strings.Status_Available },
                { "на прибиранні", Strings.Status_Cleaning },
                { "на ремонті", Strings.Status_Repair }
            };

            statusBox = new GroupBox
            {
                Text = Strings.UpdateRoomStatusTitle,
                Dock = DockStyle.None,
                Width = 800,
                Height = 300,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Padding = new Padding(25),
                ForeColor = ThemeManager.TextColor // (ЗМІНЕНО)
            };

            var layoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3
            };
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // (ЗМІНЕНО) Кольори полів вводу
            txtRoomId = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
            cmbStatus = new ComboBox { Dock = DockStyle.Fill, Margin = new Padding(5), DropDownStyle = ComboBoxStyle.DropDownList, Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };

            cmbStatus.DataSource = new BindingSource(roomStatusOptions, null);
            cmbStatus.DisplayMember = "Value";
            cmbStatus.ValueMember = "Key";

            layoutPanel.Controls.Add(CreateLabel(Strings.LabelRoomID), 0, 0);
            layoutPanel.Controls.Add(txtRoomId, 1, 0);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelNewStatus), 0, 1);
            layoutPanel.Controls.Add(cmbStatus, 1, 1);

            var buttonPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill, Padding = new Padding(0, 15, 0, 0) };

            // (ЗМІНЕНО) Кольори кнопок
            var btnSave = new Button { Text = Strings.ButtonSave, Width = 130, Height = 40, Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };
            var btnCancel = new Button { Text = Strings.ButtonCancel, Width = 130, Height = 40, Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };

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

        private void CenterControls()
        {
            statusBox.Left = (this.ClientSize.Width - statusBox.Width) / 2;
            statusBox.Top = (this.ClientSize.Height - statusBox.Height) / 2;
        }

        private void ClearForm()
        {
            txtRoomId.Clear();
            cmbStatus.SelectedIndex = -1;
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            ClearForm();
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtRoomId.Text, out int roomId)) { MessageBox.Show(Strings.ValidationIdError, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (cmbStatus.SelectedValue == null) { MessageBox.Show(Strings.ValidationStatusError, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try
            {
                using (var context = new HotelDbContext())
                {
                    var roomToUpdate = await context.HotelRooms.FindAsync(roomId);
                    if (roomToUpdate == null) { MessageBox.Show($"Кімнату з ID {roomId} не знайдено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                    roomToUpdate.RoomStatus = cmbStatus.SelectedValue.ToString()!;
                    await context.SaveChangesAsync();

                    MessageBox.Show($"Статус кімнати {roomId} успішно оновлено!", Strings.UpdateRoomStatusTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка оновлення статусу: {ex.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // (ЗМІНЕНО) Колір тексту Label
        private Label CreateLabel(string text) => new Label { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Margin = new Padding(5), Font = commonFont, ForeColor = ThemeManager.TextColor };
    }
}