using Hotel;
using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
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
        private Font commonFont = new Font("Segoe UI", 10F); // Шрифт для фільтрів

        public ListGuestsControl()
        {
            guestBox = new GroupBox
            {
                Text = "Список зареєстрованих гостей",
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
                Padding = new Padding(0, 0, 0, 10) // Відступ знизу
            };

            txtSearch = new TextBox { Width = 200, Margin = new Padding(3), Font = commonFont }; // ЗБІЛЬШЕНО
            cmbSort = new ComboBox { Width = 180, Margin = new Padding(3), DropDownStyle = ComboBoxStyle.DropDownList, Font = commonFont }; // ЗБІЛЬШЕНО
            var btnSearch = new Button { Text = "Пошук", Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont }; // ЗБІЛЬШЕНО
            var btnReset = new Button { Text = "Скинути", Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont }; // ЗБІЛЬШEНО
            var btnReport = new Button { Text = "Звіт", Size = new Size(100, 35), Margin = new Padding(3), Font = commonFont }; // ЗБІЛЬШЕНО

            cmbSort.Items.AddRange(new string[] {
            "За прізвищем (А-Я)",
            "За прізвищем (Я-А)",
            "За ID (зростання)",
            "За ID (спадання)"
            });

            filterPanel.Controls.Add(new Label { Text = "Пошук:", AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Font = commonFont, Margin = new Padding(3, 0, 0, 0) });
            filterPanel.Controls.Add(txtSearch);
            filterPanel.Controls.Add(new Label { Text = "Сортувати:", AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(10, 0, 0, 0), Font = commonFont });
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
                Font = new Font("Segoe UI", 10F) // ЗБІЛЬШЕНО шрифт таблиці
            };
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold); // ЗБІЛЬШЕНО заголовки
            dgv.RowTemplate.Height = 30; // ЗБІЛЬШЕНО висоту рядків

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
            guestBox.Height = this.ClientSize.Height - 10; // Невеликий відступ
            guestBox.Left = (this.ClientSize.Width - guestBox.Width) / 2;
            guestBox.Top = 5; // Відступ зверху
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
                        case "За прізвищем (А-Я)": query = query.OrderBy(g => g.GuestLastName); break;
                        case "За прізвищем (Я-А)": query = query.OrderByDescending(g => g.GuestLastName); break;
                        case "За ID (зростання)": query = query.OrderBy(g => g.IdGuest); break;
                        case "За ID (спадання)": query = query.OrderByDescending(g => g.IdGuest); break;
                        default: query = query.OrderBy(g => g.IdGuest); break;
                    }

                    dgv.DataSource = await query.ToListAsync();

                    if (dgv.Columns["IdGuest"] != null) dgv.Columns["IdGuest"].HeaderText = "ID";
                    if (dgv.Columns["GuestFirstName"] != null) dgv.Columns["GuestFirstName"].HeaderText = "Ім'я";
                    if (dgv.Columns["GuestLastName"] != null) dgv.Columns["GuestLastName"].HeaderText = "Прізвище";
                    if (dgv.Columns["PhoneNumber"] != null) dgv.Columns["PhoneNumber"].HeaderText = "Телефон";
                    if (dgv.Columns["DateOfBirth"] != null) dgv.Columns["DateOfBirth"].HeaderText = "Дата народження";
                    if (dgv.Columns["PassportSeries"] != null) dgv.Columns["PassportSeries"].HeaderText = "Паспорт";
                    if (dgv.Columns["IsRegularGuest"] != null) dgv.Columns["IsRegularGuest"].HeaderText = "Постійний клієнт";
                    if (dgv.Columns["PresenceOfChild"] != null) dgv.Columns["PresenceOfChild"].Visible = false;
                    if (dgv.Columns["Reservations"] != null) dgv.Columns["Reservations"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            LoadGuests(txtSearch.Text, cmbSort.SelectedItem as string);
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            txtSearch.Clear();
            cmbSort.SelectedIndex = -1;
            LoadGuests();
        }

        private void BtnReport_Click(object? sender, EventArgs e)
        {
            var originalGuestList = dgv.DataSource as List<Guest>;

            if (originalGuestList == null || !originalGuestList.Any())
            {
                MessageBox.Show("Немає даних для генерації звіту.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                frmReport.Show(); // Відкриваємо немодально
            }
            catch (Exception ex)
            {
                // MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
