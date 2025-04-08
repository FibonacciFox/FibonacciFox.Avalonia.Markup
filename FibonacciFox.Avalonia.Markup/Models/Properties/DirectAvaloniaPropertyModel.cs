using Avalonia;
using Avalonia.Controls;
using FibonacciFox.Avalonia.Markup.Helpers;

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
        if (property.IsReadOnly || ExcludedDirectProperties.Contains(property.Name))
            return null;

        return PropertyModelFactory.CreateDirectProperty<DirectAvaloniaPropertyModel>(property, control);
    }

}