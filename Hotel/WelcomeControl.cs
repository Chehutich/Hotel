using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Hotel
{
    public class WelcomeControl : UserControl
    {
        private PictureBox pictureBox;

        public WelcomeControl()
        {
            pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.CenterImage
            };

            const string YOUR_IMAGE_FILE_NAME = "hotel_image.jpg";

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = "Hotel.images." + YOUR_IMAGE_FILE_NAME;
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        pictureBox.Image = Image.FromStream(stream);
                    }
                    else
                    {
                        ShowErrorLabel(pictureBox, $"Ресурс не знайдено: {resourceName}");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorLabel(pictureBox, $"Помилка завантаження картинки: {ex.Message}");
                return;
            }

            this.Controls.Add(pictureBox);

            // Ми не центруємо цей контроль, він завжди заповнює екран
        }

        private void ShowErrorLabel(PictureBox pb, string message)
        {
            pb?.Dispose();
            var label = new Label
            {
                Text = message,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 12F) // ЗБІЛЬШЕНО
            };
            if (!this.Controls.Contains(label))
            {
                this.Controls.Add(label);
            }
        }
    }
}
