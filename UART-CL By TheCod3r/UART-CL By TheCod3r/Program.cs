using UART_CL_By_TheCod3r.UARTMenu;
using UART_CL_By_TheCod3r.Utilities;

#region Reminders (remove before publishing)
// Add check inside sub menu to confirm that the selected .bin file is a valid PS5 dump
#endregion

// Set the name of the application
string appTitle = "UART-CL by TheCod3r";

var showMenu = false;

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
    return UARTMenuService.MainMenu(appTitle, regionMap);
}
#endregion