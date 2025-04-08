using Avalonia.Controls;

namespace FibonacciFox.Avalonia.Markup.Demo;

public class DemoControl : UserControl
{
    public DemoControl()
    {
        var expander = new Expander
        {
            Header = new TextBlock { Text = "Text" },
            Content = new Grid
            {
                RowDefinitions = new RowDefinitions("*,*"),
                ColumnDefinitions = new ColumnDefinitions("150,*"),
                Children =
                {
                    new TextBlock { Text = "Search", [Grid.RowProperty] = 0, [Grid.ColumnProperty] = 0 },
                    new TextBox { Watermark = "Search text", Width = 200, [Grid.RowProperty] = 0, [Grid.ColumnProperty] = 1 },
                    new TextBlock { Text = "Case sensitive?", [Grid.RowProperty] = 1, [Grid.ColumnProperty] = 0 },
                    new CheckBox { [Grid.RowProperty] = 1, [Grid.ColumnProperty] = 1 },
                }
            }
        };

        Content = expander;
    }
}