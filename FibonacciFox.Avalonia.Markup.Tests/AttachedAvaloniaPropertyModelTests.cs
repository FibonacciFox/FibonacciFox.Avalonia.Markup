using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using FibonacciFox.Avalonia.Markup.Helpers;
using FibonacciFox.Avalonia.Markup.Models.Properties;

namespace FibonacciFox.Avalonia.Markup.Tests
{
    public class AttachedAvaloniaPropertyModelTests
    {
        [Fact]
        public void AttachedProperty_WithSimpleValue_GridRow()
        {
            var panel = new Grid();
            var child = new Button();
            Grid.SetRow(child, 2);
            panel.Children.Add(child);

            var prop = Grid.RowProperty;
            var model = AttachedAvaloniaPropertyModel.From(prop, child);

            Assert.NotNull(model);
            Assert.Equal("Grid.Row", model!.Name);
            Assert.Equal("2", model.Value);
            Assert.Equal(AvaloniaValueKind.Simple, model.ValueKind);
            Assert.True(model.CanBeSerializedToXaml);
            Assert.Null(model.SerializedValue);
        }

        [Fact]
        public void AttachedProperty_WithControlValue_ToolTipTip()
        {
            var rect = new Rectangle();
            var tipPanel = new StackPanel();
            
            tipPanel.Children.Add(new TextBlock { Text = "Tip" }); // ✅ добавляем контент

            ToolTip.SetTip(rect, tipPanel);

            var prop = ToolTip.TipProperty;
            var model = AttachedAvaloniaPropertyModel.From(prop, rect);

            Assert.NotNull(model);
            Assert.Equal("ToolTip.Tip", model!.Name);
            Assert.Equal("StackPanel", model.Value); // ToString() → "StackPanel"
            Assert.Equal(AvaloniaValueKind.Control, model.ValueKind);
            Assert.True(model.CanBeSerializedToXaml);
            Assert.NotNull(model.SerializedValue);
            Assert.Equal("StackPanel", model.SerializedValue!.ElementType);
            Assert.True(model.IsContainsControl); // ✅ теперь пройдёт
        }

        
        [Fact]
        public void AttachedProperty_WithEnumValue_DockPanelDock()
        {
            var panel = new DockPanel();
            var child = new Button();
            DockPanel.SetDock(child, Dock.Right);
            panel.Children.Add(child);

            var prop = DockPanel.DockProperty;
            var model = AttachedAvaloniaPropertyModel.From(prop, child);

            Assert.NotNull(model);
            Assert.Equal("DockPanel.Dock", model!.Name);
            Assert.Equal("Right", model.Value); // enum.ToString()
            Assert.Equal(AvaloniaValueKind.Simple, model.ValueKind);
            Assert.True(model.CanBeSerializedToXaml);
            Assert.Null(model.SerializedValue);
        }

        [Fact]
        public void AttachedProperty_WithEnumValue_ToolTipPlacement()
        {
            var control = new Button();
            ToolTip.SetPlacement(control, PlacementMode.Top);

            var prop = ToolTip.PlacementProperty;
            var model = AttachedAvaloniaPropertyModel.From(prop, control);

            Assert.NotNull(model);
            Assert.Equal("ToolTip.Placement", model!.Name);
            Assert.Equal("Top", model.Value); // enum.ToString()
            Assert.Equal(AvaloniaValueKind.Simple, model.ValueKind);
            Assert.True(model.CanBeSerializedToXaml);
            Assert.Null(model.SerializedValue);
        }
    }
}