using System.Diagnostics;
using System.IO.Ports;
using System.Net;
using System.Management;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using PS5NORModifier;
using PS5Lib;

#region Reminders (remove before publishing)
// Add check inside sub menu to confirm that the selected .bin file is a valid PS5 dump
#endregion

// Set the name of the application
namespace UART_CL;
internal class Program
{
    private const string AppTitle = "UART-CL by TheCod3r";
    private static bool _showMenu = false;
    private static NORData? _norData = null;
    private static UART _uart = new();
    public static void Main(string[] args)
    {
        Console.Title = AppTitle;

        #region Check if error database exists
        
        if (!File.Exists(UART.DatabaseFileName))
        {
            ShowHeader();
            Console.WriteLine("Downloading latest database file. Please wait...");

            try
            {
                UART.DownloadErrorDB();
                Console.WriteLine("Database downloaded successfully...");
                _showMenu = true;
            }
            catch
            {
                Console.WriteLine("Could not download the latest database file. Please ensure you're connected to the internet!");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
        else
        {
            // The database file exists. Continue to the main menu
            _showMenu = true;
        }

        while (_showMenu)
        {
            _showMenu = MainMenu(AppTitle);
        }
        _uart.Disconnect();
    }

    private static bool MainMenu(string appTitle)
    {
        Console.Clear();
        ShowHeader();
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
        Console.WriteLine(_uart.IsConnected
            ? "8. Disconnect from COM port"
            : "8. Connect to COM port");
        Console.WriteLine("X. Exit application");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("\nEnter your choice: ");
        Console.ResetColor();

        #region Menu Options
        switch (Console.ReadLine())
        {
            #region Get Error Codes From PS5
            case "1":
                try
                {
                    if (!_uart.IsConnected)
                    {
                        Console.WriteLine("You must connect to a UART device before proceeding. Please select a device and try again.");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        return true;
                    }

                    // When grabbing error codes, we want to grab the first 11 errors from the system. Let's create a loop
                    for (var i = 0; i <= 10; i++)
                    {
                        var result = _uart.GetErrorLog(i).Result;
                        if (!result.success)
                        {
                            Console.WriteLine("An error occurred while reading error codes from UART, at page " + i + ". Please try again.");
                            Console.WriteLine(result.errors[0]);
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                            return true;
                        }
                        
                        foreach (var l in result.errors)
                        {
                            Console.ForegroundColor = l.StartsWith("No error displayed")
                                ? ConsoleColor.Blue
                                : ConsoleColor.Green;
                            Console.WriteLine(l);
                            Console.ResetColor();
                        }
                    }

                    // Wait for user input
                    Console.WriteLine("");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while connecting to your selected device.");
                    Console.WriteLine("Error details:");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }

                return true;
            #endregion
            #region Clear UART codes
            case "2":
                try
                {
                    if (!_uart.IsConnected)
                    {
                        Console.WriteLine("You must connect to a UART device before proceeding. Please select a device and try again.");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        return true;
                    }

                    bool success = _uart.ClearErrorLog(out string result);
                    
                    Console.WriteLine(result);
                    Console.WriteLine(success
                        ? "Error codes cleared successfully."
                        : "An error occurred while clearing error codes. Please try again.");
                    Console.WriteLine("Press Enter to continue...");

                    // Job done. Continue
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while connecting to your selected device.");
                    Console.WriteLine("Error details:");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
                return true;
            #endregion
            #region Custom UART command
            case "3":
                try
                {
                    if (!_uart.IsConnected)
                    {
                        Console.WriteLine("You must connect to a UART device before proceeding. Please select a device and try again.");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        return true;
                    }
                    
                    while (true) // Loop until user provides a valid command or 'exit'
                    {
                        // Ask the user for their custom UART command
                        Console.Write("Please enter a custom command to send (type exit to quit): ");
                        // Get the command which the user entered
                        string uartCommand = Console.ReadLine();

                        // If the user types exit, we want to return to the main menu
                        if (uartCommand == "exit")
                        {
                            break; // Exit the while loop and return to the main menu
                        }

                        if (!string.IsNullOrEmpty(uartCommand)) // If the command is not empty or null
                        {
                            bool success = _uart.SendArbitraryCommand(uartCommand, out string result);

                            Console.ForegroundColor = success
                                ? ConsoleColor.White
                                : ConsoleColor.Red;
                            
                            Console.WriteLine(result);

                            Console.ResetColor();
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                        }
                        else
                        {
                            // The user didn't type anything. We need to ask them to type their command again!
                            Console.WriteLine("Please enter a valid command.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred while connecting to your selected device.");
                    Console.WriteLine("Error details:");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
                return true;
            #endregion
            #region BIOS Dump Tools (Sub Menu)
            case "4":
                // This is a sub menu for working with BIOS files. IMO it's not very elegant to have sub menus but at least this way
                // we don't have to use different apps for working with BIOS files...

                RunSubMenu(appTitle);
                return true;
            #endregion
            #region Launch readme
            case "5":
                Console.WriteLine("UARL-CL is designed with simplicity in mind. This command line application makes it quick and easy");
                Console.WriteLine("to obtain error codes from your PlayStation 5 console.");
                Console.WriteLine("UART stands for Universal Asynchronous Receiver-Transmitter. UART allows you to send and receive commands");
                Console.WriteLine("to any compatible serial communications device.");
                Console.WriteLine();
                Console.WriteLine("The PlayStation 5 has UART functionality built in. Unfortunately Sony don't make it easy to understand what");
                Console.WriteLine("is happening with the machine when you request error codes, which is why this application exists. UART-CL is");
                Console.WriteLine("a command-line spin off to the PS5 NOR and UART tool for Windows, which allows you to communicate via serial");
                Console.WriteLine("to your PlayStation 5. You can grab error codes from the system at the click of a button (well, a few clicks)");
                Console.WriteLine("and the software will automatically check the error codes received and attempt to convert them into plain text.");
                Console.WriteLine("This is done by splitting the error codes up into useful sections and then comparing those error codes against a");
                Console.WriteLine("database of codes collected by the repair community. If the code exists in the database, the application will");
                Console.WriteLine("automatically grab the error details and output them for you on the screen so you can figure out your next move.");
                Console.WriteLine("");
                Console.WriteLine("Where does the database come from?");
                Console.WriteLine("The database is downloaded on first launch from uartcodes.com/xml.php. The download page is hosted by \"TheCod3r\"");
                Console.WriteLine("for free and is simply a PHP script which fetches all known error codes from the uartcodes.com database and converts");
                Console.WriteLine("them into an XML document. An XML document makes it quick and easy to work with and it's a more elegant solution to");
                Console.WriteLine("provide a database which doesn't rely on an internet connection. It can also be updated whenever you like, free of charge.");
                Console.WriteLine("");
                Console.WriteLine("How do I use this to fix my PS5?");
                Console.WriteLine("You'll need a compatible serial communication (UART) device first of all. Most devices that have a transmit, receive, and");
                Console.WriteLine("ground pin and that can provide 3.3v instead of 5v should work, and you can buy one for a few bucks on eBay, Amazon or AliExpress.");
                Console.WriteLine("");
                Console.WriteLine("Once you have a compatible device, you'll need to:");
                Console.WriteLine("Solder the transmit pin on the device to the receive pin on the PS5.");
                Console.WriteLine("Solder the receive pin on the device to the transmit pin on the PS5.");
                Console.WriteLine("Solder ground on the device to ground on the PS5.");
                Console.WriteLine("Connect the PS5 power chord to the PS5 power supply (do not turn on the console)");
                Console.WriteLine("Use this software and select either option 1, 2 or 3 to run commands.");
                Console.WriteLine("Choose your device from the list of available devices.");
                Console.WriteLine("Let the software do the rest. Then working out a plan for the actual repair is up to you. We can't do everything ;)");
                Console.WriteLine("");
                Console.WriteLine("As a personal note from myself (TheCod3r). I want to thank you for trusting my software. I'm an electronics technician primarily");
                Console.WriteLine("and I write code for fun (ironic since my name is TheCod3r, I know!). I would also like to thank the following people for inspiring me:");
                Console.WriteLine("");
                Console.WriteLine("Louis Rossmann, FightToRepair.org, Jessa Jones (iPad Rehab), Andy-Man (PS5 Wee Tools), my YouTube viewers, my Patreon supporters and my mom!");
                Console.WriteLine("");
                Console.WriteLine("#FuckBwE");
                Console.WriteLine("Be sure to use the hashtag. It really pisses him off!");

                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return true;
            #endregion
            #region Buy me a coffee
            case "6":
                // Thanks for buying me a coffee :)
                Console.WriteLine("Thanks for buying me a coffee. I'll redirect you in your default browser...");
                OpenUrl("https://www.streamelements.com/thecod3r/tip");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return true;
            #endregion
            #region Update XML database
            case "7":
                Console.WriteLine("Downloading latest database file. Please wait...");

                try
                {
                    UART.DownloadErrorDB();
                    Console.WriteLine("Database downloaded successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not download the latest database file. Please ensure you're connected to the internet! " + ex);
                }
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return true;
            case "8":
                if (_uart.IsConnected)
                {
                    _uart.Disconnect();
                    Console.WriteLine("Disconnected from COM port.");
                }
                else
                {
                    // Declare a variable to store the selected COM port
                    string selectedPort;
                    // Declare a string array for a list of available port names
                    string[] ports = SerialPort.GetPortNames();

                    // No COM ports found. Let the user know and go back to the main menu
                    if (ports.Length == 0)
                    {
                        Console.WriteLine("No communication devices were found on this system.");
                        Console.WriteLine("Please insert a UART compatible device and try again.");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        return true;
                    }

                    // Devices were found. Iterate through and present them in the form of a menu
                    Console.WriteLine("Available devices:");
                    for (int i = 0; i < ports.Length; i++)
                    {
                        string friendlyName = GetFriendlyName(ports[i]);
                        Console.WriteLine($"{i + 1}. {ports[i]} - {friendlyName}");
                    }

                    // Declare an integer for the index of the selected port
                    int selectedPortIndex;
                    // Add each port to the ports array
                    do
                    {
                        Console.WriteLine("");
                        Console.Write("Enter the number of the COM port you want to use: ");
                    } while (!int.TryParse(Console.ReadLine(), out selectedPortIndex) || selectedPortIndex < 1 || selectedPortIndex > ports.Length);

                    // Get the selected port and store it inside the selectedPort string
                    selectedPort = ports[selectedPortIndex - 1];

                    try
                    {
                        _uart.Connect(selectedPort);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred while connecting to your selected device.");
                        Console.WriteLine("Error details:");
                        Console.WriteLine(e);
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                    }
                }
                return true;
            case "X":
            case "x":
                Environment.Exit(0);
                return true;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                return true;
            #endregion
        }
        #endregion
    }

    private static void RunSubMenu(string appTitle)
    {
        string pathToDump = "";
        bool subMenuRunning = true;

        while (subMenuRunning)
        {
            // Check to see if we are working with a file. If so, add it to the app title so the user is aware...
            if(_norData != null)
            {
                Console.Title = appTitle + " - Working with file: " + pathToDump;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("In order to work with a BIOS dump file, you will need to load it into memory first.");
            Console.WriteLine("Be sure to choose a file to work with by choosing option 1 before choosing any other option...");
            Console.ResetColor();
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Choose an option:");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("1. Load a BIOS dump (.bin)");
            Console.ResetColor();
            Console.WriteLine("2. View BIOS information");
            Console.WriteLine("3. Convert to Digital edition");
            Console.WriteLine("4. Convert to Disc edition");
            Console.WriteLine("5. Convert to Slim edition");
            Console.WriteLine("6. Change serial number");
            Console.WriteLine("7. Change motherboard serial number");
            Console.WriteLine("8. Change console model number");
            Console.WriteLine("X. Return to previous menu");
            Console.Write("\nEnter your choice: ");
            switch (Console.ReadLine())
            {
                #region Load a dump file
                case "1":
                    Console.WriteLine("In order to work with a BIOS file, you must first choose a file to work with.");
                    Console.WriteLine("This needs to be a valid .bin file containing your BIOS dump file.");
                    Console.WriteLine("You will need to know the full file path of your .bin file in order to continue.");
                    Console.WriteLine();

                    while (true)
                    {
                        Console.Write("Enter the full file path (type 'exit' to quit): ");
                        string userInput = Console.ReadLine().Trim();

                        if (string.IsNullOrWhiteSpace(userInput))
                        {
                            Console.WriteLine("Invalid input. File path cannot be blank.");
                        }
                        else if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                        {
                            // User wants to return to the sub-menu
                            break; // Exit the current method and return to the sub-menu
                        }
                        else if (!File.Exists(userInput))
                        {
                            Console.WriteLine("The file path you entered does not exist. Please enter the path to a valid .bin file.");
                        }
                        else if (!Path.GetExtension(userInput).Equals(".bin", StringComparison.CurrentCultureIgnoreCase))
                        {
                            Console.WriteLine("The file you provided is not a .bin file. Please enter a valid .bin file path.");
                        }
                        else
                        {
                            // User provided a valid .bin file path, you can proceed with it
                            pathToDump = userInput;
                            long length = new FileInfo(pathToDump).Length;
                            Console.WriteLine("Selected file: " + pathToDump + " - File Size: " + length + " bytes (" + length / 1024 / 1024 + " MiB)");
                            _norData = new(pathToDump);
                            Thread.Sleep(1000);
                            break; // Exit the loop
                        }
                    }
                    break;
                #endregion
                #region View BIOS information
                case "2":
                    if(_norData != null)
                    {
                        long lengthBytes = new FileInfo(_norData.Path).Length;
                        long lengthMiB = lengthBytes / 1024 / 1024;
                    
                        // Start showing the data we've found to the user
                        Console.WriteLine("File size: " + lengthBytes + " bytes (" + lengthMiB + " MiB)"); // The file size of the .bin file
                        Console.WriteLine("PS5 Version: " + _norData.Edition); // Show the version info (disc/digital/slim)
                        Console.WriteLine("Console Model: " + _norData.VariantCode); // Show the console variant (CFI-XXXX)
                        Console.WriteLine("Console Serial Number: " + _norData.Serial); // Show the serial number. This serial is the one the disc drive would use
                        Console.WriteLine("Motherboard Serial Number: " + _norData.MoboSerial); // Show the serial number to the motherboard. This is different to the console serial
                        Console.WriteLine("WiFi Mac Address: " + _norData.WiFiMAC);
                        Console.WriteLine("LAN Mac Address: " + _norData.LANMAC);
                        // Just a blank line
                        Console.WriteLine("");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        break;
                    }

                    // No file has been selected. Let the user know
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You must select a .bin file to read before proceeding. Please select a valid .bin file and try again.");
                    Console.ResetColor();
                    Console.WriteLine("");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    break;
                #endregion
                #region Convert to "Digital" edition
                case "3":
                    // First check to confirm that we've selected a file to work with
                    if (_norData != null)
                    {
                        while (true)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Are you sure you want to set the console as \"Digital\" edition?");
                            Console.ResetColor();

                            Console.WriteLine("Type 'yes' to confirm or 'no' to cancel.");

                            string changeConfirmation = Console.ReadLine().Trim().ToLower();

                            // Check user input
                            if (changeConfirmation == "yes")
                            {
                                if (_norData.Edition == Editions.Digital)
                                {
                                    // The BIOS file already contains digital edition flags
                                    Console.WriteLine("The .bin file you're working with is already a digital edition. No changes are needed.");
                                    Console.WriteLine("Press Enter to continue...");
                                    Console.ReadLine();
                                    break;
                                }

                                // Modify the values to set the file as "Digital Edition"
                                try
                                {
                                    _norData.Edition = Editions.Digital;

                                    Console.WriteLine("Your BIOS file has been updated successfully. The new .bin file will now report to");
                                    Console.WriteLine("the PlayStation 5 as a 'digital edition' console.");
                                    Console.WriteLine("Press Enter to continue...");
                                    Console.ReadLine();
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    // Handle any exceptions that occur while writing to the file
                                    Console.WriteLine("Error updating the binary file: " + ex);
                                    Console.WriteLine("Please try again! Press Enter to continue...");
                                    Console.ReadLine();
                                    break;
                                }
                            }

                            if (changeConfirmation == "no")
                            {
                                // User cancelled. Break the loop and go back to the menu!
                                break;
                            }

                            Console.WriteLine("Invalid input. Please type 'yes' to confirm or 'no' to cancel.");
                        }
                    }
                    else
                    {
                        // No file has been selected. Let the user know
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You must select a .bin file to read before proceeding. Please select a valid .bin file and try again.");
                        Console.ResetColor();
                        Console.WriteLine("");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                    }
                    break;

                #endregion
                #region Convert to "Disc" edition
                case "4":
                    // First check to confirm that we've selected a file to work with
                    if (_norData != null)
                    {
                        while (true)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Are you sure you want to set the console as \"Disc\" edition?");
                            Console.ResetColor();

                            Console.WriteLine("Type 'yes' to confirm or 'no' to cancel.");

                            string changeConfirmation = Console.ReadLine().Trim().ToLower();

                            // Check user input
                            if (changeConfirmation == "yes")
                            {
                                if (_norData.Edition == Editions.Disc)
                                {
                                    // The BIOS file already contains disc edition flags
                                    Console.WriteLine("The .bin file you're working with is already a disc edition. No changes are needed.");
                                    Console.WriteLine("Press Enter to continue...");
                                    Console.ReadLine();
                                    break;
                                }

                                // Modify the values to set the file as "Disc Edition"
                                try
                                {
                                    _norData.Edition = Editions.Disc;

                                    Console.WriteLine("Your BIOS file has been updated successfully. The new .bin file will now report to");
                                    Console.WriteLine("the PlayStation 5 as a 'disc edition' console.");
                                    Console.WriteLine("Press Enter to continue...");
                                    Console.ReadLine();
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    // Handle any exceptions that occur while writing to the file
                                    Console.WriteLine("Error updating the binary file: " + ex.Message);
                                    Console.WriteLine("Please try again! Press Enter to continue...");
                                    Console.ReadLine();
                                    break;
                                }
                            }

                            if (changeConfirmation == "no")
                            {
                                break;
                            }

                            Console.WriteLine("Invalid input. Please type 'yes' to confirm or 'no' to cancel.");
                        }
                    }
                    else
                    {
                        // No file has been selected. Let the user know
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You must select a .bin file to read before proceeding. Please select a valid .bin file and try again.");
                        Console.ResetColor();
                        Console.WriteLine("");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                    }
                    break;
                #endregion
                #region Convert to "Slim" edition
                case "5":
                    // First check to confirm that we've selected a file to work with
                    if (_norData != null)
                    {
                        while (true)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Are you sure you want to set the console as \"Slim\" edition?");
                            Console.ResetColor();

                            Console.WriteLine("Type 'yes' to confirm or 'no' to cancel.");

                            string changeConfirmation = Console.ReadLine().Trim().ToLower();

                            // Check user input
                            if (changeConfirmation == "yes")
                            {
                                if (_norData.Edition == Editions.Slim)
                                {
                                    // The BIOS file already contains slim edition flags
                                    Console.WriteLine("The .bin file you're working with is already a slim edition. No changes are needed.");
                                    Console.WriteLine("Press Enter to continue...");
                                    Console.ReadLine();
                                    break;
                                }

                                // Modify the values to set the file as "Slim Edition"
                                try
                                {
                                    _norData.Edition = Editions.Slim;

                                    Console.WriteLine("Your BIOS file has been updated successfully. The new .bin file will now report to");
                                    Console.WriteLine("the PlayStation 5 as a 'slim edition' console.");
                                    Console.WriteLine("Press Enter to continue...");
                                    Console.ReadLine();
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    // Handle any exceptions that occur while writing to the file
                                    Console.WriteLine("Error updating the binary file: " + ex.Message);
                                    Console.WriteLine("Please try again! Press Enter to continue...");
                                    Console.ReadLine();
                                    break;
                                }
                            }

                            if (changeConfirmation == "no")
                            {
                                // User cancelled. Break the loop and go back to the menu!
                                break;
                            }

                            Console.WriteLine("Invalid input. Please type 'yes' to confirm or 'no' to cancel.");
                        }
                    }
                    else
                    {
                        // No file has been selected. Let the user know
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You must select a .bin file to read before proceeding. Please select a valid .bin file and try again.");
                        Console.ResetColor();
                        Console.WriteLine("");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                    }
                    break;
                #endregion
                #region Change serial number
                case "6":
                    if (_norData != null)
                    {
                        // Create a true false to allow us to loop until the user changes the serial or cancels the operation
                        bool jobDone = false;

                        while (!jobDone)
                        {
                            string oldSerial = _norData.Serial;

                            while (true)
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.WriteLine("Enter the new serial number you would like to save (type 'exit' to exit): ");
                                Console.ResetColor();

                                string newSerial = Console.ReadLine().Trim();

                                if (newSerial == "")
                                {
                                    // The serial number is blank
                                    Console.WriteLine("Invalid serial number entered. The new serial should be characters and letters.");
                                }
                                else if (newSerial == oldSerial)
                                {
                                    Console.WriteLine("The new serial number matches the old serial number. Please enter a different value and try again...");
                                }
                                else if (newSerial == "exit")
                                {
                                    jobDone = true;
                                    break;
                                }
                                else
                                {
                                    try
                                    {
                                        _norData.Serial = newSerial;
                                        Console.WriteLine("SUCCESS: The new serial number has been updated successfully. Press enter to continue...");
                                        Console.ReadLine();
                                        jobDone = true;
                                        break;
                                    }
                                    catch (ArgumentException ex)
                                    {
                                        // Something went wrong. Notify the user
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine($"An error occurred while attempting to make changes to your dump file. Please try again. {ex}");
                                        Console.ResetColor();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // No file has been selected. Let the user know
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You must select a .bin file to read before proceeding. Please select a valid .bin file and try again.");
                        Console.ResetColor();
                        Console.WriteLine("");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                    }
                    break;
                #endregion
                #region Change motherboard serial number
                case "7":
                    if (_norData != null)
                    {
                        // Create a loop to prevent the app from returning to the main menu
                        while (true)
                        {
                            // Show the current motherboard serial to the user
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Current motherboard serial: " + _norData.MoboSerial);
                            Console.ResetColor();

                            // Ask the user to enter the new serial number
                            Console.WriteLine("Please enter a new motherboard serial number (type 'exit' to go back): ");

                            string newSerial = Console.ReadLine();

                            if (string.IsNullOrEmpty(newSerial))
                            {
                                // The user did not enter a valid string
                                Console.WriteLine("Please enter a valid model number to continue...");
                            }
                            else if (newSerial.Length != 16)
                            {
                                // The entered serial number is an invalid length
                                Console.WriteLine("The new motherboard serial you entered is invalid. The motherboard serial should be exactly 16 characters in length.");
                            }
                            else if (newSerial == "exit")
                            {
                                // The user wants to exit this menu
                                break;
                            }
                            else
                            {
                                // Everything seems OK. Now we can change the serial number
                                try
                                {
                                    _norData.MoboSerial = newSerial;

                                    Console.WriteLine("The new motherboard serial number you entered been saved successfully.");
                                    Console.WriteLine("Press Enter to continue...");
                                    Console.ReadLine();
                                    break;
                                }
                                catch (ArgumentException ex)
                                {
                                    Console.WriteLine("An error occurred while writing to the BIOS dump. Please try again..." + ex.Message);
                                    Console.WriteLine("Press Enter to continue...");
                                    Console.ReadLine();
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        // No file has been selected. Let the user know
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You must select a .bin file to read before proceeding. Please select a valid .bin file and try again.");
                        Console.ResetColor();
                        Console.WriteLine("");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                    }
                    break;
                #endregion
                #region Change console model
                case "8":
                    if (_norData != null)
                    {
                        // Create a loop to prevent the app from returning to the main menu
                        while (true)
                        {
                            // Show the current model to the user
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Current model: " + _norData.VariantCode);
                            Console.ResetColor();

                            // Ask the user to enter the new model
                            Console.WriteLine(
                                "Please enter the model you would like to set your dump file to (type 'exit' to go back): ");

                            string newModel = Console.ReadLine();

                            if (string.IsNullOrEmpty(newModel))
                            {
                                // The user did not enter a valid string
                                Console.WriteLine("Please enter a valid model number to continue...");
                            }
                            else if (newModel.Length < 9)
                            {
                                // The entered model is an invalid length
                                Console.WriteLine(
                                    "The new model you entered is invalid. The model should be 9 characters long starting with 'CFI-', followed by 4 numbers and a letter.");
                            }
                            else if (!newModel.StartsWith("CFI-") || !newModel.StartsWith("DFI-"))
                            {
                                Console.WriteLine(
                                    "The new model you entered is invalid. The model should be 9 characters long starting with 'CFI-', followed by 4 numbers and a letter.");
                            }
                            else if (newModel == "exit")
                            {
                                // The user wants to exit this menu
                                break;
                            }
                            else
                            {
                                // Everything seems OK. Now we can change the model
                                try
                                {
                                    _norData.VariantCode = newModel;

                                    Console.WriteLine(
                                        "The new console model you chose has been saved successfully.");
                                    Console.WriteLine("Press Enter to continue...");
                                    Console.ReadLine();
                                    break;

                                }
                                catch (ArgumentException ex)
                                {
                                    Console.WriteLine(
                                        "An error occurred while writing to the BIOS dump. Please try again... " +
                                        ex);
                                    Console.WriteLine("Press Enter to continue...");
                                    Console.ReadLine();
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        // No file has been selected. Let the user know
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You must select a .bin file to read before proceeding. Please select a valid .bin file and try again.");
                        Console.ResetColor();
                        Console.WriteLine("");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                    }
                    break;
                #endregion
                #region Exit sub menu
                // I put two cases here for the exit option, one for capital "X" and one for lower case.
                case "X":
                    // We should reset the app title first!
                    Console.Title = appTitle;
                    subMenuRunning = false;
                    break;
                case "x":
                    // We should reset the app title first!
                    Console.Title = appTitle;
                    subMenuRunning = false;
                    break;
                #endregion
            }
        }
    }

    private static void OpenUrl(string url)
    {
        // Let's wait two seconds first
        Thread.Sleep(2000);
        // Wrap this in a try loop so we don't get any unexpected crashes
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            // Catch any errors and let the user know
            Console.WriteLine($"Error opening URL: {ex.Message}");
        }
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
        Console.WriteLine("For more information on how to connect to UART you can use the options below or read the ReadMe.");
        Console.WriteLine("");
    }

    private static string GetFriendlyName(string portName)
    {
        // Declare the friendly name variable for later use
        string friendlyName = portName;
        // We'll wrap this in a try loop simply because this isn't available on all platforms
        try
        {
            // This is basically an SQL query. Let's search for the details of the ports based on the port name
            // Again, this is just for Windows based devices
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%" + portName + "%'"))
            {
                // Loop through and output the friendly name
                foreach (var port in searcher.Get())
                {
                    friendlyName = port["Name"]!.ToString()!;
                }
            }
        }
        // Catch errors. This would probably only happen on Linux and macOS systems
        catch(Exception ex)
        {
            // If there is an error, we'll just declare that we don't know the name of the port
            friendlyName = "Unknown Port Name";
        }
        // Send the friendly name (or unknown port name string) back to the main code for output
        return friendlyName;
    }
}

#endregion