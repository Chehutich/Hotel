using Hotel.Forms;
using Hotel.Localization;
using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks; // (ДОДАНО)

namespace Hotel
{
    public class WelcomeControl : UserControl
    {
        private GroupBox dashBox;
        private GroupBox checkInBox, checkOutBox, roomStatusBox;
        private DataGridView dgvCheckIns, dgvCheckOuts;
        private Label lblAvailable, lblOccupied, lblCleaning, lblRepair;
        private Font commonFont = new Font("Segoe UI", 10F);
        private Font boldFont = new Font("Segoe UI", 11F, FontStyle.Bold);

        public WelcomeControl()
        {
            InitializeDashboard();
            this.Load += WelcomeControl_Load;
        }

        private async void WelcomeControl_Load(object? sender, EventArgs e)
        {
            await LoadDashboardData();
        }

        private void InitializeDashboard()
        {
            this.BackColor = ThemeManager.ContentBackground;

            dashBox = new GroupBox
            {
                Text = Strings.Dashboard_Title,
                Dock = DockStyle.None,
                Width = 1100,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Padding = new Padding(15),
                ForeColor = ThemeManager.TextColor
            };

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 2
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            checkInBox = new GroupBox
            {
                Text = string.Format(Strings.Dashboard_CheckIns, 0),
                Dock = DockStyle.Fill,
                Font = boldFont,
                Padding = new Padding(10),
                ForeColor = ThemeManager.TextColor
            };
            dgvCheckIns = CreateStandardDataGridView();
            checkInBox.Controls.Add(dgvCheckIns);

            checkOutBox = new GroupBox
            {
                Text = string.Format(Strings.Dashboard_CheckOuts, 0),
                Dock = DockStyle.Fill,
                Font = boldFont,
                Padding = new Padding(10),
                ForeColor = ThemeManager.TextColor
            };
            dgvCheckOuts = CreateStandardDataGridView();
            checkOutBox.Controls.Add(dgvCheckOuts);

            roomStatusBox = new GroupBox
            {
                Text = Strings.Dashboard_RoomStatus,
                Dock = DockStyle.Fill,
                Font = boldFont,
                Padding = new Padding(10),
                ForeColor = ThemeManager.TextColor
            };
            var statusLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 4 };
            statusLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            statusLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            // (ПОЧАТОК ЗМІН) --- Прибираємо кольори ---
            lblAvailable = CreateStatusLabel();
            lblOccupied = CreateStatusLabel();
            lblCleaning = CreateStatusLabel();
            lblRepair = CreateStatusLabel();
            // (КІНЕЦЬ ЗМІН) --------------------------

            statusLayout.Controls.Add(CreateStatusHeader(Strings.Dashboard_Available), 0, 0);
            statusLayout.Controls.Add(lblAvailable, 1, 0);
            statusLayout.Controls.Add(CreateStatusHeader(Strings.Dashboard_Occupied), 0, 1);
            statusLayout.Controls.Add(lblOccupied, 1, 1);
            statusLayout.Controls.Add(CreateStatusHeader(Strings.Dashboard_Cleaning), 0, 2);
            statusLayout.Controls.Add(lblCleaning, 1, 2);
            statusLayout.Controls.Add(CreateStatusHeader(Strings.Dashboard_Repair), 0, 3);
            statusLayout.Controls.Add(lblRepair, 1, 3);
            roomStatusBox.Controls.Add(statusLayout);

            mainLayout.Controls.Add(checkInBox, 0, 0);
            mainLayout.Controls.Add(checkOutBox, 1, 0);
            mainLayout.Controls.Add(roomStatusBox, 0, 1);

            dashBox.Controls.Add(mainLayout);
            this.Controls.Add(dashBox);

            this.Resize += (sender, e) => CenterControls();
            CenterControls();
        }

