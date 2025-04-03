using Avalonia.Controls;
using FibonacciFox.Avalonia.Markup.Helpers;
using FibonacciFox.Avalonia.Markup.Models.Visual;

namespace FibonacciFox.Avalonia.Markup.Models.Properties;

/// <summary>
/// Базовая модель для сериализуемого свойства Avalonia.
/// Содержит информацию о значении, его типе и возможности сериализации в AXAML.
/// </summary>
public abstract class AvaloniaPropertyModel
{
    /// <summary>
    /// Имя свойства (например, <c>"Content"</c>, <c>"Margin"</c>).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Строковое представление значения свойства (например, <c>"Red"</c>, <c>"0,10,0,10"</c>).
    /// Используется для сериализации в XAML.
    /// </summary>
    public string? Value { get; protected set; }

    /// <summary>
    /// Тип значения свойства (Control, Binding, Brush и т.п.).
    /// Определяется автоматически на основе объекта значения.
    /// </summary>
    public AvaloniaValueKind ValueKind { get; private set; } = AvaloniaValueKind.Unknown;

    /// <summary>
    /// Сериализованное представление, если значение — визуальный элемент или контейнер.
    /// </summary>
    public VisualElement? SerializedValue { get; private set; }

    /// <summary>
    /// Флаг, указывающий, что свойство является только для времени выполнения (например, <c>ActualWidth</c>).
    /// Такие свойства не сериализуются.
    /// </summary>
    public bool IsRuntimeOnly { get; set; }

    /// <summary>
    /// Возвращает <c>true</c>, если значение может быть сериализовано в AXAML.
    /// </summary>
    public bool CanBeSerializedToXaml { get; private set; }

    /// <summary>
    /// Возвращает <c>true</c>, если сериализованное значение содержит вложенные элементы управления.
    /// </summary>
    public virtual bool IsContainsControl =>
        SerializedValue is ControlElement el &&
        el.ElementType is not (nameof(Classes) or nameof(RowDefinitions) or nameof(ColumnDefinitions)) &&
        el.Children.Count > 0;

    /// <summary>
    /// Устанавливает значение свойства и автоматически вычисляет все метаданные:
    /// <see cref="Value"/>, <see cref="ValueKind"/>, <see cref="SerializedValue"/>, <see cref="CanBeSerializedToXaml"/>.
    /// </summary>
    /// <param name="value">Объект Avalonia-свойства, полученный из контрола.</param>
    /// <returns>Текущий экземпляр модели (для fluent-стиля вызовов).</returns>
    public AvaloniaPropertyModel SetRawValue(object value)
    {
        Value = PropertySerializationHelper.SerializeValue(value);
        ValueKind = PropertySerializationHelper.ResolveValueKind(value);
        SerializedValue = PropertySerializationHelper.TryBuildSerializedValue(value);
        CanBeSerializedToXaml = IsXamlCompatible(ValueKind);
        return this;
    }

    /// <summary>
    /// Проверяет, можно ли сериализовать значение указанного типа в XAML.
    /// </summary>
    /// <param name="kind">Категория значения.</param>
    /// <returns><c>true</c>, если значение допустимо для XAML.</returns>
    private static bool IsXamlCompatible(AvaloniaValueKind kind)
    {
        return kind switch
        {
            AvaloniaValueKind.Binding => false,
            AvaloniaValueKind.Template => false,
            AvaloniaValueKind.Resource => false,
            _ => true
        };
    }
}
