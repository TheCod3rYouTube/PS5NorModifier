using System.Text;

namespace NorModifierLib.Data;

/// <summary>
/// Helper class to calculate checksums and create transmittable commands.
/// </summary>
public static class Helpers
{
	/// <summary>
	/// Creates a transmittable command with the checksum
	/// </summary>
	/// <param name="command">The command to transmit to the device</param>
	/// <returns>The full command including checksum to be transmitted to the device</returns>
	public static string CreateTransmittableCommand(string command)
	{
		var sum = CalculateChecksum(command);

		return $"{command}:{sum}";
	}

	/// <summary>
	/// Calculates the checksum for the given string
	/// </summary>
	/// <param name="input">The string to be checksummed</param>
	/// <returns>The calculated checksum of the input string</returns>
	public static string CalculateChecksum(string input)
	{
		var sum = input.Sum(x => (int)x);

		return $"{(sum & 0xFF):X2}";
	}

	/// <summary>
	/// Gets the ASCII string from the string of hex bytes
	/// </summary>
	/// <param name="hexString">The hex string representation of the bytes</param>
	/// <returns>ASCII encoded string</returns>
	/// <exception cref="ArgumentException"></exception>
	[Obsolete("Use Encoding.ASCII.GetString instead.")]
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
	[Obsolete("Use known offsets instead.")]
	public static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
	{
		int maxStart = source.Length - pattern.Length + 1;

		return Enumerable
			.Range(0, maxStart)
			.Where(x => pattern
				.Select((y, z) => source[x + z] == y)
			.All(match => match));
	}
}
