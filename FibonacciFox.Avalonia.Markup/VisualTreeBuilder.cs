using System.Reflection;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using FibonacciFox.Avalonia.Markup.Models.Visual;
using FibonacciFox.Avalonia.Markup.Serialization;
using FibonacciFox.Avalonia.Markup.Helpers;
using Avalonia.Metadata;

namespace FibonacciFox.Avalonia.Markup;

/// <summary>
/// Строит сериализуемое визуальное дерево из Avalonia Control'ов.
/// </summary>
public static class VisualTreeBuilder
{
    public static VisualElement Build(Control control)
    {
        // Контрол с Header и Content
        if (control is ContentControl contentCtrl && HasHeaderProperty(control))
        {
            var element = new HeaderedControlElement
            {
                ElementType = control.GetType().Name,
                OriginalInstance = control,
                ValueKind = ValueClassifier.ResolveValueKind(control)
            };

            PropertySerializer.SerializeProperties(control, element);

            var headerValue = control.GetType().GetProperty("Header")?.GetValue(control);
            if (headerValue is not null)
                element.Header = VisualObjectConverter.Convert(headerValue);

            if (contentCtrl.Content is { } content)
                element.Content = VisualObjectConverter.Convert(content);

            return element;
        }

        // Контрол с Content
        if (control is ContentControl cc)
        {
            var element = new ContentControlElement
            {
                ElementType = cc.GetType().Name,
                OriginalInstance = cc,
                ValueKind = ValueClassifier.ResolveValueKind(cc)
            };

            PropertySerializer.SerializeProperties(cc, element);

            if (cc.Content is { } content)
                element.Content = VisualObjectConverter.Convert(content);

            return element;
        }

        // Контрол с Items
        if (control is ItemsControl itemsControl && itemsControl.ItemsSource is null)
        {
            var element = new ItemsControlElement
            {
                ElementType = itemsControl.GetType().Name,
                OriginalInstance = itemsControl,
                ValueKind = ValueClassifier.ResolveValueKind(itemsControl)
            };

            PropertySerializer.SerializeProperties(itemsControl, element);

            foreach (var item in itemsControl.Items)
            {
                if (item is not null)
                    element.Items.Add(VisualObjectConverter.Convert(item));
            }

            return element;
        }

        // Универсальный вариант — проверяем ContentAttribute
        var contentProperty = GetContentProperty(control);
        if (contentProperty is not null)
        {
            var element = new ContentControlElement
            {
                ElementType = control.GetType().Name,
                OriginalInstance = control,
                ValueKind = ValueClassifier.ResolveValueKind(control)
            };

            PropertySerializer.SerializeProperties(control, element);

            var contentValue = contentProperty.GetValue(control);
            if (contentValue is not null)
                element.Content = VisualObjectConverter.Convert(contentValue);

            return element;
        }

        // Базовый вариант
        var generic = new ControlElement
        {
            ElementType = control.GetType().Name,
            OriginalInstance = control,
            ValueKind = ValueClassifier.ResolveValueKind(control)
        };

        PropertySerializer.SerializeProperties(control, generic);

        if (control is ILogical logical)
        {
            foreach (var child in logical.GetLogicalChildren().OfType<Control>())
            {
                generic.Children.Add(Build(child));
            }
        }

        return generic;
    }

    private static bool HasHeaderProperty(object control) =>
        control.GetType().GetProperty("Header", BindingFlags.Instance | BindingFlags.Public) is not null;

    /// <summary>
    /// Возвращает свойство, помеченное как [Content], если оно есть.
    /// </summary>
    private static PropertyInfo? GetContentProperty(Control control)
    {
        return control.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(p =>
                p.GetCustomAttribute<ContentAttribute>() != null &&
                typeof(object).IsAssignableFrom(p.PropertyType));
    }
}
