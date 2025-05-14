namespace UART_CL_By_TheCod3r.Enumerators;

public static class EnumExtensions
{
	public static byte[] GetBytes(this Edition edition)
	{
		byte[] retVal = edition switch
		{
			Edition.Disc => [0x22, 0x02, 0x01, 0x01],
			Edition.Digital => [0x22, 0x03, 0x01, 0x01], 
			Edition.Slim => [0x22, 0x01, 0x01, 0x01],
			_ => throw new ArgumentOutOfRangeException(nameof(edition), edition, "Unknown edition")
		};

		if (retVal.Length != 4)
		{
			throw new InvalidOperationException("Invalid length of Edition byte array");
		}

		return retVal;
	}
}
