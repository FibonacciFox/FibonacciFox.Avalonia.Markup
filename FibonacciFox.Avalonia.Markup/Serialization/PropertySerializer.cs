using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using FibonacciFox.Avalonia.Markup.Models.Properties;
using FibonacciFox.Avalonia.Markup.Models.Visual;

namespace FibonacciFox.Avalonia.Markup.Serialization;

/// <summary>
/// Сериализует свойства контрола (styled, attached, direct, CLR) в модель <see cref="VisualElement"/>.
/// Используется при построении сериализуемой структуры XAML.
/// </summary>
public static class PropertySerializer
{
    /// <summary>
    /// Сериализует все поддерживаемые свойства контрола и добавляет их в <see cref="VisualElement"/>.
    /// </summary>
    public static void SerializeProperties(Control control, VisualElement element)
    {
        var addedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Styled
        var styled = AvaloniaPropertyRegistry.Instance.GetRegistered(control.GetType());
        foreach (var prop in styled)
        {
            // Исключаем Content и Header, если они сериализуются отдельно как вложенные элементы
            if (IsKnownContentProperty(control, prop))
                continue;

            var node = StyledAvaloniaPropertyModel.From(prop, control);
            if (node != null)
            {
                element.StyledProperties.Add(node);
                addedNames.Add(prop.Name);
            }
        }

        // Attached
        var attached = AvaloniaPropertyRegistry.Instance.GetRegisteredAttached(control.GetType());
        foreach (var prop in attached)
        {
            var node = AttachedAvaloniaPropertyModel.From(prop, control);
            if (node != null)
            {
                element.AttachedProperties.Add(node);
            }
        }

        // Direct
        var direct = AvaloniaPropertyRegistry.Instance.GetRegisteredDirect(control.GetType());
        foreach (var prop in direct)
        {
            var node = DirectAvaloniaPropertyModel.From(prop, control);
            if (node != null)
            {
                element.DirectProperties.Add(node);
                addedNames.Add(prop.Name);
            }
        }

        // CLR
        var clr = control.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in clr)
        {
            if (addedNames.Contains(prop.Name))
                continue;

            var node = ClrAvaloniaPropertyModel.From(prop, control);
            if (node != null)
                element.ClrProperties.Add(node);
        }
    }

    /// <summary>
    /// Проверяет, является ли свойство Content или Header, которое должно быть сериализовано отдельно.
    /// </summary>
    private static bool IsKnownContentProperty(Control control, AvaloniaProperty property)
    {
        if (!control.IsSet(property) || property.IsReadOnly || property.Name is not ("Content" or "Header"))
            return false;

        var value = control.GetValue(property);
        return value is Control or ILogical;
    }
}
