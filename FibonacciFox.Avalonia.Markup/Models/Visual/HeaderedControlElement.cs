using FibonacciFox.Avalonia.Markup.Models.Visual.Interfaces;

namespace FibonacciFox.Avalonia.Markup.Models.Visual;

/// <summary>
/// Элемент управления, содержащий как Header, так и Content — например, Expander.
/// </summary>
public class HeaderedControlElement : ControlElement, IHeaderedElement, IContentElement
{
    /// <inheritdoc />
    public VisualElement? Header { get; set; }

    /// <inheritdoc />
    public VisualElement? Content { get; set; }

    /// <summary>
    /// Указывает, содержит ли элемент вложенные контролы (в Header или Content).
    /// </summary>
    public override bool IsContainsControl =>
        base.IsContainsControl || Header is ControlElement || Content is ControlElement;
}