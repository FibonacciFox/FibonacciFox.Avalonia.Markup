using Avalonia.Controls;
using FibonacciFox.Avalonia.Markup;
using Xunit;

namespace FibonacciFox.Avalonia.Markup.Tests;

/// <summary>
/// Юнит-тесты для <see cref="AxamlGenerator"/> — проверяют корректность генерации AXAML.
/// Тестируются атрибуты, контент, attached-свойства и коллекции.
/// </summary>
public class AxamlGeneratorTests
{
    /// <summary>
    /// Проверяет, что простой Control (например, Button) может быть корневым элементом AXAML,
    /// и что его свойства сериализуются корректно, включая Content="Click".
    /// </summary>
    [Fact]
    public void GenerateAxaml_WithSimpleProperties_OutputsAttributes()
    {
        var control = new Button
        {
            Width = 100,
            Height = 50,
            Content = "Click"
        };

        var tree = LogicalTreeBuilder.BuildVisualTree(control);
        var axaml = AxamlGenerator.GenerateAxaml(tree);

        Assert.Contains("<Button", axaml);
        Assert.Contains("Width=\"100\"", axaml);
        Assert.Contains("Height=\"50\"", axaml);
        Assert.Contains("Content=\"Click\"", axaml);

        // Убедимся, что строка не попала как <String/> или вложенный текст
        Assert.DoesNotContain("<String", axaml);
        Assert.DoesNotContain(">Click<", axaml);

        // Убедимся, что тег закрывается корректно
        Assert.EndsWith("/>", axaml.Trim());
    }
    
    
    /// <summary>
    /// Проверяет генерацию простого TextBlock со свойствами.
    /// </summary>
    [Fact]
    public void GenerateAxaml_TextBlock_SerializesCorrectly()
    {
        var tb = new TextBlock
        {
            Text = "Hello",
            FontSize = 20
        };

        var tree = LogicalTreeBuilder.BuildVisualTree(tb);
        var axaml = AxamlGenerator.GenerateAxaml(tree);

        Assert.Contains("<TextBlock", axaml);
        Assert.Contains("Text=\"Hello\"", axaml);
        Assert.Contains("FontSize=\"20\"", axaml);
        Assert.EndsWith("/>", axaml.Trim());
    }
    
    
    /// <summary>
    /// Проверяет генерацию ListBox с элементами.
    /// </summary>
    [Fact]
    public void GenerateAxaml_ListBoxWithItems()
    {
        var listBox = new ListBox
        {
            Items =
            {
                new TextBlock { Text = "Item 1" },
                new TextBlock { Text = "Item 2" }
            }
        };

        var tree = LogicalTreeBuilder.BuildVisualTree(listBox);
        var axaml = AxamlGenerator.GenerateAxaml(tree);

        Assert.Contains("<ListBox", axaml);
        Assert.Contains("<TextBlock Text=\"Item 1\"", axaml);
        Assert.Contains("<TextBlock Text=\"Item 2\"", axaml);
        Assert.Contains("</ListBox>", axaml);
    }
    
    /// <summary>
    /// Проверяет генерацию StackPanel с несколькими детьми.
    /// </summary>
    [Fact]
    public void GenerateAxaml_StackPanelWithChildren()
    {
        var panel = new StackPanel
        {
            Children =
            {
                new Button { Content = "OK" },
                new Button { Content = "Cancel" }
            }
        };

        var tree = LogicalTreeBuilder.BuildVisualTree(panel);
        var axaml = AxamlGenerator.GenerateAxaml(tree);

        Assert.Contains("<StackPanel", axaml);
        Assert.Contains("<Button Content=\"OK\"", axaml);
        Assert.Contains("<Button Content=\"Cancel\"", axaml);
        Assert.Contains("</StackPanel>", axaml);
    }
    
    
    /// <summary>
    /// Проверяет генерацию Expander с Header и Content.
    /// </summary>
    [Fact]
    public void GenerateAxaml_ExpanderWithHeaderWitchTextblockAndContent()
    {
        var expander = new Expander
        {
            Header = new TextBlock { Text = "Title" },
            Content = new TextBlock { Text = "Content" }
        };

        var tree = LogicalTreeBuilder.BuildVisualTree(expander);
        var axaml = AxamlGenerator.GenerateAxaml(tree);

        Assert.Contains("<Expander", axaml);
        Assert.Contains("<Expander.Header>", axaml);
        Assert.Contains("<TextBlock Text=\"Title\"", axaml);
        Assert.Contains("</Expander.Header>", axaml);
        Assert.Contains("<TextBlock Text=\"Content\"", axaml);
        Assert.Contains("</Expander>", axaml);
    }

    /// <summary Expander.Header=".
    /// Content сериализуется корректно как вложенный элемент.">
    /// Проверяет, что Header = "Title" сериализуется как атрибут, а не как вложенный
    /// </summary>
    [Fact]
    public void GenerateAxaml_ExpanderWithHeaderAsStringAndContentAsControl()
    {
        var expander = new Expander
        {
            Header = "Title",
            Content = new TextBlock { Text = "Content" }
        };

        var tree = LogicalTreeBuilder.BuildVisualTree(expander);
        var axaml = AxamlGenerator.GenerateAxaml(tree);

        Assert.Contains("<Expander", axaml);
        Assert.Contains("Header=\"Title\"", axaml); // ✅ сериализуется как атрибут
        Assert.DoesNotContain("<Expander.Header>", axaml); //  не должно быть вложенного тега
        Assert.DoesNotContain("<String", axaml);           //  не должно быть мусора
        Assert.Contains("<TextBlock Text=\"Content\"", axaml);
        Assert.Contains("</Expander>", axaml);
    }
}