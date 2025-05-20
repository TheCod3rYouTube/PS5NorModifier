using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace NorModifierLib.Services;

public class ErrorCodeService(ILogger<ErrorCodeService> logger, HttpClient client)
{
	private XDocument _xml = null!;

	private const string DatabaseUri = "http://uartcodes.com/xml.php";

	/// <summary>
	/// Parses the error code database and returns the description of the error code
	/// </summary>
	/// <param name="errorCode">The error code retrieved from the device</param>
	/// <returns>A string description of the error code or a generic message if the error code was not found</returns>
	public async Task<string> ParseError(string errorCode)
	{
		// Load the database if it hasn't been loaded yet
		if (_xml == null)
		{
			logger.LogInformation("Loading error code database from {DatabaseUri}", DatabaseUri);

			try
			{
				using var stream = await client.GetStreamAsync(DatabaseUri);
				_xml = XDocument.Load(stream);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to load error code database from {DatabaseUri}", DatabaseUri);
				throw;
			}
		}

		var match = _xml.Descendants("errorCode")
			.FirstOrDefault(x => x.Element("ErrorCode")?.Value == errorCode);

		string description = match?.Element("Description")?.Value ?? "Error Description Not Found";

		logger.LogInformation("Parsed error code {ErrorCode}: {Description}", errorCode, description);

		return description;
	}
}
