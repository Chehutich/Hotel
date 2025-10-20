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
            SizeMode = PictureBoxSizeMode.Zoom
        };

        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("hotel_image.jpg"));

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    pictureBox.Image = Image.FromStream(stream);
                }
                else
                {
                    this.BackColor = Color.LightGray;
                }
            }
        }
        catch
        {
            this.BackColor = Color.Azure;
        }

        this.Controls.Add(pictureBox);
    }
}