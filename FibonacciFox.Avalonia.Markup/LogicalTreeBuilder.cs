using System.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using FibonacciFox.Avalonia.Markup.Helpers;
using FibonacciFox.Avalonia.Markup.Models.Visual;
using FibonacciFox.Avalonia.Markup.Serialization;

namespace FibonacciFox.Avalonia.Markup;

/// <summary>
/// Строит сериализуемое визуальное дерево из Avalonia-объектов (Control, ILogical, IEnumerable).
/// Используется для генерации AXAML и анализа визуальной структуры.
/// </summary>
public static class LogicalTreeBuilder
{
    /// <summary>
    /// Построить сериализуемое дерево из экземпляра <see cref="Control"/>.
    /// Автоматически определяет тип элемента (ContentControl, Headered и т.п.).
    /// </summary>
    /// <param name="control">Экземпляр контрола.</param>
    /// <returns>Корневой элемент визуального дерева.</returns>
    public static VisualElement BuildVisualTree(Control control)
    {
        // HeaderedContentControl → HeaderedControlElement
        if (control is HeaderedContentControl headered)
        {
            var element = new HeaderedControlElement
            {
                ElementType = headered.GetType().Name,
                OriginalInstance = headered,
                ValueKind = PropertySerializationHelper.ResolveValueKind(headered)
            };

            PropertySerializer.SerializeProperties(headered, element);

            if (headered.Header is { } header)
                element.Header = BuildVisualTreeFromObject(header);

            if (headered.Content is { } content)
                element.Content = BuildVisualTreeFromObject(content);

            return element;
        }

        // ContentControl → ContentControlElement
        if (control is ContentControl contentControl)
        {
            var element = new ContentControlElement
            {
                ElementType = contentControl.GetType().Name,
                OriginalInstance = contentControl,
                ValueKind = PropertySerializationHelper.ResolveValueKind(contentControl)
            };

            PropertySerializer.SerializeProperties(contentControl, element);

            if (contentControl.Content is { } content)
                element.Content = BuildVisualTreeFromObject(content);

            return element;
        }

        // ItemsControl → ItemsControlElement (если ItemsSource не задан)
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

        // Generic Control → ControlElement
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
    /// Построить сериализуемое дерево из любого объекта Avalonia:
    /// Control, ILogical или IEnumerable.
    /// </summary>
    /// <param name="value">Объект значения.</param>
    /// <returns>Соответствующий <see cref="VisualElement"/>.</returns>
    public static VisualElement BuildVisualTreeFromObject(object? value)
    {
        if (value is null)
        {
            return new ControlElement
            {
                ElementType = "Null",
                ValueKind = AvaloniaValueKind.Unknown
            };
        }

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
    /// Построить визуальное дерево из логического элемента (не Control).
    /// Используется для построения вложенного дерева из ILogical.
    /// </summary>
    /// <param name="logical">Экземпляр логического элемента.</param>
    /// <returns>Сериализуемый визуальный элемент.</returns>
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
}
