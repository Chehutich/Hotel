using System.Drawing;
using System.Windows.Forms;

namespace Hotel
{
    // Статичний клас для доступу до кольорів теми з будь-якого місця програми
    public static class ThemeManager
    {
        // --- КОЛЬОРИ ДЛЯ КОНТРОЛІВ ---

        // Основний фон програми (був 240, 240, 240)
        public static Color FormBackground { get; private set; }

        // Фон для контенту (де UserControl'и) (був White)
        public static Color ContentBackground { get; private set; }

        // Фон лівого меню (був 225, 225, 225)
        public static Color MenuBackground { get; private set; }

        // Колір тексту (для Label, GroupBox)
        public static Color TextColor { get; private set; }

        // Фон для кнопок
        public static Color ButtonBackground { get; private set; }

        // Колір тексту на кнопках
        public static Color ButtonForeColor { get; private set; }

        // Фон для полів вводу (TextBox, ComboBox)
        public static Color InputBackground { get; private set; }

        // Колір тексту в полях вводу
        public static Color InputForeColor { get; private set; }

        // Фон для сіток (DataGridView)
        public static Color GridBackground { get; private set; }

        // Фон заголовків сіток
        public static Color GridHeaderBackground { get; private set; }

        // Колір тексту заголовків сіток
        public static Color GridHeaderForeColor { get; private set; }


        // --- Метод, який застосовує тему ---

        public static void ApplyTheme(string themeName)
        {
            if (themeName == "Dark")
            {
                // === Темна Тема ===
                FormBackground = Color.FromArgb(45, 45, 48);
                ContentBackground = Color.FromArgb(30, 30, 30);
                MenuBackground = Color.FromArgb(60, 60, 60);
                TextColor = Color.White;
                ButtonBackground = Color.FromArgb(80, 80, 80);
                ButtonForeColor = Color.White;
                InputBackground = Color.FromArgb(68, 68, 68);
                InputForeColor = Color.White;
                GridBackground = Color.FromArgb(45, 45, 48);
                GridHeaderBackground = Color.FromArgb(80, 80, 80);
                GridHeaderForeColor = Color.White;
            }
            else // "Light" або за замовчуванням
            {
                // === Світла Тема (ваші поточні кольори) ===
                FormBackground = Color.FromArgb(240, 240, 240);
                ContentBackground = Color.White;
                MenuBackground = Color.FromArgb(225, 225, 225);
                TextColor = Color.Black;
                ButtonBackground = SystemColors.Control;
                ButtonForeColor = Color.Black;
                InputBackground = Color.White;
                InputForeColor = Color.Black;
                GridBackground = Color.White;
                GridHeaderBackground = SystemColors.Control;
                GridHeaderForeColor = Color.Black;
            }
        }
    }
}