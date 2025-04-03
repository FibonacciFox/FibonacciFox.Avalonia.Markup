using FibonacciFox.Avalonia.Markup.Models.Visual.Interfaces;

namespace FibonacciFox.Avalonia.Markup.Models.Visual;

/// <summary>
/// Элемент управления, содержащий коллекцию Items (например, ListBox, ComboBox).
/// </summary>
public class ItemsControlElement : ControlElement, IItemsElement
{
    /// <summary>
    /// Коллекция элементов, которые были заданы через свойство Items.
    /// </summary>
    public List<VisualElement> Items { get; set; } = new();

    /// <summary>
    /// Указывает, что сериализуемое содержимое находится в Items, а не в обычных Children.
    /// </summary>
    public override bool IsContainsControl =>
        base.IsContainsControl || Items.Any(i => i is ControlElement);
}
