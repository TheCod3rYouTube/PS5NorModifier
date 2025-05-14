using PS5Lib;
using PS5NORModifier;

namespace UART_CL;

public class BIOSMenu : Menu
{
    private NORData? _norData = null;
    
    public BIOSMenu()
    {
        Title = "Edit BIOS";
        Description = "In order to work with a BIOS dump file, you will need to load it into memory first.\n" +
                      "Be sure to choose a file to work with by choosing option 1 before choosing any other option.";
        Items =
        [
            new MenuItem
            {
                Name = "Load a BIOS dump (.bin)",
                Color = ConsoleColor.Green,
                Action = LoadBIOS
            },
            new MenuItem
            {
                Name = "View BIOS Information",
                Action = ViewBIOS
            },
            new MenuItem
            {
                Name = "Convert to Digital edition",
                Action = () => ChangeEdition(Editions.Digital)
            },
            new MenuItem
            {
                Name = "Convert to Disc edition",
                Action = () => ChangeEdition(Editions.Disc)
            },
            new MenuItem
            {
                Name = "Convert to Slim edition",
                Action = () => ChangeEdition(Editions.Slim)
            },
            new MenuItem
            {
                Name = "Change serial number",
                Action = ChangeSerial
            },
            new MenuItem
            {
                Name = "Change motherboard serial number",
                Action = ChangeMoboSerial
            },
            new MenuItem
            {
                Name = "Change console model number",
                Action = ChangeModel
            },
        ];
    }

    private bool NorNull()
    {
        if (_norData != null) return false;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("You must select a .bin file to read before proceeding. Please select a valid .bin file and try again.");
        Console.ResetColor();
        return true;
    }
    
    private bool ChangeModel()
    {
        if (NorNull())
            return true;
        
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Current model: {_norData.VariantCode}");
            Console.ResetColor();

            Console.WriteLine(
                "Please enter the model you would like to set your dump file to (type 'exit' to go back): ");
            
            string newModel = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(newModel))
            {
                Console.WriteLine("Invalid model entered. The new model should be like CFI-XXXXX.");
            }
            else if (newModel.Length < 9)
            {
                Console.WriteLine(
                    "The new model you entered is invalid. The model should be 9 characters long.");
            }
            else if (!newModel.StartsWith("CFI-"))
            {
                Console.WriteLine(
                    "The new model you entered is invalid. The model should start with 'CFI-', followed by 4 numbers and a letter.");
            }
            else if (newModel == "exit")
            {
                break;
            }
            else
            {
                try
                {
                    _norData.VariantCode = newModel;
                    Console.WriteLine("The new console model you chose has been saved successfully.");                    break;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        $"An error occurred while attempting to make changes to your dump file. Please try again. {ex}");
                    Console.ResetColor();
                }
            }
        }
        
        return true;
    }
    
    private bool ChangeMoboSerial()
    {
        if (NorNull())
            return true;

        string oldSerial = _norData.MoboSerial;

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Enter the new motherboard serial number you would like to save (type 'exit' to exit): ");
            Console.ResetColor();

            string newSerial = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(newSerial))
            {
                Console.WriteLine("Invalid serial number entered. The new serial should be numbers and letters.");
            }
            else if (newSerial == oldSerial)
            {
                Console.WriteLine(
                    "The new serial number matches the old serial number. Please enter a different value and try again.");
            }
            else if (newSerial == "exit")
            {
                break;
            }
            else
            {
                try
                {
                    _norData.MoboSerial = newSerial;
                    Console.WriteLine("SUCCESS: The new motherboard serial number has been updated successfully.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        $"An error occurred while attempting to make changes to your dump file. Please try again. {ex}");
                    Console.ResetColor();
                }
            }
        }
        
        return true;
    }

    private bool ChangeSerial()
    {
        if (NorNull())
            return true;

        string oldSerial = _norData.Serial;

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Enter the new serial number you would like to save (type 'exit' to exit): ");
            Console.ResetColor();

            string newSerial = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(newSerial))
            {
                Console.WriteLine("Invalid serial number entered. The new serial should be numbers and letters.");
            }
            else if (newSerial == oldSerial)
            {
                Console.WriteLine(
                    "The new serial number matches the old serial number. Please enter a different value and try again.");
            }
            else if (newSerial == "exit")
            {
                break;
            }
            else
            {
                try
                {
                    _norData.Serial = newSerial;
                    Console.WriteLine("SUCCESS: The new serial number has been updated successfully.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        $"An error occurred while attempting to make changes to your dump file. Please try again. {ex}");
                    Console.ResetColor();
                }
            }
        }

        return true;
    }
    
    private bool ChangeEdition(Editions edition)
    {
        if (NorNull())
            return true;

        if (_norData.Edition == edition)
        {
            Console.WriteLine($"The .bin file you're working with is already a {edition} edition. No changes are needed.");
            return true;
        }
        
        try 
        {
            _norData.Edition = edition;
            Console.WriteLine("Your BIOS file has been updated successfully. The new .bin file " +
                              $"will now report to the PlayStation 5 as a '{edition} edition' console.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while converting to digital edition, please try again: \n" + ex);
        }
        
        return true;
    }

    private bool ViewBIOS()
    {
        if (NorNull())
            return true;

        long lengthBytes = new FileInfo(_norData.Path).Length;
        long lengthMiB = lengthBytes / 1024 / 1024;
                    
        Console.WriteLine("File size: " + lengthBytes + " bytes (" + lengthMiB + " MiB)");
        Console.WriteLine("PS5 Version: " + _norData.Edition); // Show the version info (disc/digital/slim)
        Console.WriteLine("Console Model: " + _norData.VariantCode); // Show the console variant (CFI-XXXX)
        Console.WriteLine("Console Serial Number: " + _norData.Serial); // Show the serial number. This serial is the one the disc drive would use
        Console.WriteLine("Motherboard Serial Number: " + _norData.MoboSerial); // Show the serial number to the motherboard. This is different to the console serial
        Console.WriteLine("WiFi Mac Address: " + _norData.WiFiMAC);
        Console.WriteLine("LAN Mac Address: " + _norData.LANMAC);
        
        return true;
    }
    
    private bool LoadBIOS()
    {
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
                break;
            }
            else if (!File.Exists(userInput))
            {
                Console.WriteLine(
                    "The file path you entered does not exist. Please enter the path to a valid .bin file.");
            }
            else if (!Path.GetExtension(userInput).Equals(".bin", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("The file you provided is not a .bin file. Please enter a valid .bin file path.");
            }
            else
            {
                long length = new FileInfo(userInput).Length;
                Console.WriteLine("Selected file: " + userInput + " - File Size: " + length + " bytes (" +
                                  length / 1024 / 1024 + " MiB)");
                _norData = new(userInput);
                break;
            }
        }
        
        return true;
    }
}