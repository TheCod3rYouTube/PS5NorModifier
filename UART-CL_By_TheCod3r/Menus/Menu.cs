namespace UART_CL;

public abstract class Menu
{
    public List<MenuItem> Items { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }

    /// <summary>
    /// Displays the menu, allowing the user to select its options until they choose to exit.
    /// </summary>
    /// <param name="asSubMenu">
    /// Whether the menu was opened from another menu.
    /// If true, the exit option will be "Return to previous menu" instead of "Exit application".
    /// </param>
    public void Open(bool asSubMenu = false)
    {
        int selection = 1;
        MenuItem exitItem = null!;
        bool exit = false;
        while (!exit)
        {
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine(Title);
                Console.WriteLine(new string('=', Title.Length));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(Description);
                Console.ResetColor();
                Console.WriteLine(new string('-', Description.Split('\n').Max(line => line.Length)));

                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].Write(i + 1, selection == i + 1);
                }

                exitItem = new()
                {
                    Name = asSubMenu ? "X. Return to previous menu" : "X. Exit application",
                    Color = ConsoleColor.Red,
                    Action = () => {
                        exit = true;
                        return false;
                    }
                };
                exitItem.Write(selection == Items.Count + 1);

                string selected = selection > Items.Count ? "X" : selection.ToString();
                Console.Write("\nEnter the number of your choice, or select with arrow keys: " + selected);
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                    break;

                if (key.Key is ConsoleKey.Escape or ConsoleKey.X)
                {
                    selection = Items.Count + 1;
                    continue;
                }

                if (key.Key == ConsoleKey.UpArrow)
                {
                    selection--;
                    if (selection < 1)
                        selection = Items.Count + 1;
                    continue;
                }

                if (key.Key == ConsoleKey.DownArrow)
                {
                    selection++;
                    if (selection > Items.Count + 1)
                        selection = 1;
                    continue;
                }

                if (int.TryParse(key.KeyChar.ToString(), out int result))
                {
                    if (result <= Items.Count && result > 0)
                    {
                        selection = result;
                    }
                }
            }

            Console.Clear();
            Console.WriteLine(Title);
            Console.WriteLine(new string('=', Title.Length));
            Console.WriteLine(Description);
            Console.WriteLine(new string('-', Description.Split('\n').Max(line => line.Length)));
            if (selection - 1 == Items.Count)
            {
                exitItem.Execute();
                continue;
            }
            Items[selection - 1].Execute();
        }
    }
}