using System;
using System.Drawing;
using System.Windows.Forms;

namespace Hotel
{
    public partial class Form1 : Form
    {
        private Panel pnlContent = null!;

        public Form1()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "Система управління готелем";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(950, 650);
            this.BackColor = Color.FromArgb(240, 240, 240);

            var leftPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = 220,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.TopDown,
                BackColor = Color.FromArgb(225, 225, 225)
            };
            this.Controls.Add(leftPanel);

            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(300)
            };
            this.Controls.Add(pnlContent);

            var buttonMappings = new[]
            {
                new { Text = "Перевірити доступність", ClickAction = (Action<object?, EventArgs>)BtnCheckAvailability_Click },
                new { Text = "Додати гостя", ClickAction = (Action<object?, EventArgs>)BtnAddGuest_Click },
                new { Text = "Список гостей", ClickAction = (Action<object?, EventArgs>)BtnListGuests_Click },
                new { Text = "Додати бронювання", ClickAction = (Action<object?, EventArgs>)BtnAddBooking_Click },
                new { Text = "Список бронювань", ClickAction = (Action<object?, EventArgs>)BtnListBookings_Click },
                new { Text = "Розрахувати вартість", ClickAction = (Action<object?, EventArgs>)BtnCalculatePrice_Click },
                new { Text = "Оновити статус номерів", ClickAction = (Action<object?, EventArgs>)BtnUpdateStatus_Click }
            };

            foreach (var mapping in buttonMappings)
            {
                var button = new Button
                {
                    Text = mapping.Text,
                    Size = new Size(190, 40),
                    Margin = new Padding(0, 0, 0, 10),
                    Font = new Font("Segoe UI", 10F)
                };
                button.Click += new EventHandler(mapping.ClickAction);
                leftPanel.Controls.Add(button);
            }

            this.Load += (sender, e) => ShowControl(new WelcomeControl());
        }

        private void ShowControl(Control control)
        {
            pnlContent.Controls.Clear();
            control.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(control);
        }

        private void BtnCheckAvailability_Click(object? sender, EventArgs e) => ShowControl(new CheckAvailabilityControl());
        private void BtnAddGuest_Click(object? sender, EventArgs e) => ShowControl(new AddGuestControl());
        private void BtnListGuests_Click(object? sender, EventArgs e) => ShowControl(new ListGuestsControl());
        private void BtnAddBooking_Click(object? sender, EventArgs e) => ShowControl(new AddBookingControl());
        private void BtnListBookings_Click(object? sender, EventArgs e) => ShowControl(new ListBookingsControl());
        private void BtnCalculatePrice_Click(object? sender, EventArgs e) => ShowControl(new CalculatePriceControl());
        private void BtnUpdateStatus_Click(object? sender, EventArgs e) => ShowControl(new UpdateRoomStatusControl());
    }
}