using System.Drawing;
using System.Windows.Forms;
using System.Linq;

public class SettingsControl : UserControl
{
    private GroupBox settingsBox;

    public SettingsControl()
    {
        settingsBox = new GroupBox
        {
            Text = "Налаштування",
            Dock = DockStyle.None,
            Width = 700,
            Height = 200,
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

        this.Load += (sender, e) => CenterControls();
        this.Resize += (sender, e) => CenterControls();
    }

    // Центрування головного GroupBox
    private void CenterControls()
    {
        settingsBox.Left = (this.ClientSize.Width - settingsBox.Width) / 2;
        settingsBox.Top = (this.ClientSize.Height - settingsBox.Height) / 2;
    }
}