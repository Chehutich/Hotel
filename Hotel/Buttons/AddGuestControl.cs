using System;
using System.Drawing;
using System.Windows.Forms;

public class AddGuestControl : UserControl
{
    public AddGuestControl()
    {
        // 1. Create the main container for the form elements.
        var guestBox = new GroupBox
        {
            Text = "Введіть дані гостя", // This is the title of the box.
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            Padding = new Padding(10) // Adds some space inside the box.
        };

        // 2. Create the layout panel as before.
        var layoutPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 6
        };

        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        // 3. Add the form controls to the layout panel.
        layoutPanel.Controls.Add(CreateLabel("Ім'я:"), 0, 0);
        layoutPanel.Controls.Add(new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5) }, 1, 0);

        layoutPanel.Controls.Add(CreateLabel("Прізвище:"), 0, 1);
        layoutPanel.Controls.Add(new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5) }, 1, 1);

        layoutPanel.Controls.Add(CreateLabel("Номер телефону:"), 0, 2);
        layoutPanel.Controls.Add(new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5) }, 1, 2);

        layoutPanel.Controls.Add(CreateLabel("Дата народження:"), 0, 3);
        layoutPanel.Controls.Add(new DateTimePicker { Dock = DockStyle.Fill, Margin = new Padding(5) }, 1, 3);

        layoutPanel.Controls.Add(CreateLabel("Серія паспорту:"), 0, 4);
        layoutPanel.Controls.Add(new TextBox { Dock = DockStyle.Fill, Margin = new Padding(5) }, 1, 4);

        var buttonPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill };
        buttonPanel.Controls.Add(new Button { Text = "Зберегти", Width = 100 });
        buttonPanel.Controls.Add(new Button { Text = "Скасувати", Width = 100 });
        layoutPanel.Controls.Add(buttonPanel, 1, 5);

        // 4. Place the layout panel *inside* the GroupBox.
        guestBox.Controls.Add(layoutPanel);

        // 5. Finally, add the GroupBox to the UserControl.
        this.Controls.Add(guestBox);
    }

    private Label CreateLabel(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
            Margin = new Padding(5)
        };
    }
}