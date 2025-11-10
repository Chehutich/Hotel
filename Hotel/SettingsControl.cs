using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic; // Додано
using System.Globalization;     // Додано
using System.IO;                // Додано для роботи з файлом

namespace Hotel
{
    public class SettingsControl : UserControl
    {
        private GroupBox settingsBox;
        private Font commonFont = new Font("Segoe UI", 11F);

        // Елементи керування для мови
        private ComboBox cmbLanguage;
        private Button btnSaveLanguage;
        private Label lblLanguage;

        // Словник для зв'язування коду мови (en-US) з її назвою ("English")
        private Dictionary<string, string> languageMap = new Dictionary<string, string>();

        // (НОВЕ) Назва нашого файлу налаштувань
        private const string LanguageSettingsFile = "language.cfg";

        public SettingsControl()
        {
            // Словник мов.
            languageMap.Add("uk-UA", Strings.Ukrainian); // Strings.Ukrainian береться з .resx
            languageMap.Add("en-US", Strings.English);   // Strings.English береться з .resx

            settingsBox = new GroupBox
            {
                Text = Strings.SettingsTitle, // (ЗМІНЕНО) Використовуємо ресурс
                Dock = DockStyle.None,
                Width = 800,
                Height = 250,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Padding = new Padding(25)
            };

            // Панель для розміщення елементів
            var layoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Width = 400
            };
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Елементи керування
            lblLanguage = new Label
            {
                Text = Strings.Language + ":", // Використовуємо ресурс
                Font = commonFont,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = new Padding(5)
            };

            cmbLanguage = new ComboBox
            {
                Font = commonFont,
                Dock = DockStyle.Fill,
                Margin = new Padding(5),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Наповнення ComboBox
            cmbLanguage.DataSource = new BindingSource(languageMap, null);
            cmbLanguage.DisplayMember = "Value"; // Показувати користувачу "Українська"
            cmbLanguage.ValueMember = "Key";     // Зберігати в коді "uk-UA"

            btnSaveLanguage = new Button
            {
                Text = Strings.ButtonSave, // Використовуємо ресурс
                Font = commonFont,
                Size = new Size(130, 40),
                Margin = new Padding(5)
            };

            // Додавання елементів на панель
            layoutPanel.Controls.Add(lblLanguage, 0, 0);
            layoutPanel.Controls.Add(cmbLanguage, 1, 0);
            layoutPanel.Controls.Add(btnSaveLanguage, 1, 1);

            settingsBox.Controls.Add(layoutPanel);
            this.Controls.Add(settingsBox);

            // Обробники подій
            this.Load += SettingsControl_Load;
            btnSaveLanguage.Click += BtnSaveLanguage_Click;

            this.Load += (sender, e) => CenterControls();
            this.Resize += (sender, e) => CenterControls();
        }

        // (ЗМІНЕНО) Функція: при завантаженні, виставити мову, яка збережена у ФАЙЛІ
        private void SettingsControl_Load(object? sender, EventArgs e)
        {
            try
            {
                string currentLang = "uk-UA"; // За замовчуванням
                if (File.Exists(LanguageSettingsFile))
                {
                    currentLang = File.ReadAllText(LanguageSettingsFile).Trim();
                }

                // Переконуємось, що в файлі збережено валідний код
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
        }

        // (ЗМІНЕНО) Функція: при натисканні на кнопку "Зберегти"
        private void BtnSaveLanguage_Click(object? sender, EventArgs e)
        {
            if (cmbLanguage.SelectedValue == null) return;

            string selectedLang = cmbLanguage.SelectedValue.ToString();

            try
            {
                // 1. Зберегти вибір у ФАЙЛ
                File.WriteAllText(LanguageSettingsFile, selectedLang);

                // 2. Показати повідомлення та перезапустити програму
                MessageBox.Show(
                    Strings.MsgLanguageRestart, // "Налаштування мови буде застосовано..."
                    Strings.MsgLanguageTitle,   // "Зміна мови"
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Application.Restart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving language file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // (ІСНУЮЧА) Функція центрування
        private void CenterControls()
        {
            settingsBox.Left = (this.ClientSize.Width - settingsBox.Width) / 2;
            settingsBox.Top = (this.ClientSize.Height - settingsBox.Height) / 2;
        }
    }
}