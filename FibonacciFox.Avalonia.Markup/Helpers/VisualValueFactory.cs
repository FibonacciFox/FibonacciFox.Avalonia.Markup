using FibonacciFox.Avalonia.Markup.Models.Visual;

namespace FibonacciFox.Avalonia.Markup.Helpers;

/// <summary>
/// Пытается создать вложенное дерево значений, если свойство содержит Control или коллекции.
/// </summary>
public static class VisualValueFactory
{
    public static VisualElement? TryBuildSerializedValue(object? value)
    {
        if (value is null or string)
            return null;

        return VisualObjectConverter.Convert(value);
    }

}