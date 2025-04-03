using System.Drawing;
using Avalonia.Controls;
using FibonacciFox.Avalonia.Markup;
using Xunit;
using Rectangle = Avalonia.Controls.Shapes.Rectangle;

namespace FibonacciFox.Avalonia.Markup.Tests;

/// <summary>
/// Тесты сериализации визуальных элементов с вложенными AttachedProperty.
/// </summary>
public class AttachedPropertyVisualTests
{
    [Fact]
    public void TooltipTip_Should_Generate_Nested_Element()
    {
        
        // Arrange
        var rectangle = new Rectangle()
        {
            Width = 200,
            Height = 100
        };

        ToolTip.SetTip(rectangle , new StackPanel
        {
            Children =
            {
                new TextBlock { Text = "Hello" },
                new TextBlock { Text = "World" }
            }
        });

        var userControl = new UserControl { Content = rectangle };

        // Act
        var tree = LogicalTreeBuilder.BuildVisualTree(userControl);
        string axaml = AxamlGenerator.GenerateAxaml(tree); 

        // Assert
        Assert.Contains("<Rectangle", axaml);
        Assert.Contains("<ToolTip.Tip>", axaml);
        Assert.Contains("</ToolTip.Tip>", axaml);
        Assert.Contains("<StackPanel>", axaml);
        Assert.Contains("<TextBlock Text=\"Hello\"", axaml);
        Assert.Contains("<TextBlock Text=\"World\"", axaml);
    }

}