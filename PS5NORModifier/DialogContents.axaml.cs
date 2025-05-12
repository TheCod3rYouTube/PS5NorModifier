using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DialogHostAvalonia;
using Button = Avalonia.Controls.Button;

namespace PS5NORModifier;

public partial class DialogContents : Avalonia.Controls.UserControl
{
    public DialogContents(DialogHost host, string message, string title, params string[] buttons)
    {
        InitializeComponent();
        Title.Content = title;
        Text.Text = message;
        for (var i = 0; i < buttons.Length; i++)
        {
            string name = buttons[i];
            Button button = new()
            {
                Content = name,
                Classes = { "DialogButton" },
                Padding = new(4, 2),
                CommandParameter = name
            };
            
            if (i == 0 && i == buttons.Length - 1)
                button.Margin = new(0, 2, 0, 0);
            else if (i == 0)
                button.Margin = new(0, 2, 2, 0);
            else if (i == buttons.Length - 1)
                button.Margin = new(2, 2, 0, 0);
            else
                button.Margin = new(2, 2, 2, 0);
            
            button.Click += (sender, e) => { host.CloseDialogCommand.Execute(name); };
            Buttons.Children.Add(button);
        }
    }
}