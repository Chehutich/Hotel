using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Hotel
{
    public static class ThemeManager
    {
        // --- КОЛЬОРИ (як і були) ---
        public static Color FormBackground { get; private set; }
        public static Color ContentBackground { get; private set; }
        public static Color MenuBackground { get; private set; }
        public static Color TextColor { get; private set; }
        public static Color ButtonBackground { get; private set; }
        public static Color ButtonForeColor { get; private set; }
        public static Color InputBackground { get; private set; }
        public static Color InputForeColor { get; private set; }
        public static Color GridBackground { get; private set; }
        public static Color GridHeaderBackground { get; private set; }
        public static Color GridHeaderForeColor { get; private set; }

        // --- (НОВЕ) ІКОНКИ ---
        // Ми будемо зберігати завантажені картинки тут
        public static Image? HomeIcon { get; private set; }
        public static Image? SettingsIcon { get; private set; }
        public static Image? LanguageIcon { get; private set; }
        public static Image? ThemeIcon { get; private set; }
        public static Image? EyeOpenIcon { get; private set; }
        public static Image? EyeClosedIcon { get; private set; }


        // (НОВИЙ МЕТОД) Допоміжний метод для завантаження іконок з ресурсів
        private static Image? LoadImage(string iconName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = "Hotel.images." + iconName;
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        return Image.FromStream(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                // Якщо іконку не знайдено, це проблема, але програма не має падати
                Console.WriteLine($"Failed to load icon {iconName}: {ex.Message}");
            }
            return null; // Повертаємо null, якщо не вдалося завантажити
        }

        // (ОНОВЛЕНО) ApplyTheme тепер завантажує і кольори, і іконки
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

                // (НОВЕ) Завантажуємо ТЕМНІ іконки
                HomeIcon = LoadImage("home_dark_icon.png");
                SettingsIcon = LoadImage("settings_dark_icon.png");
                LanguageIcon = LoadImage("language_dark_icon.png");
                ThemeIcon = LoadImage("theme_dark_icon.png");
                EyeOpenIcon = LoadImage("eye_open_dark_icon.png");
                EyeClosedIcon = LoadImage("eye_closed_dark_icon.png");
            }
            else // "Light"
            {
                // === Світла Тема ===
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

                // (НОВЕ) Завантажуємо СВІТЛІ (оригінальні) іконки
                HomeIcon = LoadImage("home_icon.png");
                SettingsIcon = LoadImage("settings_icon.png");
                LanguageIcon = LoadImage("language_icon.png");
                ThemeIcon = LoadImage("theme_icon.png");
                EyeOpenIcon = LoadImage("eye_open.png");
                EyeClosedIcon = LoadImage("eye_closed.png");
            }
        }
    }
}