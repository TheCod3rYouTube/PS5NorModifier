namespace UART_CL;

public abstract class Menu
{
    public required List<MenuItem> Items { get; set; }
    public required string Title { get; set; }
}