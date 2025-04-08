using System.Reflection;
using Avalonia;
using Avalonia.Controls;
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
            
            var model = ClrAvaloniaPropertyModel.From(prop, control);
            if (model != null)
                element.ClrProperties.Add(model);
        }
    }

    /// <summary>
    /// Проверяет, является ли свойство Content или Header, которые должны быть сериализованы как вложенные элементы,
    /// а не как обычные свойства.
    /// </summary>
    /// <param name="control">Контрол, содержащий свойство.</param>
    /// <param name="property">Проверяемое свойство Avalonia.</param>
    /// <returns>True, если это Content или Header.</returns>
    private static bool IsKnownContentProperty(Control control, AvaloniaProperty property)
    {
        if (!control.IsSet(property) || property.IsReadOnly)
            return false;

        return property.Name is "Content" or "Header";
    }
}
