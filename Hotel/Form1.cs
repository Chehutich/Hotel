using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
            this.Size = new Size(1024, 768); // Трохи збільшив базовий розмір
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.WindowState = FormWindowState.Maximized;

            var mainLeftContainer = new Panel
            {
                Dock = DockStyle.Left,
                Width = 260, // ЗБІЛЬШЕНО
                BackColor = Color.FromArgb(225, 225, 225)
            };

            var homePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 70, // ЗБІЛЬШЕНО
                Padding = new Padding(15, 10, 10, 10), // Змінено відступ
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight
            };

            const string YOUR_HOME_ICON_FILE_NAME = "home_icon.png";
            var homePictureBox = new PictureBox
            {
                Size = new Size(45, 45), // ЗБІЛЬШЕНО
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand
            };
            LoadIconToPictureBox(homePictureBox, YOUR_HOME_ICON_FILE_NAME);
            homePictureBox.Click += BtnHome_Click;
            homePanel.Controls.Add(homePictureBox);

            const string YOUR_SETTINGS_ICON_FILE_NAME = "settings_icon.png";
            var settingsPictureBox = new PictureBox
            {
                Size = new Size(45, 45), // ЗБІЛЬШЕНО
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 5, 0, 0) // Змінено відступ
            };
            LoadIconToPictureBox(settingsPictureBox, YOUR_SETTINGS_ICON_FILE_NAME);
            settingsPictureBox.Click += BtnSettings_Click;
            homePanel.Controls.Add(settingsPictureBox);

            var buttonFlowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15, 10, 10, 10), // Змінено відступ
                FlowDirection = FlowDirection.TopDown,
                BackColor = Color.Transparent
            };

            mainLeftContainer.Controls.Add(homePanel);
            mainLeftContainer.Controls.Add(buttonFlowPanel);

            this.Controls.Add(mainLeftContainer);

            pnlContent = new Panel
            {
                Padding = new Padding(30), // ЗБІЛЬШЕНО
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            this.Controls.Add(pnlContent);

            var buttonMappings = new[]
            {
                new { Text = "Перевірити доступність", ClickAction = (Action<object?, EventArgs>)BtnCheckAvailability_Click },
                new { Text = "Додати гостя", ClickAction = (Action<object?, EventArgs>)BtnAddGuest_Click },
                new { Text = "Список гостей", ClickAction = (Action<object?, EventArgs>)BtnListGuests_Click },
                new { Text = "Список кімнат", ClickAction = (Action<object?, EventArgs>)BtnListRooms_Click },
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
                    Size = new Size(230, 50), // ЗБІЛЬШЕНО
                    Margin = new Padding(0, 0, 0, 10),
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular), // ЗБІЛЬШЕНО
                    BackColor = SystemColors.Control
                };
                button.Click += new EventHandler(mapping.ClickAction);
                buttonFlowPanel.Controls.Add(button);
            }

            this.Load += (sender, e) => ShowControl(new WelcomeControl());
        }

        private void LoadIconToPictureBox(PictureBox pb, string iconFileName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = "Hotel.images." + iconFileName;

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        pb.Image = Image.FromStream(stream);
                    }
                    else
                    {
                        pb.BackColor = Color.LightCoral;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження іконки '{iconFileName}': {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                pb.BackColor = Color.Red;
            }
        }

        private void ShowControl(Control control)
        {
            pnlContent.Controls.Clear();
            control.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(control);
        }

        private void BtnHome_Click(object? sender, EventArgs e) => ShowControl(new WelcomeControl());
        private void BtnCheckAvailability_Click(object? sender, EventArgs e) => ShowControl(new CheckAvailabilityControl());
        private void BtnAddGuest_Click(object? sender, EventArgs e) => ShowControl(new AddGuestControl());
        private void BtnListGuests_Click(object? sender, EventArgs e) => ShowControl(new ListGuestsControl());
        private void BtnListRooms_Click(object? sender, EventArgs e) => ShowControl(new ListRoomsControl());
        private void BtnAddBooking_Click(object? sender, EventArgs e) => ShowControl(new AddBookingControl());
        private void BtnListBookings_Click(object? sender, EventArgs e) => ShowControl(new ListBookingsControl());
        private void BtnCalculatePrice_Click(object? sender, EventArgs e) => ShowControl(new CalculatePriceControl());
        private void BtnUpdateStatus_Click(object? sender, EventArgs e) => ShowControl(new UpdateRoomStatusControl());
        private void BtnSettings_Click(object? sender, EventArgs e) => ShowControl(new SettingsControl());
    }
}
