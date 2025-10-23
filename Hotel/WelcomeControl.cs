using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

public class WelcomeControl : UserControl
{
    public WelcomeControl()
    {
        var pictureBox = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.CenterImage
        };

        const string YOUR_IMAGE_FILE_NAME = "hotel_image.jpg";

        try
        {
            var assembly = Assembly.GetExecutingAssembly();

            // --- ОНОВЛЕНО: Додано "images." до шляху ---
            string resourceName = "Hotel.images." + YOUR_IMAGE_FILE_NAME;

            // (Альтернативний пошук)
            // string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(YOUR_IMAGE_FILE_NAME));

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    pictureBox.Image = Image.FromStream(stream);
                }
                else
                {
                    ShowErrorLabel(pictureBox, $"Ресурс не знайдено: {resourceName}");
                }
            }
        }
        catch (Exception ex)
        {
            ShowErrorLabel(pictureBox, $"Помилка завантаження картинки: {ex.Message}");
        }

        this.Controls.Add(pictureBox);
    }

    private void ShowErrorLabel(PictureBox pb, string message)
    {
        pb.Dispose();
        var label = new Label
        {
            Text = message,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.Red
        };
        this.Controls.Add(label);
    }
}