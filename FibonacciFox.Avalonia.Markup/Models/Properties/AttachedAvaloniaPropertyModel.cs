using Avalonia;
using Avalonia.Controls;
using FibonacciFox.Avalonia.Markup.Helpers;

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
        if (property.Name == "NameScope" || property.IsReadOnly)
            return null;

        return PropertyModelFactory.CreateAvaloniaProperty<AttachedAvaloniaPropertyModel>(property, control);
    }

}