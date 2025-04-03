using Avalonia;
using Avalonia.Controls;

namespace FibonacciFox.Avalonia.Markup.Models.Properties;

/// <summary>
/// Модель для direct-свойства Avalonia, установленного напрямую в контроле.
/// </summary>
public class DirectAvaloniaPropertyModel : AvaloniaPropertyModel
{
    private static readonly HashSet<string> ExcludedDirectProperties = new()
    {
        "Inlines", "SelectedItems", "Selection"
    };

    /// <summary>
    /// Создаёт модель для direct-свойства, если оно подходит для сериализации.
    /// </summary>
    public static DirectAvaloniaPropertyModel? From(AvaloniaProperty property, Control control)
    {
        if (ExcludedDirectProperties.Contains(property.Name))
            return null;

        if (property.IsReadOnly)
            return null;

        var value = control.GetValue(property);
        if (value == null)
            return null;

        var model = new DirectAvaloniaPropertyModel { Name = property.Name };
        model.SetRawValue(value);
        return model;
    }
}