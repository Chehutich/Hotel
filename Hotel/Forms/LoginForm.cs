using Hotel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Reflection;
using Hotel.Localization;

namespace Hotel
{
    public partial class LoginForm : Form
    {
        private const string LanguageSettingsFile = "language.cfg";
        private const string ThemeSettingsFile = "theme.cfg";

        // (НОВЕ) Змінні для зберігання завантажених іконок
        private Image imgEyeOpen;
        private Image imgEyeClosed;

        public LoginForm()
        {
            InitializeComponent();

            // Завантажуємо всі іконки
            LoadIconToPictureBox(this.btnLanguage, "language_icon.png");
            LoadIconToPictureBox(this.btnTheme, "theme_icon.png");

            // (НОВЕ) Завантажуємо іконки для ока
            imgEyeOpen = LoadImage("eye_open.png");
            imgEyeClosed = LoadImage("eye_closed.png");

            // Встановлюємо початкову іконку (закрите око)
            if (imgEyeClosed != null)
            {
                this.pbShowPassword.Image = imgEyeClosed;
            }
        }

        // --- (НОВІ МЕТОДИ) Обробка ока ---

        // (НОВИЙ МЕТОД) Обробник натискання на "око"
        private void pbShowPassword_Click(object? sender, EventArgs e)
        {
            // Перевіряємо, чи пароль зараз прихований
            if (txtPassword.UseSystemPasswordChar)
            {
                // Показуємо пароль
                txtPassword.UseSystemPasswordChar = false;
                pbShowPassword.Image = imgEyeOpen; // Показуємо "відкрите око"
            }
            else
            {
                // Приховуємо пароль
                txtPassword.UseSystemPasswordChar = true;
                pbShowPassword.Image = imgEyeClosed; // Показуємо "закрите око"
            }
        }

        // (НОВИЙ МЕТОД) Завантажує картинку з ресурсів
        private Image? LoadImage(string iconFileName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = "Hotel.images." + iconFileName;

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        return Image.FromStream(stream);
                    }
                }
            }
            catch { } // Ігноруємо помилки, якщо іконки немає
            return null;
        }


        // --- (ІСНУЮЧІ МЕТОДИ) Тема, Мова, Вхід ---

        private void btnTheme_Click(object? sender, EventArgs e)
        {
            try
            {
                string currentTheme = "Light";
                if (File.Exists(ThemeSettingsFile))
                {
                    currentTheme = File.ReadAllText(ThemeSettingsFile).Trim();
                }
                string newTheme = (currentTheme == "Light") ? "Dark" : "Light";
                File.WriteAllText(ThemeSettingsFile, newTheme);
                ThemeManager.ApplyTheme(newTheme);
                UpdateTheme();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error switching theme: {ex.Message}", "Theme Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTheme()
        {
            this.BackColor = ThemeManager.FormBackground;
            this.loginBox.ForeColor = ThemeManager.TextColor;
            this.lblUsername.ForeColor = ThemeManager.TextColor;
            this.lblPassword.ForeColor = ThemeManager.TextColor;
            this.txtUsername.BackColor = ThemeManager.InputBackground;
            this.txtUsername.ForeColor = ThemeManager.InputForeColor;
            this.txtPassword.BackColor = ThemeManager.InputBackground;
            this.txtPassword.ForeColor = ThemeManager.InputForeColor;
            this.btnLogin.BackColor = ThemeManager.ButtonBackground;
            this.btnLogin.ForeColor = ThemeManager.ButtonForeColor;
        }

        private void btnLanguage_Click(object? sender, EventArgs e)
        {
            try
            {
                string currentLang = Thread.CurrentThread.CurrentUICulture.Name;
                string newLang = (currentLang == "uk-UA" || currentLang == "uk") ? "en-US" : "uk-UA";
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(newLang);
                File.WriteAllText(LanguageSettingsFile, newLang);
                UpdateLanguage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error switching language: {ex.Message}", "Language Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateLanguage()
        {
            this.Text = Strings.AppTitle;
            this.loginBox.Text = Strings.Login_Title;
            this.lblUsername.Text = Strings.Login_Username;
            this.lblPassword.Text = Strings.Login_Password;
            this.btnLogin.Text = Strings.Login_Button;
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show(Strings.Login_Validation_Empty, Strings.Login_Error_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string hashedPassword = ComputeSha256Hash(password);

            try
            {
                using (var context = new HotelDbContext())
                {
                    var staffUser = await context.Staff.FirstOrDefaultAsync(s =>
                        s.Username == username &&
                        s.PasswordHash == hashedPassword
                    );

                    if (staffUser != null)
                    {
                        AppContext.CurrentUser = staffUser;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(Strings.Login_Error_Invalid, Strings.Login_Error_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.Login_Error_Connection} {ex.Message}", Strings.Login_Error_ConnectionTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadIconToPictureBox(PictureBox pb, string iconFileName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = "Hotel.images." + iconFileName;

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        pb.Image = Image.FromStream(stream);
                    }
                    else
                    {
                        pb.BackColor = Color.LightGray;
                        string text = "Img";
                        if (iconFileName.Contains("lang")) text = "Lang";
                        if (iconFileName.Contains("theme")) text = "Theme";
                        pb.Controls.Add(new Label { Text = text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження іконки '{iconFileName}': {ex.Message}",
                                Strings.ValidationTitle,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                pb.BackColor = Color.Red;
            }
        }
    }
}