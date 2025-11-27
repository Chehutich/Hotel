using Hotel.Forms;
using Hotel.Localization;
using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks; // (ДОДАНО)

namespace Hotel.Buttons
{
    public class ListRoomsControl : UserControl
    {
        private DataGridView dgv;
        private GroupBox roomsBox;
        private TextBox txtSearch;
        private ComboBox cmbSort;
        private Font commonFont = new Font("Segoe UI", 10F);
        private Dictionary<string, string> roomSortOptions;

        public ListRoomsControl()
        {
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
                Padding = new Padding(15),
                ForeColor = ThemeManager.TextColor
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

            txtSearch = new TextBox { Width = 200, Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };

            // Живий пошук
            txtSearch.TextChanged += (s, e) => LoadRooms(txtSearch.Text, cmbSort.SelectedValue as string);

            cmbSort = new ComboBox { Width = 220, Margin = new Padding(3), DropDownStyle = ComboBoxStyle.DropDownList, Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
            var btnSearch = new Button { Text = Strings.ButtonSearch, Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };
            var btnReset = new Button { Text = Strings.ButtonReset, Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };

            cmbSort.DataSource = new BindingSource(roomSortOptions, null);
            cmbSort.DisplayMember = "Value";
            cmbSort.ValueMember = "Key";

            filterPanel.Controls.Add(new Label { Text = Strings.LabelSearch, AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Font = commonFont, Margin = new Padding(3, 0, 0, 0), ForeColor = ThemeManager.TextColor });
            filterPanel.Controls.Add(txtSearch);
            filterPanel.Controls.Add(new Label { Text = Strings.LabelSort, AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(10, 0, 0, 0), Font = commonFont, ForeColor = ThemeManager.TextColor });
            filterPanel.Controls.Add(cmbSort);
            filterPanel.Controls.Add(btnSearch);
            filterPanel.Controls.Add(btnReset);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = ThemeManager.GridBackground,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F)
            };

            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.GridHeaderBackground;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ThemeManager.GridHeaderForeColor;
            dgv.DefaultCellStyle.BackColor = ThemeManager.InputBackground;
            dgv.DefaultCellStyle.ForeColor = ThemeManager.InputForeColor;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersDefaultCellStyle.BackColor = ThemeManager.GridHeaderBackground;

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
                    // (ЗМІНЕНО) Додаємо MaxCapacity до вибірки
                    var query = from hr in context.HotelRooms
                                join ht in context.HotelTypes on hr.RoomType equals ht.TypeName
                                select new
                                {
                                    hr.IdRooms,
                                    hr.RoomType,
                                    ht.MaxCapacity,   // (НОВЕ)
                                    ht.PricePerNight,
                                    hr.RoomStatus
                                };

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        query = query.Where(r =>
                            r.RoomType.Contains(searchTerm) ||
                            r.RoomStatus.Contains(searchTerm)
                        );
                    }

                    switch (sortBy)
                    {
                        case "ID_DESC": query = query.OrderByDescending(r => r.IdRooms); break;
                        case "Type_ASC": query = query.OrderBy(r => r.RoomType); break;
                        case "Type_DESC": query = query.OrderByDescending(r => r.RoomType); break;
                        case "Status_Available": query = query.OrderBy(r => r.RoomStatus != "доступна").ThenBy(r => r.RoomStatus); break;
                        case "Status_Repair": query = query.OrderBy(r => r.RoomStatus != "на ремонті").ThenBy(r => r.RoomStatus); break;
                        case "Status_Cleaning": query = query.OrderBy(r => r.RoomStatus != "на прибиранні").ThenBy(r => r.RoomStatus); break;
                        case "ID_ASC":
                        default: query = query.OrderBy(r => r.IdRooms); break;
                    }

                    var rooms = await query.ToListAsync();
                    dgv.DataSource = rooms;

                    // (ЗМІНЕНО) Оновлюємо заголовки
                    if (dgv.Columns["IdRooms"] != null) dgv.Columns["IdRooms"].HeaderText = Strings.Col_RoomID;
                    if (dgv.Columns["RoomType"] != null) dgv.Columns["RoomType"].HeaderText = Strings.Col_RoomType;

                    // (НОВЕ) Колонка Місткість
                    if (dgv.Columns["MaxCapacity"] != null)
                    {
                        dgv.Columns["MaxCapacity"].HeaderText = Strings.Col_MaxCapacity;
                        dgv.Columns["MaxCapacity"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }

                    if (dgv.Columns["PricePerNight"] != null)
                    {
                        dgv.Columns["PricePerNight"].HeaderText = Strings.Col_PricePerNight;
                        dgv.Columns["PricePerNight"].DefaultCellStyle.Format = "F2";
                    }

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