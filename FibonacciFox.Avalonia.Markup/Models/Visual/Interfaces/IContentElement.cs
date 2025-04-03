namespace FibonacciFox.Avalonia.Markup.Models.Visual.Interfaces;

/// <summary>
/// Интерфейс для элемента, содержащего одно значение контента (например, ContentControl).
/// </summary>
public interface IContentElement
{
    /// <summary>
    /// Значение контента, которое должно сериализоваться как вложенный элемент.
    /// </summary>
    VisualElement? Content { get; }
}