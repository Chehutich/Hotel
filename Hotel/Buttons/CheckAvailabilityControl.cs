using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Hotel
{
    public class CheckAvailabilityControl : UserControl
    {
        private DataGridView dgv;
        private DateTimePicker dtpCheckIn;
        private DateTimePicker dtpCheckOut;
        private GroupBox availabilityBox;
        private Font commonFont = new Font("Segoe UI", 10F);

        public CheckAvailabilityControl()
        {
            availabilityBox = new GroupBox
            {
                Text = "Перевірка доступних номерів",
                Dock = DockStyle.None,
                Width = 1100, // ЗБІЛЬШЕНО
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold), // ЗБІЛЬШЕНО
                Padding = new Padding(15)
            };

            var mainLayoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 10)
            };

            dtpCheckIn = new DateTimePicker { Width = 150, Font = commonFont, Margin = new Padding(3) }; // ЗБІЛЬШЕНО
            dtpCheckOut = new DateTimePicker { Width = 150, Font = commonFont, Margin = new Padding(3) }; // ЗБІЛЬШЕНО
            var btnSearch = new Button { Text = "Пошук", Size = new Size(100, 35), Font = commonFont, Margin = new Padding(10, 0, 0, 0) }; // ЗБІЛЬШЕНО

            filterPanel.Controls.Add(new Label { Text = "З:", AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(5, 8, 5, 0), Font = commonFont });
            filterPanel.Controls.Add(dtpCheckIn);
            filterPanel.Controls.Add(new Label { Text = "По:", AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(10, 8, 5, 0), Font = commonFont });
            filterPanel.Controls.Add(dtpCheckOut);
            filterPanel.Controls.Add(btnSearch);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F) // ЗБІЛЬШЕНО
            };
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold); // ЗБІЛЬШЕНО
            dgv.RowTemplate.Height = 30; // ЗБІЛЬШЕНО

            mainLayoutPanel.Controls.Add(filterPanel, 0, 0);
            mainLayoutPanel.Controls.Add(dgv, 0, 1);
            availabilityBox.Controls.Add(mainLayoutPanel);
            this.Controls.Add(availabilityBox);

            this.Load += CheckAvailabilityControl_Load;
            btnSearch.Click += BtnSearch_Click;
            this.Resize += (sender, e) => CenterControls();
        }

        private void CenterControls()
        {
            availabilityBox.Height = this.ClientSize.Height - 10;
            availabilityBox.Left = (this.ClientSize.Width - availabilityBox.Width) / 2;
            availabilityBox.Top = 5;
        }

        private async void CheckAvailabilityControl_Load(object? sender, EventArgs e)
        {
            await LoadAvailableRooms();
            CenterControls();
        }

        private async void BtnSearch_Click(object? sender, EventArgs e)
        {
            if (dtpCheckOut.Value <= dtpCheckIn.Value)
            {
                MessageBox.Show("Дата виїзду повинна бути пізніше дати заїзду.", "Помилка дати", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            await LoadAvailableRooms(DateOnly.FromDateTime(dtpCheckIn.Value), DateOnly.FromDateTime(dtpCheckOut.Value));
        }

        private async Task LoadAvailableRooms(DateOnly? checkIn = null, DateOnly? checkOut = null)
        {
            try
            {
                using (var context = new HotelDbContext())
                {
                    var availableRoomsQuery = context.HotelRooms.Where(hr => hr.RoomStatus == "доступна");

                    if (checkIn.HasValue && checkOut.HasValue)
                    {
                        var bookedRoomIds = await context.Reservations
                            .Where(r => r.BookingStatus == "підтверджено" &&
                                        checkIn.Value < r.CheckOutDate &&
                                        checkOut.Value > r.CheckInDate)
                            .Select(r => r.IdRoom)
                            .Distinct()
                            .ToListAsync();

                        availableRoomsQuery = availableRoomsQuery.Where(hr => !bookedRoomIds.Contains(hr.IdRooms));
                    }

                    var roomsToShow = await availableRoomsQuery
                        .Select(hr => new {
                            RoomId = hr.IdRooms,
                            RoomType = hr.RoomType,
                            Status = hr.RoomStatus
                        })
                        .OrderBy(r => r.RoomId)
                        .ToListAsync();

                    dgv.DataSource = roomsToShow;

                    dgv.Columns["RoomId"].HeaderText = "Номер кімнати";
                    dgv.Columns["RoomType"].HeaderText = "Тип кімнати";
                    dgv.Columns["Status"].HeaderText = "Статус";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження кімнат: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
