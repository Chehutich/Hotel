using Hotel.Forms;
using Hotel.Localization;
using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Core; // (ДОДАНО)

namespace Hotel.Buttons
{
    public class UpdateRoomStatusControl : UserControl
    {
        // (ЗМІНЕНО) txtRoomId -> cmbRoomId
        private ComboBox cmbRoomId;
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
                { "на ремонті", Strings.Status_Repair },
                { "Зайнята", Strings.Status_Occupied }
            };

            statusBox = new GroupBox
            {
                Text = Strings.UpdateRoomStatusTitle,
                Dock = DockStyle.None,
                Width = 800,
                Height = 300,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Padding = new Padding(25),
                ForeColor = ThemeManager.TextColor
            };

            var layoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3
            };
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // (ЗМІНЕНО) Ініціалізуємо ComboBox для кімнат
            cmbRoomId = new ComboBox { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor, DropDownStyle = ComboBoxStyle.DropDownList };

            cmbStatus = new ComboBox { Dock = DockStyle.Fill, Margin = new Padding(5), DropDownStyle = ComboBoxStyle.DropDownList, Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };

            var assignableStatuses = roomStatusOptions
                .Where(kvp => kvp.Key != "Зайнята")
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            cmbStatus.DataSource = new BindingSource(assignableStatuses, null);
            cmbStatus.DisplayMember = "Value";
            cmbStatus.ValueMember = "Key";

            layoutPanel.Controls.Add(CreateLabel(Strings.LabelRoomID), 0, 0);
            layoutPanel.Controls.Add(cmbRoomId, 1, 0); // (ЗМІНЕНО) Додаємо cmbRoomId
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelNewStatus), 0, 1);
            layoutPanel.Controls.Add(cmbStatus, 1, 1);

            var buttonPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill, Padding = new Padding(0, 15, 0, 0) };
            var btnSave = new Button { Text = Strings.ButtonSave, Width = 130, Height = 40, Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };
            var btnCancel = new Button { Text = Strings.ButtonCancel, Width = 130, Height = 40, Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };
            buttonPanel.Controls.Add(btnSave);
            buttonPanel.Controls.Add(btnCancel);
            layoutPanel.Controls.Add(buttonPanel, 1, 2);

            statusBox.Controls.Add(layoutPanel);
            this.Controls.Add(statusBox);

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            // (ЗМІНЕНО) Додаємо обробник завантаження форми
            this.Load += UpdateRoomStatusControl_Load;
            this.Resize += (sender, e) => CenterControls();
        }

        // (НОВИЙ МЕТОД) Завантажує список кімнат при запуску
        private async void UpdateRoomStatusControl_Load(object? sender, EventArgs e)
        {
            await LoadRoomsToComboBox();
            CenterControls();
        }

        // (НОВИЙ МЕТОД) Логіка завантаження кімнат
        private async Task LoadRoomsToComboBox()
        {
            try
            {
                using (var context = new HotelDbContext())
                {
                    var roomIds = await context.HotelRooms
                                               .OrderBy(r => r.IdRooms)
                                               .Select(r => r.IdRooms)
                                               .ToListAsync();

                    cmbRoomId.DataSource = roomIds;
                    cmbRoomId.SelectedIndex = -1; // Скидаємо вибір
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження списку кімнат: {ex.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CenterControls()
        {
            statusBox.Left = (this.ClientSize.Width - statusBox.Width) / 2;
            statusBox.Top = (this.ClientSize.Height - statusBox.Height) / 2;
        }

        private void ClearForm()
        {
            // (ЗМІНЕНО)
            cmbRoomId.SelectedIndex = -1;
            cmbStatus.SelectedIndex = -1;
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            ClearForm();
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            // (ЗМІНЕНО) Перевіряємо ComboBox замість TextBox
            if (cmbRoomId.SelectedValue == null)
            {
                MessageBox.Show("Будь ласка, виберіть номер кімнати.", Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbStatus.SelectedValue == null)
            {
                MessageBox.Show(Strings.ValidationStatusError, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int roomId = (int)cmbRoomId.SelectedValue; // (ЗМІНЕНО)
            string newStatus = cmbStatus.SelectedValue.ToString();

            try
            {
                using (var context = new HotelDbContext())
                {
                    var roomToUpdate = await context.HotelRooms.FindAsync(roomId);
                    if (roomToUpdate == null)
                    {
                        MessageBox.Show($"Кімнату з ID {roomId} не знайдено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (newStatus == "доступна")
                    {
                        if (roomToUpdate.RoomStatus == "Зайнята")
                        {
                            MessageBox.Show($"Неможливо встановити статус 'доступна'. Кімната {roomId} зараз 'Зайнята'.\nСпочатку потрібно виселити гостя.", Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        var today = DateOnly.FromDateTime(DateTime.Now);
                        bool hasActiveBooking = await context.Reservations
                            .AnyAsync(r => r.IdRoom == roomId &&
                                           r.BookingStatus == "підтверджено" &&
                                           today >= r.CheckInDate &&
                                           today < r.CheckOutDate);

                        if (hasActiveBooking)
                        {
                            MessageBox.Show($"Неможливо встановити статус 'доступна'. На кімнату {roomId} є підтверджене бронювання на сьогодні.", Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    if (roomToUpdate.RoomStatus == "Зайнята" && (newStatus == "на ремонті" || newStatus == "на прибиранні"))
                    {
                        MessageBox.Show($"Неможливо змінити статус. Кімната {roomId} зараз 'Зайнята'.\nСпочатку потрібно виселити гостя.", Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    roomToUpdate.RoomStatus = newStatus;
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

        private Label CreateLabel(string text) => new Label { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Margin = new Padding(5), Font = commonFont, ForeColor = ThemeManager.TextColor };
    }
}