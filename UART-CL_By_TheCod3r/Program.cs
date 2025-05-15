using System.Diagnostics;
using System.IO.Ports;
using System.Net;
using System.Management;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using PS5Lib;

#region Reminders (remove before publishing)
// Add check inside sub menu to confirm that the selected .bin file is a valid PS5 dump
#endregion

// Set the name of the application
namespace UART_CL;
internal class Program
{
    private const string AppTitle = "UART-CL by TheCod3r";
    public static void Main(string[] args)
    {
        Console.Title = AppTitle;
        ShowHeader();
        
        #region Check if error database exists
        if (!File.Exists(UART.DatabaseFileName))
        {
            Console.WriteLine("Downloading latest database file. Please wait...");

            try
            {
                UART.DownloadErrorDB().Wait();
                Console.WriteLine("Database downloaded successfully...");
            }
            catch
            {
                Console.WriteLine("Could not download the latest database file. Please ensure you're connected to the internet!");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
        #endregion

        new UARTMenu().Open();
    }

    private static void ShowHeader()
    {
        // This is the header.
        Console.Clear();
        Colorful.Console.WriteAscii("UART-CL v1.0.0.0");
        Colorful.Console.WriteAscii("by TheCod3r");
        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine("UART-CL is a command line UART tool to assist in the diagnosis and repair of PlayStation 5 consoles using UART.");
        Console.WriteLine("For more information on how to connect to UART you can use the options below or read the guide.");
        Console.WriteLine("");
    }
}