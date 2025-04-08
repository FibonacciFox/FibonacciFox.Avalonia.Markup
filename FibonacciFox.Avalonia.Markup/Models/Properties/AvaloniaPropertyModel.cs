using Avalonia.Controls;
using FibonacciFox.Avalonia.Markup.Helpers;
using FibonacciFox.Avalonia.Markup.Models.Visual;

namespace FibonacciFox.Avalonia.Markup.Models.Properties;

/// <summary>
/// Базовая модель для сериализуемого свойства Avalonia.
/// Хранит информацию о значении, его типе и правилах сериализации в AXAML.
/// Подклассы: <see cref="StyledAvaloniaPropertyModel"/>, <see cref="DirectAvaloniaPropertyModel"/>, <see cref="AttachedAvaloniaPropertyModel"/>, <see cref="ClrAvaloniaPropertyModel"/>.
/// </summary>
public abstract class AvaloniaPropertyModel
{
    /// <summary>
    /// Имя свойства (например, "Content", "Margin", "Grid.Row").
    /// Для attached-свойств содержит полное имя: "Grid.Row".
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Строковое значение свойства, пригодное для вставки как атрибут в XAML.
    /// Может быть null, если значение невозможно сериализовать как строку.
    /// </summary>
    public string? Value { get; protected set; }

    /// <summary>
    /// Категория значения: простое, привязка, визуальный элемент, кисть и т.п.
    /// Используется для выбора способа сериализации.
    /// </summary>
    public AvaloniaValueKind ValueKind { get; private set; } = AvaloniaValueKind.Unknown;

    /// <summary>
    /// Вложенная визуальная структура, если значение является элементом или коллекцией элементов.
    /// Например, значение типа Grid или RowDefinitions.
    /// </summary>
    public VisualElement? SerializedValue { get; private set; }

    /// <summary>
    /// Указывает, что свойство является служебным или runtime-только и не должно сериализоваться.
    /// Устанавливается вручную.
    /// </summary>
    public bool IsRuntimeOnly { get; set; }

    /// <summary>
    /// Указывает, можно ли сериализовать значение в XAML (например, не сериализуемы Binding, Resource, Template).
    /// </summary>
    public bool CanBeSerializedToXaml { get; private set; }

    /// <summary>
    /// Возвращает true, если SerializedValue содержит вложенные элементы управления.
    /// Используется при генерации вложенных тегов в AXAML (например, <Grid>...</Grid>).
    /// </summary>
    public virtual bool IsContainsControl =>
        SerializedValue is ControlElement el &&
        el.ElementType is not (nameof(Classes) or nameof(RowDefinitions) or nameof(ColumnDefinitions)) &&
        el.Children.Count > 0;

    /// <summary>
    /// Устанавливает значение свойства и вычисляет метаданные сериализации:
    /// строковое представление, категорию, вложенные элементы и пригодность к XAML.
    /// Возвращает текущую модель с типом <typeparamref name="TModel"/> для поддержки fluent-стиля.
    /// </summary>
    /// <typeparam name="TModel">Тип модели свойства (например, <see cref="StyledAvaloniaPropertyModel"/>).</typeparam>
    /// <param name="value">Новое значение свойства. Может быть null.</param>
    /// <returns>Текущая модель с указанным типом.</returns>
    public TModel SetRawValue<TModel>(object? value) where TModel : AvaloniaPropertyModel
    {
        if (value == null)
        {
            Value = null;
            ValueKind = AvaloniaValueKind.Unknown;
            SerializedValue = null;
            CanBeSerializedToXaml = false;
            return (TModel)this;
        }

        Value = ValueSerializer.SerializeValue(value);
        ValueKind = ValueClassifier.ResolveValueKind(value);
        SerializedValue = VisualValueFactory.TryBuildSerializedValue(value);
        CanBeSerializedToXaml = ValueClassifier.IsXamlCompatible(ValueKind);
        return (TModel)this;
    }
}
