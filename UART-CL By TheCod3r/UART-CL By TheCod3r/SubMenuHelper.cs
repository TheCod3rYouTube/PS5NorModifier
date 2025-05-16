using System.Text;
using System.Text.RegularExpressions;

namespace UART_CL_By_TheCod3r;

public class SubMenuHelper
{
    public static void RunSubMenu(string appTitle,
        Dictionary<string, string> regionMap,
        Func<string>? readLine = null,
        Action<string>? writeLine = null,
        Func<Func<string?>?, Action<string>?, Func<string, bool>?, Func<string, string>?, Func<string, long>?, Action<int>?, string>? loadDumpFile = null,
        Action<string>? setConsoleTitle = null,
        string pathToDump = "")
    {
        readLine ??= Console.ReadLine!;
        writeLine ??= Console.WriteLine;
        loadDumpFile ??= LoadDumpFile;
        setConsoleTitle ??= title => Console.Title = title;


        var subMenuRunning = true;

        while (subMenuRunning)
        {
            // Check to see if we are working with a file. If so, add it to the app title so the user is aware...
            if (!string.IsNullOrEmpty(pathToDump))
            {
                Console.Title = $"{appTitle} - Working with file: {pathToDump}";
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            writeLine("In order to work with a BIOS dump file, you will need to load it into memory first.");
            writeLine("Be sure to choose a file to work with by choosing option 1 before choosing any other option...");
            Console.ResetColor();
            writeLine("");
            Console.ForegroundColor = ConsoleColor.Red;
            writeLine("Choose an option:");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            writeLine("1. Load a BIOS dump (.bin)");
            Console.ResetColor();
            writeLine("2. View BIOS information");
            writeLine("3. Convert to Digital edition");
            writeLine("4. Convert to Disc edition");
            writeLine("5. Convert to Slim edition");
            writeLine("6. Change serial number");
            writeLine("7. Change motherboard serial number");
            writeLine("8. Change console model number");
            writeLine("X. Return to previous menu");
            Console.Write("\nEnter your choice: ");

            switch (readLine())
            {
                #region Load a dump file
                case "1":
                    pathToDump = loadDumpFile(null, null, null, null, null, null);
                    break;
                #endregion
                #region View BIOS information
                case "2":
                    // First check to confirm that we've selected a file to work with
                    if (!string.IsNullOrEmpty(pathToDump)) // If the pathToDump string isn't empty or null, we can try and work with it
                    {
                        ViewBIOSInformaition(regionMap, pathToDump); 
                        break;
                    }
                    else
                    {
                        // No file has been selected. Let the user know
                        NoPathError(readLine, writeLine);
                        break;
                    }
                #endregion
                #region Convert to "Digital" edition
                case "3":
                    // First check to confirm that we've selected a file to work with
                    if (!string.IsNullOrEmpty(pathToDump)) // If the pathToDump string isn't empty or null, we can try and work with it
                    {
                        ConvertToDigital(pathToDump);
                        break;
                    }
                    else
                    {
                        // No file has been selected. Let the user know
                        NoPathError(readLine, writeLine);
                        break;
                    }
                #endregion
                #region Convert to "Disc" edition
                case "4":
                    // First check to confirm that we've selected a file to work with
                    if (!string.IsNullOrEmpty(pathToDump)) // If the pathToDump string isn't empty or null, we can try and work with it
                    {
                        ConvertToDisc(pathToDump);
                        break;
                    }
                    else
                    {
                        // No file has been selected. Let the user know
                        NoPathError(readLine, writeLine);
                        break;
                    }
                #endregion
                #region Convert to "Slim" edition
                case "5":
                    // First check to confirm that we've selected a file to work with
                    if (!string.IsNullOrEmpty(pathToDump)) // If the pathToDump string isn't empty or null, we can try and work with it
                    {
                        ConvertToSlim(pathToDump);
                        break;
                    }
                    else
                    {
                        // No file has been selected. Let the user know
                        NoPathError(readLine, writeLine);
                        break;
                    }
                #endregion
                #region Change serial number
                case "6":
                    if (!string.IsNullOrEmpty(pathToDump)) // If the pathToDump string isn't empty or null, we can try and work with it
                    {
                        ChangeSerialNumber(pathToDump);
                        break;
                    }
                    else
                    {
                        // No file has been selected. Let the user know
                        NoPathError(readLine, writeLine);
                        break;
                    }
                #endregion
                #region Change motherboard serial number
                case "7":
                    if (!string.IsNullOrEmpty(pathToDump)) // If the pathToDump string isn't empty or null, we can try and work with it
                    {
                        ChangeMotherboardSerialNumber(pathToDump);
                        break;
                    }
                    else
                    {
                        // No file has been selected. Let the user know
                        NoPathError(readLine, writeLine);
                        break;
                    }
                #endregion
                #region Change console model
                case "8":

                    if (!string.IsNullOrEmpty(pathToDump)) // If the pathToDump string isn't empty or null, we can try and work with it
                    {
                        ChangeConsoleModel(pathToDump);
                        break;
                    }
                    else
                    {
                        // No file has been selected. Let the user know
                        NoPathError(readLine, writeLine);
                        break;
                    }
                #endregion
                #region Exit sub menu
                // I put two cases here for the exit option, one for capital "X" and one for lower case.
                case "X":
                    // We should reset the app title first!
                    setConsoleTitle(appTitle);
                    subMenuRunning = false;
                    break;
                case "x":
                    // We should reset the app title first!
                    setConsoleTitle(appTitle);
                    subMenuRunning = false;
                    break;
                    #endregion
            }
        }

    }

    public static string LoadDumpFile(Func<string?>? readLine = null,
        Action<string>? writeLine = null,
        Func<string, bool>? fileExists = null,
        Func<string, string>? getExtension = null,
        Func<string, long>? getFileSize = null,
        Action<int>? sleep = null)
    {
        readLine ??= Console.ReadLine;
        writeLine ??= Console.WriteLine;
        fileExists ??= File.Exists;
        getExtension ??= Path.GetExtension;
        getFileSize ??= path => new FileInfo(path).Length;
        sleep ??= Thread.Sleep;

        writeLine("In order to work with a BIOS file, you must first choose a file to work with.");
        writeLine("This needs to be a valid .bin file containing your BIOS dump file.");
        writeLine("You will need to know the full file path of your .bin file in order to continue.");
        writeLine("");

        string? pathToDump;
        do
        {
            writeLine("Enter the full file path (type 'exit' to quit): ");
            pathToDump = readLine()?.Trim(); // Trim to remove any leading/trailing whitespace

            if (string.IsNullOrWhiteSpace(pathToDump))
            {
                writeLine("Invalid input. File path cannot be blank.");
            }
            else if (pathToDump.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                // User wants to return to the sub-menu
                break; // Exit the current method and return to the sub-menu
            }
            else if (!fileExists(pathToDump))
            {
                writeLine("The file path you entered does not exist. Please enter the path to a valid .bin file.");
            }
            else if (getExtension(pathToDump) != ".bin")
            {
                writeLine("The file you provided is not a .bin file. Please enter a valid .bin file path.");
            }
            else
            {
                // User provided a valid .bin file path, you can proceed with it
                var length = getFileSize(pathToDump);
                writeLine($"Selected file: {pathToDump} - File Size: {length} bytes ({length / 1024 / 1024}MB)");
                sleep(1000);
                break; // Exit the loop
            }
        } while (true);

        return pathToDump!;
    }

    public static void ViewBIOSInformaition(Dictionary<string, string> regionMap,
        string pathToDump,
        Func<string, BinaryReader>? readerFactory = null,
        Action<string>? writeLine = null,
        Func<string>? readLine = null,
        long? fileLengthBytesOverride = null)
    {
        writeLine ??= Console.WriteLine;
        readLine ??= Console.ReadLine;
        readerFactory ??= path => new BinaryReader(new FileStream(path, FileMode.Open));

        // Let's try and get some information from the .bin file
        var lengthBytes = fileLengthBytesOverride ?? new FileInfo(pathToDump).Length; // File size of the .bin file in bytes
        var lengthMB = lengthBytes / 1024 / 1024; // File size of the .bin file in MB

        // First, declare some variables to store for later use
        string? pS5Version;
        string? consoleModelInfo;
        string? modelInfo;
        string? consoleSerialNumber;
        string? motherboardSerialNumber;

        // Set the offsets of the BIN file
        long offsetOne = 0x1c7010;
        long serialOffset = 0x1c7210;
        long variantOffset = 0x1c7226;
        long moboSerialOffset = 0x1C7200;
        long wiFiMacOffset = 0x1C73C0;
        long lANMacOffset = 0x1C4020;
        string? variantValue = null;

        // Declare the offset values (set them to null for now)
        string? offsetOneValue;
        string? offsetTwoValue;
        string? serialValue;
        string? moboSerialValue;
        string? wiFiMacValue;
        string? lANMacValue;

        #region Get PS5 version
        try
        {
            using var reader = readerFactory(pathToDump);
            //Set the position of the reader
            reader.BaseStream.Position = offsetOne;
            //Read the offset
            offsetOneValue = BitConverter.ToString(reader.ReadBytes(12)).Replace("-", null);
            reader.Close();
        }
        catch
        {
            // Obviously this value is invalid, so null the value and move on
            offsetOneValue = null;
        }

        try
        {
            using var reader = readerFactory(pathToDump);
            //Set the position of the reader
            reader.BaseStream.Position = offsetOne;
            //Read the offset
            offsetTwoValue = BitConverter.ToString(reader.ReadBytes(12)).Replace("-", null);
            reader.Close();
        }
        catch
        {
            // Obviously this value is invalid, so null the value and move on
            offsetTwoValue = null;
        }

        if (offsetOneValue.Contains("22020101"))
        {
            pS5Version = "Disc Edition";
        }
        else if (offsetTwoValue.Contains("22030101"))
        {
            pS5Version = "Digital Edition";
        }
        else if (offsetOneValue.Contains("22010101") || offsetTwoValue.Contains("22010101"))
        {
            pS5Version = "Slim Edition";
        }
        else
        {
            pS5Version = "Unknown Model";
        }
        #endregion

        #region Get console model and region
        try
        {
            using var reader = readerFactory(pathToDump);
            reader.BaseStream.Position = variantOffset;
            variantValue = BitConverter.ToString(reader.ReadBytes(19)).Replace("-", null).Replace("FF", null);
        }
        catch
        {
            // Catch any exceptions and ignore, setting variantValue to null
            variantValue = null;
        }

        consoleModelInfo = PS5UARTUtilities.HexStringToString(variantValue);
        var region = "Unknown Region";

        if (consoleModelInfo != null && consoleModelInfo.Length >= 3)
        {
            var suffix = consoleModelInfo[^3..];
            if (regionMap.ContainsKey(suffix))
            {
                region = regionMap[suffix];
            }
        }

        modelInfo = $"{PS5UARTUtilities.HexStringToString(variantValue)} - {region}";
        #endregion

        #region Get Console Serial Number
        try
        {
            using var reader = readerFactory(pathToDump);
            //Set the position of the reader
            reader.BaseStream.Position = serialOffset;
            //Read the offset
            serialValue = BitConverter.ToString(reader.ReadBytes(17)).Replace("-", null);
            reader.Close();
        }
        catch
        {
            // Obviously this value is invalid, so null the value and move on
            serialValue = null;
        }

        consoleSerialNumber = !string.IsNullOrEmpty(serialValue) ? PS5UARTUtilities.HexStringToString(serialValue) : "Unknown S/N";
        #endregion

        #region Get Motherboard Serial Number
        try
        {
            using var reader = readerFactory(pathToDump);
            //Set the position of the reader
            reader.BaseStream.Position = moboSerialOffset;
            //Read the offset
            moboSerialValue = BitConverter.ToString(reader.ReadBytes(16)).Replace("-", null);
            reader.Close();
        }
        catch
        {
            // Obviously this value is invalid, so null the value and move on
            moboSerialValue = null;
        }

        motherboardSerialNumber = !string.IsNullOrEmpty(moboSerialValue) ? PS5UARTUtilities.HexStringToString(moboSerialValue) : "Unknown S/N";
        #endregion

        #region Extract WiFi Mac Address
        try
        {
            using var reader = readerFactory(pathToDump);
            //Set the position of the reader
            reader.BaseStream.Position = wiFiMacOffset;
            //Read the offset
            wiFiMacValue = BitConverter.ToString(reader.ReadBytes(6));
            reader.Close();
        }
        catch
        {
            // Obviously this value is invalid, so null the value and move on
            wiFiMacValue = null;
        }

        if (string.IsNullOrEmpty(wiFiMacValue))
        {
            wiFiMacValue = "Unknown Mac Address";
        }
        #endregion

        #region Extract LAN Mac Address
        try
        {
            using var reader = readerFactory(pathToDump);
            //Set the position of the reader
            reader.BaseStream.Position = lANMacOffset;
            //Read the offset
            lANMacValue = BitConverter.ToString(reader.ReadBytes(6));
            reader.Close();
        }
        catch
        {
            lANMacValue = null;
        }

        if (string.IsNullOrEmpty(lANMacValue))
        {
            lANMacValue = "Unknown Mac Address";
        }
        #endregion

        #region Start show data for the BIOS dump
        // Start showing the data we've found to the user
        writeLine($"File size: {lengthBytes} bytes ({lengthMB}MB)"); // The file size of the .bin file
        writeLine($"PS5 Version: {pS5Version}"); // Show the version info (disc/digital/slim)
        writeLine($"Console Model: {modelInfo}"); // Show the console variant (CFI-XXXX)
        writeLine($"Console Serial Number: {consoleSerialNumber}"); // Show the serial number. This serial is the one the disc drive would use
        writeLine($"Motherboard Serial Number: {motherboardSerialNumber}"); // Show the serial number to the motherboard. This is different to the console serial
        writeLine($"WiFi Mac Address: {wiFiMacValue}"); // WiFi mac address
        writeLine($"LAN Mac Address: {lANMacValue}"); // LAN mac address
        writeLine(""); // A blank line
        writeLine("Press Enter to continue...");
        readLine();
        #endregion
    }

    public static void ConvertToDigital(string pathToDump,
        Func<string, string> confirmPrompt = null,
        Action<string> writeLine = null,
        Func<string> readLine = null,
        Action<string, string, string, string, string, string> convertConsoleType = null)
    {
        confirmPrompt ??= ConfirmConsoleTypeChange;
        writeLine ??= Console.WriteLine;
        readLine ??= Console.ReadLine;
        convertConsoleType ??= ConvertConsoleType;

        var confirmed = false;
        while (!confirmed)
        {
            var changeConfirmation = confirmPrompt("Digital");

            // Check user input
            if (changeConfirmation == "yes")
            {
                // Declare offsets to obtain current version info
                long offsetOne = 0x1c7010;
                long offsetTwo = 0x1c7030;
                string offsetOneValue = null;
                string offsetTwoValue = null;

                // Get PS5 version
                try
                {
                    using var reader = new BinaryReader(new FileStream(pathToDump, FileMode.Open));
                    reader.BaseStream.Position = offsetOne;
                    offsetOneValue = BitConverter.ToString(reader.ReadBytes(4)).Replace("-", string.Empty);

                    reader.BaseStream.Position = offsetTwo;
                    offsetTwoValue = BitConverter.ToString(reader.ReadBytes(4)).Replace("-", string.Empty);
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur while reading the file
                    writeLine($"Error reading the binary file: {ex.Message}");
                    writeLine("Please try again! Press Enter to continue...");
                    readLine();
                    break;
                }

                if (offsetOneValue.Contains("22030101") || offsetTwoValue.Contains("22030101"))
                {
                    // The BIOS file already contains digital edition flags
                    writeLine("The .bin file you're working with is already a digital edition. No changes are needed.");
                    writeLine("Press Enter to continue...");
                    readLine();
                    break;
                }
                else
                {
                    // Modify the values to set the file as "Digital Edition"
                    try
                    {
                        convertConsoleType("22010101", "22030101", "22020101", "22030101", "digital edition", pathToDump);
                        confirmed = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that occur while writing to the file
                        writeLine($"Error updating the binary file: {ex.Message}");
                        writeLine("Please try again! Press Enter to continue...");
                        readLine();
                        break;
                    }
                }
            }
            else if (changeConfirmation == "no")
            {
                // User cancelled. Break the loop and go back to the menu!
                break;
            }
            else
            {
                writeLine("Invalid input. Please type 'yes' to confirm or 'no' to cancel.");
            }
        }
    }

    public static void ConvertToDisc(string pathToDump,
        Func<string, string> confirmPrompt = null,
        Action<string> writeLine = null,
        Func<string> readLine = null,
        Action<string, string, string, string, string, string> convertConsoleType = null)
    {
        confirmPrompt ??= ConfirmConsoleTypeChange;
        writeLine ??= Console.WriteLine;
        readLine ??= Console.ReadLine;
        convertConsoleType ??= ConvertConsoleType;

        var confirmed = false;
        while (!confirmed)
        {
            var changeConfirmation = confirmPrompt("Disc");

            // Check user input
            if (changeConfirmation == "yes")
            {
                // Declare offsets to obtain current version info
                long offsetOne = 0x1c7010;
                long offsetTwo = 0x1c7030;
                string offsetOneValue = null;
                string offsetTwoValue = null;

                // Get PS5 version
                try
                {
                    using var reader = new BinaryReader(new FileStream(pathToDump, FileMode.Open));
                    reader.BaseStream.Position = offsetOne;
                    offsetOneValue = BitConverter.ToString(reader.ReadBytes(4)).Replace("-", string.Empty);

                    reader.BaseStream.Position = offsetTwo;
                    offsetTwoValue = BitConverter.ToString(reader.ReadBytes(4)).Replace("-", string.Empty);
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur while reading the file
                    writeLine("Error reading the binary file: " + ex.Message);
                    writeLine("Please try again! Press Enter to continue...");
                    readLine();
                    break;
                }

                if (offsetOneValue.Contains("22020101") || offsetTwoValue.Contains("22020101"))
                {
                    // The BIOS file already contains disc edition flags
                    writeLine("The .bin file you're working with is already a disc edition. No changes are needed.");
                    writeLine("Press Enter to continue...");
                    readLine();
                    break;
                }
                else
                {
                    // Modify the values to set the file as "Disc Edition"
                    try
                    {
                        convertConsoleType("22010101", "22020101", "22030101", "22020101", "disc edition", pathToDump);
                        confirmed = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that occur while writing to the file
                        writeLine("Error updating the binary file: " + ex.Message);
                        writeLine("Please try again! Press Enter to continue...");
                        readLine();
                        break;
                    }
                }
            }
            else if (changeConfirmation == "no")
            {
                // User cancelled. Break the loop and go back to the menu!
                break;
            }
            else
            {
                writeLine("Invalid input. Please type 'yes' to confirm or 'no' to cancel.");
            }
        }
    }

    public static void ConvertToSlim(string pathToDump,
        Func<string, string> confirmPrompt = null,
        Action<string> writeLine = null,
        Func<string> readLine = null,
        Action<string, string, string, string, string, string> convertConsoleType = null)
    {
        confirmPrompt ??= ConfirmConsoleTypeChange;
        writeLine ??= Console.WriteLine;
        readLine ??= Console.ReadLine;
        convertConsoleType ??= ConvertConsoleType;

        var confirmed = false;
        while (!confirmed)
        {
            var changeConfirmation = confirmPrompt("Slim");

            // Check user input
            if (changeConfirmation == "yes")
            {
                string PS5Version; // String to store the PS5 version

                // Declare offsets to obtain current version info
                long offsetOne = 0x1c7010;
                long offsetTwo = 0x1c7030;
                string offsetOneValue = null;
                string offsetTwoValue = null;

                // Get PS5 version
                try
                {
                    using var reader = new BinaryReader(new FileStream(pathToDump, FileMode.Open));
                    reader.BaseStream.Position = offsetOne;
                    offsetOneValue = BitConverter.ToString(reader.ReadBytes(4)).Replace("-", string.Empty);

                    reader.BaseStream.Position = offsetTwo;
                    offsetTwoValue = BitConverter.ToString(reader.ReadBytes(4)).Replace("-", string.Empty);
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur while reading the file
                    writeLine("Error reading the binary file: " + ex.Message);
                    writeLine("Please try again! Press Enter to continue...");
                    readLine();
                    break;
                }

                if (offsetOneValue.Contains("22010101") || offsetTwoValue.Contains("22010101"))
                {
                    // The BIOS file already contains slim edition flags
                    writeLine("The .bin file you're working with is already a slim edition. No changes are needed.");
                    writeLine("Press Enter to continue...");
                    readLine();
                    break;
                }
                else
                {
                    // Modify the values to set the file as "Slim Edition"
                    try
                    {
                        convertConsoleType("22020101", "22010101", "22030101", "22010101", "slim edition", pathToDump);
                        confirmed = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that occur while writing to the file
                        writeLine("Error updating the binary file: " + ex.Message);
                        writeLine("Please try again! Press Enter to continue...");
                        readLine();
                        break;
                    }
                }
            }
            else if (changeConfirmation == "no")
            {
                // User cancelled. Break the loop and go back to the menu!
                break;
            }
            else
            {
                writeLine("Invalid input. Please type 'yes' to confirm or 'no' to cancel.");
            }
        }
    }

    public static void ChangeSerialNumber(string pathToDump,
        Func<string> readLine = null,
        Action<string> writeLine = null,
        Action<string> waitForContinue = null,
        Func<string, bool, string?, Func<string>?, Action<string>?, bool> updateSerialNumber = null)
    {
        readLine ??= Console.ReadLine;
        writeLine ??= Console.WriteLine;
        waitForContinue ??= _ => Console.ReadLine();
        updateSerialNumber ??= UpdateSerialNumber;
        // Create a true false to allow us to loop until the user changes the serial or cancels the operation
        var jobDone = false;

        while (!jobDone)
        {
            // Set the serial number offset and value
            long serialOffset = 0x1c7210;
            string oldSerial = null;
            try
            {
                using var reader = new BinaryReader(new FileStream(pathToDump, FileMode.Open));
                //Set the position of the reader
                reader.BaseStream.Position = serialOffset;
                //Read the offset
                oldSerial = Encoding.UTF8.GetString(reader.ReadBytes(17));
            }
            catch
            {
                // Obviously this value is invalid, so null the value and move on
                oldSerial = null;
            }

            jobDone = updateSerialNumber(pathToDump, jobDone, oldSerial, readLine, writeLine);
        }
    }

    public static void ChangeMotherboardSerialNumber(string pathToDump,
        Action<string>? writeLine = null,
        Func<string>? readLine = null,
        Action<string, string, Func<string>?, Action<string>?>? updateMotherboardSerialNumber = null)
    {
        writeLine ??= Console.WriteLine;
        readLine ??= Console.ReadLine;
        updateMotherboardSerialNumber ??= UpdateMotherboardSerialNumber;

        // Declare variables to store console serial
        long moboOffset = 0x1C7200;
        string moboValue = null;
        string motherboardSerial;

        try
        {
            using var reader = new BinaryReader(new FileStream(pathToDump, FileMode.Open));
            reader.BaseStream.Position = moboOffset;
            moboValue = BitConverter.ToString(reader.ReadBytes(16)).Replace("-", null);
        }
        catch
        {
            // Catch any exceptions and ignore, setting MotherboardSerial to null
        }

        motherboardSerial = PS5UARTUtilities.HexStringToString(moboValue);

        if (!string.IsNullOrEmpty(moboValue) && !string.IsNullOrEmpty(motherboardSerial))
        {
            updateMotherboardSerialNumber(pathToDump, motherboardSerial, readLine, writeLine);
        }
        else
        {
            // No file has been selected. Let the user know
            Console.ForegroundColor = ConsoleColor.Red;
            writeLine("Could not parse your selected .bin file. Please ensure your selected file is a valid PlayStation 5 file and try again.");
            Console.ResetColor();
            writeLine("");
            writeLine("Press Enter to continue...");
            readLine();
        }
    }

    public static void ChangeConsoleModel(string pathToDump,
        Action<string>? writeLine = null,
        Func<string>? readLine = null,
        Action<string, string, string?>? updateModelOrSerial = null)
    {
        writeLine ??= Console.WriteLine;
        readLine ??= Console.ReadLine;
        updateModelOrSerial ??= UpdateModelOrSerial;

        // Declare variables to store console model
        long variantOffset = 0x1c7226;
        string variantValue = null;
        string consoleModel;

        try
        {
            using var reader = new BinaryReader(new FileStream(pathToDump, FileMode.Open));
            reader.BaseStream.Position = variantOffset;
            variantValue = BitConverter.ToString(reader.ReadBytes(19)).Replace("-", string.Empty).Replace("FF", string.Empty);
        }
        catch
        {
            // Catch any exceptions and ignore, setting variantValue to null
        }

        consoleModel = PS5UARTUtilities.HexStringToString(variantValue);

        if (!string.IsNullOrEmpty(variantValue) && !string.IsNullOrEmpty(consoleModel))
        {
            // Create a loop to prevent the app from returning to the main menu
            bool isDone = false;
            while (!isDone)
            {
                // Show the current model to the user
                Console.ForegroundColor = ConsoleColor.Blue;
                writeLine("Current model: " + consoleModel);
                Console.ResetColor();

                // Ask the user to enter the new model
                writeLine("Please enter the model you would like to set your dump file to (type 'exit' to go back): ");

                var newModel = readLine();

                if (string.IsNullOrEmpty(newModel))
                {
                    // The user did not enter a valid string
                    writeLine("Please enter a valid model number to continue...");
                }
                else if (newModel == "exit")
                {
                    // The user wants to exit this menu
                    break;
                }
                else if (newModel.Length < 9)
                {
                    // The entered model is an invalid length
                    writeLine("The new model you entered is invalid. The model should be 9 characters long starting with 'CFI-', followed by 4 numbers and a letter.");
                }
                else if (!newModel.StartsWith("CFI-"))
                {
                    writeLine("The new model you entered is invalid. The model should be 9 characters long starting with 'CFI-', followed by 4 numbers and a letter.");
                }
                else
                {
                    // Everything seems OK. Now we can change the model
                    try
                    {
                        updateModelOrSerial(pathToDump, consoleModel, newModel);

                        writeLine("The new console model you have chosen has been saved successfully.");
                        writeLine("Press Enter to continue...");
                        readLine();
                        isDone = true;
                        break;
                    }
                    catch (ArgumentException ex)
                    {
                        writeLine("An error occurred while writing to the BIOS dump. Please try again..." + ex.Message);
                        writeLine("Press Enter to continue...");
                        readLine();
                        break;
                    }
                }
            }
        }
        else
        {
            // No file has been selected. Let the user know
            Console.ForegroundColor = ConsoleColor.Red;
            writeLine("Could not parse your selected .bin file. Please ensure your selected file is a valid PlayStation 5 file and try again.");
            Console.ResetColor();
            writeLine("");
            writeLine("Press Enter to continue...");
            readLine();
        }
    }

    private static bool UpdateSerialNumber(string pathToDump,
        bool jobDone,
        string? oldSerial,
        Func<string> readLine = null,
        Action<string> writeLine = null)
    {
        readLine ??= Console.ReadLine;
        writeLine ??= Console.WriteLine;

        var newSerialValid = false;
        while (!newSerialValid)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            writeLine("Enter the new serial number you would like to save (type 'exit' to exit): ");
            Console.ResetColor();

            var newSerial = readLine().Trim();

            if (newSerial == "")
            {
                // The serial number is blank
                writeLine("Invalid serial number entered. The new serial should be characters and letters.");
            }
            else if (newSerial == "exit")
            {
                jobDone = true;
                break;
            }
            else if (newSerial == oldSerial)
            {
                writeLine("The new serial number matches the old serial number. Please enter a different value and try again...");
            }
            else
            {
                try
                {
                    byte[] existingFile;
                    using var reader = new BinaryReader(new FileStream(pathToDump, FileMode.Open));
                    existingFile = reader.ReadBytes((int)reader.BaseStream.Length);

                    var oldSerialBytes = Encoding.UTF8.GetBytes(oldSerial);
                    var newSerialBytes = Encoding.UTF8.GetBytes(newSerial);

                    // Ensure the new serial number is either padded or truncated to fit 17 characters
                    var newSerialBytesPadded = new byte[17];
                    Array.Copy(newSerialBytes, newSerialBytesPadded, Math.Min(newSerialBytes.Length, 17));

                    // Find the index of the old serial number in the file
                    var index = PS5UARTUtilities.PatternAt(existingFile, oldSerialBytes).FirstOrDefault();

                    if (index != -1)
                    {
                        // Replace the old serial number with the new one
                        for (int i = 0; i < newSerialBytesPadded.Length; i++)
                        {
                            existingFile[index + i] = newSerialBytesPadded[i];
                        }

                        // Write modified bytes back to the file
                        using var writer = new BinaryWriter(new FileStream(pathToDump, FileMode.Create));
                        writer.Write(existingFile);

                        writeLine("SUCCESS: The new serial number has been updated successfully. Press enter to continue...");
                        readLine();
                        jobDone = true;
                        break;
                    }
                    else
                    {
                        writeLine("Failed to find the old serial number in the file. Aborting...");
                        jobDone = true;
                    }
                }
                catch (Exception)
                {
                    // Something went wrong. Notify the user
                    Console.ForegroundColor = ConsoleColor.Red;
                    writeLine("An error occurred while attempting to make changes to your dump file. Please try again.");
                    Console.ResetColor();
                }
            }
        }

        return jobDone;
    }

