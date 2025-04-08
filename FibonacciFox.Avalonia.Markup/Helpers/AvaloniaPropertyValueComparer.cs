using System.Reflection;
using Avalonia;
using Avalonia.Controls;

namespace FibonacciFox.Avalonia.Markup.Helpers;

/// <summary>
/// Сравнивает значения свойств Avalonia с дефолтными или значениями базового типа.
/// Используется для определения необходимости сериализации.
/// </summary>
public static class AvaloniaPropertyValueComparer
{
    private static readonly Dictionary<Type, Control> BaseControlCache = new();

    /// <summary>
    /// Сравнивает styled-свойство с его значением по умолчанию из metadata.
    /// </summary>
    public static bool IsStyledValueRedundant(AvaloniaProperty property, Control control)
    {
        try
        {
            var metadata = property.GetMetadata(control.GetType()) as IStyledPropertyMetadata;
            var defaultValue = metadata?.DefaultValue;
            var actualValue = control.GetValue(property);

            return IsRedundant(actualValue, defaultValue);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Сравнивает значение любого AvaloniaProperty с тем же свойством на базовом типе.
    /// </summary>
    public static bool IsValueSameAsBase(AvaloniaProperty property, Control control)
    {
        try
        {
            var baseType = GetBaseAvaloniaType(control.GetType());
            var baseInstance = GetOrCreateBaseInstance(baseType);

            var baseValue = baseInstance.GetValue(property);
            var currentValue = control.GetValue(property);

            return IsRedundant(currentValue, baseValue);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Сравнивает CLR-свойство с тем же значением на базовом типе.
    /// </summary>
    public static bool IsClrValueSameAsBase(PropertyInfo prop, Control control)
    {
        try
        {
            var baseType = GetBaseAvaloniaType(control.GetType());
            var baseInstance = GetOrCreateBaseInstance(baseType);

            var baseValue = prop.GetValue(baseInstance);
            var currentValue = prop.GetValue(control);

            return IsRedundant(currentValue, baseValue);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Определяет, эквивалентны ли два значения — через Equals или ToString().
    /// </summary>
    public static bool IsRedundant(object? value, object? defaultValue)
    {
        if (value == null && defaultValue == null)
            return true;

        if (Equals(value, defaultValue))
            return true;

        return string.Equals(value?.ToString(), defaultValue?.ToString(), StringComparison.Ordinal);
    }

    /// <summary>
    /// Возвращает базовый тип Avalonia (например, UserControl) для пользовательского типа.
    /// </summary>
    public static Type GetBaseAvaloniaType(Type type)
    {
        while (type.BaseType != null)
        {
            if (type.BaseType.Namespace?.StartsWith("Avalonia") == true)
                return type.BaseType;
            type = type.BaseType;
        }

        return typeof(Control); // fallback
    }

    /// <summary>
    /// Создаёт или извлекает кэшированный базовый экземпляр типа.
    /// </summary>
    private static Control GetOrCreateBaseInstance(Type type)
    {
        if (BaseControlCache.TryGetValue(type, out var cached))
            return cached;

        var instance = (Control)Activator.CreateInstance(type)!;
        BaseControlCache[type] = instance;
        return instance;
    }
}
