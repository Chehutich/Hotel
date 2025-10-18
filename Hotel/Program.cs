namespace Hotel
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DotNetEnv.Env.Load(".env");
            try {
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
                MessageBox.Show($"Connection String: {connectionString}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading environment variables: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}
