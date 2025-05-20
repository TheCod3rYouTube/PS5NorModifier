namespace NorModifierLib.Enumerators;

/// <summary>
/// Extenion methods for the <see cref="Edition"/> enumeration.
/// </summary>
public static class EnumExtensions
{
	/// <summary>
	/// Converts the <see cref="Edition"/> enumeration to a byte array.
	/// </summary>
	/// <param name="edition">The Edition to convert to a byte array.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if the Edition provided does not have matching bytes.</exception>
	/// <exception cref="InvalidOperationException"></exception>
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
