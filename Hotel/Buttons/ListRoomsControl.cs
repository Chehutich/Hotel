using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic; // (ДОДАНО)
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

        // (НОВЕ) Словник для сортування
        private Dictionary<string, string> roomSortOptions;

        public ListRoomsControl()
        {
            // (НОВЕ) Ініціалізація словника сортування
            roomSortOptions = new Dictionary<string, string>
            {
                { "ID_ASC", Strings.Sort_Room_ID_ASC },
                { "ID_DESC", Strings.Sort_Room_ID_DESC },
                { "Type_ASC", Strings.Sort_Room_Type_ASC },
                { "Type_DESC", Strings.Sort_Room_Type_DESC },
                { "Status_Available", Strings.Sort_Room_Status_Available },
                { "Status_Repair", Strings.Sort_Room_Status_Repair },
                { "Status_Cleaning", Strings.Sort_Room_Status_Cleaning }
            };

            roomsBox = new GroupBox
            {
                Text = Strings.ListRoomsTitle,
                Dock = DockStyle.None,
                Width = 1100,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
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

            txtSearch = new TextBox { Width = 200, Margin = new Padding(3), Font = commonFont };
            cmbSort = new ComboBox { Width = 220, Margin = new Padding(3), DropDownStyle = ComboBoxStyle.DropDownList, Font = commonFont };
            var btnSearch = new Button { Text = Strings.ButtonSearch, Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont };
            var btnReset = new Button { Text = Strings.ButtonReset, Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont };

            // (ЗМІНЕНО) Прив'язка ComboBox до словника
            cmbSort.DataSource = new BindingSource(roomSortOptions, null);
            cmbSort.DisplayMember = "Value";
            cmbSort.ValueMember = "Key";

            filterPanel.Controls.Add(new Label { Text = Strings.LabelSearch, AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Font = commonFont, Margin = new Padding(3, 0, 0, 0) });
            filterPanel.Controls.Add(txtSearch);
            filterPanel.Controls.Add(new Label { Text = Strings.LabelSort, AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(10, 0, 0, 0), Font = commonFont });
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
                Font = new Font("Segoe UI", 10F)
            };
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            dgv.RowTemplate.Height = 30;

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

                    // (ЗМІНЕНО) Switch тепер використовує КЛЮЧІ
                    // (ПРИМІТКА) Логіка сортування за статусами залишилася залежною від рядків "доступна", "на ремонті"
                    // Це тому, що ваші ключі в .resx не містять цих значень.
                    switch (sortBy)
                    {
                        case "ID_DESC": query = query.OrderByDescending(hr => hr.IdRooms); break;
                        case "Type_ASC": query = query.OrderBy(hr => hr.RoomType); break;
                        case "Type_DESC": query = query.OrderByDescending(hr => hr.RoomType); break;
                        case "Status_Available": query = query.OrderBy(hr => hr.RoomStatus != "доступна").ThenBy(hr => hr.RoomStatus); break;
                        case "Status_Repair": query = query.OrderBy(hr => hr.RoomStatus != "на ремонті").ThenBy(hr => hr.RoomStatus); break;
                        case "Status_Cleaning": query = query.OrderBy(hr => hr.RoomStatus != "на прибиранні").ThenBy(hr => hr.RoomStatus); break;
                        case "ID_ASC":
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

                    if (dgv.Columns["IdRooms"] != null) dgv.Columns["IdRooms"].HeaderText = Strings.Col_RoomID;
                    if (dgv.Columns["RoomType"] != null) dgv.Columns["RoomType"].HeaderText = Strings.Col_RoomType;
                    if (dgv.Columns["RoomStatus"] != null) dgv.Columns["RoomStatus"].HeaderText = Strings.Col_Status;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження списку кімнат: {ex.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            // (ЗМІНЕНО) Передаємо КЛЮЧ (SelectedValue)
            LoadRooms(txtSearch.Text, cmbSort.SelectedValue as string);
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            txtSearch.Clear();
            cmbSort.SelectedIndex = -1;
            LoadRooms();
        }
    }
}