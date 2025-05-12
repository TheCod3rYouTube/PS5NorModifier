using System.Xml;
using Microsoft.Extensions.Configuration;
using UART.Core.Abstractions;
using UART.Core.Configuration;
using UART.Core.Models;

namespace UART.Core.Data;

public class XmlUartProvider : IUartProvider
{
    private readonly string? _localDatabaseFile;
    private readonly string? _uartSourceUrl;
    private readonly INotificationHandler _notificationHandler;
    
    public XmlUartProvider(IConfiguration configuration, INotificationHandler notificationHandler)
    {
        _localDatabaseFile = configuration[Constants.UartDbFilePathKey];
        _uartSourceUrl = configuration[Constants.UartSourceUrlKey];
        _notificationHandler = notificationHandler;
    }
    
    public async Task UpdateErrorDatabase()
    {
        // Define the file path to save the XML

        try
        {
            // Create a WebClient instance
            using (HttpClient client = new())
            {
                // Download the XML data from the URL
                string xmlData = await client.GetStringAsync(_uartSourceUrl);

                // Create an XmlDocument instance and load the XML data
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlData);

                // Save the XML data to a file
                xmlDoc.Save(_localDatabaseFile);

                _notificationHandler.HandleMessage(new Notification
                {
                    Message = "The most recent offline database has been updated successfully.",
                    Title = "Offline Database Updated!",
                    Type = NotificationType.Information,
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
    
    public string ParseErrorsOffline(string errorCode)
    {
        string results = "";

        try
        {
            // Check if the XML file exists
            if (File.Exists(_localDatabaseFile))
            {
                // Load the XML file
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_localDatabaseFile);

                // Get the root node
                XmlNode? root = xmlDoc.DocumentElement;
                if (root is null) return results;

                // Check if the root node is <errorCodes>
                if (root.Name == "errorCodes")
                {
                    // Loop through each errorCode node
                    foreach (XmlNode errorCodeNode in root.ChildNodes)
                    {
                        // Check if the node is <errorCode>
                        if (errorCodeNode.Name == "errorCode")
                        {
                            // Get ErrorCode and Description
                            string errorCodeValue = errorCodeNode.SelectSingleNode("ErrorCode")?.InnerText??"";
                            string description = errorCodeNode.SelectSingleNode("Description")?.InnerText??"";

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

    public async Task<string> ParseErrorsOnline(string errorCode)
    {
        string results = "";

        try
        {
            string response = "";
            // Create a WebClient instance to send the request
            using (HttpClient client = new()) 
            {
                // Send the request and retrieve the response as a string
                response = await client.GetStringAsync($"{_uartSourceUrl}?errorCode={errorCode}");
            }
            // Load the XML response into an XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            
            // Get the root node
            XmlNode? root = xmlDoc.DocumentElement;
            if (root is null) {
                throw new Exception("Error reading the file");
            }

            // Check if the root node is <errorCodes>
            if (root.Name == "errorCodes")
            {
                // Loop through each errorCode node
                foreach (XmlNode errorCodeNode in root.ChildNodes)
                {
                    // Check if the node is <errorCode>
                    if (errorCodeNode.Name == "errorCode")
                    {
                        // Get ErrorCode and Description
                        string parsedErrorCode = errorCodeNode.SelectSingleNode("ErrorCode")?.InnerText ?? "";
                        string description = errorCodeNode.SelectSingleNode("Description")?.InnerText??"";

                        // Output the results
                        results = "Error code: "
                            + parsedErrorCode
                            + Environment.NewLine
                            + "Description: "
                            + description;
                    }
                }
            }
            else
            {
                results = "Error code: "
                            + errorCode
                            + Environment.NewLine
                            + "An error occurred while fetching a result for this error. Please try again!";
            }
        }
        catch (Exception ex)
        {
            results = "Error code: "
                + errorCode
                + Environment.NewLine
                + ex.Message;
        }
            
        return results;
    }
}