using Avalonia.Controls;
using FibonacciFox.Avalonia.Markup.Models.Properties;
using Xunit;
using Avalonia.Collections;
using FibonacciFox.Avalonia.Markup.Helpers;

namespace FibonacciFox.Avalonia.Markup.Tests
{
    public class StyledClassesTests
    {
        [Fact]
        public void SetRawValue_WithNonEmptyClasses_SerializesCorrectly()
        {
            var classes = new Classes { "H1", "Bold", "Test" };

            var model = new ClrAvaloniaPropertyModel { Name = "Classes" }
                .SetRawValue(classes);

            Assert.Equal("H1,Bold,Test", model.Value);
            Assert.Equal(AvaloniaValueKind.StyledClasses, model.ValueKind);
            Assert.True(model.CanBeSerializedToXaml);
            Assert.Null(model.SerializedValue);
        }

        [Fact]
        public void SetRawValue_WithEmptyClasses_SerializesToEmptyString()
        {
            var classes = new Classes(); // пустой список

            var model = new ClrAvaloniaPropertyModel { Name = "Classes" }
                .SetRawValue(classes);

            Assert.Equal(string.Empty, model.Value);
            Assert.Equal(AvaloniaValueKind.StyledClasses, model.ValueKind);
            Assert.True(model.CanBeSerializedToXaml);
            Assert.Null(model.SerializedValue);
        }

        [Fact]
        public void SetRawValue_WithNullClasses_ProducesUnknownKindAndNoValue()
        {
            var model = new ClrAvaloniaPropertyModel { Name = "Classes" };

            model.SetRawValue(null!);

            Assert.Null(model.Value);
            Assert.Equal(AvaloniaValueKind.Unknown, model.ValueKind);
            Assert.False(model.CanBeSerializedToXaml);
            Assert.Null(model.SerializedValue);
        }
    }
}