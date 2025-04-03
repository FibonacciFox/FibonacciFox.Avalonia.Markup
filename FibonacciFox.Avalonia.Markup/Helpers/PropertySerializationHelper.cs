using System.Globalization;
using System.Reflection;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Styling;
using FibonacciFox.Avalonia.Markup.Models.Visual;

namespace FibonacciFox.Avalonia.Markup.Helpers;

/// <summary>
/// Вспомогательные методы для сериализации свойств Avalonia в XAML-представление.
/// Включает преобразование значений, определение типа (ValueKind) и логического дерева.
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
    /// Определяет, является ли свойство недопустимым для сериализации (только во время выполнения).
    /// </summary>
    public static bool IsRuntimeProperty(PropertyInfo prop) =>
        RuntimeOnlyProperties.Contains(prop.Name);

    /// <summary>
    /// Определяет, можно ли сериализовать CLR-свойство в XAML.
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

    /// <summary>
    /// Сериализует значение свойства в строковое представление для XAML.
    /// </summary>
    public static string SerializeValue(object value)
    {
        return value switch
        {
            string s => s,
            bool b => b.ToString().ToLowerInvariant(),
            double d => d.ToString("G", CultureInfo.InvariantCulture),
            float f => f.ToString("G", CultureInfo.InvariantCulture),
            decimal m => m.ToString("G", CultureInfo.InvariantCulture),
            Enum e => e.ToString(),
            AvaloniaList<string> list => string.Join(",", list),
            IBrush brush => brush.ToString() ?? "Unknown",
            IBinding binding => SerializeBinding(binding),
            IResourceProvider => "DynamicResource",
            ITemplate => "Template",
            ILogical logical => logical.GetType().Name,
            _ when value.GetType().IsValueType => value.ToString() ?? string.Empty,
            _ => value.ToString() ?? string.Empty
        };
    }

    /// <summary>
    /// Упрощённая сериализация привязки.
    /// </summary>
    private static string SerializeBinding(IBinding binding) =>
        binding switch
        {
            Binding b => $"Binding Path={b.Path}, Mode={b.Mode}",
            _ => "Binding"
        };

    /// <summary>
    /// Определяет тип значения Avalonia (ValueKind), например: Control, Brush, Binding и т.д.
    /// </summary>
    public static AvaloniaValueKind ResolveValueKind(object value)
    {
        return value switch
        {
            Control => AvaloniaValueKind.Control,
            ILogical logical => logical.GetLogicalChildren().Any()
                ? AvaloniaValueKind.Logical
                : AvaloniaValueKind.Simple,
            AvaloniaList<string> when value is Classes => AvaloniaValueKind.StyledClasses,
            AvaloniaList<string> => AvaloniaValueKind.Complex,
            IBinding => AvaloniaValueKind.Binding,
            ITemplate => AvaloniaValueKind.Template,
            IResourceProvider => AvaloniaValueKind.Resource,
            IBrush => AvaloniaValueKind.Brush,
            _ => value.GetType() is { } type &&
                 (type.IsPrimitive || type.IsEnum || value is string || type.IsValueType)
                ? AvaloniaValueKind.Simple
                : AvaloniaValueKind.Unknown
        };
    }

    /// <summary>
    /// Пытается построить сериализуемую визуальную структуру из значения.
    /// </summary>
    public static VisualElement? TryBuildSerializedValue(object value)
    {
        return value switch
        {
            Control or ILogical => LogicalTreeBuilder.BuildVisualTreeFromObject(value),
            System.Collections.IEnumerable when value is not AvaloniaList<string>
                                                    and not Classes
                                                    and not IPseudoClasses
                => LogicalTreeBuilder.BuildVisualTreeFromObject(value),
            _ => null
        };
    }
}
