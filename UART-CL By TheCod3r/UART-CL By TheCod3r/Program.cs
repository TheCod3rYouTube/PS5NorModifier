using UART_CL_By_TheCod3r;

#region Reminders (remove before publishing)
// Add check inside sub menu to confirm that the selected .bin file is a valid PS5 dump
#endregion

// Set the name of the application
string appTitle = "UART-CL by TheCod3r";

bool showMenu = false;

// Set the application title
Console.Title = appTitle;

#region Check if error database exists
// Let's check and see if the database exists. If not, download it!
if (!System.IO.File.Exists("errorDB.xml"))
{
    PS5UARTUtilities.ShowHeader();
    Console.WriteLine("Downloading latest database file. Please wait...");

    bool success = PS5UARTUtilities.DownloadDatabase("http://uartcodes.com/xml.php", "errorDB.xml");

    if (success)
    {
        Console.WriteLine("Database downloaded successfully...");
        showMenu = true;
    }
    else
    {
        Console.WriteLine("Could not download the latest database file. Please ensure you're connected to the internet!");
        Environment.Exit(0);
    }
}
else
{
    // The database file exists. Continue to the main menu
    showMenu = true;
}
#endregion

#region Map suffixes to regions (for console variant)
// Define a dictionary to map suffixes to regions
var regionMap = new Dictionary<string, string>
{
    { "00A", "Japan" },
    { "00B", "Japan" },
    { "01A", "US, Canada, (North America)" },
    { "01B", "US, Canada, (North America)" },
    { "15A", "US, Canada, (North America)" },
    { "15B", "US, Canada, (North America)" },
    { "02A", "Australia / New Zealand, (Oceania)" },
    { "02B", "Australia / New Zealand, (Oceania)" },
    { "03A", "United Kingdom / Ireland" },
    { "03B", "United Kingdom / Ireland" },
    { "04A", "Europe / Middle East / Africa" },
    { "04B", "Europe / Middle East / Africa" },
    { "05A", "South Korea" },
    { "05B", "South Korea" },
    { "06A", "Southeast Asia / Hong Kong" },
    { "06B", "Southeast Asia / Hong Kong" },
    { "07A", "Taiwan" },
    { "07B", "Taiwan" },
    { "08A", "Russia, Ukraine, India, Central Asia" },
    { "08B", "Russia, Ukraine, India, Central Asia" },
    { "09A", "Mainland China" },
    { "09B", "Mainland China" },
    { "11A", "Mexico, Central America, South America" },
    { "11B", "Mexico, Central America, South America" },
    { "14A", "Mexico, Central America, South America" },
    { "14B", "Mexico, Central America, South America" },
    { "16A", "Europe / Middle East / Africa" },
    { "16B", "Europe / Middle East / Africa" },
    { "18A", "Singapore, Korea, Asia" },
    { "18B", "Singapore, Korea, Asia" }
};
#endregion

#region Display main menu
// Show the menu if showMenu is set to true
while (showMenu)
{
    showMenu = MainMenu(appTitle, regionMap);
}
#endregion

#region Main
static bool MainMenu(string appTitle, Dictionary<string, string> regionMap)
{

    Console.Clear();
    PS5UARTUtilities.ShowHeader();
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Choose an option:");
    Console.ResetColor();
    Console.WriteLine("1. Get error codes from PS5");
    Console.WriteLine("2. Clear error codes on PS5");
    Console.WriteLine("3. Enter custom UART command");
    Console.WriteLine("4. BIOS Dump Tools");
    Console.WriteLine("5. View readme guide");
    // Thanks for leaving this here!
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("6. Buy TheCod3r a coffee");
    Console.ResetColor();
    Console.WriteLine("7. Update error database");
    Console.WriteLine("X. Exit application");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write("\nEnter your choice: ");
    Console.ResetColor();

    #region Menu Options
    switch (Console.ReadLine())
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
            Console.WriteLine("Invalid choice. Please try again.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return true;
    }
    #endregion
}
#endregion