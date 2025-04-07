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
    /// Имя свойства (например, "Content", "Margin", "Grid.Row").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Строковое значение свойства, предназначенное для XAML-сериализации.
    /// Может быть null, если значение невозможно сериализовать.
    /// </summary>
    public string? Value { get; protected set; }

    /// <summary>
    /// Категория значения (Simple, Control, Binding и т.д.).
    /// </summary>
    public AvaloniaValueKind ValueKind { get; private set; } = AvaloniaValueKind.Unknown;

    /// <summary>
    /// Сериализуемое дерево, если значение — визуальный или логический элемент.
    /// </summary>
    public VisualElement? SerializedValue { get; private set; }

    /// <summary>
    /// Указывает, что свойство является только для времени выполнения и не должно сериализоваться.
    /// </summary>
    public bool IsRuntimeOnly { get; set; }

    /// <summary>
    /// Указывает, можно ли сериализовать значение в XAML.
    /// </summary>
    public bool CanBeSerializedToXaml { get; private set; }

    /// <summary>
    /// Возвращает true, если SerializedValue содержит вложенные элементы управления.
    /// Используется при генерации вложенных тегов в AXAML.
    /// </summary>
    public virtual bool IsContainsControl =>
        SerializedValue is ControlElement el &&
        el.ElementType is not (nameof(Classes) or nameof(RowDefinitions) or nameof(ColumnDefinitions)) &&
        el.Children.Count > 0;

    /// <summary>
    /// Устанавливает значение свойства и автоматически определяет:
    /// - строковое представление,
    /// - категорию значения (ValueKind),
    /// - возможность сериализации,
    /// - вложенную структуру (если применимо).
    /// </summary>
    /// <param name="value">Значение свойства. Может быть null.</param>
    /// <returns>Текущий экземпляр модели (для fluent-стиля).</returns>
    public AvaloniaPropertyModel SetRawValue(object? value)
    {
        if (value == null)
        {
            Value = null;
            ValueKind = AvaloniaValueKind.Unknown;
            SerializedValue = null;
            CanBeSerializedToXaml = false;
            return this;
        }

        Value = PropertySerializationHelper.SerializeValue(value);
        ValueKind = PropertySerializationHelper.ResolveValueKind(value);
        SerializedValue = PropertySerializationHelper.TryBuildSerializedValue(value);
        CanBeSerializedToXaml = PropertySerializationHelper.IsXamlCompatible(ValueKind);
        return this;
    }
}