    private static void UpdateMotherboardSerialNumber(string pathToDump,
        string motherboardSerial,
        Func<string>? readLine = null,
        Action<string>? writeLine = null)
    {
        readLine ??= Console.ReadLine;
        writeLine ??= Console.WriteLine;

        // Create a loop to prevent the app from returning to the main menu
        var isDone = false;
        while (!isDone)
        {
            // Show the current motherboard serial to the user
            Console.ForegroundColor = ConsoleColor.Blue;
            writeLine("Current motherboard serial: " + motherboardSerial);
            Console.ResetColor();

            // Ask the user to enter the new serial number
            writeLine("Please enter a new motherboard serial number (type 'exit' to go back): ");

            var newSerial = readLine();

            if (string.IsNullOrEmpty(newSerial))
            {
                // The user did not enter a valid string
                writeLine("Please enter a valid model number to continue...");
            }
            else if (newSerial == "exit")
            {
                // The user wants to exit this menu
                break;
            }
            else if (newSerial.Length != 16)
            {
                // The entered serial number is an invalid length
                writeLine("The new motherboard serial you entered is invalid. The motherboard serial should be exactly 16 characters in length.");
            }
            else
            {
                // Everything seems OK. Now we can change the serial number
                try
                {
                    UpdateModelOrSerial(pathToDump, motherboardSerial, newSerial);

                    writeLine("The new motherboard serial number you entered has been saved successfully.");
                    writeLine("Press Enter to continue...");
                    readLine();
                    isDone = true;
                    break;

                }
                catch (ArgumentException ex)
                {
                    writeLine("An error occurred while writing to the BIOS dump. Please try again..." + ex.Message);
                    writeLine("Press Enter to continue...");
                    readLine();
                    break;
                }
            }
        }
    }

