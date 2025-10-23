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

        // Ініціалізація компонентів головної форми
        private void InitializeUI()
        {
            this.Text = "Система управління готелем";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(950, 650);
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.WindowState = FormWindowState.Maximized;

            var mainLeftContainer = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(225, 225, 225)
            };

            var homePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10),
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight
            };

            const string YOUR_HOME_ICON_FILE_NAME = "home_icon.png";
            var homePictureBox = new PictureBox
            {
                Size = new Size(40, 40),
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand
            };
            LoadIconToPictureBox(homePictureBox, YOUR_HOME_ICON_FILE_NAME);
            homePictureBox.Click += BtnHome_Click;
            homePanel.Controls.Add(homePictureBox);

            const string YOUR_SETTINGS_ICON_FILE_NAME = "settings_icon.png";
            var settingsPictureBox = new PictureBox
            {
                Size = new Size(40, 40),
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 3, 0, 0)
            };
            LoadIconToPictureBox(settingsPictureBox, YOUR_SETTINGS_ICON_FILE_NAME);
            settingsPictureBox.Click += BtnSettings_Click;
            homePanel.Controls.Add(settingsPictureBox);

            var buttonFlowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.TopDown,
                BackColor = Color.Transparent
            };

            mainLeftContainer.Controls.Add(homePanel);
            mainLeftContainer.Controls.Add(buttonFlowPanel);

            this.Controls.Add(mainLeftContainer);

            pnlContent = new Panel
            {
                Padding = new Padding(20),
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

            // Створення кнопок головного меню
            foreach (var mapping in buttonMappings)
            {
                var button = new Button
                {
                    Text = mapping.Text,
                    Size = new Size(190, 40),
                    Margin = new Padding(0, 0, 0, 10),
                    Font = new Font("Segoe UI", 10F),
                    BackColor = SystemColors.Control
                };
                button.Click += new EventHandler(mapping.ClickAction);
                buttonFlowPanel.Controls.Add(button);
            }

            this.Load += (sender, e) => ShowControl(new WelcomeControl());
        }

        // Завантаження іконки з вбудованих ресурсів
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

        // Відображення вибраного UserControl на панелі контенту
        private void ShowControl(Control control)
        {
            pnlContent.Controls.Clear();
            control.Dock = DockStyle.Fill; // Завжди розтягуємо
            pnlContent.Controls.Add(control);
        }

        // Обробники натискання кнопок для відображення відповідних UserControl
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