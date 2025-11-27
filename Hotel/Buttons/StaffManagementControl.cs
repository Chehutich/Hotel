using Hotel.Core;
using Hotel.Forms;
using Hotel.Localization;
using Hotel.Models;
using Hotel.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hotel.Buttons
{
    public class StaffManagementControl : UserControl
    {
        private DataGridView dgv;
        private GroupBox staffBox;

        private TextBox txtSearch;
        private ComboBox cmbSort;

        private TextBox txtFirstName, txtLastName, txtPhone, txtUsername, txtPassword;
        private ComboBox cmbJobTitle;
        private ComboBox cmbStatus;
        private Button btnSave, btnClear;

        private Font commonFont = new Font("Segoe UI", 10F);
        private int? editingStaffId = null;
        private Dictionary<string, string> sortOptions;
        private Dictionary<string, string> statusOptions;

        public StaffManagementControl()
        {
            InitializeDictionaries();
            InitializeUI();
            this.Load += async (s, e) => {
                await LoadStaff();
                CenterControls();
            };
            this.Resize += (s, e) => CenterControls();
        }

        private void InitializeDictionaries()
        {
            sortOptions = new Dictionary<string, string>
            {
                { "Name_ASC", Strings.Sort_Staff_Name_ASC },
                { "Name_DESC", Strings.Sort_Staff_Name_DESC },
                { "Status", Strings.Sort_Staff_Status }
            };

            statusOptions = new Dictionary<string, string>
            {
                { "Працює", Strings.Status_Working },
                { "У відпустці", Strings.Status_Vacation },
                { "На лікарняному", Strings.Status_Sick },
                { "Звільнено", Strings.Status_Fired }
            };
        }

        private void InitializeUI()
        {
            staffBox = new GroupBox
            {
                Text = Strings.Admin_Staff,
                Dock = DockStyle.None,
                Width = 1250,
                Height = 720,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Padding = new Padding(15),
                ForeColor = ThemeManager.TextColor
            };

            var mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));

            // --- ЛІВА ЧАСТИНА ---
            var leftPanel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
            leftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var filterPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, AutoSize = true, Padding = new Padding(0, 0, 0, 10) };

            txtSearch = new TextBox { Width = 200, Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
            txtSearch.TextChanged += (s, e) => LoadStaff(txtSearch.Text, cmbSort.SelectedValue as string);

            cmbSort = new ComboBox { Width = 180, Margin = new Padding(3), DropDownStyle = ComboBoxStyle.DropDownList, Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
            cmbSort.DataSource = new BindingSource(sortOptions, null);
            cmbSort.DisplayMember = "Value"; cmbSort.ValueMember = "Key";
            cmbSort.SelectedIndexChanged += (s, e) => LoadStaff(txtSearch.Text, cmbSort.SelectedValue as string);

            filterPanel.Controls.Add(new Label { Text = Strings.LabelSearch, AutoSize = true, Anchor = AnchorStyles.Left, Font = commonFont, ForeColor = ThemeManager.TextColor });
            filterPanel.Controls.Add(txtSearch);
            filterPanel.Controls.Add(new Label { Text = Strings.LabelSort, AutoSize = true, Anchor = AnchorStyles.Left, Font = commonFont, ForeColor = ThemeManager.TextColor });
            filterPanel.Controls.Add(cmbSort);

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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoGenerateColumns = false
            };
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.GridHeaderBackground;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ThemeManager.GridHeaderForeColor;
            dgv.DefaultCellStyle.BackColor = ThemeManager.InputBackground;
            dgv.DefaultCellStyle.ForeColor = ThemeManager.InputForeColor;
            dgv.EnableHeadersVisualStyles = false;
            dgv.SelectionChanged += Dgv_SelectionChanged;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdStaff", DataPropertyName = "IdStaff", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "StaffFirstName", DataPropertyName = "StaffFirstName", HeaderText = Strings.Col_StaffName });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "StaffLastName", DataPropertyName = "StaffLastName", HeaderText = Strings.Col_StaffSurname });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "JobTitle", DataPropertyName = "JobTitle", HeaderText = Strings.Col_JobTitle });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "StaffPhoneNumber", DataPropertyName = "StaffPhoneNumber", HeaderText = Strings.Col_Phone });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Username", DataPropertyName = "Username", HeaderText = Strings.LabelUsername });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", DataPropertyName = "Status", HeaderText = Strings.Label_Status });

            leftPanel.Controls.Add(filterPanel, 0, 0);
            leftPanel.Controls.Add(dgv, 0, 1);

            // --- ПРАВА ЧАСТИНА ---

            // (ЗМІНЕНО) Додано великий відступ зліва (40px), щоб посунути форму до центру
            var formPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(40, 20, 10, 10) // (ОСЬ ТУТ ЗМІНА)
            };

            txtFirstName = CreateInput();
            txtLastName = CreateInput();
            txtPhone = CreateInput();
            txtUsername = CreateInput();
            txtPassword = CreateInput();
            txtPassword.UseSystemPasswordChar = true;

            cmbJobTitle = new ComboBox { Width = 250, Margin = new Padding(0, 5, 0, 10), Font = commonFont, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
            cmbJobTitle.Items.AddRange(new string[] { "Рецепціоніст", "Адміністратор", "Покоївка", "Ремонтник" });

            cmbStatus = new ComboBox { Width = 250, Margin = new Padding(0, 5, 0, 10), Font = commonFont, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
            cmbStatus.DataSource = new BindingSource(statusOptions, null);
            cmbStatus.DisplayMember = "Value"; cmbStatus.ValueMember = "Key";

            btnSave = new Button { Text = Strings.ButtonSave, Width = 120, Height = 40, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor, Font = commonFont };
            btnSave.Click += BtnSave_Click;

            btnClear = new Button { Text = Strings.ButtonClear, Width = 120, Height = 40, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor, Font = commonFont };
            btnClear.Click += (s, e) => ClearForm();

            formPanel.Controls.Add(CreateLabel(Strings.LabelFirstName));
            formPanel.Controls.Add(txtFirstName);
            formPanel.Controls.Add(CreateLabel(Strings.LabelLastName));
            formPanel.Controls.Add(txtLastName);
            formPanel.Controls.Add(CreateLabel(Strings.LabelPhoneNumber));
            formPanel.Controls.Add(txtPhone);

            formPanel.Controls.Add(CreateLabel(Strings.LabelJobTitle));
            formPanel.Controls.Add(cmbJobTitle);

            formPanel.Controls.Add(CreateLabel(Strings.Label_Status));
            formPanel.Controls.Add(cmbStatus);

            formPanel.Controls.Add(CreateLabel(Strings.LabelUsername));
            formPanel.Controls.Add(txtUsername);
            formPanel.Controls.Add(CreateLabel(Strings.LabelPassword));
            formPanel.Controls.Add(txtPassword);
            formPanel.Controls.Add(new Label { Text = Strings.MsgPasswordHint, Font = new Font(commonFont.FontFamily, 8), ForeColor = Color.Gray, AutoSize = true });

            var btnPanel = new FlowLayoutPanel { AutoSize = true };
            btnPanel.Controls.Add(btnSave);
            btnPanel.Controls.Add(btnClear);
            formPanel.Controls.Add(btnPanel);

            mainLayout.Controls.Add(leftPanel, 0, 0);
            mainLayout.Controls.Add(formPanel, 1, 0);
            staffBox.Controls.Add(mainLayout);
            this.Controls.Add(staffBox);
        }

        private void CenterControls()
        {
            staffBox.Left = (this.ClientSize.Width - staffBox.Width) / 2;
            staffBox.Top = (this.ClientSize.Height - staffBox.Height) / 2;
        }

        private TextBox CreateInput() => new TextBox { Width = 250, Margin = new Padding(0, 5, 0, 10), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
        private Label CreateLabel(string text) => new Label { Text = text, AutoSize = true, Font = commonFont, ForeColor = ThemeManager.TextColor };

        private async Task LoadStaff(string? searchTerm = null, string? sortBy = null)
        {
            try
            {
                using (var context = new HotelDbContext())
                {
                    var query = context.Staff.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        query = query.Where(s =>
                            s.StaffFirstName.Contains(searchTerm) ||
                            s.StaffLastName.Contains(searchTerm) ||
                            s.JobTitle.Contains(searchTerm));
                    }

                    switch (sortBy)
                    {
                        case "Name_DESC": query = query.OrderByDescending(s => s.StaffLastName); break;
                        case "Status": query = query.OrderBy(s => s.Status); break;
                        default: query = query.OrderBy(s => s.StaffLastName); break;
                    }

                    var staffList = await query.ToListAsync();
                    dgv.DataSource = staffList;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                var row = dgv.SelectedRows[0];
                editingStaffId = (int)row.Cells["IdStaff"].Value;
                txtFirstName.Text = row.Cells["StaffFirstName"].Value?.ToString();
                txtLastName.Text = row.Cells["StaffLastName"].Value?.ToString();
                txtPhone.Text = row.Cells["StaffPhoneNumber"].Value?.ToString();
                txtUsername.Text = row.Cells["Username"].Value?.ToString();

                string job = row.Cells["JobTitle"].Value?.ToString();
                if (cmbJobTitle.Items.Contains(job)) cmbJobTitle.SelectedItem = job;

                string status = row.Cells["Status"].Value?.ToString();
                if (statusOptions.ContainsKey(status ?? "")) cmbStatus.SelectedValue = status;
                else cmbStatus.SelectedIndex = 0;

                txtPassword.Clear();
                btnSave.Text = Strings.ButtonSave == "Save" ? "Update" : "Оновити";
            }
        }

        private void ClearForm()
        {
            editingStaffId = null;
            txtFirstName.Clear(); txtLastName.Clear(); txtPhone.Clear(); txtUsername.Clear(); txtPassword.Clear();
            cmbJobTitle.SelectedIndex = -1;
            cmbStatus.SelectedIndex = 0;
            btnSave.Text = Strings.ButtonSave;
            dgv.ClearSelection();
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text) || cmbJobTitle.SelectedItem == null)
            {
                MessageBox.Show("Заповніть обов'язкові поля!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var context = new HotelDbContext())
                {
                    Staff staff;
                    if (editingStaffId == null)
                    {
                        staff = new Staff();
                        context.Staff.Add(staff);
                    }
                    else
                    {
                        staff = await context.Staff.FindAsync(editingStaffId);
                    }

                    staff.StaffFirstName = txtFirstName.Text;
                    staff.StaffLastName = txtLastName.Text;
                    staff.StaffPhoneNumber = txtPhone.Text;
                    staff.JobTitle = cmbJobTitle.SelectedItem.ToString();
                    staff.Username = txtUsername.Text;
                    staff.Status = cmbStatus.SelectedValue?.ToString() ?? "Працює";

                    if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        staff.PasswordHash = SecurityHelper.ComputeSha256Hash(txtPassword.Text);
                    }

                    await context.SaveChangesAsync();
                    MessageBox.Show("Дані збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    await LoadStaff(txtSearch.Text, cmbSort.SelectedValue as string);
                }
            }
            catch (Exception ex) { MessageBox.Show($"Помилка: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}