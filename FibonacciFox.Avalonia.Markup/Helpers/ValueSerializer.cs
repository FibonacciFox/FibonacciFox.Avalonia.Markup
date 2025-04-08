using System.Globalization;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Styling;

namespace FibonacciFox.Avalonia.Markup.Helpers;

/// <summary>
/// Отвечает за сериализацию значений в строковое представление.
/// </summary>
public static class ValueSerializer
{
    public static string SerializeValue(object? value)
    {
        if (value is null)
            return string.Empty;

        return value switch
        {
            string s => s,
            bool b => b.ToString().ToLowerInvariant(),
            double d => d.ToString("G", CultureInfo.InvariantCulture),
            float f => f.ToString("G", CultureInfo.InvariantCulture),
            decimal m => m.ToString("G", CultureInfo.InvariantCulture),
            Enum e => e.ToString(),
            AvaloniaList<string> list => string.Join(",", list),
            IBrush brush => brush.ToString() ?? "Unknown",
            IBinding binding => SerializeBinding(binding),
            IResourceProvider => "DynamicResource",
            ITemplate => "Template",
            _ when value.GetType().IsValueType => value.ToString() ?? string.Empty,
            _ => value.ToString() ?? string.Empty
        };
    }

    private static string SerializeBinding(IBinding binding) =>
        binding switch
        {
            Binding b => $"Binding Path={b.Path}, Mode={b.Mode}",
            _ => "Binding"
        };
}