using System.Reflection;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;

namespace FibonacciFox.Avalonia.Markup.Helpers;

/// <summary>
/// Проверки и условия для сериализуемости CLR-свойств и AvaloniaProperty.
/// </summary>
public static class PropertySerializationHelper
{
    private static readonly HashSet<string> RuntimeOnlyProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "ActualWidth", "ActualHeight", "Bounds", "LayoutSlot",
        "IsFocused", "IsKeyboardFocusWithin", "IsPointerOver",
        "IsEffectivelyVisible", "IsInitialized", "TemplatedParent", "Parent"
    };

    /// <summary>
    /// Является ли свойство "только во время выполнения".
    /// </summary>
    public static bool IsRuntimeProperty(PropertyInfo prop) =>
        RuntimeOnlyProperties.Contains(prop.Name);

    /// <summary>
    /// Можно ли сериализовать свойство в XAML как CLR-свойство.
    /// </summary>
    public static bool IsXamlSerializableClrProperty(PropertyInfo prop, Control control)
    {
        if (IsRuntimeProperty(prop) || prop.GetIndexParameters().Length > 0)
            return false;

        var fieldName = prop.Name + "Property";
        var fieldInfo = control.GetType().GetField(fieldName,
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        if (fieldInfo != null && typeof(AvaloniaProperty).IsAssignableFrom(fieldInfo.FieldType))
            return false;

        if (prop.SetMethod?.IsPublic == true)
            return true;

        if (typeof(AvaloniaList<string>).IsAssignableFrom(prop.PropertyType))
            return true;

        return false;
    }
}