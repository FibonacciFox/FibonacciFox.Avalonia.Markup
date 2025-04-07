using System.Collections;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using FibonacciFox.Avalonia.Markup.Helpers;
using FibonacciFox.Avalonia.Markup.Models.Visual;
using FibonacciFox.Avalonia.Markup.Serialization;

namespace FibonacciFox.Avalonia.Markup;

/// <summary>
/// Строит сериализуемое визуальное дерево из Avalonia-контролов.
/// </summary>
public static class LogicalTreeBuilder
{
    public static VisualElement BuildVisualTree(Control control)
    {
        // 🔹 Контрол с Header и Content
        if (control is ContentControl contentCtrl && HasHeaderProperty(control))
        {
            var element = new HeaderedControlElement
            {
                ElementType = control.GetType().Name,
                OriginalInstance = control,
                ValueKind = PropertySerializationHelper.ResolveValueKind(control)
            };

            PropertySerializer.SerializeProperties(control, element);

            var headerProp = control.GetType().GetProperty("Header");
            var headerValue = headerProp?.GetValue(control);
            if (headerValue is not null)
                element.Header = BuildVisualTreeFromObject(headerValue);

            if (contentCtrl.Content is { } content)
                element.Content = BuildVisualTreeFromObject(content);

            return element;
        }

        // 🔹 Контрол с Content
        if (control is ContentControl cc)
        {
            var element = new ContentControlElement
            {
                ElementType = cc.GetType().Name,
                OriginalInstance = cc,
                ValueKind = PropertySerializationHelper.ResolveValueKind(cc)
            };

            PropertySerializer.SerializeProperties(cc, element);

            if (cc.Content is { } content)
                element.Content = BuildVisualTreeFromObject(content);

            return element;
        }

        // 🔹 Контрол с Items
        if (control is ItemsControl itemsControl && itemsControl.ItemsSource is null)
        {
            var element = new ItemsControlElement
            {
                ElementType = itemsControl.GetType().Name,
                OriginalInstance = itemsControl,
                ValueKind = PropertySerializationHelper.ResolveValueKind(itemsControl)
            };

            PropertySerializer.SerializeProperties(itemsControl, element);

            foreach (var item in itemsControl.Items)
            {
                if (item is not null)
                    element.Items.Add(BuildVisualTreeFromObject(item));
            }

            return element;
        }

        // 🔹 Обычный Control
        var generic = new ControlElement
        {
            ElementType = control.GetType().Name,
            OriginalInstance = control,
            ValueKind = PropertySerializationHelper.ResolveValueKind(control)
        };

        PropertySerializer.SerializeProperties(control, generic);

        if (control is ILogical logical)
        {
            foreach (var child in logical.GetLogicalChildren().OfType<Control>())
            {
                generic.Children.Add(BuildVisualTree(child));
            }
        }

        return generic;
    }

    /// <summary>
    /// Построение дерева из произвольного объекта.
    /// </summary>
    public static VisualElement BuildVisualTreeFromObject(object value)
    {
        return value switch
        {
            Control control => BuildVisualTree(control),
            ILogical logical => BuildVisualTreeFromILogical(logical),

            IEnumerable enumerable when value is not string => new ControlElement
            {
                ElementType = value.GetType().Name,
                OriginalInstance = value,
                ValueKind = PropertySerializationHelper.ResolveValueKind(value),
                Children = enumerable.Cast<object>().Select(BuildVisualTreeFromObject).ToList()
            },

            _ => new ControlElement
            {
                ElementType = value.GetType().Name,
                OriginalInstance = value,
                ValueKind = PropertySerializationHelper.ResolveValueKind(value)
            }
        };
    }

    /// <summary>
    /// Строит дерево из ILogical.
    /// </summary>
    public static VisualElement BuildVisualTreeFromILogical(ILogical logical)
    {
        var element = new ControlElement
        {
            ElementType = logical.GetType().Name,
            OriginalInstance = logical,
            ValueKind = PropertySerializationHelper.ResolveValueKind(logical)
        };

        foreach (var child in logical.GetLogicalChildren())
        {
            element.Children.Add(BuildVisualTreeFromObject(child));
        }

        return element;
    }

    /// <summary>
    /// Проверяет, есть ли у контрола свойство Header.
    /// </summary>
    private static bool HasHeaderProperty(object control) =>
        control.GetType().GetProperty("Header", BindingFlags.Instance | BindingFlags.Public) is not null;
}
