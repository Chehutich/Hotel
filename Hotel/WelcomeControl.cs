using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

public class WelcomeControl : UserControl
{
    private PictureBox pictureBox; 

    public WelcomeControl()
    {
        pictureBox = new PictureBox
        {
            Dock = DockStyle.None, 
            Size = new Size(1200, 1000), 
            SizeMode = PictureBoxSizeMode.Zoom 
        };

        const string YOUR_IMAGE_FILE_NAME = "hotel_image.jpg";

        // Завантаження зображення з вбудованих ресурсів
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
                    return; // Виходимо, щоб не додавати PictureBox
                }
            }
        }
        catch (Exception ex)
        {
            ShowErrorLabel(pictureBox, $"Помилка завантаження картинки: {ex.Message}");
            return; // Виходимо, щоб не додавати PictureBox
        }

        this.Controls.Add(pictureBox);

        // Додаємо обробники для центрування
        this.Load += (sender, e) => CenterPictureBox();
        this.Resize += (sender, e) => CenterPictureBox();
    }

    // Новий метод для центрування PictureBox
    private void CenterPictureBox()
    {
        pictureBox.Left = (this.ClientSize.Width - pictureBox.Width) / 2;
        pictureBox.Top = (this.ClientSize.Height - pictureBox.Height) / 2;
    }

    // Відображення помилки, якщо зображення не завантажилось
    private void ShowErrorLabel(PictureBox pb, string message)
    {
        pb?.Dispose(); // Безпечно видаляємо PictureBox, якщо він був створений
        var label = new Label
        {
            Text = message,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.Red
        };
        // Переконуємось, що мітка додається, навіть якщо PictureBox не додано
        if (!this.Controls.Contains(label))
        {
            this.Controls.Add(label);
        }
    }
}