using UART.Core.Configuration;

namespace PS5_NOR_Modifier.Configuration;

public class InMemoryConfiguration
{
    public static IEnumerable<KeyValuePair<string, string?>> Configuration = new Dictionary<string, string?>
    {
        {Constants.UartDbFilePathKey, "errorDB.xml"},
        {Constants.UartSourceUrlKey, "http://uartcodes.com/xml.php"},
    };
}