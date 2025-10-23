using System.Drawing;
using System.Windows.Forms;

public class SettingsControl : UserControl
{
    public SettingsControl()
    {
        var settingsBox = new GroupBox
        {
            Text = "Налаштування",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            Padding = new Padding(20)
        };

        var label = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12F, FontStyle.Italic)
        };

        settingsBox.Controls.Add(label);
        this.Controls.Add(settingsBox);
    }
}