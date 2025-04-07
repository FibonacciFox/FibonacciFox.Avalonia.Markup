using FibonacciFox.Avalonia.Markup.Helpers;
using FibonacciFox.Avalonia.Markup.Models.Properties;

namespace FibonacciFox.Avalonia.Markup.Models.Visual;

/// <summary>
/// Представляет абстрактный элемент визуальной структуры Avalonia (логическое дерево).
/// Может быть связан как с Control, так и с другими ILogical.
/// </summary>
public abstract class VisualElement
{
    public string? ElementType { get; set; }
    public object? OriginalInstance { get; set; }
    public AvaloniaValueKind ValueKind { get; set; } = AvaloniaValueKind.Unknown;

    public List<StyledAvaloniaPropertyModel> StyledProperties { get; set; } = new();
    public List<AttachedAvaloniaPropertyModel> AttachedProperties { get; set; } = new();
    public List<DirectAvaloniaPropertyModel> DirectProperties { get; set; } = new();
    public List<ClrAvaloniaPropertyModel> ClrProperties { get; set; } = new();
    public List<VisualElement> Children { get; set; } = new();

    public virtual bool IsContainsControl =>
        Children.Any(c => c is ControlElement);

    public IEnumerable<AvaloniaPropertyModel> GetAllProperties(bool includeAttached = true) =>
        StyledProperties.Cast<AvaloniaPropertyModel>()
            .Concat(DirectProperties)
            .Concat(ClrProperties)
            .Concat(includeAttached ? AttachedProperties : Enumerable.Empty<AttachedAvaloniaPropertyModel>());
}