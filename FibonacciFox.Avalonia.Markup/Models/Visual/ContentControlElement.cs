using FibonacciFox.Avalonia.Markup.Models.Visual.Interfaces;

namespace FibonacciFox.Avalonia.Markup.Models.Visual;

/// <summary>
/// Элемент управления, представляющий ContentControl с вложенным содержимым.
/// </summary>
public class ContentControlElement : ControlElement, IContentElement
{
    /// <inheritdoc />
    public VisualElement? Content { get; set; }

    /// <summary>
    /// Указывает, содержит ли элемент вложенные Controls.
    /// </summary>
    public override bool IsContainsControl =>
        base.IsContainsControl || Content is ControlElement;
}