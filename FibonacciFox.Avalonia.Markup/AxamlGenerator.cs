using System.Text;
using FibonacciFox.Avalonia.Markup.Helpers;
using FibonacciFox.Avalonia.Markup.Models.Properties;
using FibonacciFox.Avalonia.Markup.Models.Visual;
using FibonacciFox.Avalonia.Markup.Models.Visual.Interfaces;

namespace FibonacciFox.Avalonia.Markup;

/// <summary>
/// Генерирует AXAML-документ по сериализованному визуальному дереву Avalonia.
/// Поддерживает свойства, контент, заголовки, элементы коллекций и вложенные теги.
/// </summary>
public static class AxamlGenerator
{
    /// <summary>
    /// Генерирует AXAML-документ по корневому элементу.
    /// </summary>
    /// <param name="root">Корневой <see cref="VisualElement"/>, например, UserControl или Button.</param>
    /// <returns>AXAML-документ в виде строки.</returns>
    public static string GenerateAxaml(VisualElement root)
    {
        var sb = new StringBuilder();
        GenerateElement(root, sb, "");
        return sb.ToString();
    }

    /// <summary>
    /// Рекурсивно генерирует AXAML-представление для указанного элемента.
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

        // 🧩 Добавляем xmlns и x:Class только для корневого UserControl
        if (string.IsNullOrWhiteSpace(indent) && tag == "UserControl")
        {
            attributes.Insert(0, @"x:Class=""GeneratedNamespace.MyUserControl""");
            attributes.Insert(0, @"xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""");
            attributes.Insert(0, @"xmlns=""https://github.com/avaloniaui""");
        }

        var children = GetChildren(element);

        // 📥 Content как Content="..." если это простое значение
        if (element is IContentElement contentElement && contentElement.Content is ControlElement ce)
        {
            if (ce.ValueKind == AvaloniaValueKind.Simple &&
                ce.OriginalInstance is string str &&
                !string.IsNullOrWhiteSpace(str) &&
                !children.Any())
            {
                attributes.Add($"Content=\"{EscapeXml(str)}\"");
                ce = null!;
            }
            else
            {
                children.Insert(0, ce);
            }
        }

        // 🧠 Логика генерации <Tag.Header> только если Header — Control
        ControlElement? headerCe = null;
        bool shouldWriteHeaderAsElement =
            element is IHeaderedElement h &&
            h.Header is ControlElement controlHeader &&
            controlHeader.ValueKind == AvaloniaValueKind.Control &&
            !attributes.Any(attr => attr.StartsWith("Header=", StringComparison.OrdinalIgnoreCase)) &&
            (headerCe = controlHeader) != null;

        var hasAttrs = attributes.Count > 0;
        var openTag = hasAttrs ? $"<{tag} {string.Join(" ", attributes)}>" : $"<{tag}>";

        if (shouldWriteHeaderAsElement)
        {
            sb.AppendLine($"{indent}{openTag}");

            sb.AppendLine($"{indent}  <{tag}.Header>");
            GenerateElement(headerCe!, sb, indent + "    ");
            sb.AppendLine($"{indent}  </{tag}.Header>");

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

        // 📋 Items
        if (element is IItemsElement itemsElement && itemsElement.Items.Count > 0)
        {
            sb.AppendLine($"{indent}<{tag}{(hasAttrs ? " " + string.Join(" ", attributes) : "")}>");
            foreach (var item in itemsElement.Items)
                GenerateElement(item, sb, indent + "  ");
            sb.AppendLine($"{indent}</{tag}>");
            return;
        }

        // 👶 Дети или вложенные attached
        if (children.Count > 0 || element.AttachedProperties.Any(p => p.IsContainsControl))
        {
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
            var selfClosing = hasAttrs ? $"<{tag} {string.Join(" ", attributes)} />" : $"<{tag} />";
            sb.AppendLine($"{indent}{selfClosing}");
        }
    }

    /// <summary>
    /// Возвращает дочерние элементы VisualElement.
    /// </summary>
    private static List<VisualElement> GetChildren(VisualElement element) =>
        element.Children.ToList();

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
