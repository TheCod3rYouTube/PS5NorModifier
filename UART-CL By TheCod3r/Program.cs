using System;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using Colorful;
using System.Net;
using System.Management;
using System.Xml;

bool showMenu = false;

// Set the application title
System.Console.Title = "UART-CL by TheCod3r";

#region Checksum generation
static string CalculateChecksum(string str)
{
    // Math stuff. I don't understand it either!
    int sum = 0;
    foreach (char c in str)
    {
        sum += (int)c;
    }
    return str + ":" + (sum & 0xFF).ToString("X2");
}
#endregion

#region Error parsing (via XML database)

// When fetching errors from the PS5 we want to be able to convert the received codes into readable text to make it easier
// for the user to understand what the problem is. By the time this function is called we should have an up to date XML
// database to compare error codes with.
static string ParseErrors(string errorCode)
{
    string results = "";

    try
    {
        // Check if the XML file exists
        if (File.Exists("errorDB.xml"))
        {
            // Load the XML file
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("errorDB.xml");

            // Get the root node
            XmlNode root = xmlDoc.DocumentElement;

            // Check if the root node is <errorCodes>
            if (root.Name == "errorCodes")
            {
                // No error was found in the database
                if (root.ChildNodes.Count == 0)
                {
                    results = "No result found for error code " + errorCode;
                }
                else
                {
                    // Loop through each errorCode node
                    foreach (XmlNode errorCodeNode in root.ChildNodes)
                    {
                        // Check if the node is <errorCode>
                        if (errorCodeNode.Name == "errorCode")
                        {
                            // Get ErrorCode and Description
                            string errorCodeValue = errorCodeNode.SelectSingleNode("ErrorCode").InnerText;
                            string description = errorCodeNode.SelectSingleNode("Description").InnerText;

                            // Check if the current error code matches the requested error code
                            if (errorCodeValue == errorCode)
                            {
                                // Output the results
                                results = "Error code: " + errorCodeValue + Environment.NewLine + "Description: " + description;
                                break; // Exit the loop after finding the matching error code
                            }
                        }
                    }
                }
            }
            else
            {
                results = "Error: Invalid XML database file. Please reconfigure the application, redownload the offline database, or uncheck the option to use the offline database.";
            }
        }
        else
        {
            results = "Error: Local XML file not found.";
        }
    }
    catch (Exception ex)
    {
        results = "Error: " + ex.Message;
    }

    return results;
}

#endregion

#region Obtian the friendly name of the available COM ports
static string GetFriendlyName(string portName)
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
                friendlyName = port["Name"].ToString();
            }
        }
    }
    // Catch errors. This would probably only happen on Linux systems
    catch(Exception ex)
    {
        // If there is an error, we'll just declare that we don't know the name of the port
        friendlyName = "Unknown Port Name";
    }
    // Send the friendly name (or unknown port name string) back to the main code for output
    return friendlyName;
}
#endregion

#region Console header
static void ShowHeader()
{
    // This is the header.
    System.Console.Clear();
    Colorful.Console.WriteAscii("UART-CL v1.0.0.0");
    Colorful.Console.WriteAscii("by TheCod3r");
    System.Console.WriteLine("");
    System.Console.WriteLine("");
    System.Console.WriteLine("UART-CL is a command line UART tool to assist in the diagnosis and repair of PlayStation 5 consoles using UART.");
    System.Console.WriteLine("For more information on how to connect to UART you can use the options below to be sent to a guide on YouTube.");
    System.Console.WriteLine("");
}
#endregion

#region Check if error database exists
// Let's check and see if the database exists. If not, download it!
if (!System.IO.File.Exists("errorDB.xml"))
{
    ShowHeader();
    System.Console.WriteLine("Downloading latest database file. Please wait...");

    bool success = DownloadDatabase("http://uartcodes.com/xml.php", "errorDB.xml");

    if (success)
    {
        System.Console.WriteLine("Database downloaded successfully...");
        showMenu = true;
    }
    else
    {
        System.Console.WriteLine("Could not download the latest database file. Please ensure you're connected to the internet!");
        Environment.Exit(0);
    }
}
else
{
    // The database file exists. Continue to the main menu
    showMenu = true;
}
#endregion

#region URL Handling

