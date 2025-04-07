using Avalonia.Controls;
using Avalonia.Media;
using FibonacciFox.Avalonia.Markup.Helpers;
using FibonacciFox.Avalonia.Markup.Models.Properties;

namespace FibonacciFox.Avalonia.Markup.Tests
{
    public class AvaloniaPropertyModelTests
    {
        [Fact]
        public void SetRawValue_WithSimpleString_SerializesCorrectly()
        {
            var model = new StyledAvaloniaPropertyModel { Name = "Text" }
                .SetRawValue("Hello");

            Assert.Equal("Hello", model.Value);
            Assert.Equal(AvaloniaValueKind.Simple, model.ValueKind);
            Assert.True(model.CanBeSerializedToXaml);
            Assert.Null(model.SerializedValue);
        }

        [Fact]
        public void SetRawValue_WithBrush_SerializesAsBrush()
        {
            var model = new StyledAvaloniaPropertyModel { Name = "Background" }
                .SetRawValue(Brushes.Red);

            Assert.Equal("Red", model.Value);
            Assert.Equal(AvaloniaValueKind.Brush, model.ValueKind);
            Assert.True(model.CanBeSerializedToXaml);
        }
        
        [Fact]
        public void SetRawValue_WithControl_BuildsVisualElement()
        {
            var control = new Button { Content = "Click" };

            var model = new StyledAvaloniaPropertyModel { Name = "Content" }
                .SetRawValue(control);

            Assert.Equal("Button", model.Value);
            Assert.Equal(AvaloniaValueKind.Control, model.ValueKind);
            Assert.True(model.CanBeSerializedToXaml);
            Assert.NotNull(model.SerializedValue);
            Assert.Equal("Button", model.SerializedValue!.ElementType);
        }
    }
}
