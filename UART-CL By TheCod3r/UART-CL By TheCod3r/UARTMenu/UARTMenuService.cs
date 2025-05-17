using UART_CL_By_TheCod3r.Utilities;

namespace UART_CL_By_TheCod3r.UARTMenu;

public class UARTMenuService
{
    public static bool MainMenu(string appTitle, Dictionary<string, string> regionMap,
        Func<string?>? readLine = null, Action<string>? writeLine = null)
    {
        readLine ??= Console.ReadLine;
        writeLine ??= Console.WriteLine;

        Console.Clear();
        PS5UARTUtilities.ShowHeader();
        Console.ForegroundColor = ConsoleColor.Red;
        writeLine("Choose an option:");
        Console.ResetColor();
        writeLine("1. Get error codes from PS5");
        writeLine("2. Clear error codes on PS5");
        writeLine("3. Enter custom UART command");
        writeLine("4. BIOS Dump Tools");
        writeLine("5. View readme guide");
        // Thanks for leaving this here!
        Console.ForegroundColor = ConsoleColor.Green;
        writeLine("6. Buy TheCod3r a coffee");
        Console.ResetColor();
        writeLine("7. Update error database");
        writeLine("X. Exit application");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("\nEnter your choice: ");
        Console.ResetColor();

        #region Menu Options
        switch (readLine())
        {
            #region Get Error Codes From PS5
            case "1":
                return UARTMenuHelper.GetErrorCodesFromPS5();
            #endregion
            #region Clear UART codes
            case "2":
                return UARTMenuHelper.ClearUARTCodes();
            #endregion
            #region Custom UART command
            case "3":
                return UARTMenuHelper.RunCustomUARTCommand();
            #endregion
            #region Launch readme
            case "5":
                return UARTMenuHelper.ShowReadMe();
            #endregion
            #region BIOS Dump Tools (Sub Menu)
            case "4":
                return UARTMenuHelper.LaunchBIOSDumpSubMenu(appTitle, regionMap);
            #endregion
            #region Buy me a coffee
            case "6":
                return UARTMenuHelper.BuyTheCod3erACoffee();
            #endregion
            #region Update XML database
            case "7":
                return UARTMenuHelper.UpdateXmlDatabase();
            #endregion
            #region Exit Application
            case "X":
                // Run the exit environment command to close the application
                Environment.Exit(0);
                return true;
            case "x":
                // Run the exit environment command to close the application
                Environment.Exit(0);
                return true;
            #endregion
            default:
                writeLine("Invalid choice. Please try again.");
                writeLine("Press Enter to continue...");
                readLine();
                return true;
        }
        #endregion
    }
}