// Let's create a function that will allow us to download the latest version of the database if we have access to the internet.
static bool DownloadDatabase(string url, string savePath)
{
    using (WebClient client = new WebClient())
    {
        try
        {
            client.DownloadFile(url, savePath);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}

// Function to open a new URL in the default browser
static void OpenUrl(string url)
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
        System.Console.WriteLine($"Error opening URL: {ex.Message}");
    }
}

#endregion

#region Display main menu
// Show the menu if showMenu is set to true
while (showMenu)
{
    showMenu = MainMenu();
}
#endregion

#region Main
static bool MainMenu()
{

    System.Console.Clear();
    ShowHeader();
    System.Console.ForegroundColor = ConsoleColor.Red;
    System.Console.WriteLine("Choose an option:");
    System.Console.ResetColor();
    System.Console.WriteLine("1. Get error codes from PS5");
    System.Console.WriteLine("2. Clear error codes on PS5");
    System.Console.WriteLine("3. Enter custom UART command");
    System.Console.WriteLine("4. View readme guide");
    // Thanks for leaving this here!
    System.Console.ForegroundColor = ConsoleColor.Green;
    System.Console.WriteLine("5. Buy TheCod3r a coffee");
    System.Console.ResetColor();
    System.Console.WriteLine("6. Update error database");
    System.Console.WriteLine("X. Exit application");
    System.Console.ForegroundColor = ConsoleColor.Red;
    System.Console.Write("\nEnter your choice: ");
    System.Console.ResetColor();

    #region Menu Options
    switch (System.Console.ReadLine())
    {
        #region Get Error Codes From PS5
        case "1":
            // Declare a variable to store the selected COM port
            string selectedPort;
            // Declare a string array for a list of available port names
            string[] ports = SerialPort.GetPortNames();

            // No COM ports found. Let the user know and go back to the main menu
            if (ports.Length == 0)
            {
                System.Console.WriteLine("No communication devices were found on this system.");
                System.Console.WriteLine("Please insert a UART compatible device and try again.");
                System.Console.WriteLine("Press Enter to continue...");
                System.Console.ReadLine();
                return true;
            }

            // Devices were found. Iterate through and present them in the form of a menu
            System.Console.WriteLine("Available devices:");
            for (int i = 0; i < ports.Length; i++)
            {
                string friendlyName = GetFriendlyName(ports[i]);
                System.Console.WriteLine($"{i + 1}. {ports[i]} - {friendlyName}");
            }

            // Declare an integer for the index of the selected port
            int selectedPortIndex;
            // Add each port to the ports array
            do
            {
                System.Console.WriteLine("");
                System.Console.Write("Enter the number of the COM port you want to use: ");
            } while (!int.TryParse(System.Console.ReadLine(), out selectedPortIndex) || selectedPortIndex < 1 || selectedPortIndex > ports.Length);

            // Get the selected port and store it inside the selectedPort string
            selectedPort = ports[selectedPortIndex - 1];

            // Select and lock the chosen device
            SerialPort serialPort = new SerialPort(selectedPort);
            // Configure settings for the selected device
            serialPort.BaudRate = 115200; // The PS5 requires a BAUD rate of 115200
            serialPort.RtsEnable = true; // We need to enable ready to send (RTS) mode
            // Now we can get a list of error codes. We're going to wrap this in a try loop to prevent unexpected crashes
            try
            {
                // Open the selected port for use
                serialPort.Open();
                // Let's display the selected port (change the color to blue) so the user is aware of what device is in use
                System.Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.WriteLine("Selected port: " + GetFriendlyName(selectedPort));
                // Reset the foreground color to default before proceeding
                System.Console.ResetColor();

                // Let's start grabbing error codes from the PS5
                int loopLimit = 10;
                // Create a list to store error codes in
                List<string> UARTLines = new();

                // When grabbing error codes, we want to grab the first 10 errors from the system. Let's create a loop
                for (var i = 0; i <= loopLimit; i++)
                {
                    // Create a command variable depending on what number we're at in the loop (where "i" is the current number)
                    var command = $"errlog {i}";
                    // Add the checksum to the command
                    var checksum = CalculateChecksum(command);
                    // Send the current command to the UART device
                    serialPort.WriteLine(checksum);
                    
                    // Read the UART response
                    var line = serialPort.ReadLine();

                    // Ensure we have a valid response, then add it to the errors list
                    if (!string.Equals($"{command}:{checksum:X2}", line, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!line.Contains("errlog"))
                        {
                            // Let's make sure we haven't already added the same error code to the error list
                            // This way we only show each error code once and keep the output window clean
                            if (!UARTLines.Contains(line))
                            {
                                UARTLines.Add(line);
                            }
                        }
                    }
                }

                // Now let's iterate through the lines and show them to the user
                foreach (var l in UARTLines)
                {
                    var split = l.Split(' ');
                    if (!split.Any()) continue;
                    switch (split[0])
                    {
                        case "NG":
                            break;
                        case "OK":
                            var errorCode = split[2];
                            if (errorCode.StartsWith("FFFFFF"))
                            {
                                // The returned code is blank
                                System.Console.ForegroundColor = ConsoleColor.Blue;
                                System.Console.WriteLine("No error displayed");
                                System.Console.ResetColor();
                            }
                            else
                            {
                                // Now that the error code has been isolated from the rest of the junk sent by the system
                                // let's check it against the database. The error database will need to return XML results
                                System.Console.ForegroundColor = ConsoleColor.Green;
                                string errorResult = ParseErrors(errorCode);
                                System.Console.WriteLine(errorResult);
                                System.Console.ResetColor();
                            }
                            break;
                        default:
                            break;
                    }
                }

                // Wait for user input
                System.Console.WriteLine("");
                System.Console.WriteLine("Press Enter to continue...");
                System.Console.ReadLine();

                // Before exiting, close and free up the selected device
                serialPort.Close();
            }
            catch(Exception ex)
            {
                System.Console.WriteLine("An error occurred while connecting to your selected device.");
                System.Console.WriteLine("Error details:");
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine("Press Enter to continue...");
                System.Console.ReadLine();
            }

            return true;
        #endregion
        #region Clear UART codes
        case "2":
            // Declare a string array for a list of available port names
            ports = SerialPort.GetPortNames();

            // No COM ports found. Let the user know and go back to the main menu
            if (ports.Length == 0)
            {
                System.Console.WriteLine("No communication devices were found on this system.");
                System.Console.WriteLine("Please insert a UART compatible device and try again.");
                System.Console.WriteLine("Press Enter to continue...");
                System.Console.ReadLine();
                return true;
            }

            // Devices were found. Iterate through and present them in the form of a menu
            System.Console.WriteLine("Available devices:");
            for (int i = 0; i < ports.Length; i++)
            {
                string friendlyName = GetFriendlyName(ports[i]);
                System.Console.WriteLine($"{i + 1}. {ports[i]} - {friendlyName}");
            }

            // Add each port to the ports array
            do
            {
                System.Console.WriteLine("");
                System.Console.Write("Enter the number of the COM port you want to use: ");
            } while (!int.TryParse(System.Console.ReadLine(), out selectedPortIndex) || selectedPortIndex < 1 || selectedPortIndex > ports.Length);

            // Get the selected port and store it inside the selectedPort string
            selectedPort = ports[selectedPortIndex - 1];

            // Select and lock the chosen device
            serialPort = new SerialPort(selectedPort);
            // Configure settings for the selected device
            serialPort.BaudRate = 115200; // The PS5 requires a BAUD rate of 115200
            serialPort.RtsEnable = true; // We need to enable ready to send (RTS) mode
            // Now we can wipe the error codes. We're going to wrap this in a try loop to prevent unexpected crashes
            try
            {
                // Open the selected port for use
                serialPort.Open();
                // Let's display the selected port (change the color to blue) so the user is aware of what device is in use
                System.Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.WriteLine("Selected port: " + GetFriendlyName(selectedPort));
                // Reset the foreground color to default before proceeding
                System.Console.ResetColor();

                var checksum = CalculateChecksum("errlog clear");
                serialPort.WriteLine(checksum);

                List<string> UARTLines = new();

                do
                {
                    var line = serialPort.ReadLine();
                    UARTLines.Add(line);
                } while (serialPort.BytesToRead != 0);

                foreach (var l in UARTLines)
                {
                    System.Console.WriteLine(l);
                }

                System.Console.WriteLine("Press Enter to continue...");

                // Job done. Continue
                System.Console.ReadLine();

                // Before exiting, close and free up the selected device
                serialPort.Close();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("An error occurred while connecting to your selected device.");
                System.Console.WriteLine("Error details:");
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine("Press Enter to continue...");
                System.Console.ReadLine();
            }
            return true;
        #endregion
        #region Custom UART command
        case "3":
            // Declare a string array for a list of available port names
            ports = SerialPort.GetPortNames();

            // No COM ports found. Let the user know and go back to the main menu
            if (ports.Length == 0)
            {
                System.Console.WriteLine("No communication devices were found on this system.");
                System.Console.WriteLine("Please insert a UART compatible device and try again.");
                System.Console.WriteLine("Press Enter to continue...");
                System.Console.ReadLine();
                return true;
            }

            // Devices were found. Iterate through and present them in the form of a menu
            System.Console.WriteLine("Available devices:");
            for (int i = 0; i < ports.Length; i++)
            {
                string friendlyName = GetFriendlyName(ports[i]);
                System.Console.WriteLine($"{i + 1}. {ports[i]} - {friendlyName}");
            }

            // Add each port to the ports array
            do
            {
                System.Console.WriteLine("");
                System.Console.Write("Enter the number of the COM port you want to use: ");
            } while (!int.TryParse(System.Console.ReadLine(), out selectedPortIndex) || selectedPortIndex < 1 || selectedPortIndex > ports.Length);

            // Get the selected port and store it inside the selectedPort string
            selectedPort = ports[selectedPortIndex - 1];

            // Select and lock the chosen device
            serialPort = new SerialPort(selectedPort);
            // Configure settings for the selected device
            serialPort.BaudRate = 115200; // The PS5 requires a BAUD rate of 115200
            serialPort.RtsEnable = true; // We need to enable ready to send (RTS) mode
            
            // Now we can run the custom command. We're going to wrap this in a try loop to prevent unexpected crashes
            try
            {
                // Open the selected port for use
                serialPort.Open();
                // Let's display the selected port (change the color to blue) so the user is aware of what device is in use
                System.Console.ForegroundColor = ConsoleColor.Blue;
                System.Console.WriteLine("Selected port: " + GetFriendlyName(selectedPort));
                // Reset the foreground color to default before proceeding
                System.Console.ResetColor();

                while (true) // Loop until user provides a valid command or 'exit'
                {
                    // Ask the user for their custom UART command
                    System.Console.Write("Please enter a custom command to send (type exit to quit): ");
                    // Get the command which the user entered
                    string UARTCommand = System.Console.ReadLine();

                    // If the user types exit, we want to return to the main menu
                    if (UARTCommand == "exit")
                    {
                        break; // Exit the while loop and return to the main menu
                    }
                    else if (!string.IsNullOrEmpty(UARTCommand)) // If the command is not empty or null
                    {
                        var checksum = CalculateChecksum(UARTCommand);
                        serialPort.WriteLine(checksum);

                        List<string> UARTLines = new();

                        do
                        {
                            var line = serialPort.ReadLine();
                            UARTLines.Add(line);
                        } while (serialPort.BytesToRead != 0);

                        foreach (var l in UARTLines)
                        {
                            System.Console.WriteLine(l);
                        }

                        System.Console.WriteLine("Press Enter to continue...");
                        System.Console.ReadLine();
                    }
                    else
                    {
                        // The user didn't type anything. We need to ask them to type their command again!
                        System.Console.WriteLine("Please enter a valid command.");
                    }
                }

                // Before exiting, close and free up the selected device
                serialPort.Close();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("An error occurred while connecting to your selected device.");
                System.Console.WriteLine("Error details:");
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine("Press Enter to continue...");
                System.Console.ReadLine();
            }
            return true;
        #endregion
        #region Launch readme
        case "4":
            System.Console.WriteLine("UARL-CL is designed with simplicity in mind. This command line application makes it quick and easy");
            System.Console.WriteLine("to obtain error codes from your PlayStation 5 console.");
            System.Console.WriteLine("UART stands for Universal Asynchronous Receiver-Transmitter. UART allows you to send and receive commands");
            System.Console.WriteLine("to any compatible serial communications device.");
            System.Console.WriteLine();
            System.Console.WriteLine("The PlayStation 5 has UART functionality built in. Unfortunately Sony don't make it easy to understand what");
            System.Console.WriteLine("is happening with the machine when you request error codes, which is why this application exists. UART-CL is");
            System.Console.WriteLine("a command-line spin off to the PS5 NOR and UART tool for Windows, which allows you to communicate via serial");
            System.Console.WriteLine("to your PlayStation 5. You can grab error codes from the system at the click of a button (well, a few clicks)");
            System.Console.WriteLine("and the software will automatically check the error codes received and attempt to convert them into plain text.");
            System.Console.WriteLine("This is done by splitting the error codes up into useful sections and then comparing those error codes against a");
            System.Console.WriteLine("database of codes collected by the repair community. If the code exists in the database, the application will");
            System.Console.WriteLine("automatically grab the error details and output them for you on the screen so you can figure out your next move.");
            System.Console.WriteLine("");
            System.Console.WriteLine("Where does the database come from?");
            System.Console.WriteLine("The database is downloaded on first launch from uartcodes.com/xml.php. The download page is hosted by \"TheCod3r\"");
            System.Console.WriteLine("for free and is simply a PHP script which fetches all known error codes from the uartcodes.com database and converts");
            System.Console.WriteLine("them into an XML document. An XML document makes it quick and easy to work with and it's a more elegant solution to");
            System.Console.WriteLine("provide a database which doesn't rely on an internet connection. It can also be updated whenever you like, free of charge.");
            System.Console.WriteLine("");
            System.Console.WriteLine("How do I use this to fix my PS5?");
            System.Console.WriteLine("You'll need a compatible serial communication (UART) device first of all. Most devices that have a transmit, receive, and");
            System.Console.WriteLine("ground pin and that can provide 3.3v instead of 5v should work, and you can buy one for a few bucks on eBay, Amazon or AliExpress.");
            System.Console.WriteLine("");
            System.Console.WriteLine("Once you have a compatible device, you'll need to:");
            System.Console.WriteLine("Solder the transmit pin on the device to the receive pin on the PS5.");
            System.Console.WriteLine("Solder the receive pin on the device to the transmit pin on the PS5.");
            System.Console.WriteLine("Solder ground on the device to ground on the PS5.");
            System.Console.WriteLine("Connect the PS5 power chord to the PS5 power supply (do not turn on the console)");
            System.Console.WriteLine("Use this software and select either option 1, 2 or 3 to run commands.");
            System.Console.WriteLine("Choose your device from the list of available devices.");
            System.Console.WriteLine("Let the software do the rest. Then working out a plan for the actual repair is up to you. We can't do everything ;)");
            System.Console.WriteLine("");
            System.Console.WriteLine("As a personal note from myself (TheCod3r). I want to thank you for trusting my software. I'm an electronics technician primarily");
            System.Console.WriteLine("and I write code for fun (ironic since my name is TheCod3r, I know!). I would also like to thank the following people for inspiring me:");
            System.Console.WriteLine("");
            System.Console.WriteLine("Louis Rossmann, FightToRepair.org, Jessa Jones (iPad Rehab), Andy-Man (PS5 Wee Tools), my YouTube viewers, my Patreon supporters and my mom!");
            System.Console.WriteLine("");
            System.Console.WriteLine("#FuckBwE");
            System.Console.WriteLine("Be sure to use the hashtag. It really pisses him off!");

            System.Console.WriteLine("Press Enter to continue...");
            System.Console.ReadLine();
            return true;
        #endregion
        #region Buy me a coffee
        case "5":
            // Thanks for buying me a coffee :)
            System.Console.WriteLine("Thanks for buying me a coffee. I'll redirect you in your default browser...");
            OpenUrl("https://www.streamelements.com/thecod3r/tip");
            System.Console.WriteLine("Press Enter to continue...");
            System.Console.ReadLine();
            return true;
        #endregion
        #region Update XML database
        case "6":
            System.Console.WriteLine("Downloading latest database file. Please wait...");

            bool success = DownloadDatabase("http://uartcodes.com/xml.php", "errorDB.xml");

            if (success)
            {
                System.Console.WriteLine("Database downloaded successfully...");
                System.Console.WriteLine("Press Enter to continue...");
            }
            else
            {
                System.Console.WriteLine("Could not download the latest database file. Please ensure you're connected to the internet!");
                System.Console.WriteLine("Press Enter to continue...");
            }
            System.Console.ReadLine();
            return true;
        default:
            System.Console.WriteLine("Invalid choice. Please try again.");
            System.Console.WriteLine("Press Enter to continue...");
            System.Console.ReadLine();
            return true;
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
    }
    #endregion
}
#endregion