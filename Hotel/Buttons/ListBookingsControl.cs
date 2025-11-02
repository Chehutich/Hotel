using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Hotel
{
    public class ListBookingsControl : UserControl
    {
        private DataGridView dgv;
        private TextBox txtSearch;
        private ComboBox cmbSort;
        private GroupBox bookingBox;
        private Font commonFont = new Font("Segoe UI", 10F);

        public ListBookingsControl()
        {
            bookingBox = new GroupBox
            {
                Text = "Список бронювань",
                Dock = DockStyle.None,
                Width = 1100, // ЗБІЛЬШЕНО
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold), // ЗБІЛЬШЕНО
                Padding = new Padding(15)
            };

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 10)
            };

            txtSearch = new TextBox { Width = 200, Margin = new Padding(3), Font = commonFont }; // ЗБІЛЬШЕНО
            cmbSort = new ComboBox { Width = 200, Margin = new Padding(3), DropDownStyle = ComboBoxStyle.DropDownList, Font = commonFont }; // ЗБІЛЬШЕНО
            var btnSearch = new Button { Text = "Пошук", Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont }; // ЗБІЛЬШЕНО
            var btnReset = new Button { Text = "Скинути", Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont }; // ЗБІЛЬШЕНО

            cmbSort.Items.AddRange(new string[] {
            "За датою заїзду (новіші)",
            "За датою заїзду (старіші)",
            "За ID бронювання"
            });

            filterPanel.Controls.Add(new Label { Text = "Пошук:", AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Font = commonFont, Margin = new Padding(3, 0, 0, 0) });
            filterPanel.Controls.Add(txtSearch);
            filterPanel.Controls.Add(new Label { Text = "Сортувати:", AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(10, 0, 0, 0), Font = commonFont });
            filterPanel.Controls.Add(cmbSort);
            filterPanel.Controls.Add(btnSearch);
            filterPanel.Controls.Add(btnReset);

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

            mainLayout.Controls.Add(filterPanel, 0, 0);
            mainLayout.Controls.Add(dgv, 0, 1);

            bookingBox.Controls.Add(mainLayout);
            this.Controls.Add(bookingBox);

            this.Load += ListBookingsControl_Load;
            btnSearch.Click += BtnSearch_Click;
            btnReset.Click += BtnReset_Click;
            this.Resize += (sender, e) => CenterControls();
        }

        private void CenterControls()
        {
            bookingBox.Height = this.ClientSize.Height - 10;
            bookingBox.Left = (this.ClientSize.Width - bookingBox.Width) / 2;
            bookingBox.Top = 5;
        }

        private void ListBookingsControl_Load(object? sender, EventArgs e)
        {
            LoadBookings();
            CenterControls();
        }

        private async void LoadBookings(string? searchTerm = null, string? sortBy = null)
        {
            try
            {
                using (var context = new HotelDbContext())
                {
                    var query = context.Reservations
                                       .Include(r => r.IdGuestNavigation)
                                       .AsQueryable();

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        query = query.Where(r =>
                            r.IdGuestNavigation.GuestLastName.Contains(searchTerm) ||
                            r.IdRoom.ToString().Contains(searchTerm) ||
                            r.BookingStatus.Contains(searchTerm)
                        );
                    }

                    switch (sortBy)
                    {
                        case "За датою заїзду (новіші)": query = query.OrderByDescending(r => r.CheckInDate); break;
                        case "За датою заїзду (старіші)": query = query.OrderBy(r => r.CheckInDate); break;
                        case "За ID бронювання": query = query.OrderBy(r => r.IdBooking); break;
                        default: query = query.OrderByDescending(r => r.CheckInDate); break;
                    }

                    var bookings = await query
                        .Select(r => new
                        {
                            BookingId = r.IdBooking,
                            GuestName = r.IdGuestNavigation.GuestFirstName + " " + r.IdGuestNavigation.GuestLastName,
                            RoomId = r.IdRoom,
                            CheckIn = r.CheckInDate,
                            CheckOut = r.CheckOutDate,
                            Status = r.BookingStatus
                        })
                        .ToListAsync();

                    dgv.DataSource = bookings;

                    dgv.Columns["BookingId"].HeaderText = "ID Бронювання";
                    dgv.Columns["GuestName"].HeaderText = "Ім'я гостя";
                    dgv.Columns["RoomId"].HeaderText = "Номер кімнати";
                    dgv.Columns["CheckIn"].HeaderText = "Дата заїзду";
                    dgv.Columns["CheckOut"].HeaderText = "Дата виїзду";
                    dgv.Columns["Status"].HeaderText = "Статус";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження бронювань: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            LoadBookings(txtSearch.Text, cmbSort.SelectedItem as string);
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            txtSearch.Clear();
            cmbSort.SelectedIndex = -1;
            LoadBookings();
        }
    }
}
