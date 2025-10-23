using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class ListGuestsControl : UserControl
{
    private DataGridView dgv;
    private TextBox txtSearch;
    private ComboBox cmbSort;
    private GroupBox guestBox;

    public ListGuestsControl()
    {
        guestBox = new GroupBox
        {
            Text = "Список зареєстрованих гостей",
            Dock = DockStyle.None,
            Width = 900,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom,
            Font = new Font("Segoe UI", 10F),
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
            AutoSize = true
        };

        txtSearch = new TextBox { Width = 200, Margin = new Padding(3) };
        cmbSort = new ComboBox { Width = 150, Margin = new Padding(3), DropDownStyle = ComboBoxStyle.DropDownList };
        var btnSearch = new Button { Text = "Пошук", Width = 100, Margin = new Padding(3) };
        var btnReset = new Button { Text = "Скинути", Width = 100, Margin = new Padding(3) };
        cmbSort.Items.AddRange(new string[] {
            "За прізвищем (А-Я)",
            "За прізвищем (Я-А)",
            "За ID (зростання)",
            "За ID (спадання)"
        });

        filterPanel.Controls.Add(new Label { Text = "Пошук:", AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft });
        filterPanel.Controls.Add(txtSearch);
        filterPanel.Controls.Add(new Label { Text = "Сортувати:", AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(10, 0, 0, 0) });
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
            BorderStyle = BorderStyle.None
        };

        mainLayout.Controls.Add(filterPanel, 0, 0);
        mainLayout.Controls.Add(dgv, 0, 1);

        guestBox.Controls.Add(mainLayout);
        this.Controls.Add(guestBox);

        this.Load += ListGuestsControl_Load;
        btnSearch.Click += BtnSearch_Click;
        btnReset.Click += BtnReset_Click;
        this.Resize += (sender, e) => CenterControls();
    }

    // Центрування головного GroupBox
    private void CenterControls()
    {
        guestBox.Height = this.ClientSize.Height;
        guestBox.Left = (this.ClientSize.Width - guestBox.Width) / 2;
    }

    // Завантаження початкових даних
    private void ListGuestsControl_Load(object? sender, EventArgs e)
    {
        LoadGuests();
        CenterControls();
    }

    // Завантаження даних гостей з опціональною фільтрацією та сортуванням
    private async void LoadGuests(string? searchTerm = null, string? sortBy = null)
    {
        try
        {
            using (var context = new HotelDbContext())
            {
                var query = context.Guests.AsQueryable();

                // Застосування фільтру пошуку
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(g =>
                        g.GuestFirstName.Contains(searchTerm) ||
                        g.GuestLastName.Contains(searchTerm) ||
                        g.PhoneNumber.Contains(searchTerm)
                    );
                }

                // Застосування сортування
                switch (sortBy)
                {
                    case "За прізвищем (А-Я)": query = query.OrderBy(g => g.GuestLastName); break;
                    case "За прізвищем (Я-А)": query = query.OrderByDescending(g => g.GuestLastName); break;
                    case "За ID (зростання)": query = query.OrderBy(g => g.IdGuest); break;
                    case "За ID (спадання)": query = query.OrderByDescending(g => g.IdGuest); break;
                    default: query = query.OrderBy(g => g.IdGuest); break;
                }

                dgv.DataSource = await query.ToListAsync();

                // Налаштування стовпців DataGridView
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

    // Обробка натискання кнопки "Пошук"
    private void BtnSearch_Click(object? sender, EventArgs e)
    {
        LoadGuests(txtSearch.Text, cmbSort.SelectedItem as string);
    }

    // Обробка натискання кнопки "Скинути"
    private void BtnReset_Click(object? sender, EventArgs e)
    {
        txtSearch.Clear();
        cmbSort.SelectedIndex = -1;
        LoadGuests();
    }
}