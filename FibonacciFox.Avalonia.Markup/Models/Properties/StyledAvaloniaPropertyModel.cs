using Avalonia;
using Avalonia.Controls;

namespace FibonacciFox.Avalonia.Markup.Models.Properties;

/// <summary>
/// Модель для styled-свойства Avalonia, установленного через стили, темы или вручную.
/// </summary>
public class StyledAvaloniaPropertyModel : AvaloniaPropertyModel
{
    /// <summary>
    /// Создаёт экземпляр <see cref="StyledAvaloniaPropertyModel"/> из styled-свойства.
    /// </summary>
    /// <param name="property">Styled-свойство Avalonia.</param>
    /// <param name="control">Контрол, из которого извлекается значение.</param>
    /// <returns>Модель свойства или <c>null</c>, если не подходит для сериализации.</returns>
    public static StyledAvaloniaPropertyModel? From(AvaloniaProperty property, Control control)
    {
        // Исключаем Content из ContentControl (будет сериализовано как вложенный элемент)
        if ((control is ContentControl && property.Name == "Content") ||
            property.IsReadOnly ||
            !control.IsSet(property))
            return null;

        var value = control.GetValue(property);
        if (value == null)
            return null;

        var model = new StyledAvaloniaPropertyModel { Name = property.Name };
        model.SetRawValue(value);
        return model;
    }
}