using System.Xml;

namespace UARTLib;

public static class UART
{
    public static string CalculateChecksum(string str)
    {
        int sum = str.Aggregate(0, (current, c) => current + c);
        return str + ":" + (sum & 0xFF).ToString("X2");
    }
    
    public static string ParseErrorsOffline(string errorCode)
    {
        try
        {
            if (!File.Exists("errorDB.xml"))
                return "Error: Local XML file not found.";
            
            XmlDocument xmlDoc = new();
            xmlDoc.Load("errorDB.xml");

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
    
    public static async Task<string> ParseErrorsOnline(string error)
    {
        string url = "https://uartcodes.com/xml.php?errorCode=" + error;

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
}