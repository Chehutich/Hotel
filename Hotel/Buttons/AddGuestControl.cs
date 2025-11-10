using Hotel.Models;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq;

namespace Hotel
{
    public class AddGuestControl : UserControl
    {
        private TextBox txtFirstName, txtLastName, txtPhoneNumber, txtPassport;
        private DateTimePicker dtpDateOfBirth;
        private Button btnSave, btnCancel;
        private GroupBox guestBox;
        private Font commonFont = new Font("Segoe UI", 11F); // Виносимо шрифт

        public AddGuestControl()
        {
            guestBox = new GroupBox
            {
                Text = Strings.AddGuestTitle, // (ЗМІНЕНО)
                Dock = DockStyle.None,
                Width = 800,
                Height = 480,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Padding = new Padding(25)
            };

            var layoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6
            };

            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            txtFirstName = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont };
            txtLastName = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont };
            txtPhoneNumber = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont };
            dtpDateOfBirth = new DateTimePicker { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont, Format = DateTimePickerFormat.Long };
            txtPassport = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5), Font = commonFont };

            btnSave = new Button { Text = Strings.ButtonSave, Width = 130, Height = 40, Font = commonFont }; // (ЗМІНЕНО)
            btnCancel = new Button { Text = Strings.ButtonCancel, Width = 130, Height = 40, Font = commonFont }; // (ЗМІНЕНО)

            layoutPanel.Controls.Add(CreateLabel(Strings.LabelFirstName), 0, 0); // (ЗМІНЕНО)
            layoutPanel.Controls.Add(txtFirstName, 1, 0);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelLastName), 0, 1); // (ЗМІНЕНО)
            layoutPanel.Controls.Add(txtLastName, 1, 1);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelPhoneNumber), 0, 2); // (ЗМІНЕНО)
            layoutPanel.Controls.Add(txtPhoneNumber, 1, 2);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelDateOfBirth), 0, 3); // (ЗМІНЕНО)
            layoutPanel.Controls.Add(dtpDateOfBirth, 1, 3);
            layoutPanel.Controls.Add(CreateLabel(Strings.LabelPassport), 0, 4); // (ЗМІНЕНО)
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
            // (ЗМІНЕНО) Використання ключів валідації
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text)) { MessageBox.Show(Strings.ValidationNamesRequired, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string phoneNumber = txtPhoneNumber.Text;
            if (!string.IsNullOrWhiteSpace(phoneNumber)) { string phoneRegexPattern = @"^(\+380\d{9}|0\d{9})$"; if (!Regex.IsMatch(phoneNumber, phoneRegexPattern)) { MessageBox.Show(Strings.ValidationPhoneFormat, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning); return; } }
            string passport = txtPassport.Text;
            if (!string.IsNullOrWhiteSpace(passport)) { string passportRegexPattern = @"^(\d{9}|[A-Z]{2}\d{6})$"; if (!Regex.IsMatch(passport, passportRegexPattern)) { MessageBox.Show(Strings.ValidationPassportFormat, Strings.ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning); return; } }

            try
            {
                using (var context = new HotelDbContext())
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

                    // (ПРИМІТКА) Ключа для "Успіх" не було, залишаю ваш текст
                    MessageBox.Show("Гостя успішно додано!", Strings.AddGuestTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                // (ЗМІНЕНО)
                MessageBox.Show($"Помилка збереження гостя: {ex.Message}\n\n{ex.InnerException?.Message}", Strings.ErrorDBTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                Margin = new Padding(5),
                Font = commonFont
            };
        }
    }
}