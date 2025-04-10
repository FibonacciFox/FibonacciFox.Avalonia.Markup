using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using FibonacciFox.Avalonia.Markup.Models.Properties;

namespace FibonacciFox.Avalonia.Markup.Helpers;

/// <summary>
/// Фабрика для создания моделей свойств различных типов (Styled, Direct, Attached, CLR).
/// </summary>
public static class PropertyModelFactory
{
    /// <summary>
    /// Создаёт модель для styled- или attached-свойства Avalonia (с проверкой IsSet).
    /// </summary>
    public static TModel? CreateAvaloniaProperty<TModel>(AvaloniaProperty property, Control control)
        where TModel : AvaloniaPropertyModel, new()
    {
        if (property.IsReadOnly || !control.IsSet(property))
            return null;

        var value = control.GetValue(property);
        if (value == null)
            return null;

        return new TModel
        {
            Name = GetPropertyFullName<TModel>(property)
        }.SetRawValue<TModel>(value);
    }

    /// <summary>
    /// Создаёт модель для DirectProperty без проверки IsSet (например, для Name).
    /// </summary>
    public static TModel? CreateDirectProperty<TModel>(AvaloniaProperty property, Control control)
        where TModel : AvaloniaPropertyModel, new()
    {
        if (property.IsReadOnly)
            return null;

        var value = control.GetValue(property);
        if (value == null)
            return null;

        return new TModel
        {
            Name = GetPropertyFullName<TModel>(property)
        }.SetRawValue<TModel>(value);
    }

    /// <summary>
    /// Создаёт модель для CLR-свойства.
    /// </summary>
    public static ClrAvaloniaPropertyModel? CreateClrProperty(PropertyInfo prop, Control control)
    {
        if (!PropertySerializationHelper.IsXamlSerializableClrProperty(prop, control))
            return null;

        try
        {
            var value = prop.GetValue(control);
            if (value == null)
                return null;

            // Исключаем Classes, если там только псевдоклассы
            if (value is Classes classes && !classes.Any(c => !c.StartsWith(":")))
                return null;

            return new ClrAvaloniaPropertyModel
            {
                Name = prop.Name
            }.SetRawValue<ClrAvaloniaPropertyModel>(value);
        }
        catch
        {
            return null;
        }
    }
    
    /// <summary>
    /// Возвращает полное имя свойства (например, Grid.Row) для attached-свойств.
    /// </summary>
    private static string GetPropertyFullName<TModel>(AvaloniaProperty property)
    {
        return typeof(TModel) == typeof(AttachedAvaloniaPropertyModel)
            ? $"{property.OwnerType.Name}.{property.Name}"
            : property.Name;
    }
}
