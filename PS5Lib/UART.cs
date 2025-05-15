using System.IO.Ports;
using System.Xml;

namespace PS5Lib;

public class UART
{
    public const string DatabaseFileName = "errorDB.xml";
    private const string DatabaseUrl = "https://uartcodes.com/xml.php";
    
    private readonly SerialPort _uartSerial = new();
    public bool UseOfflineDB { get; set; }
    public bool IsConnected => _uartSerial.IsOpen;

    private static string CalculateChecksum(string str)
    {
        int sum = str.Aggregate(0, (current, c) => current + c);
        return str + ":" + (sum & 0xFF).ToString("X2");
    }
    
    /// <summary>
    /// Gets the description of an error code from the <see cref="DatabaseFileName"/> XML file.
    /// </summary>
    /// <param name="errorCode">The error code to look up.</param>
    /// <returns>The description of the error code, or an error message if the file is not found or invalid.</returns>
    public static string ParseErrorsOffline(string errorCode)
    {
        try
        {
            if (!File.Exists(DatabaseFileName))
                return "Error: Local XML file not found.";
            
            XmlDocument xmlDoc = new();
            xmlDoc.Load(DatabaseFileName);

            XmlNode? errorNode = xmlDoc.SelectSingleNode($"//errorCode[ErrorCode='{errorCode}']");
            XmlNode? descriptionNode = errorNode?.SelectSingleNode("Description");
            
            if (descriptionNode == null)
                return "Error: Invalid XML database file. Please re-download the offline database or use the online database.";
            
            string description = descriptionNode.InnerText;
            return $"Error code: {errorCode}\n" +
                   $"Description: {description}";
        }
        catch (Exception ex)
        {
            return $"Error: {ex}\n" +
                   "Is the database present and valid?";
        }
    }
    
    /// <summary>
    /// Gets the description of an error code from the online database at <see cref="DatabaseUrl"/>.
    /// </summary>
    /// <param name="error">The error code to look up.</param>
    /// <returns>The description of the error code, or an error message if an error occurred.</returns>
    public static async Task<string> ParseErrorsOnline(string error)
    {
        string url = $"{DatabaseUrl}?errorCode={error}";

        try
        {
            string response;
            using (HttpClient client = new())
            {
                response = await client.GetStringAsync(url);
            }
            
            XmlDocument xmlDoc = new();
            xmlDoc.LoadXml(response);
            
            XmlNode? description = xmlDoc.SelectSingleNode("errorCodes/errorCode/Description");
            if (description == null)
            {
                return $"Error code: {error}\n" +
                       "An error occurred while fetching a result for this error. Please try again!";
            }
            
            return $"Error code: {error}\n" +
                   $"Description: {description.InnerText}";
        }
        catch (Exception ex)
        {
            return $"Error code: {error}\n" +
                   $"{ex}";
        }
    }
    
    /// <summary>
    /// Downloads the error database from <see cref="DatabaseUrl"/> and saves it to the local file system.
    /// </summary>
    public static async Task DownloadErrorDB()
    {
        string xmlData;
        using (HttpClient client = new())
        {
            xmlData = await client.GetStringAsync(DatabaseUrl);
        }
            
        XmlDocument xmlDoc = new();
        xmlDoc.LoadXml(xmlData);

        xmlDoc.Save(DatabaseFileName);
    }
    
    public void Connect(string portName, int baudRate = 115200)
    {
        if (_uartSerial.IsOpen)
            return;
        
        _uartSerial.PortName = portName;
        _uartSerial.BaudRate = baudRate;
        _uartSerial.RtsEnable = true;
        _uartSerial.Open();
    }
    
    public void Disconnect()
    {
        if (_uartSerial.IsOpen)
            _uartSerial.Close();
    }
    
