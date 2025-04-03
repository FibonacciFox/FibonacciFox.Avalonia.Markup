using FibonacciFox.Avalonia.Markup.Helpers;
using FibonacciFox.Avalonia.Markup.Models.Properties;
using FibonacciFox.Avalonia.Markup.Models.Visual;
using FibonacciFox.Avalonia.Markup.Models.Visual.Interfaces;

namespace FibonacciFox.Avalonia.Markup.Demo;

/// <summary>
/// Печатает дерево элементов Avalonia в консоль для отладки.
/// </summary>
public static class TreePrinter
{
    /// <summary>
    /// Печатает дерево начиная с указанного элемента.
    /// </summary>
    public static void PrintVisualTree(VisualElement element, string indent = "", bool isLast = true)
    {
        string prefix = isLast ? "└" : "├";
        string childIndent = indent + (isLast ? "   " : "│  ");

        // 🎨 Цвета по ValueKind
        ConsoleColor GetKindColor(AvaloniaValueKind kind) => kind switch
        {
            AvaloniaValueKind.Control => ConsoleColor.Cyan,
            AvaloniaValueKind.Logical => ConsoleColor.DarkCyan,
            AvaloniaValueKind.StyledClasses => ConsoleColor.Magenta,
            AvaloniaValueKind.Complex => ConsoleColor.DarkYellow,
            AvaloniaValueKind.Brush => ConsoleColor.Green,
            AvaloniaValueKind.Template => ConsoleColor.DarkGray,
            AvaloniaValueKind.Binding => ConsoleColor.Blue,
            AvaloniaValueKind.Resource => ConsoleColor.DarkGreen,
            _ => ConsoleColor.Gray
        };

        // 🔷 Заголовок элемента
        Console.ForegroundColor = ConsoleColor.Cyan;
        string name = element is ControlElement ctrl && !string.IsNullOrWhiteSpace(ctrl.Name)
            ? ctrl.DisplayName
            : element.ElementType ?? "Unknown";

        Console.Write($"{indent}{prefix} Element: {name}");

        Console.ForegroundColor = GetKindColor(element.ValueKind);
        Console.Write($" (Kind: {element.ValueKind})");

        if (element.IsContainsControl)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" [HasChildControls]");
        }

        Console.ResetColor();
        Console.WriteLine();

        // 📥 Свойства
        void PrintProperty(AvaloniaPropertyModel prop, string category)
        {
            ConsoleColor catColor = category switch
            {
                "StyledProperty" => ConsoleColor.Yellow,
                "AttachedProperty" => ConsoleColor.Magenta,
                "DirectProperty" => ConsoleColor.Green,
                "ClrProperty" => ConsoleColor.Blue,
                _ => ConsoleColor.White
            };

            Console.ForegroundColor = catColor;
            Console.Write($"{childIndent} ■ {category}: ");
            Console.ResetColor();

            Console.Write($"{prop.Name} = {prop.Value} ");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("(Kind: ");
            Console.ForegroundColor = GetKindColor(prop.ValueKind);
            Console.Write($"{prop.ValueKind}");
            Console.ResetColor();

            Console.Write(", Xaml: ");
            Console.ForegroundColor = prop.CanBeSerializedToXaml ? ConsoleColor.Green : ConsoleColor.DarkRed;
            Console.Write(prop.CanBeSerializedToXaml ? "yes" : "no");

            if (prop.IsContainsControl)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(", ContainsControl: true");
            }

            if (prop.IsRuntimeOnly)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write(", RuntimeOnly: true");
            }

            Console.ResetColor();
            Console.WriteLine();

            // Вложенные сериализованные значения
            if (prop.SerializedValue is { } inner && prop.ValueKind != AvaloniaValueKind.Simple)
                PrintVisualTree(inner, childIndent, true);
        }

        foreach (var p in element.StyledProperties) PrintProperty(p, "StyledProperty");
        foreach (var p in element.AttachedProperties) PrintProperty(p, "AttachedProperty");
        foreach (var p in element.DirectProperties) PrintProperty(p, "DirectProperty");
        foreach (var p in element.ClrProperties) PrintProperty(p, "ClrProperty");

        // 👶 Вложенный Content
        if (element is IContentElement content && content.Content is { } contentValue)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{childIndent} ▸ Content:");
            Console.ResetColor();
            PrintVisualTree(contentValue, childIndent + "  ", true);
        }

        // 🏷 Header
        if (element is IHeaderedElement headered && headered.Header is { } headerValue)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{childIndent} ▸ Header:");
            Console.ResetColor();
            PrintVisualTree(headerValue, childIndent + "  ", true);
        }

        // 📋 Items
        if (element is IItemsElement items && items.Items.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{childIndent} ▸ Items:");
            Console.ResetColor();

            for (int i = 0; i < items.Items.Count; i++)
            {
                var item = items.Items[i];
                bool last = i == items.Items.Count - 1;
                PrintVisualTree(item, childIndent + "  ", last);
            }
        }

        // 👶 Обычные дети
        for (int i = 0; i < element.Children.Count; i++)
        {
            var child = element.Children[i];
            bool last = i == element.Children.Count - 1;
            PrintVisualTree(child, childIndent, last);
        }
    }
}
