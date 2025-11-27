using Hotel.Forms;
using Hotel.Localization;
using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq;
using Hotel.Core;

namespace Hotel.Buttons
{
    public class AddGuestControl : UserControl
    {
        private TextBox txtFirstName, txtLastName, txtPhoneNumber, txtPassport;
        private DateTimePicker dtpDateOfBirth;
        private Button btnSave, btnCancel;
        private GroupBox guestBox;
        private Font commonFont = new Font("Segoe UI", 11F);
        private Font pickerFont = new Font("Segoe UI", 12F);

        private int? editingGuestId = null;

        public AddGuestControl()
        {
            InitializeComponent();
        }

        // Конструктор для редагування
        public AddGuestControl(int guestIdToEdit)
        {
            InitializeComponent();
            this.editingGuestId = guestIdToEdit;
            guestBox.Text = Strings.EditGuest_Title;
            LoadGuestDataAsync(guestIdToEdit);
        }

        // (НОВИЙ КОНСТРУКТОР) Для швидкого створення з відомим номером телефону
        public AddGuestControl(string phoneNumberToPreset)
        {
            InitializeComponent();
            // Одразу заповнюємо поле телефону
            txtPhoneNumber.Text = phoneNumberToPreset;
        }

        private async void LoadGuestDataAsync(int guestId)
        {
            try
            {
                using (var context = new HotelDbContext())
                {
                    var guest = await context.Guests.FindAsync(guestId);
                    if (guest != null)
                    {
                        txtFirstName.Text = guest.GuestFirstName;
                        txtLastName.Text = guest.GuestLastName;
                        txtPhoneNumber.Text = guest.PhoneNumber;
                        txtPassport.Text = guest.PassportSeries;
                        if (guest.DateOfBirth.HasValue)
                        {
                            dtpDateOfBirth.Value = guest.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не вдалося завантажити дані гостя.", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ClearForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних: {ex.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            guestBox = new GroupBox
            {
                Text = Strings.AddGuestTitle,
                Dock = DockStyle.None,
                Width = 800,
                Height = 480,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Padding = new Padding(25),
                ForeColor = ThemeManager.TextColor
            };

            var layoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6
            };

            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            txtFirstName = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
            txtLastName = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
            txtPhoneNumber = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };

            dtpDateOfBirth = new DateTimePicker { Dock = DockStyle.Fill, Margin = new Padding(5), Font = pickerFont, Format = DateTimePickerFormat.Long };
            dtpDateOfBirth.CalendarMonthBackground = ThemeManager.InputBackground;
            dtpDateOfBirth.CalendarForeColor = ThemeManager.InputForeColor;
            dtpDateOfBirth.CalendarTitleBackColor = ThemeManager.ButtonBackground;
            dtpDateOfBirth.CalendarTitleForeColor = ThemeManager.ButtonForeColor;
            dtpDateOfBirth.CalendarTrailingForeColor = Color.Gray;

            txtPassport = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont, BackColor = ThemeManager.InputBackground, ForeColor = ThemeManager.InputForeColor };
            btnSave = new Button { Text = Strings.ButtonSave, Width = 130, Height = 40, Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };
            btnCancel = new Button { Text = Strings.ButtonCancel, Width = 130, Height = 40, Font = commonFont, BackColor = ThemeManager.ButtonBackground, ForeColor = ThemeManager.ButtonForeColor };

            layoutPanel.Controls.Add(CreateLabel(Strings.LabelFirstName), 0, 0);
            layoutPanel.Controls.Add(txtFirstName, 1, 0);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelLastName), 0, 1);
            layoutPanel.Controls.Add(txtLastName, 1, 1);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelPhoneNumber), 0, 2);
            layoutPanel.Controls.Add(txtPhoneNumber, 1, 2);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelDateOfBirth), 0, 3);
            layoutPanel.Controls.Add(dtpDateOfBirth, 1, 3);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelPassport), 0, 4);
            layoutPanel.Controls.Add(txtPassport, 1, 4);
            var buttonPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill, Padding = new Padding(0, 15, 0, 0) };
            buttonPanel.Controls.Add(btnSave);
            buttonPanel.Controls.Add(btnCancel);
            layoutPanel.Controls.Add(buttonPanel, 1, 5);

            guestBox.Controls.Add(layoutPanel);
            this.Controls.Add(guestBox);

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;

            this.Load += (sender, e) => CenterControls();
            this.Resize += (sender, e) => CenterControls();
        }

        private void CenterControls()
        {
            guestBox.Left = (this.ClientSize.Width - guestBox.Width) / 2;
            guestBox.Top = (this.ClientSize.Height - guestBox.Height) / 2;
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show(Strings.ValidationNamesRequired, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string phoneNumber = txtPhoneNumber.Text.Trim();
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                MessageBox.Show(Strings.ValidationPhoneFormat, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string phoneRegexPattern = @"^(\+380\d{9}|0\d{9})$";
            if (!Regex.IsMatch(phoneNumber, phoneRegexPattern))
            {
                MessageBox.Show(Strings.ValidationPhoneFormat, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string passport = txtPassport.Text.Trim();
            if (!string.IsNullOrWhiteSpace(passport))
            {
                string passportRegexPattern = @"^(\d{9}|[A-Z]{2}\d{6})$";
                if (!Regex.IsMatch(passport, passportRegexPattern))
                {
                    MessageBox.Show(Strings.ValidationPassportFormat, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                passport = null;
            }

            try
            {
                using (var context = new HotelDbContext())
                {
                    bool phoneExists = await context.Guests.AnyAsync(g =>
                        g.PhoneNumber == phoneNumber &&
                        g.IdGuest != editingGuestId);

                    if (phoneExists)
                    {
                        MessageBox.Show(Strings.Validation_PhoneExists, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (passport != null)
                    {
                        bool passportExists = await context.Guests.AnyAsync(g =>
                            g.PassportSeries == passport &&
                            g.IdGuest != editingGuestId);

                        if (passportExists)
                        {
                            MessageBox.Show(Strings.Validation_PassportExists, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    if (editingGuestId == null)
                    {
                        var newGuest = new Guest
                        {
                            GuestFirstName = txtFirstName.Text,
                            GuestLastName = txtLastName.Text,
                            PhoneNumber = phoneNumber,
                            PassportSeries = passport,
                            DateOfBirth = DateOnly.FromDateTime(dtpDateOfBirth.Value),
                            IsRegularGuest = false
                        };

                        var childInfo = new PresenceOfChild
                        {
                            ChildrenPresence = false,
                            NumberOfChild = 0,
                            AgeOfChild = null
                        };
                        newGuest.PresenceOfChild = childInfo;

                        context.Guests.Add(newGuest);
                        await context.SaveChangesAsync();
                        MessageBox.Show("Гостя успішно додано!", Strings.AddGuestTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        var guestToUpdate = await context.Guests.FindAsync(editingGuestId.Value);
                        if (guestToUpdate != null)
                        {
                            guestToUpdate.GuestFirstName = txtFirstName.Text;
                            guestToUpdate.GuestLastName = txtLastName.Text;
                            guestToUpdate.PhoneNumber = phoneNumber;
                            guestToUpdate.PassportSeries = passport;
                            guestToUpdate.DateOfBirth = DateOnly.FromDateTime(dtpDateOfBirth.Value);

                            await context.SaveChangesAsync();
                            MessageBox.Show(Strings.Guest_Update_Success, Strings.EditGuest_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

                    ClearForm();

                    var form1 = this.FindForm() as Form1;
                    if (form1 != null)
                    {
                        form1.ShowListGuests();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження гостя: {ex.Message}\n\n{ex.InnerException?.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            ClearForm();
            var form1 = this.FindForm() as Form1;
            if (form1 != null)
            {
                form1.ShowListGuests();
            }
        }

        private void ClearForm()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtPhoneNumber.Clear();
            txtPassport.Clear();
            dtpDateOfBirth.Value = DateTime.Now;
            editingGuestId = null;
            guestBox.Text = Strings.AddGuestTitle;
        }

        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = new Padding(5),
                Font = commonFont,
                ForeColor = ThemeManager.TextColor
            };
        }
    }
}