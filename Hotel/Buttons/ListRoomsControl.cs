using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Hotel
{
    public class ListRoomsControl : UserControl
    {
        private DataGridView dgv;
        private GroupBox roomsBox;
        private TextBox txtSearch;
        private ComboBox cmbSort;
        private Font commonFont = new Font("Segoe UI", 10F);

        public ListRoomsControl()
        {
            roomsBox = new GroupBox
            {
                Text = "Список кімнат готелю",
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
            cmbSort = new ComboBox { Width = 220, Margin = new Padding(3), DropDownStyle = ComboBoxStyle.DropDownList, Font = commonFont }; // ЗБІЛЬШЕНО
            var btnSearch = new Button { Text = "Пошук", Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont }; // ЗБІЛЬШЕНО
            var btnReset = new Button { Text = "Скинути", Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont }; // ЗБІЛЬШЕНО

            cmbSort.Items.AddRange(new string[] {
            "За номером (зростання)",
            "За номером (спадання)",
            "За типом (А-Я)",
            "За типом (Я-А)",
            "За статусом (Доступні спочатку)",
            "За статусом (На ремонті спочатку)",
            "За статусом (На прибиранні спочатку)"
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

            roomsBox.Controls.Add(mainLayout);
            this.Controls.Add(roomsBox);

            this.Load += ListRoomsControl_Load;
            btnSearch.Click += BtnSearch_Click;
            btnReset.Click += BtnReset_Click;
            this.Resize += (sender, e) => CenterControls();
        }

        private void CenterControls()
        {
            roomsBox.Height = this.ClientSize.Height - 10;
            roomsBox.Left = (this.ClientSize.Width - roomsBox.Width) / 2;
            roomsBox.Top = 5;
        }

        private async void ListRoomsControl_Load(object? sender, EventArgs e)
        {
            await LoadRooms();
            CenterControls();
        }

        private async Task LoadRooms(string? searchTerm = null, string? sortBy = null)
        {
            try
            {
                using (var context = new HotelDbContext())
                {
                    var query = context.HotelRooms.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        query = query.Where(hr =>
                            hr.RoomType.Contains(searchTerm) ||
                            hr.RoomStatus.Contains(searchTerm)
                        );
                    }

                    switch (sortBy)
                    {
                        case "За номером (спадання)": query = query.OrderByDescending(hr => hr.IdRooms); break;
                        case "За типом (А-Я)": query = query.OrderBy(hr => hr.RoomType); break;
                        case "За типом (Я-А)": query = query.OrderByDescending(hr => hr.RoomType); break;
                        case "За статусом (Доступні спочатку)": query = query.OrderBy(hr => hr.RoomStatus != "доступна").ThenBy(hr => hr.RoomStatus); break;
                        case "За статусом (На ремонті спочатку)": query = query.OrderBy(hr => hr.RoomStatus != "на ремонті").ThenBy(hr => hr.RoomStatus); break;
                        case "За статусом (На прибиранні спочатку)": query = query.OrderBy(hr => hr.RoomStatus != "на прибиранні").ThenBy(hr => hr.RoomStatus); break;
                        case "За номером (зростання)":
                        default: query = query.OrderBy(hr => hr.IdRooms); break;
                    }

                    var rooms = await query
                        .Select(hr => new
                        {
                            hr.IdRooms,
                            hr.RoomType,
                            hr.RoomStatus
                        })
                        .ToListAsync();

                    dgv.DataSource = rooms;

                    if (dgv.Columns["IdRooms"] != null) dgv.Columns["IdRooms"].HeaderText = "Номер кімнати";
                    if (dgv.Columns["RoomType"] != null) dgv.Columns["RoomType"].HeaderText = "Тип кімнати";
                    if (dgv.Columns["RoomStatus"] != null) dgv.Columns["RoomStatus"].HeaderText = "Статус";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження списку кімнат: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            LoadRooms(txtSearch.Text, cmbSort.SelectedItem as string);
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            txtSearch.Clear();
            cmbSort.SelectedIndex = -1;
            LoadRooms();
        }
    }
}
