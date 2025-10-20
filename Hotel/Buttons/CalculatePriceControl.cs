using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class CalculatePriceControl : UserControl
{
    private TextBox txtGuestId, txtRoomId, txtResult;
    private DateTimePicker dtpCheckIn, dtpCheckOut;

    public CalculatePriceControl()
    {
        var calcBox = new GroupBox
        {
            Text = "Розрахунок вартості проживання",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            Padding = new Padding(20)
        };

        var layoutPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 6 };
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        txtGuestId = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5) };
        txtRoomId = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5) };
        dtpCheckIn = new DateTimePicker { Dock = DockStyle.Fill, Margin = new Padding(5) };
        dtpCheckOut = new DateTimePicker { Dock = DockStyle.Fill, Margin = new Padding(5) };
        txtResult = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), ReadOnly = true, BackColor = Color.White };

        layoutPanel.Controls.Add(CreateLabel("Номер гостя:"), 0, 0);
        layoutPanel.Controls.Add(txtGuestId, 1, 0);
        layoutPanel.Controls.Add(CreateLabel("Номер кімнати:"), 0, 1);
        layoutPanel.Controls.Add(txtRoomId, 1, 1);
        layoutPanel.Controls.Add(CreateLabel("Дата реєстрації:"), 0, 2);
        layoutPanel.Controls.Add(dtpCheckIn, 1, 2);
        layoutPanel.Controls.Add(CreateLabel("Дата виїзду:"), 0, 3);
        layoutPanel.Controls.Add(dtpCheckOut, 1, 3);
        layoutPanel.Controls.Add(CreateLabel("Результат:"), 0, 4);
        layoutPanel.Controls.Add(txtResult, 1, 4);

        var buttonPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill };
        var btnCalculate = new Button { Text = "Розрахувати", Width = 120 };
        buttonPanel.Controls.Add(btnCalculate);
        layoutPanel.Controls.Add(buttonPanel, 1, 5);

        calcBox.Controls.Add(layoutPanel);
        this.Controls.Add(calcBox);

        btnCalculate.Click += BtnCalculate_Click;
    }

    private async void BtnCalculate_Click(object? sender, EventArgs e)
    {
        if (!int.TryParse(txtGuestId.Text, out int guestId) || !int.TryParse(txtRoomId.Text, out int roomId))
        {
            MessageBox.Show("ID гостя та кімнати повинні бути числами.", "Помилка вводу", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            using (var context = new HotelDbContext())
            {
                var guest = await context.Guests.FindAsync(guestId);
                if (guest == null)
                {
                    MessageBox.Show($"Гостя з ID {guestId} не знайдено.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create parameters for the stored procedure
                var lastNameParam = new MySqlParameter("p_name_guest", guest.GuestLastName);
                var roomIdParam = new MySqlParameter("p_id_room", roomId);
                var checkInParam = new MySqlParameter("p_check_in_date", dtpCheckIn.Value);
                var checkOutParam = new MySqlParameter("p_check_out_date", dtpCheckOut.Value);
                var totalPriceParam = new MySqlParameter("p_total_price", MySqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Output
                };

                // Execute the stored procedure
                await context.Database.ExecuteSqlRawAsync(
                    "CALL calculate_total_price(@p_name_guest, @p_id_room, @p_check_in_date, @p_check_out_date, @p_total_price)",
                    lastNameParam, roomIdParam, checkInParam, checkOutParam, totalPriceParam);

                // Get the result from the output parameter
                if (totalPriceParam.Value != DBNull.Value)
                {
                    txtResult.Text = Convert.ToDecimal(totalPriceParam.Value).ToString("F2");
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка розрахунку: {ex.Message}", "Помилка бази даних", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Label CreateLabel(string text) => new Label { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, Margin = new Padding(5) };
}