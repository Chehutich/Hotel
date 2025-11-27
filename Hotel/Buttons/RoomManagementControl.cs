using Hotel.Core;
using Hotel.Forms;
using Hotel.Localization;
using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hotel.Buttons
{
    public class RoomManagementControl : UserControl
    {
        private DataGridView dgv;
        private GroupBox box;
        private NumericUpDown numPrice, numCapacity;
        private TextBox txtTypeName;
        private Button btnSave;
        private Font commonFont = new Font("Segoe UI", 10F);
        private int? selectedTypeId = null;

        public RoomManagementControl()
        {
            InitializeUI();
            this.Load += async (s, e) => {
                await LoadTypes();
                CenterControls();
            };
            this.Resize += (s, e) => CenterControls();
        }

        private void InitializeUI()
        {
            box = new GroupBox
            {
                Text = Strings.Admin_Prices,
                Dock = DockStyle.None,
                Width = 1100,
                Height = 650,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Padding = new Padding(15),
                ForeColor = ThemeManager.TextColor
            };

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = ThemeManager.GridBackground,
                BorderStyle = BorderStyle.None,
                Font = commonFont,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.GridHeaderBackground;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ThemeManager.GridHeaderForeColor;
            dgv.DefaultCellStyle.BackColor = ThemeManager.InputBackground;
            dgv.DefaultCellStyle.ForeColor = ThemeManager.InputForeColor;
            dgv.EnableHeadersVisualStyles = false;
            dgv.SelectionChanged += Dgv_SelectionChanged;

            // (ЗМІНЕНО) Додано відступ зліва (40px)
            var formPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(40, 20, 10, 10) // (ОСЬ ТУТ ЗМІНА)
            };

            txtTypeName = new TextBox { Width = 200, ReadOnly = true, Margin = new Padding(0, 5, 0, 10), Font = commonFont, BackColor = Color.LightGray };
            numPrice = new NumericUpDown { Width = 200, Maximum = 100000, DecimalPlaces = 2, Margin = new Padding(0, 5, 0, 10), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
            numCapacity = new NumericUpDown { Width = 200, Maximum = 20, Margin = new Padding(0, 5, 0, 10), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };

            btnSave = new Button { Text = Strings.ButtonSave, Width = 120, Height = 40, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor, Font = commonFont, Enabled = false };
            btnSave.Click += BtnSave_Click;

            formPanel.Controls.Add(new Label { Text = Strings.Col_RoomType + ":", AutoSize = true, Font = commonFont, ForeColor = ThemeManager.TextColor });
            formPanel.Controls.Add(txtTypeName);
            formPanel.Controls.Add(new Label { Text = Strings.Col_Price + " (грн):", AutoSize = true, Font = commonFont, ForeColor = ThemeManager.TextColor });
            formPanel.Controls.Add(numPrice);
            formPanel.Controls.Add(new Label { Text = Strings.Col_Capacity + ":", AutoSize = true, Font = commonFont, ForeColor = ThemeManager.TextColor });
            formPanel.Controls.Add(numCapacity);
            formPanel.Controls.Add(btnSave);

            layout.Controls.Add(dgv, 0, 0);
            layout.Controls.Add(formPanel, 1, 0);
            box.Controls.Add(layout);
            this.Controls.Add(box);
        }

        private void CenterControls()
        {
            box.Left = (this.ClientSize.Width - box.Width) / 2;
            box.Top = (this.ClientSize.Height - box.Height) / 2;
        }

        private async Task LoadTypes()
        {
            try
            {
                using (var context = new HotelDbContext())
                {
                    var types = await context.HotelTypes.ToListAsync();
                    dgv.DataSource = types;
                    if (dgv.Columns["RoomType"] != null) dgv.Columns["RoomType"].Visible = false;
                    if (dgv.Columns["TypeName"] != null) dgv.Columns["TypeName"].HeaderText = Strings.Col_RoomType;
                    if (dgv.Columns["PricePerNight"] != null) dgv.Columns["PricePerNight"].HeaderText = Strings.Col_Price;
                    if (dgv.Columns["MaxCapacity"] != null) dgv.Columns["MaxCapacity"].HeaderText = Strings.Col_Capacity;
                }
            }
            catch { }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                var row = dgv.SelectedRows[0];
                selectedTypeId = (int)row.Cells["RoomType"].Value;
                txtTypeName.Text = row.Cells["TypeName"].Value.ToString();
                numPrice.Value = Convert.ToDecimal(row.Cells["PricePerNight"].Value);

                var cap = row.Cells["MaxCapacity"].Value;
                numCapacity.Value = cap != null ? Convert.ToInt32(cap) : 1;

                btnSave.Enabled = true;
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (selectedTypeId == null) return;
            try
            {
                using (var context = new HotelDbContext())
                {
                    var type = await context.HotelTypes.FindAsync(selectedTypeId);
                    if (type != null)
                    {
                        type.PricePerNight = numPrice.Value;
                        type.MaxCapacity = (int)numCapacity.Value;
                        await context.SaveChangesAsync();
                        MessageBox.Show("Оновлено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadTypes();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}