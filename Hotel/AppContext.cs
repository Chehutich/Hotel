using Hotel.Models; // Додаємо, щоб знати, що таке Staff

namespace Hotel
{
    public static class AppContext
    {
        // Рядок підключення з правами root, завантажений з .env
        public static string MasterConnectionString { get; set; } = string.Empty;

        // Інформація про користувача, який увійшов
        public static Staff? CurrentUser { get; set; }
    }
}