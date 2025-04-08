using System.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using FibonacciFox.Avalonia.Markup.Helpers;
using FibonacciFox.Avalonia.Markup.Models.Visual;

namespace FibonacciFox.Avalonia.Markup;

/// <summary>
/// Конвертирует любой объект Avalonia (Control, ILogical, IEnumerable) в VisualElement.
/// </summary>
public static class VisualObjectConverter
{
    public static VisualElement Convert(object value)
    {
        return value switch
        {
            Control control => VisualTreeBuilder.Build(control),

            ILogical logical => ConvertFromILogical(logical),

            IEnumerable enumerable when value is not string => new ControlElement
            {
                ElementType = value.GetType().Name,
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