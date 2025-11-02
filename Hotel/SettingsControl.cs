using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Hotel
{
    public class SettingsControl : UserControl
    {
        private GroupBox settingsBox;
        private Font commonFont = new Font("Segoe UI", 11F);

        public SettingsControl()
        {
            settingsBox = new GroupBox
            {
                Text = "Налаштування",
                Dock = DockStyle.None,
                Width = 800, // ЗБІЛЬШЕНО
                Height = 250, // ЗБІЛЬШЕНО
                Font = new Font("Segoe UI", 12F, FontStyle.Bold), // ЗБІЛЬШЕНО
                Padding = new Padding(25)
            };

            var label = new Label
            {
                Text = "Цей розділ ще в розробці.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12F, FontStyle.Italic) // ЗБІЛЬШЕНО
            };

            settingsBox.Controls.Add(label);
            this.Controls.Add(settingsBox);

            this.Load += (sender, e) => CenterControls();
            this.Resize += (sender, e) => CenterControls();
        }

        private void CenterControls()
        {
            settingsBox.Left = (this.ClientSize.Width - settingsBox.Width) / 2;
            settingsBox.Top = (this.ClientSize.Height - settingsBox.Height) / 2;
        }
    }
}
