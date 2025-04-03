namespace FibonacciFox.Avalonia.Markup.Helpers;

/// <summary>
/// Определяет, какого типа значение свойства Avalonia.
/// </summary>
public enum AvaloniaValueKind
{
    /// <summary>
    /// Простое значение: строка, число, логическое значение и т.п.
    /// </summary>
    Simple,

    /// <summary>
    /// Значение является Control и содержит вложенные элементы.
    /// </summary>
    Control,

    /// <summary>
    /// Значение реализует ILogical, но не является Control.
    /// </summary>
    Logical,

    /// <summary>
    /// Значение является коллекцией, списком или другим составным типом.
    /// </summary>
    Complex,

    /// <summary>
    /// Значение представляет собой привязку (например, Binding).
    /// </summary>
    Binding,

    /// <summary>
    /// Значение представляет собой шаблон (Template, ControlTemplate, DataTemplate).
    /// </summary>
    Template,

    /// <summary>
    /// Значение является ссылкой на ресурс (StaticResource, DynamicResource).
    /// </summary>
    Resource,
    
    /// <summary>
    /// Значение является кистью (Background, BorderBrush, Foreground, ...).
    /// </summary>
    Brush,
    
    /// <summary>
    /// Значение является классом стилизованного элемента.
    /// </summary>
    StyledClasses,


    /// <summary>
    /// Неопределённый или нестандартный тип.
    /// </summary>
    Unknown
}