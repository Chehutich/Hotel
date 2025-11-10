using Hotel;
using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic; // (ДОДАНО)
using System.IO;
using System.Globalization;

namespace Hotel
{
    public class ListGuestsControl : UserControl
    {
        private DataGridView dgv;
        private TextBox txtSearch;
        private ComboBox cmbSort;
        private GroupBox guestBox;
        private Font commonFont = new Font("Segoe UI", 10F);

        // (НОВЕ) Словник для сортування: Ключ (для коду), Значення (для користувача)
        private Dictionary<string, string> guestSortOptions;

        public ListGuestsControl()
        {
            // (НОВЕ) Ініціалізація словника сортування
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
            cmbSort = new ComboBox { Width = 180, Margin = new Padding(3), DropDownStyle = ComboBoxStyle.DropDownList, Font = commonFont };
            var btnSearch = new Button { Text = Strings.ButtonSearch, Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont };
            var btnReset = new Button { Text = Strings.ButtonReset, Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont };
            var btnReport = new Button { Text = Strings.ButtonReport, Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont };

            // (ЗМІНЕНО) Прив'язка ComboBox до словника
            cmbSort.DataSource = new BindingSource(guestSortOptions, null);
            cmbSort.DisplayMember = "Value"; // Показуємо користувачу локалізований текст
            cmbSort.ValueMember = "Key";     // В коді використовуємо ключ (напр. "LastName_ASC")

            filterPanel.Controls.Add(new Label { Text = Strings.LabelSearch, AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Font = commonFont, Margin = new Padding(3, 0, 0, 0) });
            filterPanel.Controls.Add(txtSearch);
            filterPanel.Controls.Add(new Label { Text = Strings.LabelSort, AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(10, 0, 0, 0), Font = commonFont });
            filterPanel.Controls.Add(cmbSort);
            filterPanel.Controls.Add(btnSearch);
            filterPanel.Controls.Add(btnReset);
            filterPanel.Controls.Add(btnReport);

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

            guestBox.Controls.Add(mainLayout);
            this.Controls.Add(guestBox);

            this.Load += ListGuestsControl_Load;
            btnSearch.Click += BtnSearch_Click;
            btnReset.Click += BtnReset_Click;
            btnReport.Click += BtnReport_Click;
            this.Resize += (sender, e) => CenterControls();
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

                    // (ЗМІНЕНО) Switch тепер використовує КЛЮЧІ, а не текст
                    switch (sortBy)
                    {
                        case "LastName_ASC": query = query.OrderBy(g => g.GuestLastName); break;
                        case "LastName_DESC": query = query.OrderByDescending(g => g.GuestLastName); break;
                        case "ID_ASC": query = query.OrderBy(g => g.IdGuest); break;
                        case "ID_DESC": query = query.OrderByDescending(g => g.IdGuest); break;
                        default: query = query.OrderBy(g => g.IdGuest); break;
                    }

                    dgv.DataSource = await query.ToListAsync();

                    if (dgv.Columns["IdGuest"] != null) dgv.Columns["IdGuest"].HeaderText = Strings.Col_ID;
                    if (dgv.Columns["GuestFirstName"] != null) dgv.Columns["GuestFirstName"].HeaderText = Strings.Col_FirstName;
                    if (dgv.Columns["GuestLastName"] != null) dgv.Columns["GuestLastName"].HeaderText = Strings.Col_LastName;
                    if (dgv.Columns["PhoneNumber"] != null) dgv.Columns["PhoneNumber"].HeaderText = Strings.Col_Phone;
                    if (dgv.Columns["DateOfBirth"] != null) dgv.Columns["DateOfBirth"].HeaderText = Strings.Col_BirthDate;
                    if (dgv.Columns["PassportSeries"] != null) dgv.Columns["PassportSeries"].HeaderText = Strings.Col_Passport;
                    if (dgv.Columns["IsRegularGuest"] != null) dgv.Columns["IsRegularGuest"].HeaderText = Strings.Col_IsRegular;
                    if (dgv.Columns["PresenceOfChild"] != null) dgv.Columns["PresenceOfChild"].Visible = false;
                    if (dgv.Columns["Reservations"] != null) dgv.Columns["Reservations"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних: {ex.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            // (ЗМІНЕНО) Передаємо КЛЮЧ (SelectedValue) замість тексту (SelectedItem)
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
            // (Код не змінено, він був коректним)
            var originalGuestList = dgv.DataSource as List<Guest>;

            if (originalGuestList == null || !originalGuestList.Any())
            {
                MessageBox.Show(Strings.MsgNoDataForReport, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<GuestReportDto> guestsToSerialize = originalGuestList.Select(guest => new GuestReportDto
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

            try
            {
                Hotel.ClassSerializare.SerializeToXml<List<GuestReportDto>>(ref guestsToSerialize, fileName);
                FormReport frmReport = new FormReport();
                frmReport.Show();
            }
            catch (Exception ex)
            {
                // MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}