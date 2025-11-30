using Hotel.Core; // Для HotelAppContext, ThemeManager
using Hotel.Forms;
using Hotel.Localization;
using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hotel
{
    public class WelcomeControl : UserControl
    {
        // Загальні
        private GroupBox dashBox;
        private Font commonFont = new Font("Segoe UI", 10F);
        private Font boldFont = new Font("Segoe UI", 11F, FontStyle.Bold);
        private Font titleFont = new Font("Segoe UI", 16F, FontStyle.Bold);

        // Елементи Рецепціоніста
        private GroupBox checkInBox, checkOutBox, roomStatusBox;
        private DataGridView dgvCheckIns, dgvCheckOuts;
        private Label lblAvailable, lblOccupied, lblCleaning, lblRepair;

        // Елементи Адміністратора
        private Label lblRevenueToday, lblRevenueMonth, lblOccupancy;
        private DataGridView dgvPopularRooms, dgvActiveStaff;
        private GroupBox statsBox, staffBox;

        public WelcomeControl()
        {
            if (HotelAppContext.CurrentUser != null && HotelAppContext.CurrentUser.JobTitle == "Адміністратор")
            {
                InitializeAdminDashboard();
            }
            else
            {
                InitializeReceptionistDashboard();
            }

            this.Load += WelcomeControl_Load;
            this.Resize += (sender, e) => CenterControls();
        }

        private async void WelcomeControl_Load(object? sender, EventArgs e)
        {
            if (HotelAppContext.CurrentUser != null && HotelAppContext.CurrentUser.JobTitle == "Адміністратор")
            {
                await LoadAdminData();
            }
            else
            {
                await LoadReceptionistData();
            }
            CenterControls();
        }

        // ==========================================
        //           ЛОГІКА РЕЦЕПЦІОНІСТА
        // ==========================================
        private void InitializeReceptionistDashboard()
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

            // 1. Check-Ins
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

            // 2. Check-Outs
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

            // 3. Статус кімнат
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

            lblAvailable = CreateStatusLabel();
            lblOccupied = CreateStatusLabel();
            lblCleaning = CreateStatusLabel();
            lblRepair = CreateStatusLabel();

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
        }

        private async Task LoadReceptionistData()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            try
            {
                using (var context = new HotelDbContext())
                {
                    var checkIns = await context.Reservations
                        .Include(r => r.IdGuestNavigation)
                        .Where(r => r.BookingStatus == "підтверджено" && r.CheckInDate == today)
                        .Select(r => new {
                            Гість = r.IdGuestNavigation.GuestFirstName + " " + r.IdGuestNavigation.GuestLastName,
                            Кімната = r.IdRoom,
                            Гостей = r.NumberOfGuests
                        }).ToListAsync();

                    dgvCheckIns.DataSource = checkIns;
                    checkInBox.Text = string.Format(Strings.Dashboard_CheckIns, checkIns.Count);
                    if (dgvCheckIns.Columns.Contains("Кімната")) dgvCheckIns.Columns["Кімната"].FillWeight = 40;

                    if (dgvCheckIns.Columns.Contains("Гість")) dgvCheckIns.Columns["Гість"].HeaderText = Strings.Dashboard_Guest;
                    if (dgvCheckIns.Columns.Contains("Кімната")) dgvCheckIns.Columns["Кімната"].HeaderText = Strings.Col_RoomID;
                    if (dgvCheckIns.Columns.Contains("Гостей")) dgvCheckIns.Columns["Гостей"].HeaderText = Strings.Col_NumGuests;

                    var checkOuts = await context.Reservations
                        .Include(r => r.IdGuestNavigation)
                        .Where(r => r.BookingStatus == "Проживає" && r.CheckOutDate == today)
                        .Select(r => new {
                            Гість = r.IdGuestNavigation.GuestFirstName + " " + r.IdGuestNavigation.GuestLastName,
                            Кімната = r.IdRoom
                        }).ToListAsync();

                    dgvCheckOuts.DataSource = checkOuts;
                    checkOutBox.Text = string.Format(Strings.Dashboard_CheckOuts, checkOuts.Count);
                    if (dgvCheckOuts.Columns.Contains("Кімната")) dgvCheckOuts.Columns["Кімната"].FillWeight = 40;

                    if (dgvCheckOuts.Columns.Contains("Гість")) dgvCheckOuts.Columns["Гість"].HeaderText = Strings.Dashboard_Guest;
                    if (dgvCheckOuts.Columns.Contains("Кімната")) dgvCheckOuts.Columns["Кімната"].HeaderText = Strings.Col_RoomID;

                    var roomStats = await context.HotelRooms.AsNoTracking()
                        .GroupBy(r => r.RoomStatus)
                        .Select(g => new { Status = g.Key, Count = g.Count() })
                        .ToListAsync();

                    lblAvailable.Text = roomStats.FirstOrDefault(s => s.Status == "доступна")?.Count.ToString() ?? "0";
                    lblOccupied.Text = roomStats.FirstOrDefault(s => s.Status == "Зайнята")?.Count.ToString() ?? "0";
                    lblCleaning.Text = roomStats.FirstOrDefault(s => s.Status == "на прибиранні")?.Count.ToString() ?? "0";
                    lblRepair.Text = roomStats.FirstOrDefault(s => s.Status == "на ремонті")?.Count.ToString() ?? "0";
                }
            }
            catch (Exception ex) { ShowErrorLabel(this, ex.Message); }
        }

        // ==========================================
        //           ЛОГІКА АДМІНІСТРАТОРА
        // ==========================================
        private void InitializeAdminDashboard()
        {
            this.BackColor = ThemeManager.ContentBackground;

            dashBox = new GroupBox
            {
                Text = "Панель Адміністратора",
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
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            var kpiPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            kpiPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));

            lblRevenueToday = CreateKpiCard(Strings.Dashboard_RevenueToday, "0.00 ₴", Color.LightGreen);
            lblRevenueMonth = CreateKpiCard(Strings.Dashboard_RevenueMonth, "0.00 ₴", Color.LightSkyBlue);
            lblOccupancy = CreateKpiCard(Strings.Dashboard_Occupancy, "0%", Color.Orange);

            kpiPanel.Controls.Add(lblRevenueToday, 0, 0);
            kpiPanel.Controls.Add(lblRevenueMonth, 1, 0);
            kpiPanel.Controls.Add(lblOccupancy, 2, 0);

            mainLayout.Controls.Add(kpiPanel, 0, 0);
            mainLayout.SetColumnSpan(kpiPanel, 2);

            statsBox = new GroupBox
            {
                Text = Strings.Dashboard_PopularRooms,
                Dock = DockStyle.Fill,
                Font = boldFont,
                ForeColor = ThemeManager.TextColor
            };
            dgvPopularRooms = CreateStandardDataGridView();
            statsBox.Controls.Add(dgvPopularRooms);
            mainLayout.Controls.Add(statsBox, 0, 1);

            staffBox = new GroupBox
            {
                Text = Strings.Dashboard_ActiveStaff,
                Dock = DockStyle.Fill,
                Font = boldFont,
                ForeColor = ThemeManager.TextColor
            };
            dgvActiveStaff = CreateStandardDataGridView();
            staffBox.Controls.Add(dgvActiveStaff);
            mainLayout.Controls.Add(staffBox, 1, 1);

            dashBox.Controls.Add(mainLayout);
            this.Controls.Add(dashBox);
        }

        private async Task LoadAdminData()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var firstDayOfMonth = new DateOnly(today.Year, today.Month, 1);

            try
            {
                using (var context = new HotelDbContext())
                {
                    var todayRevenue = await context.Reservations
                        .Where(r => r.CheckInDate == today && r.BookingStatus != "скасовано")
                        .SumAsync(r => r.TotalPrice ?? 0);
                    lblRevenueToday.Text = $"{todayRevenue:N0} ₴\n{Strings.Dashboard_RevenueToday}";

                    var monthRevenue = await context.Reservations
                        .Where(r => r.CheckInDate >= firstDayOfMonth && r.BookingStatus != "скасовано")
                        .SumAsync(r => r.TotalPrice ?? 0);
                    lblRevenueMonth.Text = $"{monthRevenue:N0} ₴\n{Strings.Dashboard_RevenueMonth}";

                    int totalRooms = await context.HotelRooms.CountAsync();
                    int occupiedRooms = await context.HotelRooms.CountAsync(r => r.RoomStatus == "Зайнята");
                    double occupancy = totalRooms > 0 ? (double)occupiedRooms / totalRooms * 100 : 0;
                    lblOccupancy.Text = $"{occupancy:F1}%\n{Strings.Dashboard_Occupancy}";

                    var popularRooms = await context.Reservations
                        .Include(r => r.IdRoomNavigation)
                        .GroupBy(r => r.IdRoomNavigation.RoomType)
                        .Select(g => new { Тип = g.Key, Бронювань = g.Count() })
                        .OrderByDescending(x => x.Бронювань)
                        .Take(5)
                        .ToListAsync();
                    dgvPopularRooms.DataSource = popularRooms;

                    // (ЗМІНЕНО) Property "Ім_я" залишається для створення об'єкту
                    var activeStaff = await context.Staff
                        .Where(s => s.Status == "Працює")
                        .Select(s => new {
                            Ім_я = s.StaffFirstName + " " + s.StaffLastName,
                            Посада = s.JobTitle,
                            Телефон = s.StaffPhoneNumber
                        })
                        .ToListAsync();
                    dgvActiveStaff.DataSource = activeStaff;

                    // (НОВЕ) Ось тут ми вручну змінюємо заголовок "Ім_я" на "Ім'я" (або First Name)
                    if (dgvActiveStaff.Columns.Contains("Ім_я")) dgvActiveStaff.Columns["Ім_я"].HeaderText = Strings.Col_StaffName; // Беремо з ресурсів
                    if (dgvActiveStaff.Columns.Contains("Посада")) dgvActiveStaff.Columns["Посада"].HeaderText = Strings.Col_JobTitle;
                    if (dgvActiveStaff.Columns.Contains("Телефон")) dgvActiveStaff.Columns["Телефон"].HeaderText = Strings.Col_Phone;
                }
            }
            catch (Exception ex) { ShowErrorLabel(this, "Error loading dashboard: " + ex.Message); }
        }

        // --- ДОПОМІЖНІ МЕТОДИ ---

        private Label CreateKpiCard(string title, string value, Color bgColor)
        {
            var lbl = new Label
            {
                Text = value + "\n" + title,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = titleFont,
                BackColor = ThemeManager.InputBackground,
                ForeColor = ThemeManager.TextColor,
                Margin = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };
            return lbl;
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

        private Label CreateStatusLabel() => new Label
        {
            Text = "0",
            Dock = DockStyle.Fill,
            Font = new Font(boldFont, FontStyle.Bold),
            ForeColor = ThemeManager.TextColor,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(10, 0, 0, 0)
        };

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