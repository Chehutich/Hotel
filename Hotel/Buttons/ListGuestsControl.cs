using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Windows.Forms;

public class ListGuestsControl : UserControl
{
    private DataGridView dgv;

    public ListGuestsControl()
    {
        // Створюємо GroupBox та DataGridView, як і раніше
        var guestBox = new GroupBox
        {
            Text = "Список зареєстрованих гостей",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            Padding = new Padding(15)
        };

        dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None
        };

        guestBox.Controls.Add(dgv);
        this.Controls.Add(guestBox);

        // Важливо: підписуємося на подію Load.
        // Код у методі ListGuestsControl_Load виконається, коли цей екран буде показаний.
        this.Load += ListGuestsControl_Load;
    }

    // async void дозволяє методу виконувати операції (наприклад, запит до БД)
    // у фоновому режимі, не "заморожуючи" інтерфейс користувача.
    private async void ListGuestsControl_Load(object? sender, EventArgs e)
    {
        try
        {
            // 1. Створюємо екземпляр нашого DbContext.
            // Конструкція 'using' гарантує, що з'єднання з БД буде автоматично закрито.
            using (var context = new HotelDbContext())
            {
                // 2. Робимо запит до бази даних.
                // await наказує програмі зачекати, поки дані прийдуть з БД.
                // context.Guests представляє вашу таблицю 'guests'.
                // ToListAsync() отримує всі записи з таблиці.
                var guests = await context.Guests.ToListAsync();

                // 3. Встановлюємо отриманий список гостей як джерело даних для нашої таблиці.
                // DataGridView автоматично створить стовпці для кожної публічної властивості об'єкта Guest.
                dgv.DataSource = guests;

                // 4. Налаштовуємо вигляд стовпців для кращої читабельності.
                if (dgv.Columns["IdGuest"] != null) dgv.Columns["IdGuest"].HeaderText = "ID";
                if (dgv.Columns["GuestFirstName"] != null) dgv.Columns["GuestFirstName"].HeaderText = "Ім'я";
                if (dgv.Columns["GuestLastName"] != null) dgv.Columns["GuestLastName"].HeaderText = "Прізвище";
                if (dgv.Columns["PhoneNumber"] != null) dgv.Columns["PhoneNumber"].HeaderText = "Телефон";
                if (dgv.Columns["DateOfBirth"] != null) dgv.Columns["DateOfBirth"].HeaderText = "Дата народження";
                if (dgv.Columns["PassportSeries"] != null) dgv.Columns["PassportSeries"].HeaderText = "Паспорт";
                if (dgv.Columns["IsRegularGuest"] != null) dgv.Columns["IsRegularGuest"].HeaderText = "Постійний клієнт";

                // Ховаємо стовпці, які містять складні об'єкти і не призначені для прямого відображення.
                if (dgv.Columns["PresenceOfChild"] != null) dgv.Columns["PresenceOfChild"].Visible = false;
                if (dgv.Columns["Reservations"] != null) dgv.Columns["Reservations"].Visible = false;
            }
        }
        catch (Exception ex)
        {
            // Якщо щось піде не так (наприклад, немає зв'язку з БД), показуємо помилку.
            MessageBox.Show($"Помилка завантаження даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}