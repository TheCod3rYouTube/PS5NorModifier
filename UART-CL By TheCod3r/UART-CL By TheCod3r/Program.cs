namespace UART_CL_By_TheCod3r;

/// <summary>
/// Main entry point for the UART-CL application.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        // Instantiate and run the main console application class.
        var consoleApp = new CLI();
        consoleApp.Run();
    }
}
