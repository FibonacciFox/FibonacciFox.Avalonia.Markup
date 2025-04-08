using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Styling;

namespace FibonacciFox.Avalonia.Markup.Helpers;

/// <summary>
/// Отвечает за определение типа значения Avalonia.
/// </summary>
public static class ValueClassifier
{
    public static AvaloniaValueKind ResolveValueKind(object? value)
    {
        if (value is null)
            return AvaloniaValueKind.Unknown;

        if (value is Control)
            return AvaloniaValueKind.Control;

        if (value is IBinding)
            return AvaloniaValueKind.Binding;

        if (value is ITemplate)
            return AvaloniaValueKind.Template;

        if (value is IResourceProvider)
            return AvaloniaValueKind.Resource;

        if (value is IBrush)
            return AvaloniaValueKind.Brush;

        if (value is AvaloniaList<string>)
            return value is Classes ? AvaloniaValueKind.StyledClasses : AvaloniaValueKind.Complex;

        if (value is ILogical logical)
            return logical.GetLogicalChildren().Any()
                ? AvaloniaValueKind.Logical
                : AvaloniaValueKind.Simple;

        if (value is string || value.GetType().IsPrimitive || value.GetType().IsEnum || value.GetType().IsValueType)
            return AvaloniaValueKind.Simple;

        if (value is System.Collections.IEnumerable)
            return AvaloniaValueKind.Complex;

        return AvaloniaValueKind.Unknown;
    }

    public static bool IsXamlCompatible(AvaloniaValueKind kind) => kind switch
    {
        AvaloniaValueKind.Binding => false,
        AvaloniaValueKind.Template => false,
        AvaloniaValueKind.Resource => false,
        _ => true
    };
}