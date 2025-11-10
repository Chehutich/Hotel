using System.Globalization; // Додано
using System.Threading;     // Додано
using System.IO;            // Додано для роботи з файлом

namespace Hotel
{
    internal static class Program
    {
        // (НОВЕ) Назва нашого файлу налаштувань мови
        private const string LanguageSettingsFile = "language.cfg";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // === КРОК 1: ВСТАНОВЛЕННЯ ЗБЕРЕЖЕНОЇ МОВИ (з файлу) ===
            string cultureName = "uk-UA"; // Мова за замовчуванням

            try
            {
                // Перевіряємо, чи існує файл налаштувань
                if (File.Exists(LanguageSettingsFile))
                {
                    // Читаємо мову з файлу
                    string langFromFile = File.ReadAllText(LanguageSettingsFile).Trim();
                    if (!string.IsNullOrEmpty(langFromFile))
                    {
                        cultureName = langFromFile;
                    }
                }
                else
                {
                    // Якщо файлу немає, створюємо його з мовою за замовчуванням
                    File.WriteAllText(LanguageSettingsFile, cultureName);
                }

                // Створюємо "культуру" (мову) з коду "uk-UA" або "en-US"
                var culture = new CultureInfo(cultureName);

                // Встановлюємо цю культуру для всього потоку програми
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture; // Це змушує .NET використовувати .resx
            }
            catch (Exception ex)
            {
                // На випадок, якщо файл пошкоджено або немає прав на читання/запис
                MessageBox.Show($"Error loading/saving language file ({LanguageSettingsFile}): {ex.Message}", "Language Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // === КІНЕЦЬ КРОКУ 1 ===


            // === ВАШ ІСНУЮЧИЙ КОД ЗАВАНТАЖЕННЯ .env ===
            DotNetEnv.Env.Load(".env");
            try
            {
                var host = Environment.GetEnvironmentVariable("DB_HOST");
                var port = Environment.GetEnvironmentVariable("DB_PORT");
                var database = Environment.GetEnvironmentVariable("DB_NAME");
                var user = Environment.GetEnvironmentVariable("DB_USER");
                var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(database))
                {
                    throw new InvalidOperationException("Database connection parameters are not configured");
                }
                var connectionString = $"Server={host};Port={port};Database={database};User={user};Password={password};";

                // MessageBox.Show($"Connection String: {connectionString}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading environment variables: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // === КІНЕЦЬ ІСНУЮЧОГО КОДУ ===

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}