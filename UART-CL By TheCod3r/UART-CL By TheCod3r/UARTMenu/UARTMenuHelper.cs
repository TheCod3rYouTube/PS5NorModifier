using System.IO.Ports;
using UART_CL_By_TheCod3r.SubMenu;
using UART_CL_By_TheCod3r.Utilities;

namespace UART_CL_By_TheCod3r.UARTMenu;

public static class UARTMenuHelper
{
    public static bool GetErrorCodesFromPS5(Func<string[]>? getAvailablePorts = null,
        Func<int, Func<string?>?, int>? promptPortSelection = null,
        Func<string, SerialPort>? configureSerialPorts = null,
        Action<SerialPort, string, Action<string>?>? openSerialPort = null,
        Func<string, string>? parseErrors = null,
        Func<SerialPort, List<string>>? collectErrorLines = null,
        Func<string?>? readLine = null,
        Action<string>? writeLine = null)
    {
        getAvailablePorts ??= GetAvailablePorts;
        promptPortSelection ??= PromptPortSelection;
        configureSerialPorts ??= ConfigureSerialPort;
        openSerialPort ??= OpenSerialPort;
        collectErrorLines ??= CollectErrorLines;
        readLine ??= Console.ReadLine;
        writeLine ??= Console.WriteLine;

        var ports = getAvailablePorts();

        if (ports.Length == 0)
        {
            ShowNoDevicesMessage(readLine, writeLine);
            return true;
        }

        PrintAvailableDevices(ports);

        var selectedPortIndex = promptPortSelection(ports.Length, readLine);
        var selectedPort = ports[selectedPortIndex - 1];

        try
        {
            var serialPort = configureSerialPorts(selectedPort);

            openSerialPort(serialPort, selectedPort, writeLine);

            var UARTLines = collectErrorLines(serialPort);

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
                            Console.ForegroundColor = ConsoleColor.Blue;
                            writeLine("No error displayed");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            var result = (parseErrors ?? PS5UARTUtilities.ParseErrors).Invoke(errorCode);
                            writeLine(result);
                            Console.ResetColor();
                        }
                        break;
                }
            }

            writeLine("");
            writeLine("Press Enter to continue...");
            readLine();

            serialPort.Close();
        }
        catch (Exception ex)
        {
            PrintErrorMessage(ex, writeLine);
            readLine();
        }

        return true;
    }

    public static bool ClearUARTCodes(Func<string[]>? getAvailablePorts = null,
        Func<string, SerialPort>? configureSerialPort = null,
        Action<SerialPort, string, Action<string>?>? openSerialPort = null,
        Func<int, Func<string?>?, int>? promptPortSelection = null,
        Func<SerialPort, List<string>>? readUARTLines = null,
        Func<string?>? readLine = null,
        Action<string>? writeLine = null,
        Action<SerialPort, string>? writeLineToPort = null,
        Action<SerialPort>? closePort = null)
    {
        getAvailablePorts ??= GetAvailablePorts;
        configureSerialPort ??= ConfigureSerialPort;
        openSerialPort ??= OpenSerialPort;
        promptPortSelection ??= PromptPortSelection;
        readUARTLines ??= ReadUARTLines;
        readLine ??= Console.ReadLine;
        writeLine ??= Console.WriteLine;

        // Declare a string array for a list of available port names
        var ports = getAvailablePorts();

        // No COM ports found. Let the user know and go back to the main menu
        if (ports.Length == 0)
        {
            ShowNoDevicesMessage(readLine, writeLine);
            return true;
        }

        // Devices were found. Iterate through and present them in the form of a menu
        PrintAvailableDevices(ports);

        var selectedPortIndex = promptPortSelection(ports.Length, readLine);

        // Get the selected port and store it inside the selectedPort string
        var selectedPort = ports[selectedPortIndex - 1];

        // Now we can wipe the error codes. We're going to wrap this in a try loop to prevent unexpected crashes
        try
        {
            var serialPort = configureSerialPort(selectedPort);

            // Open the selected port for use
            openSerialPort(serialPort, selectedPort, writeLine);

            var checksum = PS5UARTUtilities.CalculateChecksum("errlog clear");

            if (writeLineToPort != null)
                writeLineToPort(serialPort, checksum);
            else
                serialPort.WriteLine(checksum);

            var UARTLines = readUARTLines(serialPort);

            foreach (var l in UARTLines)
            {
                writeLine(l);
            }

            writeLine("Press Enter to continue...");

            // Job done. Continue
            readLine();

            // Before exiting, close and free up the selected device
            if (closePort != null)
                closePort(serialPort);
            else
                serialPort.Close();
        }
        catch (Exception ex)
        {
            PrintErrorMessage(ex, writeLine);
            readLine();
        }
        return true;
    }

    public static bool RunCustomUARTCommand(Func<string[]>? getAvailablePorts = null,
        Func<string, SerialPort>? configureSerialPort = null,
        Action<SerialPort, string, Action<string>?>? openSerialPort = null,
        Func<int, Func<string?>?, int>? promptPortSelection = null,
        Func<SerialPort, List<string>>? readUARTLines = null,
        Func<string?>? readLine = null,
        Action<string>? writeLine = null,
        Action<SerialPort, string>? writeLineToPort = null,
        Action<SerialPort>? closePort = null)
    {
        getAvailablePorts ??= GetAvailablePorts;
        configureSerialPort ??= ConfigureSerialPort;
        openSerialPort ??= OpenSerialPort;
        promptPortSelection ??= PromptPortSelection;
        readUARTLines ??= ReadUARTLines;
        readLine ??= Console.ReadLine;
        writeLine ??= Console.WriteLine;

        var ports = getAvailablePorts();

        if (ports.Length == 0)
        {
            ShowNoDevicesMessage(readLine, writeLine);
            return true;
        }

        PrintAvailableDevices(ports);

        var selectedPortIndex = promptPortSelection(ports.Length, readLine);
        var selectedPort = ports[selectedPortIndex - 1];

        try
        {
            var serialPort = configureSerialPort(selectedPort);
            openSerialPort(serialPort, selectedPort, writeLine);

            while (true)
            {
                Console.Write("Please enter a custom command to send (type exit to quit): ");

                var UARTCommand = readLine();

                if (UARTCommand == "exit")
                {
                    break;
                }
                else if (!string.IsNullOrEmpty(UARTCommand))
                {
                    var checksum = PS5UARTUtilities.CalculateChecksum(UARTCommand);

                    if (writeLineToPort != null)
                        writeLineToPort(serialPort, checksum);
                    else
                        serialPort.WriteLine(checksum);

                    var UARTLines = readUARTLines(serialPort);

                    foreach (var line in UARTLines)
                    {
                        writeLine(line);
                    }

                    readLine();
                }
                else
                {
                    writeLine("Please enter a valid command.");
                }
            }

            if (closePort != null)
                closePort(serialPort);
            else
                serialPort.Close();
        }
        catch (Exception ex)
        {
            PrintErrorMessage(ex, writeLine);
        }

        return true;
    }

    public static bool LaunchBIOSDumpSubMenu(string appTitle, Dictionary<string, string> regionMap)
    {
        // This is a sub menu for working with BIOS files. IMO it's not very elegant to have sub menus but at least this way
        // we don't have to use different apps for working with BIOS files...

        SubMenuService.RunSubMenu(appTitle, regionMap);
        return true;
    }

    public static bool ShowReadMe()
    {
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
    }

    public static bool BuyTheCod3erACoffee()
    {
        // Thanks for buying me a coffee :)
        Console.WriteLine("Thanks for buying me a coffee. I'll redirect you in your default browser...");
        PS5UARTUtilities.OpenUrl("https://www.streamelements.com/thecod3r/tip");
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
        return true;
    }

    public static bool UpdateXmlDatabase()
    {
        Console.WriteLine("Downloading latest database file. Please wait...");

        var success = PS5UARTUtilities.DownloadDatabase("http://uartcodes.com/xml.php", "errorDB.xml");

        if (success)
        {
            Console.WriteLine("Database downloaded successfully...");
            Console.WriteLine("Press Enter to continue...");
        }
        else
        {
            Console.WriteLine("Could not download the latest database file. Please ensure you're connected to the internet!");
            Console.WriteLine("Press Enter to continue...");
        }
        Console.ReadLine();
        return true;
    }

    #region Private methods for repeated logic
    private static string[] GetAvailablePorts() => SerialPort.GetPortNames();

    private static void ShowNoDevicesMessage(Func<string?>? readLine = null,
        Action<string>? writeLine = null)
    {
        writeLine ??= Console.WriteLine;
        readLine ??= Console.ReadLine;
        writeLine("No communication devices were found on this system.");
        writeLine("Please insert a UART compatible device and try again.");
        writeLine("Press Enter to continue...");
        readLine();
    }

    private static void PrintAvailableDevices(string[] ports)
    {
        Console.WriteLine("Available devices:");
        for (int i = 0; i < ports.Length; i++)
        {
            var friendlyName = PS5UARTUtilities.GetFriendlyName(ports[i]);
            Console.WriteLine($"{i + 1}. {ports[i]} - {friendlyName}");
        }
    }

    private static int PromptPortSelection(int max, Func<string?>? readLine = null)
    {
        readLine ??= Console.ReadLine;

        int selectedPortIndex;
        do
        {
            Console.WriteLine();
            Console.Write("Enter the number of the COM port you want to use: ");
        } while (!int.TryParse(readLine(), out selectedPortIndex) || selectedPortIndex < 1 || selectedPortIndex > max);

        return selectedPortIndex;
    }

    private static SerialPort ConfigureSerialPort(string portName) =>
        new(portName)
        {
            // Configure settings for the selected device
            BaudRate = 115200,// The PS5 requires a BAUD rate of 115200
            RtsEnable = true  // We need to enable ready to send (RTS) mode
        };

    private static void OpenSerialPort(SerialPort serialPort, string selectedPort,
        Action<string>? writeLine = null)
    {
        writeLine ??= Console.WriteLine;
        // Open the selected port for use
        serialPort.Open();
        // Let's display the selected port (change the color to blue) so the user is aware of what device is in use
        Console.ForegroundColor = ConsoleColor.Blue;
        writeLine("Selected port: " + PS5UARTUtilities.GetFriendlyName(selectedPort));
        // Reset the foreground color to default before proceeding
        Console.ResetColor();
    }

    private static List<string> CollectErrorLines(SerialPort serialPort)
    {
        List<string> UARTLines = new();
        for (var i = 0; i <= 10; i++)
        {
            var command = $"errlog {i}";
            var checksum = PS5UARTUtilities.CalculateChecksum(command);
            serialPort.WriteLine(checksum);

            var line = serialPort.ReadLine();

            if (!string.Equals($"{command}:{checksum:X2}", line, StringComparison.InvariantCultureIgnoreCase))
            {
                if (!line.Contains("errlog") && !UARTLines.Contains(line))
                {
                    UARTLines.Add(line);
                }
            }
        }

        return UARTLines;
    }

    private static List<string> ReadUARTLines(SerialPort serialPort)
    {
        List<string> lines = new();
        do
        {
            var line = serialPort.ReadLine();
            lines.Add(line);
        } while (serialPort.BytesToRead != 0);

        return lines;
    }

    private static void PrintErrorMessage(Exception ex,
        Action<string>? writeLine = null)
    {
        writeLine ??= Console.WriteLine;
        writeLine("An error occurred while connecting to your selected device.");
        writeLine("Error details:");
        writeLine(ex.Message);
        writeLine("Press Enter to continue...");
    }
    #endregion
}

