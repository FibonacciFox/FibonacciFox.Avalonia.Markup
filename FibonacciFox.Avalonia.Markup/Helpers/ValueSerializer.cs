using System.Globalization;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Styling;

namespace FibonacciFox.Avalonia.Markup.Helpers;

/// <summary>
/// Отвечает за преобразование значений свойств Avalonia в строковые представления,
/// пригодные для сериализации в AXAML.
/// </summary>
public static class ValueSerializer
{
    /// <summary>
    /// Преобразует значение свойства Avalonia в строку для использования в XAML.
    /// Поддерживаются простые типы, стили, кисти, привязки и шаблоны.
    /// </summary>
    /// <param name="value">Значение свойства.</param>
    /// <returns>Строковое представление значения или пустая строка, если значение не задано.</returns>
    public static string SerializeValue(object? value)
    {
        if (value is null)
            return string.Empty;

        return value switch
        {
            // Строки — напрямую
            string s => s,

            // Логические значения — true / false в нижнем регистре
            bool b => b.ToString().ToLowerInvariant(),

            // Числовые типы — через InvariantCulture
            double d => d.ToString("G", CultureInfo.InvariantCulture),
            float f => f.ToString("G", CultureInfo.InvariantCulture),
            decimal m => m.ToString("G", CultureInfo.InvariantCulture),

            // Перечисления — стандартное строковое представление
            Enum e => e.ToString(),

            // Классы — сериализуем как строку через запятую
            AvaloniaList<string> list => string.Join(",", list),

            // Кисти (Brush) — ToString() возвращает цвет или описание
            IBrush brush => brush.ToString() ?? "Unknown",

            // Привязки — упрощённая сериализация с указанием пути
            IBinding binding => SerializeBinding(binding),

            // Ссылки на ресурсы — сериализуются как "DynamicResource"
            IResourceProvider => "DynamicResource",

            // Шаблоны — метка "Template"
            ITemplate => "Template",

            // Прочие значимые типы значений — через ToString()
            _ when value.GetType().IsValueType => value.ToString() ?? string.Empty,

            // Остальные объекты — общее строковое представление
            _ => value.ToString() ?? string.Empty
        };
    }

    /// <summary>
    /// Сериализует Binding-объект в текстовое представление.
    /// </summary>
    private static string SerializeBinding(IBinding binding) =>
        binding switch
        {
            Binding b => $"Binding Path={b.Path}, Mode={b.Mode}",
            _ => "Binding"
        };
}
