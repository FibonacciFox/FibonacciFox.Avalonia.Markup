using Avalonia;
using Avalonia.Controls;

namespace FibonacciFox.Avalonia.Markup.Models.Properties;

/// <summary>
/// Модель для attached-свойства Avalonia (например, <c>Grid.Row</c> или <c>DockPanel.Dock</c>).
/// </summary>
public class AttachedAvaloniaPropertyModel : AvaloniaPropertyModel
{
    /// <summary>
    /// Создаёт модель для attached-свойства, если оно установлено и сериализуемо.
    /// </summary>
    public static AttachedAvaloniaPropertyModel? From(AvaloniaProperty property, Control control)
    {
        if (property.Name == "NameScope" || property.IsReadOnly || !control.IsSet(property))
            return null;

        var value = control.GetValue(property);
        if (value == null)
            return null;

        var model = new AttachedAvaloniaPropertyModel
        {
            Name = $"{property.OwnerType.Name}.{property.Name}"
        };
        model.SetRawValue(value);
        return model;
    }
}