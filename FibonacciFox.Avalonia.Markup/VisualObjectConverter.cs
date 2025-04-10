using System.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using FibonacciFox.Avalonia.Markup.Helpers;
using FibonacciFox.Avalonia.Markup.Models.Visual;

namespace FibonacciFox.Avalonia.Markup;

/// <summary>
/// Конвертирует объект Avalonia (Control, ILogical, IEnumerable) в сериализуемый VisualElement.
/// Используется при построении дерева для AXAML.
/// </summary>
public static class VisualObjectConverter
{
    /// <summary>
    /// Преобразует объект в сериализуемую модель VisualElement.
    /// </summary>
    /// <param name="value">Объект Avalonia (контрол, коллекция и т.п.).</param>
    /// <returns>VisualElement, отражающий структуру объекта.</returns>
    public static VisualElement Convert(object value)
    {
        return value switch
        {
            Control control => VisualTreeBuilder.Build(control),

            ILogical logical => ConvertFromILogical(logical),

            IEnumerable enumerable when value is not string => new ControlElement
            {
                ElementType = null, // Не задаём тег — для группировки дочерних элементов
                OriginalInstance = value,
                ValueKind = ValueClassifier.ResolveValueKind(value),
                Children = enumerable.Cast<object>().Select(Convert).ToList()
            },

            _ => new ControlElement
            {
                ElementType = value.GetType().Name,
                OriginalInstance = value,
                ValueKind = ValueClassifier.ResolveValueKind(value)
            }
        };
    }

    /// <summary>
    /// Обрабатывает объекты, реализующие ILogical, преобразуя их логических потомков.
    /// </summary>
    private static VisualElement ConvertFromILogical(ILogical logical)
    {
        var element = new ControlElement
        {
            ElementType = logical.GetType().Name,
            OriginalInstance = logical,
            ValueKind = ValueClassifier.ResolveValueKind(logical)
        };

        foreach (var child in logical.GetLogicalChildren())
        {
            element.Children.Add(Convert(child));
        }

        return element;
    }
}
