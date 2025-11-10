using System.Globalization;
using System.Threading;
using System.IO;
using Hotel.Localization;

namespace Hotel
{
    internal static class Program
    {
        private const string LanguageSettingsFile = "language.cfg";
        private const string ThemeSettingsFile = "theme.cfg";

        [STAThread]
        static void Main()
        {
            // === КРОК 1: ЗАВАНТАЖЕННЯ .env (для даних про хост/БД) ===
            // (ПЕРЕМІЩЕНО ВГОРУ) Це має статися до запуску будь-якої форми
            DotNetEnv.Env.Load(".env");
            try
            {
                var host = Environment.GetEnvironmentVariable("DB_HOST");
                var database = Environment.GetEnvironmentVariable("DB_NAME");
                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(database))
                {
                    throw new InvalidOperationException("Database connection parameters are not configured in .env");
                }

                // (ВАЖЛИВО) Ми завантажуємо root-рядок в AppContext.
                // DbContext буде використовувати цей рядок.
                var port = Environment.GetEnvironmentVariable("DB_PORT");
                var user = Environment.GetEnvironmentVariable("DB_USER");
                var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
                AppContext.MasterConnectionString = $"Server={host};Port={port};Database={database};User={user};Password={password};";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading environment variables: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // === КРОК 2: ВСТАНОВЛЕННЯ МОВИ ===
            string cultureName = "uk-UA";
            try
            {
                if (File.Exists(LanguageSettingsFile))
                {
                    string langFromFile = File.ReadAllText(LanguageSettingsFile).Trim();
                    if (!string.IsNullOrEmpty(langFromFile)) { cultureName = langFromFile; }
                }
                else
                {
                    File.WriteAllText(LanguageSettingsFile, cultureName);
                }
                var culture = new CultureInfo(cultureName);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading/saving language file ({LanguageSettingsFile}): {ex.Message}", "Language Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // === КРОК 3: ВСТАНОВЛЕННЯ ТЕМИ ===
            string themeName = "Light";
            try
            {
                if (File.Exists(ThemeSettingsFile))
                {
                    string themeFromFile = File.ReadAllText(ThemeSettingsFile).Trim();
                    if (!string.IsNullOrEmpty(themeFromFile)) { themeName = themeFromFile; }
                }
                else
                {
                    File.WriteAllText(ThemeSettingsFile, themeName);
                }
                ThemeManager.ApplyTheme(themeName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading/saving theme file ({ThemeSettingsFile}): {ex.Message}", "Theme Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // === КРОК 4: ЗАПУСК АВТОРИЗАЦІЇ ===
            ApplicationConfiguration.Initialize();

            LoginForm loginForm = new LoginForm();

            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // Вхід успішний, AppContext.CurrentUser тепер заповнений
                Application.Run(new Form1());
            }
            // (Якщо не OK, програма просто завершить роботу)
        }
    }
}