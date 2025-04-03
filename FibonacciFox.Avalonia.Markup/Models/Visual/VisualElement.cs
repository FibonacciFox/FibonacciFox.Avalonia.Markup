namespace FibonacciFox.Avalonia.Markup.Models.Visual;

using Helpers;
using Properties;

/// <summary>
/// Представляет абстрактный элемент визуальной структуры Avalonia (логическое дерево).
/// Может быть связан как с Control, так и с другими ILogical.
/// </summary>
public abstract class VisualElement
{
    /// <summary>
    /// Название типа элемента (например, "StackPanel", "TextBlock").
    /// </summary>
    public string? ElementType { get; set; }

    /// <summary>
    /// Оригинальный экземпляр, из которого построен элемент (Control, ILogical и т.д.).
    /// </summary>
    public object? OriginalInstance { get; set; }

    /// <summary>
    /// Тип содержимого элемента (Control, Logical, Complex и т.п.).
    /// </summary>
    public AvaloniaValueKind ValueKind { get; set; } = AvaloniaValueKind.Unknown;

    /// <summary>
    /// Список styled-свойств элемента.
    /// </summary>
    public List<StyledAvaloniaPropertyModel> StyledProperties { get; set; } = new();

    /// <summary>
    /// Список attached-свойств элемента.
    /// </summary>
    public List<AttachedAvaloniaPropertyModel> AttachedProperties { get; set; } = new();

    /// <summary>
    /// Список direct-свойств элемента.
    /// </summary>
    public List<DirectAvaloniaPropertyModel> DirectProperties { get; set; } = new();

    /// <summary>
    /// Список CLR-свойств элемента.
    /// </summary>
    public List<ClrAvaloniaPropertyModel> ClrProperties { get; set; } = new();

    /// <summary>
    /// Дочерние визуальные элементы.
    /// </summary>
    public List<VisualElement> Children { get; set; } = new();

    /// <summary>
    /// Возвращает true, если элемент содержит дочерние элементы управления (ControlElement).
    /// </summary>
    public virtual bool IsContainsControl =>
        Children.Any(c => c is ControlElement);

}