    private static void UpdateModelOrSerial(string pathToDump, string motherboardSerial, string? newSerial)
    {
        var oldSerial = Encoding.UTF8.GetBytes(motherboardSerial);
        var oldSerialHex = Convert.ToHexString(oldSerial);

        var newSerialBytes = Encoding.UTF8.GetBytes(newSerial);
        var newSerialHex = Convert.ToHexString(newSerialBytes);

        var find = PS5UARTUtilities.ConvertHexStringToByteArray(Regex.Replace(oldSerialHex, "0x|[ ,]", string.Empty).Normalize().Trim());
        var replace = PS5UARTUtilities.ConvertHexStringToByteArray(Regex.Replace(newSerialHex, "0x|[ ,]", string.Empty).Normalize().Trim());

        var bytes = File.ReadAllBytes(pathToDump);
        foreach (var index in PS5UARTUtilities.PatternAt(bytes, find))
        {
            for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace.Length; i++, replaceIndex++)
            {
                bytes[i] = replace[replaceIndex];
            }
            File.WriteAllBytes(pathToDump, bytes);
        }
    }

    private static void ConvertConsoleType(string findHex1, string replaceHex1, string findHex2, string replaceHex2, string consoleType, string pathToDump)
    {
        var find = PS5UARTUtilities.ConvertHexStringToByteArray(Regex.Replace(findHex1, "0x|[ ,]", string.Empty).Normalize().Trim());
        var replace = PS5UARTUtilities.ConvertHexStringToByteArray(Regex.Replace(replaceHex1, "0x|[ ,]", string.Empty).Normalize().Trim());

        var bytes = File.ReadAllBytes(pathToDump);
        foreach (int index in PS5UARTUtilities.PatternAt(bytes, find))
        {
            for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace.Length; i++, replaceIndex++)
            {
                bytes[i] = replace[replaceIndex];
            }
            File.WriteAllBytes(pathToDump, bytes);
        }

        var find2 = PS5UARTUtilities.ConvertHexStringToByteArray(Regex.Replace(findHex2, "0x|[ ,]", string.Empty).Normalize().Trim());
        var replace2 = PS5UARTUtilities.ConvertHexStringToByteArray(Regex.Replace(replaceHex2, "0x|[ ,]", string.Empty).Normalize().Trim());

        foreach (var index in PS5UARTUtilities.PatternAt(bytes, find2))
        {
            for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace2.Length; i++, replaceIndex++)
            {
                bytes[i] = replace2[replaceIndex];
            }
            File.WriteAllBytes(pathToDump, bytes);
        }

        Console.WriteLine("Your BIOS file has been updated successfully. The new .bin file will now report to");
        Console.WriteLine($"the PlayStation 5 as a '{consoleType}' console.");
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    private static string ConfirmConsoleTypeChange(string consoleType)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Are you sure you want to set the console as \"{consoleType}\" edition?");
        Console.ResetColor();

        Console.WriteLine("Type 'yes' to confirm or 'no' to cancel.");

        return Console.ReadLine().Trim().ToLower();
    }

    private static void NoPathError(Func<string>? readLine = null, Action<string>? writeLine = null)
    {
        writeLine ??= Console.WriteLine;
        readLine ??= Console.ReadLine;
        Console.ForegroundColor = ConsoleColor.Red;
        writeLine("You must select a .bin file to read before proceeding. Please select a valid .bin file and try again.");
        Console.ResetColor();
        writeLine("");
        writeLine("Press Enter to continue...");
        readLine();
    }
}
