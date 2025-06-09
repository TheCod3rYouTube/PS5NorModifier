using System.Diagnostics;
using System.IO.Ports;
using System.Management;
using PS5Lib;

namespace UART_CL;

public class UARTMenu : Menu
{
    private UART _uart = new();
    private MenuItem _toggleConnection;
    private BIOSMenu _biosMenu = new();

    public UARTMenu()
    {
        Title = "UART Menu";
        Description = "Select an option to perform UART operations.";
        Items =
        [
            new MenuItem
            {
                Name = "Get error codes from PS5",
                Action = GetErrors
            },

            new MenuItem
            {
                Name = "Clear error codes on PS5",
                Action = ClearErrors
            },

            new MenuItem
            {
                Name = "Enter custom UART command",
                Action = EnterCustomCommand
            },
            
            new MenuItem
            {
                Name = "BIOS dump tools",
                Action = BiosDumpTools
            },
            
            new MenuItem
            {
                Name = "View guide",
                Action = ViewGuide
            },
            
            new MenuItem
            {
                Name = "Buy TheCod3r a coffee",
                Color = ConsoleColor.Green,
                Action = OpenDonationPage
            },
            
            new MenuItem
            {
                Name = "Update error database",
                Action = UpdateDatabase
            },
            
            _toggleConnection = new()
            {
                Name = "Connect to COM port",
                Action = ToggleConnection
            }
        ];
    }

    private bool Disconnected()
    {
        if (_uart.IsConnected) return false;
        Console.WriteLine("You must connect to a UART device before proceeding. Please select a device and try again.");
        return true;
    }
    
    private bool GetErrors()
    {
        if (Disconnected()) return true;

        try
        {
            for (var i = 0; i <= 10; i++)
            {
                var result = _uart.GetErrorLog(i).Result;
                if (!result.success)
                {
                    Console.WriteLine("An error occurred while reading error codes from UART, at page " + i + ". Please try again.");
                    Console.WriteLine(result.errors[0]);
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
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while connecting to your selected device.");
            Console.WriteLine("Error details:");
            Console.WriteLine(ex.Message);
        }
        
        return true;
    }
    
    private bool ClearErrors()
    {
        if (Disconnected()) return true;

        try
        {
            bool success = _uart.ClearErrorLog(out string result);
                    
            Console.WriteLine(result);
            Console.WriteLine(success
                ? $"Error codes cleared successfully. {result}"
                : $"An error occurred while clearing error codes. Please try again. {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while connecting to your selected device.");
            Console.WriteLine("Error details:");
            Console.WriteLine(ex.Message);
        }

        return true;
    }

    private bool EnterCustomCommand()
    {
        if (Disconnected()) return true;

        try
        {
            while (true)
            {
                Console.Write("Please enter a custom command to send (type exit to quit): ");
                string uartCommand = Console.ReadLine() ?? string.Empty;

                if (uartCommand == "exit")
                {
                    break;
                }

                if (string.IsNullOrEmpty(uartCommand))
                {
                    Console.WriteLine("Please enter a valid command.");
                }
                else
                {
                    bool success = _uart.SendArbitraryCommand(uartCommand, out string result);

                    Console.ForegroundColor = success
                        ? ConsoleColor.White
                        : ConsoleColor.Red;

                    Console.WriteLine(result);
                    Console.ResetColor();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while connecting to your selected device.");
            Console.WriteLine("Error details:");
            Console.WriteLine(ex.Message);
        }

        return true;
    }
    
    private bool BiosDumpTools()
    {
        _biosMenu.Open(true);
        return false;
    }
    
    private static bool ViewGuide()
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
        return true;
    }

    private static bool OpenDonationPage()
    {
        // Thanks for buying me a coffee :)
        Console.WriteLine("Thanks for buying me a coffee. I'll redirect you in your default browser...");
        OpenUrl("https://www.streamelements.com/thecod3r/tip");

        return false;
    }
    
    private static bool UpdateDatabase()
    {
        Console.WriteLine("Downloading latest database file. Please wait...");

        try
        {
            UART.DownloadErrorDB().Wait();
            Console.WriteLine("Database downloaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Could not download the latest database file. Please ensure you're connected to the internet! " + ex);
        }

        return true;
    }
    
    private bool ToggleConnection()
    {
        if (_uart.IsConnected)
        {
            _uart.Disconnect();
            _toggleConnection.Name = "Connect to COM port";
            Console.WriteLine("Disconnected from COM port.");
            return true;
        }

        string selectedPort;
        string[] ports = SerialPort.GetPortNames();

        if (ports.Length == 0)
        {
            Console.WriteLine("No communication devices were found on this system.");
            Console.WriteLine("Please insert a UART compatible device and try again.");
        }

        Console.WriteLine("Available devices:");
        for (int i = 0; i < ports.Length; i++)
        {
            string friendlyName = GetFriendlyName(ports[i]);
            Console.WriteLine($"{i + 1}. {ports[i]} - {friendlyName}");
        }

        int selectedPortIndex;
        do
        {
            Console.WriteLine("Enter the number of the COM port you want to use: ");
        } while (!int.TryParse(Console.ReadLine(), out selectedPortIndex) || selectedPortIndex < 1 ||
                 selectedPortIndex > ports.Length);

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
        }

        return true;
    }
    
    private static void OpenUrl(string url)
    {
        Thread.Sleep(2000);
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error opening URL: {ex.Message}");
        }
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