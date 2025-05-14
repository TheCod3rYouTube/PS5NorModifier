namespace UART_CL;

public class MenuItem
{
    public required string Name { private get; set; }
    public ConsoleColor Color { private get; set; } = ConsoleColor.White;
    public required Func<bool> Action { private get; set; }

    public void Write(int index, bool selected)
    {
        if (selected)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            Console.ForegroundColor = Color;
        }
        Console.WriteLine($"{index}. {Name}");
        Console.ResetColor();
    }

    public void Write(bool selected)
    {
        if (selected)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            Console.ForegroundColor = Color;
        }
        Console.WriteLine(Name);
        Console.ResetColor();
    }

    
    public void Execute()
    {
        if (Action.Invoke())
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}