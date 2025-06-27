using System.Net;
using System.Xml;

namespace UART_CL_By_TheCod3r;

/// <summary>
/// Manages the local XML error code database.
/// </summary>
public class ErrorCodeService
{
    private const string DbFileName = "errorDB.xml";
    private const string DbDownloadUrl = "http://uartcodes.com/xml.php";

    /// <summary>
    /// Checks if the database exists, and downloads it if not.
    /// </summary>
    /// <returns>True if the database is ready, false otherwise.</returns>
    public bool EnsureDatabaseExists()
    {
        if (File.Exists(DbFileName))
        {
            return true;
        }

        Console.WriteLine("Local error database not found.");
        return DownloadDatabase();
    }

    /// <summary>
    /// Forces an update of the database from the web.
    /// </summary>
    public void UpdateDatabase()
    {
        Console.WriteLine("\nAttempting to update error database...");
        DownloadDatabase();
    }

    /// <summary>
    /// Looks up an error code in the local XML database and returns its description.
    /// </summary>
    /// <param name="errorCode">The error code to look up.</param>
    /// <returns>A formatted string with the error description.</returns>
    public string GetErrorDescription(string errorCode)
    {
        if (!File.Exists(DbFileName))
        {
            return $"Error: Database file '{DbFileName}' not found.";
        }

        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(DbFileName);

            // Use XPath for a more direct and efficient lookup.
            XmlNode? errorNode = xmlDoc.SelectSingleNode($"//errorCode[ErrorCode='{errorCode}']");

            if (errorNode != null)
            {
                string? code = errorNode.SelectSingleNode("ErrorCode")?.InnerText;
                string? description = errorNode.SelectSingleNode("Description")?.InnerText;
                return $"Code: {code}\nDescription: {description}\n";
            }

            return $"Code: {errorCode}\nDescription: No result found in the local database.\n";
        }
        catch (Exception ex)
        {
            return $"Error parsing database file: {ex.Message}";
        }
    }

    private bool DownloadDatabase()
    {
        Console.WriteLine($"Downloading latest database from {DbDownloadUrl}...");
        using (var client = new WebClient())
        {
            try
            {
                client.DownloadFile(DbDownloadUrl, DbFileName);
                Console.WriteLine("Database downloaded successfully.");
                return true;
            }
            catch (WebException ex)
            {
                Console.WriteLine($"Network error during download: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return false;
            }
        }
    }
}
