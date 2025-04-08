using FibonacciFox.Avalonia.Markup.Models.Visual;

namespace FibonacciFox.Avalonia.Markup.Demo;


class Program
{
    static void Main(string[] args)
    {
        // 1. Создаём экземпляр пользовательского контрола
        var control = new DemoControl();

        // 2. Строим сериализуемое визуальное дерево
        VisualElement root = LogicalTreeBuilder.BuildVisualTree(control);

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