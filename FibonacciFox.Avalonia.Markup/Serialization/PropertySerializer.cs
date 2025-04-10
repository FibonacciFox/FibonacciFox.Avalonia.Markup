using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;
using FibonacciFox.Avalonia.Markup.Helpers;
using FibonacciFox.Avalonia.Markup.Models.Properties;
using FibonacciFox.Avalonia.Markup.Models.Visual;

namespace FibonacciFox.Avalonia.Markup.Serialization;

/// <summary>
/// Сериализует свойства Avalonia-элемента (styled, attached, direct и CLR) в визуальную модель <see cref="VisualElement"/>.
/// Используется при построении сериализуемого дерева AXAML.
/// </summary>
public static class PropertySerializer
{
    /// <summary>
    /// Сериализует все поддерживаемые свойства контрола и добавляет их в <see cref="VisualElement"/>.
    /// </summary>
    /// <param name="control">Контрол, из которого извлекаются свойства.</param>
    /// <param name="element">Сериализуемая модель визуального элемента, в которую будут добавлены свойства.</param>
    public static void SerializeProperties(Control control, VisualElement element)
    {
        var addedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 1. Styled-свойства
        var styled = AvaloniaPropertyRegistry.Instance.GetRegistered(control.GetType());
        foreach (var prop in styled)
        {
            // Content и Header обрабатываются отдельно
            if (IsKnownContentProperty(control, prop))
                continue;

            var model = StyledAvaloniaPropertyModel.From(prop, control);
            if (model != null)
            {
                element.StyledProperties.Add(model);
                addedNames.Add(prop.Name);
            }
        }

        // 2. Attached-свойства
        var attached = AvaloniaPropertyRegistry.Instance.GetRegisteredAttached(control.GetType());
        foreach (var prop in attached)
        {
            var model = AttachedAvaloniaPropertyModel.From(prop, control);
            if (model != null)
                element.AttachedProperties.Add(model);
        }

        // 3. Direct-свойства
        var direct = AvaloniaPropertyRegistry.Instance.GetRegisteredDirect(control.GetType());
        foreach (var prop in direct)
        {
            // Content и Header обрабатываются отдельно
            if (IsKnownContentProperty(control, prop))
                continue;

            var model = DirectAvaloniaPropertyModel.From(prop, control);
            if (model != null)
            {
                element.DirectProperties.Add(model);
                addedNames.Add(prop.Name);
            }
        }

        // 4. CLR-свойства
        var clr = control.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in clr)
        {
            // Исключаем свойства, которые уже покрыты AvaloniaProperty
            if (addedNames.Contains(prop.Name))
                continue;

            // Пропускаем Content/Header и свойства с атрибутом [Content]
            if (IsKnownContentProperty(control, prop))
                continue;

            var model = ClrAvaloniaPropertyModel.From(prop, control);
            if (model != null)
                element.ClrProperties.Add(model);
        }
    }

    /// <summary>
    /// Проверяет, является ли AvaloniaProperty свойством, обрабатываемым как контент (Content или Header).
    /// Также проверяет наличие атрибута [Content].
    /// </summary>
    private static bool IsKnownContentProperty(Control control, AvaloniaProperty property)
    {
        if (!control.IsSet(property) || property.IsReadOnly)
            return false;

        if (property.Name is "Content" or "Header")
            return true;

        return IsPropertyMarkedWithContent(control.GetType(), property.Name);
    }

    /// <summary>
    /// Проверяет, является ли CLR-свойство помеченным как [Content] или соответствует Content/Header.
    /// </summary>
    private static bool IsKnownContentProperty(Control control, PropertyInfo prop)
    {
        if (PropertySerializationHelper.IsRuntimeProperty(prop))
            return false;

        if (prop.Name is "Content" or "Header")
            return true;

        return IsPropertyMarkedWithContent(control.GetType(), prop.Name);
    }

    /// <summary>
    /// Проверяет, помечено ли свойство [Content]-атрибутом из Avalonia.Metadata.
    /// </summary>
    private static bool IsPropertyMarkedWithContent(Type controlType, string propertyName)
    {
        // Ищем только те свойства, которые:
        // - публичные
        // - экземплярные
        // - без параметров (то есть не индексаторы)
        var props = controlType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.Name == propertyName && p.GetIndexParameters().Length == 0);

        foreach (var prop in props)
        {
            if (prop.GetCustomAttribute<ContentAttribute>() is not null)
                return true;
        }

        return false;
    }

}
