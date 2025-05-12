using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UART_CL_By_TheCod3r;

public static class Uart
{
	// Define a dictionary to map suffixes to regions
	public static Dictionary<string, string> RegionMap { get; set; } = new Dictionary<string, string>
	{
		{ "00A", "Japan" },
		{ "00B", "Japan" },
		{ "01A", "US, Canada, (North America)" },
		{ "01B", "US, Canada, (North America)" },
		{ "15A", "US, Canada, (North America)" },
		{ "15B", "US, Canada, (North America)" },
		{ "02A", "Australia / New Zealand, (Oceania)" },
		{ "02B", "Australia / New Zealand, (Oceania)" },
		{ "03A", "United Kingdom / Ireland" },
		{ "03B", "United Kingdom / Ireland" },
		{ "04A", "Europe / Middle East / Africa" },
		{ "04B", "Europe / Middle East / Africa" },
		{ "05A", "South Korea" },
		{ "05B", "South Korea" },
		{ "06A", "Southeast Asia / Hong Kong" },
		{ "06B", "Southeast Asia / Hong Kong" },
		{ "07A", "Taiwan" },
		{ "07B", "Taiwan" },
		{ "08A", "Russia, Ukraine, India, Central Asia" },
		{ "08B", "Russia, Ukraine, India, Central Asia" },
		{ "09A", "Mainland China" },
		{ "09B", "Mainland China" },
		{ "11A", "Mexico, Central America, South America" },
		{ "11B", "Mexico, Central America, South America" },
		{ "14A", "Mexico, Central America, South America" },
		{ "14B", "Mexico, Central America, South America" },
		{ "16A", "Europe / Middle East / Africa" },
		{ "16B", "Europe / Middle East / Africa" },
		{ "18A", "Singapore, Korea, Asia" },
		{ "18B", "Singapore, Korea, Asia" }
	};

	/// <summary>
	/// Calculates the checksum for the given string
	/// </summary>
	/// <param name="str"></param>
	/// <returns>The string representation of the calculated checksum hex value</returns>
	public static string CalculateChecksum(string str)
	{
		var sum = str.Sum(x => (int)x);

		return $"{str}:{(sum & 0xFF):X2}";
	}

	/// <summary>
	/// Gets the ASCII string from the string of hex bytes
	/// </summary>
	/// <param name="hexString">The hex string representation of the bytes</param>
	/// <returns>ASCII encoded string</returns>
	/// <exception cref="ArgumentException"></exception>
	public static string HexStringToString(string hexString)
	{
		var bytes = Convert.FromHexString(hexString);
		return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
	}

	/// <summary>
	/// Returns all found indexes of the pattern in the source byte array
	/// </summary>
	/// <param name="source">The source byte array</param>
	/// <param name="pattern">The pattern to search for</param>
	/// <returns></returns>
	public static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
	{
		int maxStart = source.Length - pattern.Length + 1;

		return Enumerable
			.Range(0, maxStart)
			.Where(x => pattern
				.Select((y, z) => source[x + z] == y)
			.All(match => match));
	}

	public static byte[] ConvertHexStringToByteArray(string hexString)
	{
		var bytes = Convert.FromHexString(hexString);
		return bytes;
	}
}
