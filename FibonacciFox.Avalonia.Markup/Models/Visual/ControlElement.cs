namespace FibonacciFox.Avalonia.Markup.Models.Visual;

/// <summary>
/// Представляет элемент, соответствующий конкретному Avalonia Control.
/// </summary>
public class ControlElement : VisualElement
{
    /// <summary>
    /// Имя контрола, если задано через свойство Name.
    /// </summary>
    public string? Name =>
        StyledProperties.FirstOrDefault(p => p.Name == "Name")?.Value
        ?? DirectProperties.FirstOrDefault(p => p.Name == "Name")?.Value
        ?? ClrProperties.FirstOrDefault(p => p.Name == "Name")?.Value;
    
    /// <summary>
    /// Отображаемое имя элемента для отладки: "Тип (Name: ...)" или просто "Тип".
    /// </summary>
    public string DisplayName =>
        string.IsNullOrWhiteSpace(Name)
            ? ElementType ?? "Unknown"
            : $"{ElementType} (Name: {Name})";
}