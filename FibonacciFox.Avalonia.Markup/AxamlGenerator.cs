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
    /// <summary>
    /// Генерирует AXAML-документ по сериализованному визуальному дереву.
    /// </summary>
    /// <param name="root">Корневой элемент (например, UserControl).</param>
    /// <returns>AXAML как строка.</returns>
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
                     .Concat(root.ClrProperties)
                     .Concat(root.AttachedProperties.Where(p => !p.IsContainsControl)))
        {
            if (!prop.CanBeSerializedToXaml || string.IsNullOrWhiteSpace(prop.Value))
                continue;

            rootAttributes.Add($"{prop.Name}=\"{EscapeXml(prop.Value)}\"");
        }

        var rootAttrs = string.Join("\n             ", rootAttributes);
        sb.AppendLine($"<{rootTag} {rootAttrs}>");

        if (root is IContentElement contentRoot && contentRoot.Content is not null)
        {
            GenerateElement(contentRoot.Content, sb, "    ");
        }
        else
        {
            foreach (var child in GetChildren(root))
                GenerateElement(child, sb, "    ");
        }

        sb.AppendLine($"</{rootTag}>");
        return sb.ToString();
    }

    /// <summary>
    /// Рекурсивно генерирует AXAML-элемент по VisualElement.
    /// </summary>
    private static void GenerateElement(VisualElement element, StringBuilder sb, string indent)
    {
        var tag = element.ElementType ?? "Unknown";

        var attributes = new List<string>();
        foreach (var prop in element.StyledProperties
                     .Concat<AvaloniaPropertyModel>(element.DirectProperties)
                     .Concat(element.ClrProperties)
                     .Concat(element.AttachedProperties.Where(p => !p.IsContainsControl)))
        {
            if (!prop.CanBeSerializedToXaml || string.IsNullOrWhiteSpace(prop.Value))
                continue;

            attributes.Add($"{prop.Name}=\"{EscapeXml(prop.Value)}\"");
        }

        var attrs = string.Join(" ", attributes);
        var hasAttrs = !string.IsNullOrWhiteSpace(attrs);
        var children = GetChildren(element);

        // Content
        if (element is IContentElement contentElement && contentElement.Content is not null)
        {
            children.Insert(0, contentElement.Content);
        }

        // Header
        if (element is IHeaderedElement headered && headered.Header is not null)
        {
            var openTag = hasAttrs ? $"<{tag} {attrs}>" : $"<{tag}>";
            sb.AppendLine($"{indent}{openTag}");

            sb.AppendLine($"{indent}  <{tag}.Header>");
            GenerateElement(headered.Header, sb, indent + "    ");
            sb.AppendLine($"{indent}  </{tag}.Header>");

            // Вложенные attached свойства
            foreach (var prop in element.AttachedProperties)
            {
                if (prop.SerializedValue is ControlElement nested && prop.IsContainsControl)
                {
                    sb.AppendLine($"{indent}  <{prop.Name}>");
                    GenerateElement(nested, sb, indent + "    ");
                    sb.AppendLine($"{indent}  </{prop.Name}>");
                }
            }

            foreach (var child in children)
                GenerateElement(child, sb, indent + "  ");

            sb.AppendLine($"{indent}</{tag}>");
            return;
        }

        // Items
        if (element is IItemsElement itemsElement && itemsElement.Items.Count > 0)
        {
            var openTag = hasAttrs ? $"<{tag} {attrs}>" : $"<{tag}>";
            sb.AppendLine($"{indent}{openTag}");
            foreach (var item in itemsElement.Items)
                GenerateElement(item, sb, indent + "  ");
            sb.AppendLine($"{indent}</{tag}>");
            return;
        }

        // Элемент с дочерними и вложенными attached-свойствами
        if (children.Count > 0 || element.AttachedProperties.Any(p => p.IsContainsControl))
        {
            var openTag = hasAttrs ? $"<{tag} {attrs}>" : $"<{tag}>";
            sb.AppendLine($"{indent}{openTag}");

            foreach (var prop in element.AttachedProperties)
            {
                if (prop.SerializedValue is ControlElement nested && prop.IsContainsControl)
                {
                    sb.AppendLine($"{indent}  <{prop.Name}>");
                    GenerateElement(nested, sb, indent + "    ");
                    sb.AppendLine($"{indent}  </{prop.Name}>");
                }
            }

            foreach (var child in children)
                GenerateElement(child, sb, indent + "  ");

            sb.AppendLine($"{indent}</{tag}>");
        }
        else
        {
            var selfTag = hasAttrs ? $"<{tag} {attrs} />" : $"<{tag} />";
            sb.AppendLine($"{indent}{selfTag}");
        }
    }

    /// <summary>
    /// Возвращает список дочерних элементов.
    /// </summary>
    private static List<VisualElement> GetChildren(VisualElement element)
    {
        return element.Children.ToList();
    }

    /// <summary>
    /// Экранирует специальные символы XML.
    /// </summary>
    private static string EscapeXml(string input)
    {
        return input.Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    .Replace("\"", "&quot;");
    }
}
