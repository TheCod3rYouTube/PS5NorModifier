using System.Xml.Linq;

namespace UART_CL_By_TheCod3r;

public class CodeDatabase()
{
	private static readonly HttpClient _httpClient = new();
	private XDocument _xml = null!;

	private const string DatabaseUri = "http://uartcodes.com/xml.php";

	public async Task<string> ParseErrors(string errorCode)
	{
		// Load the database if it hasn't been loaded yet
		if (_xml == null)
		{
			using var stream = await _httpClient.GetStreamAsync(DatabaseUri);

			_xml = XDocument.Load(stream);
		}

		var match = _xml.Descendants("errorCode")
			.FirstOrDefault(x => x.Element("ErrorCode")?.Value == errorCode);

		string description = match?.Element("Description")?.Value ?? "Error Description Not Found";

		return description;
	}
}
