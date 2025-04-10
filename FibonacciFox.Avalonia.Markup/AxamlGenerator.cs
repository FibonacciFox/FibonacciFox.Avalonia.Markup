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
    private static readonly XNamespace AvaloniaNs = "https://github.com/avaloniaui";
    private static readonly XNamespace XNs = "http://schemas.microsoft.com/winfx/2006/xaml";
    private static readonly XNamespace DNs = "http://schemas.microsoft.com/expression/blend/2008";
    private static readonly XNamespace McNs = "http://schemas.openxmlformats.org/markup-compatibility/2006";

    public static string GenerateAxaml(VisualElement root)
    {
        var rootElement = GenerateElement(root, isRoot: true);
        var document = new XDocument(rootElement);
        return document.ToString();
    }

    private static XElement GenerateElement(VisualElement element, bool isRoot = false)
    {
        if (string.IsNullOrWhiteSpace(element.ElementType))
            throw new ArgumentException("ElementType must not be null or empty for serialization");

        // Применяем Avalonia namespace
        var elementName = AvaloniaNs + element.ElementType;
        var xmlElement = new XElement(elementName);

        // 📌 Добавляем пространства имён и x:Class только в корневой элемент
        if (isRoot)
        {
            xmlElement.Add(new XAttribute("xmlns", AvaloniaNs));
            xmlElement.Add(new XAttribute(XNamespace.Xmlns + "x", XNs));
            xmlElement.Add(new XAttribute(XNamespace.Xmlns + "d", DNs));
            xmlElement.Add(new XAttribute(XNamespace.Xmlns + "mc", McNs));
            xmlElement.Add(new XAttribute(McNs + "Ignorable", "d"));
            // ✅ Design-time размеры добавлять из визуального дизайнера! Но пока оставлю так
            xmlElement.Add(new XAttribute(DNs + "DesignWidth", "800"));
            xmlElement.Add(new XAttribute(DNs + "DesignHeight", "450"));
            
            //x:Class добавлять из визуального дизайнера! Но пока оставлю так
            xmlElement.Add(new XAttribute(XNs + "Class", $"GeneratedNamespace.{element.ElementType}"));
        }


        // 🧱 Простейшие свойства
        foreach (var prop in element.GetAllProperties(includeAttached: false))
        {
            if (prop.CanBeSerializedToXaml && !string.IsNullOrWhiteSpace(prop.Value))
                xmlElement.SetAttributeValue(prop.Name, prop.Value);
        }

        // 🏷 Header
        if (element is IHeaderedElement headered && headered.Header is VisualElement header)
        {
            if (header.ValueKind == AvaloniaValueKind.Simple && header.OriginalInstance is string str)
            {
                xmlElement.SetAttributeValue("Header", str);
            }
            else
            {
                xmlElement.Add(new XElement(AvaloniaNs + $"{element.ElementType}.Header", GenerateElement(header)));
            }
        }

        // 📥 Content
        if (element is IContentElement contentHolder && contentHolder.Content is VisualElement content)
        {
            if (content.ElementType == null)
            {
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
                xmlElement.Add(new XElement(AvaloniaNs + attached.Name, GenerateElement(nested)));
            }
            else if (attached.CanBeSerializedToXaml && !string.IsNullOrWhiteSpace(attached.Value))
            {
                xmlElement.SetAttributeValue(attached.Name, attached.Value);
            }
        }

        // 👶 Children
        foreach (var child in element.Children)
        {
            xmlElement.Add(GenerateElement(child));
        }

        return xmlElement;
    }
}
