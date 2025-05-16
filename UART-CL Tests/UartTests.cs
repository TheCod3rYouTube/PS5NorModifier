using System.Globalization;
using System.Text;
using UART_CL_By_TheCod3r;
using UART_CL_By_TheCod3r.Data;

namespace UART_CL_Tests;

public class UartTests
{
	[Fact]
	public void ParseErrorCode_Test()
	{
		// Arrange
		var rawError = "OK 00000000 80C00140 0000008D FFFF0005 00000100 2157 0016 46E4 1A80:27";

		// Act
		var errorCode = new ErrorCode(rawError);
		var checksumValid = errorCode.ChecksumValid;

		// Assert
		Assert.Equal("00000000", errorCode.FirstPart);
		Assert.Equal("80C00140", errorCode.SecondPart);
		Assert.Equal("0000008D", errorCode.ThirdPart);
		Assert.Equal("FFFF0005", errorCode.FourthPart);
		Assert.Equal("00000100", errorCode.FifthPart);
		Assert.Equal("2157", errorCode.SixthPart);
		Assert.Equal("0016", errorCode.SeventhPart);
		Assert.Equal("46E4", errorCode.EighthPart);
		Assert.Equal("1A80", errorCode.NinthPart);
		Assert.Equal("27", errorCode.Checksum);
		Assert.True(checksumValid);
	}

	[Fact]
	public void CalculateChecksum_Test()
	{
		// Arrange
		var str = "OK 00000000 80C00140 0000008D FFFF0005 00000100 2157 0016 46E4 1A80";
		var checksum = "27";

		// Act
		var calculatedChecksum = Uart.CalculateChecksum(str);

		// Assert
		Assert.Equal(checksum, calculatedChecksum);
	}

	[Fact]
	public void CreateTransmittableCommand_Test()
	{
		// Arrange
		var str = "Hello World";

		// Act
		var oldChecksum = OldCalculateChecksum(str);
		var newChecksum = Uart.CreateTransmittableCommand(str);

		// Assert
		Assert.Equal(oldChecksum, newChecksum);
	}

	[Fact]
	[Obsolete("The methods tested by this test are no longer used.")]
	public void ConvertHexStringToString_Test()
	{
		// Arrange
		var str = "48454C4C4F";

		// Act
		var oldString = OldHexStringToString(str);
		var newString = Uart.HexStringToString(str);

		// Assert
		Assert.Equal(oldString, newString);
	}

	[Fact]
	[Obsolete("The methods tested by this test are no longer used.")]
	public void ConvertHexStringToByteArray_Test()
	{
		// Arrange
		var str = "DEADBEEF";

		// Act
		var oldBytes = OldConvertHexStringToByteArray(str);
		var newBytes = Convert.FromHexString(str);

		// Assert
		Assert.Equal(oldBytes, newBytes);
	}

	[Fact]
	[Obsolete("The methods tested by this test are no longer used.")]
	public void PatternAt_Test()
	{
		// Arrange
		var array = new byte[] { 0x00, 0x01, 0x02, 0x03,
								 0x00, 0x02, 0x02, 0x03,
								 0x00, 0x01, 0x02, 0x03,
								 0x00, 0x03, 0x02, 0x03 };

		var pattern = new byte[] { 0x00, 0x01 };

		// Act
		var oldPattern = OldPatternAt(array, pattern);
		var newPattern = Uart.PatternAt(array, pattern);

		// Assert
		Assert.Equal(expected: 2, oldPattern.Count());
		Assert.Equal(oldPattern.Count(), newPattern.Count());
		Assert.Equal(oldPattern, newPattern);
	}

	//
	// Old methods to verify new implementations behave the same
	//
	public static string OldCalculateChecksum(string str)
	{
		int sum = 0;
		foreach (char c in str)
		{
			sum += (int)c;
		}
		return str + ":" + (sum & 0xFF).ToString("X2");
	}

	[Obsolete("Use Encoding.ASCII.GetString instead.")]
	static string OldHexStringToString(string hexString)
	{
		if (hexString == null || (hexString.Length & 1) == 1)
		{
			throw new ArgumentException("Hex string parameter must have an even number of characters");
		}
		var sb = new StringBuilder();
		for (var i = 0; i < hexString.Length; i += 2)
		{
			var hexChar = hexString.Substring(i, 2);
			sb.Append((char)Convert.ToByte(hexChar, 16));
		}
		return sb.ToString();
	}

	[Obsolete("Use known offsets instead.")]
	static IEnumerable<int> OldPatternAt(byte[] source, byte[] pattern)
	{
		for (int i = 0; i < source.Length; i++)
		{
			if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
			{
				yield return i;
			}
		}
	}

	[Obsolete("Use Convert.FromHexString instead.")]
	static byte[] OldConvertHexStringToByteArray(string hexString)
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
}
