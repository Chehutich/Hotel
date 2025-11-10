using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Hotel.Localization;

namespace Hotel
{
    public class SettingsControl : UserControl
    {
        private GroupBox settingsBox;
        private Font commonFont = new Font("Segoe UI", 11F);

        // Елементи керування
        private ComboBox cmbLanguage, cmbTheme; // (ОНОВЛЕНО)
        private Button btnSaveSettings;         // (ОНОВЛЕНО)
        private Label lblLanguage, lblTheme;    // (ОНОВЛЕНО)

        // Словники
        private Dictionary<string, string> languageMap = new Dictionary<string, string>();
        private Dictionary<string, string> themeMap = new Dictionary<string, string>(); // (НОВЕ)

        // Файли налаштувань
        private const string LanguageSettingsFile = "language.cfg";
        private const string ThemeSettingsFile = "theme.cfg";    // (НОВЕ)

        public SettingsControl()
        {
            // Словник мов
            languageMap.Add("uk-UA", Strings.Ukrainian);
            languageMap.Add("en-US", Strings.English);

            // (НОВЕ) Словник тем
            themeMap.Add("Light", "Світла"); // (ПРИМІТКА: ці рядки не локалізовані, але можна)
            themeMap.Add("Dark", "Темна");

            settingsBox = new GroupBox
            {
                Text = Strings.SettingsTitle,
                Dock = DockStyle.None,
                Width = 800,
                Height = 300, // (ЗБІЛЬШЕНО)
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Padding = new Padding(25),
                ForeColor = ThemeManager.TextColor // (НОВЕ)
            };

            var layoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3, // (ЗБІЛЬШЕНО)
                Width = 400
            };
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // (НОВИЙ РЯДОК)
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // --- Налаштування Мови ---
            lblLanguage = new Label
            {
                Text = Strings.Language + ":",
                Font = commonFont,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = new Padding(5),
                ForeColor = ThemeManager.TextColor // (НОВЕ)
            };

            cmbLanguage = new ComboBox
            {
                Font = commonFont,
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = ThemeManager.InputBackground, // (НОВЕ)
                ForeColor = ThemeManager.InputForeColor   // (НОВЕ)
            };
            cmbLanguage.DataSource = new BindingSource(languageMap, null);
            cmbLanguage.DisplayMember = "Value";
            cmbLanguage.ValueMember = "Key";

            // --- (НОВЕ) Налаштування Теми ---
            lblTheme = new Label
            {
                Text = "Тема:", // (ПРИМІТКА: цей рядок не локалізовано)
                Font = commonFont,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = new Padding(5),
                ForeColor = ThemeManager.TextColor
            };

            cmbTheme = new ComboBox
            {
                Font = commonFont,
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = ThemeManager.InputBackground,
                ForeColor = ThemeManager.InputForeColor
            };
            cmbTheme.DataSource = new BindingSource(themeMap, null);
            cmbTheme.DisplayMember = "Value";
            cmbTheme.ValueMember = "Key";

            // --- Кнопка Зберегти ---
            btnSaveSettings = new Button
            {
                Text = Strings.ButtonSave,
                Font = commonFont,
                Size = new Size(130, 40),
                Margin = new Padding(5),
                BackColor = ThemeManager.ButtonBackground, // (НОВЕ)
                ForeColor = ThemeManager.ButtonForeColor   // (НОВЕ)
            };

            // Додавання елементів на панель
            layoutPanel.Controls.Add(lblLanguage, 0, 0);
            layoutPanel.Controls.Add(cmbLanguage, 1, 0);
            layoutPanel.Controls.Add(lblTheme, 0, 1);    // (НОВЕ)
            layoutPanel.Controls.Add(cmbTheme, 1, 1);    // (НОВЕ)
            layoutPanel.Controls.Add(btnSaveSettings, 1, 2); // (Рядок змінено на 2)

            settingsBox.Controls.Add(layoutPanel);
            this.Controls.Add(settingsBox);

            // Обробники подій
            this.Load += SettingsControl_Load;
            btnSaveSettings.Click += BtnSaveSettings_Click; // (ОНОВЛЕНО)

            this.Load += (sender, e) => CenterControls();
            this.Resize += (sender, e) => CenterControls();
        }

        private void SettingsControl_Load(object? sender, EventArgs e)
        {
            // Завантаження мови
            try
            {
                string currentLang = "uk-UA";
                if (File.Exists(LanguageSettingsFile))
                {
                    currentLang = File.ReadAllText(LanguageSettingsFile).Trim();
                }
                if (languageMap.ContainsKey(currentLang))
                {
                    cmbLanguage.SelectedValue = currentLang;
                }
                else
                {
                    cmbLanguage.SelectedValue = "uk-UA";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading language file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbLanguage.SelectedValue = "uk-UA";
            }

            // (НОВЕ) Завантаження теми
            try
            {
                string currentTheme = "Light";
                if (File.Exists(ThemeSettingsFile))
                {
                    currentTheme = File.ReadAllText(ThemeSettingsFile).Trim();
                }
                if (themeMap.ContainsKey(currentTheme))
                {
                    cmbTheme.SelectedValue = currentTheme;
                }
                else
                {
                    cmbTheme.SelectedValue = "Light";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading theme file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbTheme.SelectedValue = "Light";
            }
        }

        // (ОНОВЛЕНО) Зберігає ОБИДВА налаштування
        private void BtnSaveSettings_Click(object? sender, EventArgs e)
        {
            if (cmbLanguage.SelectedValue == null || cmbTheme.SelectedValue == null) return;

            string selectedLang = cmbLanguage.SelectedValue.ToString();
            string selectedTheme = cmbTheme.SelectedValue.ToString();

            try
            {
                // 1. Зберегти мову
                File.WriteAllText(LanguageSettingsFile, selectedLang);

                // 2. (НОВЕ) Зберегти тему
                File.WriteAllText(ThemeSettingsFile, selectedTheme);

                // 3. Показати повідомлення та перезапустити програму
                MessageBox.Show(
                    Strings.MsgLanguageRestart, // "Налаштування... буде застосовано після перезапуску."
                    Strings.MsgLanguageTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Application.Restart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings files: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CenterControls()
        {
            settingsBox.Left = (this.ClientSize.Width - settingsBox.Width) / 2;
            settingsBox.Top = (this.ClientSize.Height - settingsBox.Height) / 2;
        }
    }
}