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
/// Предоставляет вспомогательные методы для определения типа значения (ValueKind),
/// сериализации значений в строки и построения вложенной визуальной структуры.
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
    /// Определяет, является ли CLR-свойство "только во время выполнения".
    /// </summary>
    public static bool IsRuntimeProperty(PropertyInfo prop) =>
        RuntimeOnlyProperties.Contains(prop.Name);

    /// <summary>
    /// Определяет, можно ли сериализовать CLR-свойство в XAML.
    /// Исключает AvaloniaProperty, индексы, readonly и без публичного сеттера.
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
    public static string SerializeValue(object? value)
    {
        if (value is null)
            return string.Empty;

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
    /// Упрощённая сериализация Binding.
    /// </summary>
    private static string SerializeBinding(IBinding binding) =>
        binding switch
        {
            Binding b => $"Binding Path={b.Path}, Mode={b.Mode}",
            _ => "Binding"
        };

    /// <summary>
    /// Определяет категорию значения: Control, Binding, Brush и т.п.
    /// </summary>
    public static AvaloniaValueKind ResolveValueKind(object? value)
    {
        if (value is null)
            return AvaloniaValueKind.Unknown;

        if (value is Control)
            return AvaloniaValueKind.Control;

        if (value is IBinding)
            return AvaloniaValueKind.Binding;

        if (value is ITemplate)
            return AvaloniaValueKind.Template;

        if (value is IResourceProvider)
            return AvaloniaValueKind.Resource;

        if (value is IBrush)
            return AvaloniaValueKind.Brush;

        if (value is AvaloniaList<string>)
            return value is Classes ? AvaloniaValueKind.StyledClasses : AvaloniaValueKind.Complex;

        if (value is ILogical logical)
            return logical.GetLogicalChildren().Any()
                ? AvaloniaValueKind.Logical
                : AvaloniaValueKind.Simple;

        if (value is string || value.GetType().IsPrimitive || value.GetType().IsEnum || value.GetType().IsValueType)
            return AvaloniaValueKind.Simple;

        if (value is System.Collections.IEnumerable)
            return AvaloniaValueKind.Complex;

        return AvaloniaValueKind.Unknown;
    }

    /// <summary>
    /// Пытается построить визуальную модель (VisualElement), если значение — Control или логическое дерево.
    /// Возвращает null, если не применимо.
    /// </summary>
    public static VisualElement? TryBuildSerializedValue(object? value)
    {
        if (value is null or string)
            return null;

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

    /// <summary>
    /// Определяет, можно ли сериализовать свойство с заданным ValueKind в XAML.
    /// </summary>
    public static bool IsXamlCompatible(AvaloniaValueKind kind) => kind switch
    {
        AvaloniaValueKind.Binding => false,
        AvaloniaValueKind.Template => false,
        AvaloniaValueKind.Resource => false,
        _ => true
    };
}
