using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Hotel.Localization;

namespace Hotel
{
    public class CalculatePriceControl : UserControl
    {
        private TextBox txtGuestId, txtRoomId, txtResult;
        private DateTimePicker dtpCheckIn, dtpCheckOut;
        private GroupBox calcBox;
        private Font commonFont = new Font("Segoe UI", 11F);
        private Font pickerFont = new Font("Segoe UI", 12F);

        public CalculatePriceControl()
        {
            calcBox = new GroupBox
            {
                Text = Strings.CalculatePriceTitle,
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

            // (ЗМІНЕНО) Колір поля результату
            txtResult = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), ReadOnly = true, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor, Font = commonFont };

            layoutPanel.Controls.Add(CreateLabel(Strings.LabelGuestID), 0, 0);
            layoutPanel.Controls.Add(txtGuestId, 1, 0);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelRoomID), 0, 1);
            layoutPanel.Controls.Add(txtRoomId, 1, 1);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelCheckIn), 0, 2);
            layoutPanel.Controls.Add(dtpCheckIn, 1, 2);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelCheckOut), 0, 3);
            layoutPanel.Controls.Add(dtpCheckOut, 1, 3);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelResult), 0, 4);
            layoutPanel.Controls.Add(txtResult, 1, 4);

            var buttonPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill, Padding = new Padding(0, 15, 0, 0) };

            // (ЗМІНЕНО) Кольори кнопок
            var btnCalculate = new Button { Text = Strings.ButtonCalculate, Width = 140, Height = 40, Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };
            var btnClear = new Button { Text = Strings.ButtonClear, Width = 130, Height = 40, Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };

            buttonPanel.Controls.Add(btnCalculate);
            buttonPanel.Controls.Add(btnClear);
            layoutPanel.Controls.Add(buttonPanel, 1, 5);

            calcBox.Controls.Add(layoutPanel);
            this.Controls.Add(calcBox);

            btnCalculate.Click += BtnCalculate_Click;
            btnClear.Click += BtnClear_Click;

            this.Load += (sender, e) => CenterControls();
            this.Resize += (sender, e) => CenterControls();
        }

        private void CenterControls()
        {
            calcBox.Left = (this.ClientSize.Width - calcBox.Width) / 2;
            calcBox.Top = (this.ClientSize.Height - calcBox.Height) / 2;
        }

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            txtGuestId.Clear();
            txtRoomId.Clear();
            dtpCheckIn.Value = DateTime.Now;
            dtpCheckOut.Value = DateTime.Now;
            txtResult.Clear();
        }

        private async void BtnCalculate_Click(object? sender, EventArgs e)
        {
            int guestId;
            int roomId;

            if (!int.TryParse(txtGuestId.Text, out guestId) || !int.TryParse(txtRoomId.Text, out roomId)) { MessageBox.Show(Strings.ValidationIdError, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try
            {
                using (var context = new HotelDbContext())
                {
                    var guest = await context.Guests.FindAsync(guestId);
                    if (guest == null) { MessageBox.Show($"Гостя з ID {guestId} не знайдено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                    var lastNameParam = new MySqlParameter("p_name_guest", guest.GuestLastName);
                    var roomIdParam = new MySqlParameter("p_id_room", roomId);
                    var checkInParam = new MySqlParameter("p_check_in_date", dtpCheckIn.Value);
                    var checkOutParam = new MySqlParameter("p_check_out_date", dtpCheckOut.Value);
                    var totalPriceParam = new MySqlParameter("p_total_price", MySqlDbType.Decimal) { Direction = ParameterDirection.Output };

                    await context.Database.ExecuteSqlRawAsync(
                        "CALL calculate_total_price(@p_name_guest, @p_id_room, @p_check_in_date, @p_check_out_date, @p_total_price)",
                        lastNameParam, roomIdParam, checkInParam, checkOutParam, totalPriceParam);

                    if (totalPriceParam.Value != DBNull.Value)
                    {
                        txtResult.Text = Convert.ToDecimal(totalPriceParam.Value).ToString("F2");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка розрахунку: {ex.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // (ЗМІНЕНО) Колір тексту Label
        private Label CreateLabel(string text) => new Label { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Margin = new Padding(5), Font = commonFont, ForeColor = ThemeManager.TextColor };
    }
}