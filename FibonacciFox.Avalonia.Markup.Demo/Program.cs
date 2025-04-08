using FibonacciFox.Avalonia.Markup.Models.Visual;
using FibonacciFox.Avalonia.Markup.Serialization;

namespace FibonacciFox.Avalonia.Markup.Demo;

class Program
{
    static void Main(string[] args)
    {
        // 1. Создаём экземпляр пользовательского контрола
        var control = new DemoControl();
        
        var usercontrol1 = new UserControl1();
       
        // 2. Строим сериализуемое визуальное дерево
        VisualElement root = VisualTreeBuilder.Build(usercontrol1);

        // 3. Печатаем логическое дерево в консоль
        Console.WriteLine("=== Visual Tree ===\n");
        TreePrinter.PrintVisualTree(root);

        // 4. Генерируем AXAML
        Console.WriteLine("\n=== AXAML ===\n");
        string axaml = AxamlGenerator.GenerateAxaml(root);
        Console.WriteLine(axaml);
        Console.WriteLine("\n=== Завершено ===");
        
        
    }
}