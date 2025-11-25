using Hotel.Forms;
using Hotel.Localization;
using Hotel.Models;
using Hotel.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Hotel.Buttons
{
    public class ListGuestsControl : UserControl
    {
        private DataGridView dgv;
        private TextBox txtSearch;
        private ComboBox cmbSort;
        private GroupBox guestBox;
        private Font commonFont = new Font("Segoe UI", 10F);
        private Dictionary<string, string> guestSortOptions;
        private Button btnEdit;

        public ListGuestsControl()
        {
            guestSortOptions = new Dictionary<string, string>
            {
                { "LastName_ASC", Strings.Sort_Guest_LastName_ASC },
                { "LastName_DESC", Strings.Sort_Guest_LastName_DESC },
                { "ID_ASC", Strings.Sort_Guest_ID_ASC },
                { "ID_DESC", Strings.Sort_Guest_ID_DESC }
            };

            guestBox = new GroupBox
            {
                Text = Strings.ListGuestsTitle,
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
                RowCount = 3,
                ColumnCount = 1
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
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

            // (НОВЕ) Живий пошук: при зміні тексту одразу оновлюємо список
            txtSearch.TextChanged += (s, e) => LoadGuests(txtSearch.Text, cmbSort.SelectedValue as string);

            cmbSort = new ComboBox { Width = 180, Margin = new Padding(3), DropDownStyle = ComboBoxStyle.DropDownList, Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
            var btnSearch = new Button { Text = Strings.ButtonSearch, Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };
            var btnReset = new Button { Text = Strings.ButtonReset, Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };
            var btnReport = new Button { Text = Strings.ButtonReport, Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };

            cmbSort.DataSource = new BindingSource(guestSortOptions, null);
            cmbSort.DisplayMember = "Value";
            cmbSort.ValueMember = "Key";

            filterPanel.Controls.Add(new Label { Text = Strings.LabelSearch, AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Font = commonFont, Margin = new Padding(3, 0, 0, 0), ForeColor = ThemeManager.TextColor });
            filterPanel.Controls.Add(txtSearch);
            filterPanel.Controls.Add(new Label { Text = Strings.LabelSort, AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(10, 0, 0, 0), Font = commonFont, ForeColor = ThemeManager.TextColor });
            filterPanel.Controls.Add(cmbSort);
            filterPanel.Controls.Add(btnSearch);
            filterPanel.Controls.Add(btnReset);
            filterPanel.Controls.Add(btnReport);

            var actionsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 10)
            };

            btnEdit = new Button { Text = Strings.Button_Edit, Size = new Size(120, 40), Margin = new Padding(3), Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor, Enabled = false };
            actionsPanel.Controls.Add(btnEdit);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = ThemeManager.GridBackground,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.GridHeaderBackground;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ThemeManager.GridHeaderForeColor;
            dgv.DefaultCellStyle.BackColor = ThemeManager.InputBackground;
            dgv.DefaultCellStyle.ForeColor = ThemeManager.InputForeColor;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersDefaultCellStyle.BackColor = ThemeManager.GridHeaderBackground;
            dgv.RowTemplate.Height = 30;

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "IdGuest",
                DataPropertyName = "IdGuest",
                HeaderText = Strings.Col_ID,
                FillWeight = 50
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "GuestFirstName",
                HeaderText = Strings.Col_FirstName,
                FillWeight = 100
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "GuestLastName",
                HeaderText = Strings.Col_LastName,
                FillWeight = 100
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PhoneNumber",
                HeaderText = Strings.Col_Phone,
                FillWeight = 100
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DateOfBirth",
                HeaderText = Strings.Col_BirthDate,
                FillWeight = 80
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PassportSeries",
                HeaderText = Strings.Col_Passport,
                FillWeight = 80
            });
            dgv.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "IsRegularGuest",
                HeaderText = Strings.Col_IsRegular,
                FillWeight = 60
            });

            mainLayout.Controls.Add(filterPanel, 0, 0);
            mainLayout.Controls.Add(actionsPanel, 0, 1);
            mainLayout.Controls.Add(dgv, 0, 2);

            guestBox.Controls.Add(mainLayout);
            this.Controls.Add(guestBox);

            this.Load += ListGuestsControl_Load;
            btnSearch.Click += BtnSearch_Click;
            btnReset.Click += BtnReset_Click;
            btnReport.Click += BtnReport_Click;
            btnEdit.Click += BtnEdit_Click;
            dgv.SelectionChanged += Dgv_SelectionChanged;
            this.Resize += (sender, e) => CenterControls();
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            btnEdit.Enabled = (dgv.SelectedRows.Count > 0);
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            int selectedGuestId = (int)dgv.SelectedRows[0].Cells["IdGuest"].Value;

            var form1 = this.FindForm() as Form1;
            if (form1 != null)
            {
                var editGuestControl = new AddGuestControl(selectedGuestId);
                form1.ShowControl(editGuestControl);
            }
        }


        private void CenterControls()
        {
            guestBox.Height = this.ClientSize.Height - 10;
            guestBox.Left = (this.ClientSize.Width - guestBox.Width) / 2;
            guestBox.Top = 5;
        }

        private void ListGuestsControl_Load(object? sender, EventArgs e)
        {
            LoadGuests();
            CenterControls();
        }

        private async void LoadGuests(string? searchTerm = null, string? sortBy = null)
        {
            try
            {
                using (var context = new HotelDbContext())
                {
                    var query = context.Guests.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        query = query.Where(g =>
                            g.GuestFirstName.Contains(searchTerm) ||
                            g.GuestLastName.Contains(searchTerm) ||
                            g.PhoneNumber.Contains(searchTerm)
                        );
                    }

                    switch (sortBy)
                    {
                        case "LastName_ASC": query = query.OrderBy(g => g.GuestLastName); break;
                        case "LastName_DESC": query = query.OrderByDescending(g => g.GuestLastName); break;
                        case "ID_ASC": query = query.OrderBy(g => g.IdGuest); break;
                        case "ID_DESC": query = query.OrderByDescending(g => g.IdGuest); break;
                        default: query = query.OrderBy(g => g.IdGuest); break;
                    }

                    var guestsList = await query.Select(g => new
                    {
                        g.IdGuest,
                        g.GuestFirstName,
                        g.GuestLastName,
                        g.PhoneNumber,
                        g.DateOfBirth,
                        g.PassportSeries,
                        g.IsRegularGuest
                    }).ToListAsync();

                    dgv.DataSource = guestsList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних: {ex.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            LoadGuests(txtSearch.Text, cmbSort.SelectedValue as string);
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            txtSearch.Clear();
            cmbSort.SelectedIndex = -1;
            LoadGuests();
        }

        private void BtnReport_Click(object? sender, EventArgs e)
        {
            try
            {
                using (var context = new HotelDbContext())
                {
                    var query = context.Guests.AsQueryable();
                    if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                    {
                        query = query.Where(g => g.GuestFirstName.Contains(txtSearch.Text) || g.GuestLastName.Contains(txtSearch.Text) || g.PhoneNumber.Contains(txtSearch.Text));
                    }
                    switch (cmbSort.SelectedValue as string)
                    {
                        case "LastName_ASC": query = query.OrderBy(g => g.GuestLastName); break;
                        case "LastName_DESC": query = query.OrderByDescending(g => g.GuestLastName); break;
                        case "ID_ASC": query = query.OrderBy(g => g.IdGuest); break;
                        case "ID_DESC": query = query.OrderByDescending(g => g.IdGuest); break;
                        default: query = query.OrderBy(g => g.IdGuest); break;
                    }

                    var guestsToReport = query.ToList();

                    if (!guestsToReport.Any())
                    {
                        MessageBox.Show(Strings.MsgNoDataForReport, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    List<GuestReportDto> guestsToSerialize = guestsToReport.Select(guest => new GuestReportDto
                    {
                        IdGuest = guest.IdGuest,
                        GuestFirstName = guest.GuestFirstName,
                        GuestLastName = guest.GuestLastName,
                        PhoneNumber = guest.PhoneNumber,
                        DateOfBirth = guest.DateOfBirth?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        IsRegularGuest = guest.IsRegularGuest,
                        PassportSeries = guest.PassportSeries
                    }).ToList();

                    string fileName = "guests_report.xml";

                    Hotel.Utils.ClassSerializare.SerializeToXml<List<GuestReportDto>>(ref guestsToSerialize, fileName);

                    FormReport frmReport = new FormReport();
                    frmReport.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка підготовки звіту: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}