    /// <summary>
    /// Sends <c>errlog clear</c> to the UART device and returns the result.
    /// </summary>
    /// <param name="result">The result of the command, or an error message if an error occurred.</param>
    /// <returns>True if the command was sent successfully, false otherwise.</returns>
    public bool ClearErrorLog(out string result)
    {
        bool success = SendCommand("errlog clear", out var results);

        if (!success)
        {
            result = results[0];
            return false;
        }

        result = "";
        foreach (string[] split in results.Select(l => l.Split(' ')).Where(split => split.Length != 0))
        {
            switch (split[0])
            {
                case "NG":
                    result += "Response: FAIL" + Environment.NewLine +
                              "Information: An error occurred while clearing the error logs from the system. Please try again..." + Environment.NewLine +
                              split[0];
                        
                    break;
                case "OK":
                    result += "Response: SUCCESS" + Environment.NewLine +
                              "Information: All error codes cleared successfully" + Environment.NewLine +
                              split[0];

                    break;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Gets the error log from the UART device.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="hideDuplicates">Whether to filter out duplicate errors.</param>
    /// <returns>A tuple containing a success flag and a list of error messages.</returns>
    public async Task<(bool success, List<string> errors)> GetErrorLog(int page, bool hideDuplicates = false)
    {
        bool success = SendCommand($"errlog {page}", out var results);

        if (!success)
            return (false, results);
        
        if (hideDuplicates)
        {
            results = results.Distinct().ToList();
        }
        
        List<string> errors = [];
        foreach (string[] split in results.Select(l => l.Split(' ')).Where(split => split.Length != 0))
        {
            switch (split[0])
            {
                case "NG":
                    break;
                case "OK":
                    string errorCode = split[2];
                    string errorResult;
                    if (errorCode.StartsWith("FFFFFF"))
                        errorResult = $"No error displayed ({errorCode})";
                    else
                        errorResult = UseOfflineDB
                        ? ParseErrorsOffline(errorCode)
                        : await ParseErrorsOnline(errorCode);
                            
                    errors.Add(errorResult + Environment.NewLine);
                    break;
            }
        }
        return (true, errors);

    }
    
    /// <summary>
    /// Sends an arbitrary command to the UART device and returns the result.
    /// If an error occurs, the result will be the error message.
    /// </summary>
    /// <param name="command">The command to send.</param>
    /// <param name="result">The result of the command, or an error message if an error occurred.</param>
    /// <returns>True if the command was sent successfully, false otherwise.</returns>
    public bool SendArbitraryCommand(string command, out string result)
    {
        bool success = SendCommand(command, out var results);

        if (!success)
        {
            result = results[0];
            return false;
        }

        result = "";
        foreach (string l in results)
        {
            string[] split = l.Split(' ');
            if (split.Length == 0) continue;
                
            result += split[0] switch
            {
                "NG" => "ERROR: " + l,
                "OK" => "SUCCESS: " + l,
                _ => l
            } + Environment.NewLine;
        }
        return true;
    }
    
    /// <summary>
    /// Sends a command to the UART device and returns the result.
    /// If an error occurs, the result will contain one item, the error message.
    /// </summary>
    /// <param name="command">The command to send.</param>
    /// <param name="result">The result of the command, or an error message if an error occurred.</param>
    /// <returns>True if the command was sent successfully, false otherwise.</returns>
    private bool SendCommand(string command, out List<string> result)
    {
        if (!_uartSerial.IsOpen)
        {
            result = [ "Please connect to UART before attempting to send commands." ];
            return false;
        }

        try
        {
            result = [];
            string checksum = CalculateChecksum(command);
            
            _uartSerial.WriteLine(checksum);
            do
            {
                string? line = _uartSerial.ReadLine();
                if (!string.Equals($"{command}:{checksum}", line, StringComparison.InvariantCultureIgnoreCase))
                {
                    result.Add(line);
                }
            } while (_uartSerial.BytesToRead != 0);
             
            return true;
        }
        catch (Exception ex)
        {
            result = [ $"Error: {ex}" ];
            return false;
        }
    }
}