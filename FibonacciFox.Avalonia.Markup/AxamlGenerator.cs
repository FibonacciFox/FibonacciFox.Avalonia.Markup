using System.Text;
using FibonacciFox.Avalonia.Markup.Models.Properties;
using FibonacciFox.Avalonia.Markup.Models.Visual;
using FibonacciFox.Avalonia.Markup.Models.Visual.Interfaces;

namespace FibonacciFox.Avalonia.Markup;

/// <summary>
/// Генерирует AXAML-документ на основе дерева VisualElement.
/// </summary>
public static class AxamlGenerator
{
    public static string GenerateAxaml(VisualElement root)
    {
        var sb = new StringBuilder();

        var rootTag = root.ElementType ?? "UserControl";

        // 🧩 Атрибуты корневого элемента
        var rootAttributes = new List<string>
        {
            @"xmlns=""https://github.com/avaloniaui""",
            @"xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""",
            @"x:Class=""GeneratedNamespace.MyUserControl"""
        };

        foreach (var prop in root.StyledProperties.Cast<AvaloniaPropertyModel>()
                     .Concat(root.DirectProperties)
                     .Concat(root.AttachedProperties)
                     .Concat(root.ClrProperties))
        {
            if (!prop.CanBeSerializedToXaml || string.IsNullOrWhiteSpace(prop.Value))
                continue;

            rootAttributes.Add($"{prop.Name}=\"{EscapeXml(prop.Value)}\"");
        }

        // 🧾 Открывающий тег корня
        sb.AppendLine($"<{rootTag} {string.Join("\n             ", rootAttributes)}>");

        // 📥 Контент или дети
        if (root is IContentElement contentRoot && contentRoot.Content is not null)
        {
            GenerateElement(contentRoot.Content, sb, "    ");
        }
        else
        {
            foreach (var child in GetChildren(root))
                GenerateElement(child, sb, "    ");
        }

        // 🧾 Закрывающий тег корня
        sb.AppendLine($"</{rootTag}>");
        return sb.ToString();
    }

    private static void GenerateElement(VisualElement element, StringBuilder sb, string indent)
    {
        var tag = element.ElementType ?? "Unknown";

        var attributes = new List<string>();
        foreach (var prop in element.StyledProperties.Cast<AvaloniaPropertyModel>()
                     .Concat(element.DirectProperties)
                     .Concat(element.AttachedProperties)
                     .Concat(element.ClrProperties))
        {
            if (!prop.CanBeSerializedToXaml || string.IsNullOrWhiteSpace(prop.Value))
                continue;

            attributes.Add($"{prop.Name}=\"{EscapeXml(prop.Value)}\"");
        }

        var attrs = string.Join(" ", attributes);
        var children = GetChildren(element);

        // Content
        if (element is IContentElement contentElement && contentElement.Content is not null)
        {
            children.Insert(0, contentElement.Content);
        }

        // Header
        if (element is IHeaderedElement headered && headered.Header is not null)
        {
            sb.AppendLine($"{indent}<{tag} {attrs}>");

            sb.AppendLine($"{indent}  <{tag}.Header>");
            GenerateElement(headered.Header, sb, indent + "    ");
            sb.AppendLine($"{indent}  </{tag}.Header>");

            foreach (var child in children)
                GenerateElement(child, sb, indent + "  ");

            sb.AppendLine($"{indent}</{tag}>");
            return;
        }

        // Items
        if (element is IItemsElement itemsElement && itemsElement.Items.Count > 0)
        {
            sb.AppendLine($"{indent}<{tag} {attrs}>");
            foreach (var item in itemsElement.Items)
                GenerateElement(item, sb, indent + "  ");
            sb.AppendLine($"{indent}</{tag}>");
            return;
        }

        // Default
        if (children.Count > 0)
        {
            sb.AppendLine($"{indent}<{tag} {attrs}>");
            foreach (var child in children)
                GenerateElement(child, sb, indent + "  ");
            sb.AppendLine($"{indent}</{tag}>");
        }
        else
        {
            sb.AppendLine($"{indent}<{tag} {attrs} />");
        }
    }

    private static List<VisualElement> GetChildren(VisualElement element)
    {
        return element.Children.ToList();
    }

    private static string EscapeXml(string input)
    {
        return input.Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    .Replace("\"", "&quot;");
    }
}
