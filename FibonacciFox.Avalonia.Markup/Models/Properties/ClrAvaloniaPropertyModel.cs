using System.Reflection;
using Avalonia.Controls;
using FibonacciFox.Avalonia.Markup.Helpers;

namespace FibonacciFox.Avalonia.Markup.Models.Properties;

/// <summary>
/// Модель для обычного CLR-свойства контрола (не связанного с AvaloniaProperty).
/// </summary>
public class ClrAvaloniaPropertyModel : AvaloniaPropertyModel
{
    private static readonly HashSet<string> ExcludedProperties = new()
    {
        "DefiningGeometry", "RenderedGeometry", "Resources"
    };

    /// <summary>
    /// Создаёт модель для CLR-свойства, если оно пригодно для сериализации.
    /// </summary>
    public static ClrAvaloniaPropertyModel? From(PropertyInfo prop, Control control)
    {
        if (ExcludedProperties.Contains(prop.Name))
            return null;

        if (!PropertySerializationHelper.IsXamlSerializableClrProperty(prop, control))
            return null;

        try
        {
            var value = prop.GetValue(control);
            if (value == null)
                return null;

            var model = new ClrAvaloniaPropertyModel { Name = prop.Name };
            model.SetRawValue(value);
            return model;
        }
        catch
        {
            return null;
        }
    }
}