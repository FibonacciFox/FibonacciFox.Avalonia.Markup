namespace FibonacciFox.Avalonia.Markup.Models.Visual.Interfaces;

/// <summary>
/// Интерфейс для элемента, содержащего коллекцию элементов (например, ListBox, Menu).
/// </summary>
public interface IItemsElement
{
    /// <summary>
    /// Коллекция элементов, которая должна быть сериализована как дочерние теги.
    /// </summary>
    List<VisualElement> Items { get; }
}