        private async Task LoadDashboardData()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            try
            {
                using (var context = new HotelDbContext())
                {
                    // --- 1. Запит Check-Ins ---
                    var checkIns = await context.Reservations
                        .Include(r => r.IdGuestNavigation)
                        .Where(r => r.BookingStatus == "підтверджено" && r.CheckInDate == today)
                        .Select(r => new {
                            Гість = r.IdGuestNavigation.GuestFirstName + " " + r.IdGuestNavigation.GuestLastName,
                            Кімната = r.IdRoom,
                            Гостей = r.NumberOfGuests
                        })
                        .ToListAsync();

                    dgvCheckIns.DataSource = checkIns;
                    checkInBox.Text = string.Format(Strings.Dashboard_CheckIns, checkIns.Count);
                    if (dgvCheckIns.Columns.Contains("Гість")) dgvCheckIns.Columns["Гість"].HeaderText = Strings.Dashboard_Guest;
                    if (dgvCheckIns.Columns.Contains("Кімната")) dgvCheckIns.Columns["Кімната"].HeaderText = Strings.Col_RoomID;
                    if (dgvCheckIns.Columns.Contains("Гостей")) dgvCheckIns.Columns["Гостей"].HeaderText = Strings.Col_NumGuests;

                    if (dgvCheckIns.Columns.Contains("Кімната")) dgvCheckIns.Columns["Кімната"].FillWeight = 50;
                    if (dgvCheckIns.Columns.Contains("Гостей")) dgvCheckIns.Columns["Гостей"].FillWeight = 50;


                    // --- 2. Запит Check-Outs ---
                    var checkOuts = await context.Reservations
                        .Include(r => r.IdGuestNavigation)
                        .Where(r => r.BookingStatus == "Проживає" && r.CheckOutDate == today)
                        .Select(r => new {
                            Гість = r.IdGuestNavigation.GuestFirstName + " " + r.IdGuestNavigation.GuestLastName,
                            Кімната = r.IdRoom
                        })
                        .ToListAsync();

                    dgvCheckOuts.DataSource = checkOuts;
                    checkOutBox.Text = string.Format(Strings.Dashboard_CheckOuts, checkOuts.Count);
                    if (dgvCheckOuts.Columns.Contains("Гість")) dgvCheckOuts.Columns["Гість"].HeaderText = Strings.Dashboard_Guest;
                    if (dgvCheckOuts.Columns.Contains("Кімната")) dgvCheckOuts.Columns["Кімната"].HeaderText = Strings.Col_RoomID;

                    if (dgvCheckOuts.Columns.Contains("Кімната")) dgvCheckOuts.Columns["Кімната"].FillWeight = 50;

                    // --- 3. Запит Статусу Кімнат ---
                    var roomStats = await context.HotelRooms
                        .AsNoTracking()
                        .GroupBy(r => r.RoomStatus)
                        .Select(g => new { Status = g.Key, Count = g.Count() })
                        .ToListAsync();

                    lblAvailable.Text = roomStats.FirstOrDefault(s => s.Status == "доступна")?.Count.ToString() ?? "0";
                    lblOccupied.Text = roomStats.FirstOrDefault(s => s.Status == "Зайнята")?.Count.ToString() ?? "0";
                    lblCleaning.Text = roomStats.FirstOrDefault(s => s.Status == "на прибиранні")?.Count.ToString() ?? "0";
                    lblRepair.Text = roomStats.FirstOrDefault(s => s.Status == "на ремонті")?.Count.ToString() ?? "0";
                }
            }
            catch (Exception ex)
            {
                ShowErrorLabel(this, $"Помилка завантаження панелі: {ex.Message}");
            }
        }

        private DataGridView CreateStandardDataGridView()
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = ThemeManager.GridBackground,
                BorderStyle = BorderStyle.None,
                Font = commonFont
            };

            dgv.ColumnHeadersDefaultCellStyle.Font = boldFont;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.GridHeaderBackground;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ThemeManager.GridHeaderForeColor;
            dgv.DefaultCellStyle.BackColor = ThemeManager.InputBackground;
            dgv.DefaultCellStyle.ForeColor = ThemeManager.InputForeColor;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersVisible = false;
            dgv.RowTemplate.Height = 30;
            return dgv;
        }

        private Label CreateStatusHeader(string text) => new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            Font = boldFont,
            ForeColor = ThemeManager.TextColor,
            TextAlign = ContentAlignment.MiddleRight
        };

        // (ПОЧАТОК ЗМІН) --- Прибираємо 'Color color' ---
        private Label CreateStatusLabel() => new Label
        {
            Text = "0",
            Dock = DockStyle.Fill,
            Font = new Font(boldFont, FontStyle.Bold),
            ForeColor = ThemeManager.TextColor, // (ЗМІНЕНО)
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(10, 0, 0, 0)
        };
        // (КІНЕЦЬ ЗМІН) --------------------------

        private void CenterControls()
        {
            dashBox.Height = this.ClientSize.Height - 10;
            dashBox.Left = (this.ClientSize.Width - dashBox.Width) / 2;
            dashBox.Top = 5;
        }

        private void ShowErrorLabel(Control parent, string message)
        {
            parent.Controls.Clear();
            var label = new Label
            {
                Text = message,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Red,
                BackColor = ThemeManager.ContentBackground,
                Font = new Font("Segoe UI", 12F)
            };
            parent.Controls.Add(label);
        }
    }
}