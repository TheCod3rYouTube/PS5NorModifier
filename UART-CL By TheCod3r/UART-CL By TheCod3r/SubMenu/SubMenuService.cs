namespace UART_CL_By_TheCod3r.SubMenu;

public static class SubMenuService
{
    public static void RunSubMenu(string appTitle,
        Dictionary<string, string> regionMap,
        Func<string>? readLine = null,
        Action<string>? writeLine = null,
        Func<Func<string?>?, Action<string>?, Func<string, bool>?, Func<string, string>?, Action<int>?, string>? loadDumpFile = null,
        Action<string>? setConsoleTitle = null,
        string pathToDump = "")
    {
        readLine ??= Console.ReadLine!;
        writeLine ??= Console.WriteLine;
        loadDumpFile ??= SubMenuHelper.LoadDumpFile;
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
                    pathToDump = loadDumpFile(null, null, null, null, null);
                    break;
                #endregion
                #region View BIOS information
                case "2":
                    // First check to confirm that we've selected a file to work with
                    if (!string.IsNullOrEmpty(pathToDump)) // If the pathToDump string isn't empty or null, we can try and work with it
                    {
                        SubMenuHelper.ViewBIOSInformaition(regionMap, pathToDump);
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
                        SubMenuHelper.ConvertToDigital(pathToDump);
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
                        SubMenuHelper.ConvertToDisc(pathToDump);
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
                        SubMenuHelper.ConvertToSlim(pathToDump);
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
                        SubMenuHelper.ChangeSerialNumber(pathToDump);
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
                        SubMenuHelper.ChangeMotherboardSerialNumber(pathToDump);
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
                        SubMenuHelper.ChangeConsoleModel(pathToDump);
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
