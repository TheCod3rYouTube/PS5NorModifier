using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Net;
using System.Text;
using System.Xml;

namespace UART_CL_By_TheCod3r;

public static class PS5UARTUtilities
{
    #region Checksum generation
    public static string CalculateChecksum(string str)
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

    #region Hex conversions
    public static string HexStringToString(string hexString)
    {
        if (hexString == null || (hexString.Length & 1) == 1)
        {
            throw new ArgumentException();
        }
        var sb = new StringBuilder();
        for (var i = 0; i < hexString.Length; i += 2)
        {
            var hexChar = hexString.Substring(i, 2);
            sb.Append((char)Convert.ToByte(hexChar, 16));
        }
        return sb.ToString();
    }

    public static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
    {
        if (pattern.Length == 0)
        {
            yield break; // Return an empty sequence
        }

        for (var i = 0; i < source.Length; i++)
        {
            if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
            {
                yield return i;
            }
        }
    }

    public static byte[] ConvertHexStringToByteArray(string hexString)
    {
        if (hexString.Length % 2 != 0)
        {
            throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
        }

        byte[] data = new byte[hexString.Length / 2];
        for (int index = 0; index < data.Length; index++)
        {
            string byteValue = hexString.Substring(index * 2, 2);
            data[index] = Convert.ToByte(byteValue, 16); // Parse hex string directly
        }

        return data;
    }
    #endregion

    #region Error parsing (via XML database)

    // When fetching errors from the PS5 we want to be able to convert the received codes into readable text to make it easier
    // for the user to understand what the problem is. By the time this function is called we should have an up to date XML
    // database to compare error codes with.
    public static string ParseErrors(string errorCode)
    {
        var results = "";

        if (string.IsNullOrEmpty(errorCode))
        {
            return "No error code given. " + errorCode;
        }

        try
        {
            if (File.Exists("errorDB.xml"))
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load("errorDB.xml");

                var root = xmlDoc.DocumentElement;

                if (root.Name == "errorCodes")
                {
                    if (root.ChildNodes.Count == 0)
                    {
                        results = "No result found for error code " + errorCode;
                    }
                    else
                    {
                        foreach (XmlNode errorCodeNode in root.ChildNodes)
                        {
                            if (errorCodeNode.Name == "errorCode")
                            {
                                var errorCodeValue = errorCodeNode.SelectSingleNode("ErrorCode")?.InnerText;
                                var description = errorCodeNode.SelectSingleNode("Description")?.InnerText;

                                if (errorCodeValue == errorCode)
                                {
                                    results = "Error code: " + errorCodeValue + Environment.NewLine + "Description: " + description;
                                    break;
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
    public static string GetFriendlyName(string portName)
    {
        // Handle null port
        if (string.IsNullOrWhiteSpace(portName))
        {
            return "Unknown Port Name";
        }

        // Declare the friendly name variable for later use
        var friendlyName = portName;
        // We'll wrap this in a try loop simply because this isn't available on all platforms
        try
        {
            // This is basically an SQL query. Let's search for the details of the ports based on the port name
            // Again, this is just for Windows based devices
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%" + portName + "%'");
            var found = false;
            // Loop through and output the friendly name
            foreach (var port in searcher.Get())
            {
                friendlyName = port["Name"]?.ToString();
                found = true;
                break;
            }

            // Handle if searcher returns nothing
            if (!found)
            {
                friendlyName = "Unknown Port Name";
            }
        }
        // Catch any thrown exception
        catch (Exception)
        {
            // If there is an error, we'll just declare that we don't know the name of the port
            friendlyName = "Unknown Port Name";
        }
        // Send the friendly name (or unknown port name string) back to the main code for output
        return friendlyName;
    }
    #endregion

    #region Console header
    public static void ShowHeader()
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
    #endregion

    #region URL Handling

    // Let's create a function that will allow us to download the latest version of the database if we have access to the internet.
    public static bool DownloadDatabase(string url, string savePath)
    {
        using var client = new WebClient();
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

    // Function to open a new URL in the default browser
    public static void OpenUrl(string url)
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

#endregion
}
