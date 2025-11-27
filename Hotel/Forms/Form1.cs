using Hotel.Buttons;
using Hotel.Core;
using Hotel.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Hotel.Forms
{
    public partial class Form1 : Form
    {
        private Panel pnlContent = null!;

        public Form1()
        {
            InitializeUI();
        }

        public void ShowControl(Control control)
        {
            pnlContent.Controls.Clear();
            control.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(control);
        }

        public void ShowListGuests()
        {
            ShowControl(new ListGuestsControl());
        }

        private string GetLocalizedRoleName(string dbJobTitle)
        {
            switch (dbJobTitle)
            {
                case "Рецепціоніст":
                    return Strings.Role_Receptionist;
                case "Адміністратор":
                    return Strings.Role_Administrator;
                default:
                    return Strings.Role_Unknown;
            }
        }

        private void RoleLabel_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                Strings.Logout_Confirm_Text,
                Strings.Logout_Confirm_Title,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Restart();
            }
        }

        private void InitializeUI()
        {
            string userNameDisplay = "Гість";
            string userRoleDisplay = Strings.Role_Unknown;

            // (ЗМІНЕНО) Використовуємо HotelAppContext
            if (HotelAppContext.CurrentUser != null)
            {
                userNameDisplay = $"{HotelAppContext.CurrentUser.StaffFirstName} {HotelAppContext.CurrentUser.StaffLastName}";
                userRoleDisplay = GetLocalizedRoleName(HotelAppContext.CurrentUser.JobTitle);
            }

            this.Text = $"{Strings.AppTitle} - ({userNameDisplay})";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1024, 768);
            this.BackColor = ThemeManager.FormBackground;
            this.WindowState = FormWindowState.Maximized;

            this.FormClosed += (sender, e) => Application.Exit();

            var mainLeftContainer = new Panel
            {
                Dock = DockStyle.Left,
                Width = 260,
                BackColor = ThemeManager.MenuBackground
            };

            var homePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 70,
                Padding = new Padding(15, 10, 10, 10),
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight
            };

            var homePictureBox = new PictureBox
            {
                Size = new Size(45, 45),
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand,
                Image = ThemeManager.HomeIcon
            };
            homePictureBox.Click += BtnHome_Click;
            homePanel.Controls.Add(homePictureBox);

            var settingsPictureBox = new PictureBox
            {
                Size = new Size(45, 45),
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 5, 0, 0),
                Image = ThemeManager.SettingsIcon
            };
            settingsPictureBox.Click += BtnSettings_Click;
            homePanel.Controls.Add(settingsPictureBox);

            var buttonFlowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15, 10, 10, 10),
                FlowDirection = FlowDirection.TopDown,
                BackColor = Color.Transparent
            };

            mainLeftContainer.Controls.Add(homePanel);
            mainLeftContainer.Controls.Add(buttonFlowPanel);

            this.Controls.Add(mainLeftContainer);

            pnlContent = new Panel
            {
                Padding = new Padding(30),
                Dock = DockStyle.Fill,
                BackColor = ThemeManager.ContentBackground
            };
            this.Controls.Add(pnlContent);

            var buttonList = new List<dynamic>();

            buttonList.Add(new { Text = Strings.CheckAvailability, ClickAction = (Action<object?, EventArgs>)BtnCheckAvailability_Click });
            buttonList.Add(new { Text = Strings.AddGuest, ClickAction = (Action<object?, EventArgs>)BtnAddGuest_Click });
            buttonList.Add(new { Text = Strings.ListGuests, ClickAction = (Action<object?, EventArgs>)BtnListGuests_Click });
            buttonList.Add(new { Text = Strings.ListRooms, ClickAction = (Action<object?, EventArgs>)BtnListRooms_Click });
            buttonList.Add(new { Text = Strings.AddBooking, ClickAction = (Action<object?, EventArgs>)BtnAddBooking_Click });
            buttonList.Add(new { Text = Strings.ListBookings, ClickAction = (Action<object?, EventArgs>)BtnListBookings_Click });
            buttonList.Add(new { Text = Strings.UpdateRoomStatus, ClickAction = (Action<object?, EventArgs>)BtnUpdateStatus_Click });

            // (ЗМІНЕНО) Використовуємо HotelAppContext
            if (HotelAppContext.CurrentUser != null && HotelAppContext.CurrentUser.JobTitle == "Адміністратор")
            {
                buttonList.Add(new { Text = Strings.Admin_Staff, ClickAction = (Action<object?, EventArgs>)((s, e) => ShowControl(new StaffManagementControl())) });
                buttonList.Add(new { Text = Strings.Admin_Prices, ClickAction = (Action<object?, EventArgs>)((s, e) => ShowControl(new RoomManagementControl())) });
            }

            foreach (var mapping in buttonList)
            {
                var button = new Button
                {
                    Text = mapping.Text,
                    Size = new Size(230, 50),
                    Margin = new Padding(0, 0, 0, 10),
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                    BackColor = ThemeManager.ButtonBackground,
                    ForeColor = ThemeManager.ButtonForeColor
                };

                Action<object?, EventArgs> action = mapping.ClickAction;
                button.Click += (s, e) => action(s, e);

                buttonFlowPanel.Controls.Add(button);
            }

            var roleLabel = new Label
            {
                Text = userRoleDisplay,
                Size = new Size(230, 50),
                Margin = new Padding(0, 20, 0, 10),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                BackColor = ThemeManager.ButtonBackground,
                ForeColor = ThemeManager.ButtonForeColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            roleLabel.Click += RoleLabel_Click;

            buttonFlowPanel.Controls.Add(roleLabel);

            this.Load += (sender, e) => ShowControl(new WelcomeControl());
        }

        private void BtnHome_Click(object? sender, EventArgs e) => ShowControl(new WelcomeControl());
        private void BtnCheckAvailability_Click(object? sender, EventArgs e) => ShowControl(new CheckAvailabilityControl());
        private void BtnAddGuest_Click(object? sender, EventArgs e) => ShowControl(new AddGuestControl());
        private void BtnListGuests_Click(object? sender, EventArgs e) => ShowListGuests();
        private void BtnListRooms_Click(object? sender, EventArgs e) => ShowControl(new ListRoomsControl());
        private void BtnAddBooking_Click(object? sender, EventArgs e) => ShowControl(new AddBookingControl());
        private void BtnListBookings_Click(object? sender, EventArgs e) => ShowControl(new ListBookingsControl());
        private void BtnUpdateStatus_Click(object? sender, EventArgs e) => ShowControl(new UpdateRoomStatusControl());
        private void BtnSettings_Click(object? sender, EventArgs e) => ShowControl(new SettingsControl());
    }
}