using System.Drawing;

namespace UART_CL_By_TheCod3r;

/// <summary>
/// Handles all console UI, menu display, and user interaction.
/// </summary>
public class CLI
{
    private const string AppTitle = "UART-CL by TheCod3r";
    private readonly ErrorCodeService _errorCodeService;
    private readonly UartService _uartService;
    private readonly BiosTools _biosTools;

    public CLI()
    {
        _errorCodeService = new ErrorCodeService();
        _uartService = new UartService();
        _biosTools = new BiosTools();
        Console.Title = AppTitle;
    }

    /// <summary>
    /// Runs the main application loop.
    /// </summary>
    public void Run()
    {
        // Ensure the error database exists before showing the main menu.
        if (!_errorCodeService.EnsureDatabaseExists())
        {
            // If download fails, exit the application.
            Console.WriteLine("Could not download the latest database file. Please ensure you're connected to the internet!");
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
            return;
        }

        bool showMenu = true;
        while (showMenu)
        {
            showMenu = DisplayMainMenu();
        }
    }

    /// <summary>
    /// Displays the main application header.
    /// </summary>
    private static void ShowHeader()
    {
        Console.Clear();
        Colorful.Console.WriteAscii("UART-CL v1.0.0.0", Color.CornflowerBlue);
        Colorful.Console.WriteAscii("by TheCod3r", Color.Gray);
        Console.WriteLine("\nUART-CL is a command line UART tool to assist in the diagnosis and repair of PlayStation 5 consoles.");
        Console.WriteLine("For more information, use the options below or read the ReadMe.\n");
    }

    /// <summary>
    /// Displays and handles the main menu options.
    /// </summary>
    private bool DisplayMainMenu()
    {
        ShowHeader();
        Console.WriteLine("Choose an option:", Color.Red);
        Console.WriteLine("1. Get error codes from PS5");
        Console.WriteLine("2. Clear error codes on PS5");
        Console.WriteLine("3. Enter custom UART command");
        Console.WriteLine("4. BIOS Dump Tools");
        Console.WriteLine("5. View readme guide");
        Console.WriteLine("6. Buy TheCod3r a coffee", Color.Green);
        Console.WriteLine("7. Update error database");
        Console.WriteLine("X. Exit application");
        Console.Write("\nEnter your choice: ", Color.Red);

        switch (Console.ReadLine()?.ToLower())
        {
            case "1":
                _uartService.GetErrorLogsFromPs5();
                break;
            case "2":
                _uartService.ClearErrorLogsOnPs5();
                break;
            case "3":
                _uartService.SendCustomCommandToPs5();
                break;
            case "4":
                DisplayBiosToolsMenu();
                break;
            case "5":
                DisplayReadme();
                break;
            case "6":
                BuyCoffee();
                break;
            case "7":
                _errorCodeService.UpdateDatabase();
                break;
            case "x":
                return false; // Exit the loop
            default:
                Console.WriteLine("\nInvalid choice. Please try again.");
                break;
        }

        if (ShouldContinue()) return true;

        return false;
    }

    /// <summary>
    /// Displays and handles the BIOS tools sub-menu.
    /// </summary>
    private void DisplayBiosToolsMenu()
    {
        bool subMenuRunning = true;
        while (subMenuRunning)
        {
            Console.Clear();
            ShowHeader();

            if (!string.IsNullOrEmpty(_biosTools.LoadedDumpPath))
            {
                Console.Title = $"{AppTitle} - Working with: {_biosTools.LoadedDumpPath}";
                Console.WriteLine($"Current File: {_biosTools.LoadedDumpPath}\n", Color.Aqua);
            }

            Console.WriteLine("In order to work with a BIOS dump, you must load it into memory first (Option 1).\n", Color.Yellow);
            Console.WriteLine("Choose an option:", Color.Red);
            Console.WriteLine("1. Load a BIOS dump (.bin)", Color.Green);
            Console.WriteLine("2. View BIOS information");
            Console.WriteLine("3. Convert to Digital edition");
            Console.WriteLine("4. Convert to Disc edition");
            Console.WriteLine("5. Convert to Slim edition");
            Console.WriteLine("6. Change console serial number");
            Console.WriteLine("7. Change motherboard serial number");
            Console.WriteLine("8. Change console model number");
            Console.WriteLine("X. Return to previous menu");
            Console.Write("\nEnter your choice: ");

            switch (Console.ReadLine()?.ToLower())
            {
                case "1":
                    _biosTools.LoadDumpFile();
                    break;
                case "2":
                    _biosTools.DisplayBiosInfo();
                    break;
                case "3":
                    _biosTools.ConvertEdition(BiosTools.ConsoleEdition.Digital);
                    break;
                case "4":
                    _biosTools.ConvertEdition(BiosTools.ConsoleEdition.Disc);
                    break;
                case "5":
                    _biosTools.ConvertEdition(BiosTools.ConsoleEdition.Slim);
                    break;
                case "6":
                    _biosTools.ChangeConsoleSerialNumber();
                    break;
                case "7":
                    _biosTools.ChangeMotherboardSerialNumber();
                    break;
                case "8":
                    _biosTools.ChangeConsoleModelNumber();
                    break;
                case "x":
                    subMenuRunning = false;
                    Console.Title = AppTitle; // Reset title
                    break;
                default:
                    Console.WriteLine("\nInvalid choice. Please try again.");
                    break;
            }

            if (subMenuRunning)
            {
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }
        }
    }

    private void DisplayReadme()
    {
        Console.Clear();
        ShowHeader();
        Console.WriteLine("UARL-CL is designed with simplicity in mind to obtain error codes from your PS5.");
        Console.WriteLine("UART (Universal Asynchronous Receiver-Transmitter) allows sending/receiving commands to any compatible serial device.");
        Console.WriteLine("\nThe PS5 has built-in UART, but its output can be cryptic. This tool helps decipher it by checking codes against a community-driven database.");
        Console.WriteLine("\nWhere does the database come from?");
        Console.WriteLine("The database is downloaded on first launch from uartcodes.com/xml.php. It is an offline XML file that can be updated any time.");
        Console.WriteLine("\nHow do I use this to fix my PS5?");
        Console.WriteLine("1. Get a 3.3v compatible UART device.");
        Console.WriteLine("2. Solder the device to the PS5: TX to RX, RX to TX, and GND to GND.");
        Console.WriteLine("3. Connect the PS5 power cord (do not turn the console on).");
        Console.WriteLine("4. Use this software to run commands and diagnose the issue.");
        Console.WriteLine("\nSpecial thanks to: Louis Rossmann, FightToRepair.org, Jessa Jones, Andy-Man, my YouTube viewers, Patreon supporters, and my mom!");
        Console.WriteLine("\n#FuckBwE - Be sure to use the hashtag. It really pisses him off!");
    }

    private void BuyCoffee()
    {
        Console.WriteLine("\nThanks for the support! Redirecting you to the donation page in your browser...");
        Utilities.System.OpenUrl("https://www.streamelements.com/thecod3r/tip");
    }

    private bool ShouldContinue()
    {
        Console.WriteLine("\nPress Enter to return to the main menu, or type 'exit' to quit.");
        return Console.ReadLine()?.ToLower() != "exit";
    }
}
