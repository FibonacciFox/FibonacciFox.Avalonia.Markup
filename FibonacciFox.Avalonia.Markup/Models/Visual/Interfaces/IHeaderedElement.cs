namespace FibonacciFox.Avalonia.Markup.Models.Visual.Interfaces;

/// <summary>
/// Интерфейс для элемента, содержащего заголовок (например, Expander, GroupBox).
/// </summary>
public interface IHeaderedElement
{
    /// <summary>
    /// Заголовок, который должен быть сериализован как вложенное свойство.
    /// </summary>
    VisualElement? Header { get; }
}