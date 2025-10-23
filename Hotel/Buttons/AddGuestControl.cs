using Hotel.Models;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions; // Додано для Regex

public class AddGuestControl : UserControl
{
    // Оголошуємо елементи керування як поля класу
    private TextBox txtFirstName, txtLastName, txtPhoneNumber, txtPassport;
    private DateTimePicker dtpDateOfBirth;
    private Button btnSave, btnCancel;

    public AddGuestControl()
    {
        var guestBox = new GroupBox
        {
            Text = "Введіть дані гостя",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            Padding = new Padding(20)
        };

        var layoutPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 6
        };

        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        // Ініціалізуємо поля класу
        txtFirstName = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5) };
        txtLastName = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5) };
        txtPhoneNumber = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5) };
        dtpDateOfBirth = new DateTimePicker { Dock = DockStyle.Fill, Margin = new Padding(5) };
        txtPassport = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5) };

        btnSave = new Button { Text = "Зберегти", Width = 100 };
        btnCancel = new Button { Text = "Скасувати", Width = 100 };

        // Додаємо елементи на панель
        layoutPanel.Controls.Add(CreateLabel("Ім'я:"), 0, 0);
        layoutPanel.Controls.Add(txtFirstName, 1, 0);

        layoutPanel.Controls.Add(CreateLabel("Прізвище:"), 0, 1);
        layoutPanel.Controls.Add(txtLastName, 1, 1);

        layoutPanel.Controls.Add(CreateLabel("Номер телефону:"), 0, 2);
        layoutPanel.Controls.Add(txtPhoneNumber, 1, 2);

        layoutPanel.Controls.Add(CreateLabel("Дата народження:"), 0, 3);
        layoutPanel.Controls.Add(dtpDateOfBirth, 1, 3);

        layoutPanel.Controls.Add(CreateLabel("Серія паспорту:"), 0, 4);
        layoutPanel.Controls.Add(txtPassport, 1, 4);

        var buttonPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill };
        buttonPanel.Controls.Add(btnSave);
        buttonPanel.Controls.Add(btnCancel);
        layoutPanel.Controls.Add(buttonPanel, 1, 5);

        guestBox.Controls.Add(layoutPanel);
        this.Controls.Add(guestBox);

        // Прив'язуємо обробники подій
        btnSave.Click += BtnSave_Click;
        btnCancel.Click += BtnCancel_Click;
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        // 1. Валідація імені та прізвища
        if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
        {
            MessageBox.Show("Ім'я та Прізвище є обов'язковими полями.", "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // 2. Валідація номера телефону
        string phoneNumber = txtPhoneNumber.Text;
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            string phoneRegexPattern = @"^(\+380\d{9}|0\d{9})$";
            if (!Regex.IsMatch(phoneNumber, phoneRegexPattern))
            {
                MessageBox.Show("Номер телефону введено неправильно.\nДопустимі формати: +380XXXXXXXXX (13 цифр) або 0XXXXXXXXX (10 цифр).", "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        // 3. ОНОВЛЕНО: Валідація серії паспорту
        string passport = txtPassport.Text;
        if (!string.IsNullOrWhiteSpace(passport)) // Перевіряємо, тільки якщо поле не порожнє
        {
            // Шаблон: 9 цифр (ID-картка) АБО 2 укр. літери (включаючи І, Ї, Є) та 6 цифр (книжечка)
            string passportRegexPattern = @"^(\d{9}|[А-ЯІЇЄа-яіїє]{2}\d{6})$";

            if (!Regex.IsMatch(passport, passportRegexPattern))
            {
                // Якщо номер не відповідає шаблону, показуємо помилку
                MessageBox.Show("Серію паспорту введено неправильно.\nДопустимі формати: 9 цифр (ID-картка) або 2 літери та 6 цифр (паспорт-книжечка).", "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Зупиняємо збереження
            }
        }
        // Кінець блоку валідації паспорту

        try
        {
            using (var context = new HotelDbContext())
            {
                var newGuest = new Guest
                {
                    GuestFirstName = txtFirstName.Text,
                    GuestLastName = txtLastName.Text,
                    PhoneNumber = phoneNumber,
                    PassportSeries = passport, // Зберігаємо валідний паспорт
                    DateOfBirth = DateOnly.FromDateTime(dtpDateOfBirth.Value),
                    IsRegularGuest = false
                };

                var childInfo = new PresenceOfChild
                {
                    IdGuestNavigation = newGuest,
                    ChildrenPresence = false,
                    NumberOfChild = 0,
                    AgeOfChild = null
                };

                context.Guests.Add(newGuest);
                context.PresenceOfChildren.Add(childInfo);

                await context.SaveChangesAsync();

                MessageBox.Show("Гостя успішно додано!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка збереження гостя: {ex.Message}\n\n{ex.InnerException?.Message}", "Помилка бази даних", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        ClearForm();
    }

    private void ClearForm()
    {
        txtFirstName.Clear();
        txtLastName.Clear();
        txtPhoneNumber.Clear();
        txtPassport.Clear();
        dtpDateOfBirth.Value = DateTime.Now;
    }

    private Label CreateLabel(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
            Margin = new Padding(5)
        };
    }
}