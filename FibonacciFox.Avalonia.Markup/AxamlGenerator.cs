using System.Xml.Linq;
using FibonacciFox.Avalonia.Markup.Helpers;
using FibonacciFox.Avalonia.Markup.Models.Visual;
using FibonacciFox.Avalonia.Markup.Models.Visual.Interfaces;

namespace FibonacciFox.Avalonia.Markup;

/// <summary>
/// Генерирует AXAML-документ из сериализованного визуального дерева Avalonia с помощью LINQ to XML.
/// </summary>
public static class AxamlGenerator
{
    public static string GenerateAxaml(VisualElement root)
    {
        var rootElement = GenerateElement(root);
        var document = new XDocument(rootElement);
        return document.ToString();
    }

    private static XElement GenerateElement(VisualElement element)
    {
        // ⚠ Если ElementType == null, это "обёртка", возвращаем только вложенные элементы
        if (element.ElementType == null)
        {
            var placeholder = new XElement("Placeholder"); // временно
            foreach (var child in element.Children)
                placeholder.Add(GenerateElement(child));

            return placeholder;
        }

        string tag = element.ElementType;
        var xmlElement = new XElement(tag);

        if (tag == "UserControl")
        {
            xmlElement.Add(
                new XAttribute("xmlns", "https://github.com/avaloniaui"),
                new XAttribute(XNamespace.Xmlns + "x", "http://schemas.microsoft.com/winfx/2006/xaml"),
                new XAttribute(XName.Get("Class", "http://schemas.microsoft.com/winfx/2006/xaml"), "GeneratedNamespace.MyUserControl")
            );
        }

        // 🧱 Простейшие свойства — в атрибуты
        foreach (var prop in element.GetAllProperties(includeAttached: false))
        {
            if (prop.CanBeSerializedToXaml && !string.IsNullOrWhiteSpace(prop.Value))
                xmlElement.SetAttributeValue(prop.Name, prop.Value);
        }

        // 🏷 Header
        if (element is IHeaderedElement headered && headered.Header is VisualElement header)
        {
            xmlElement.Add(new XElement($"{tag}.Header", GenerateElement(header)));
        }

        // 📥 Content
        if (element is IContentElement contentHolder && contentHolder.Content is VisualElement content)
        {
            if (content.ElementType == null)
            {
                // Добавляем дочерние элементы напрямую (без <Controls>)
                foreach (var child in content.Children)
                    xmlElement.Add(GenerateElement(child));
            }
            else if (content.ValueKind == AvaloniaValueKind.Simple && content.OriginalInstance is string str)
            {
                xmlElement.SetAttributeValue("Content", str);
            }
            else
            {
                xmlElement.Add(GenerateElement(content));
            }
        }

        // 📋 Items
        if (element is IItemsElement items && items.Items.Count > 0)
        {
            foreach (var item in items.Items)
                xmlElement.Add(GenerateElement(item));
        }

        // 📎 Attached свойства
        foreach (var attached in element.AttachedProperties)
        {
            if (attached.IsContainsControl && attached.SerializedValue is VisualElement nested)
            {
                xmlElement.Add(new XElement(attached.Name, GenerateElement(nested)));
            }
            else if (attached.CanBeSerializedToXaml && !string.IsNullOrWhiteSpace(attached.Value))
            {
                xmlElement.SetAttributeValue(attached.Name, attached.Value);
            }
        }

        // 👶 Обычные дочерние элементы
        foreach (var child in element.Children)
        {
            xmlElement.Add(GenerateElement(child));
        }

        return xmlElement;
    }
}
