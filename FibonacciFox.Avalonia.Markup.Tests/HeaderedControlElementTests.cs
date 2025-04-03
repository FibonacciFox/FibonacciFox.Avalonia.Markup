using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using FibonacciFox.Avalonia.Markup;
using FibonacciFox.Avalonia.Markup.Models.Visual;
using Xunit;

namespace FibonacciFox.Avalonia.Markup.Tests;

/// <summary>
/// Тесты для элементов с Header и Content (HeaderedControlElement).
/// </summary>
public class HeaderedControlElementTests
{
    [Fact]
    public void Expander_Should_Serialize_Header_And_Content()
    {
        // Arrange
        var expander = new Expander
        {
            Header = new TextBlock { Text = "Header" },
            Content = new TextBlock { Text = "Body" }
        };

        // Act
        var element = LogicalTreeBuilder.BuildVisualTree(expander);

        // Assert
        var headered = Assert.IsType<HeaderedControlElement>(element);

        Assert.NotNull(headered.Header);
        Assert.Equal("TextBlock", headered.Header?.ElementType);

        Assert.NotNull(headered.Content);
        Assert.Equal("TextBlock", headered.Content?.ElementType);
    }

    [Fact]
    public void TabItem_Should_Serialize_Header_And_Content()
    {
        // Arrange
        var tabItem = new TabItem
        {
            Header = "Tab A",
            Content = new TextBlock { Text = "Tab Body" }
        };

        // Act
        var element = LogicalTreeBuilder.BuildVisualTree(tabItem);

        // Assert
        var headered = Assert.IsType<HeaderedControlElement>(element);

        Assert.NotNull(headered.Header);
        Assert.Equal("String", headered.Header?.ElementType);

        Assert.NotNull(headered.Content);
        Assert.Equal("TextBlock", headered.Content?.ElementType);
    }
    
    [Fact]
    public void Expander_Axaml_Should_Include_Header_And_Content()
    {
        var userControl = new TestUserControl();
        // Arrange
        var expander = new Expander
        {
            Header = new TextBlock { Text = "Title" },
            Content = new TextBlock { Text = "Body" }
        };
        
        userControl.Content = expander;

        var tree = LogicalTreeBuilder.BuildVisualTree(userControl);

        // Act
        string axaml = AxamlGenerator.GenerateAxaml(tree);

        // Assert
        Assert.Contains("<Expander", axaml);
        Assert.Contains("<Expander.Header>", axaml);
        Assert.Contains("</Expander.Header>", axaml);
        Assert.Contains("Text=\"Title\"", axaml);
        Assert.Contains("Text=\"Body\"", axaml);
    }
}
