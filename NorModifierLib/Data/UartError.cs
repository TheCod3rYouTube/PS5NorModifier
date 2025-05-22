using System.Buffers.Binary;

namespace NorModifierLib.Data;

/// <summary>
/// Represents a UART error.
/// </summary>
/// <param name="errorBytes">The raw bytes of the error as read from the device.</param>
public class UartError(byte[] errorBytes) : BaseError()
{
	public override uint RawCode => BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt32(errorBytes.AsSpan()[0..4]));
	public override uint Rtc => BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt32(errorBytes.AsSpan()[4..8]));
	public override uint RawPowerState => BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt32(errorBytes.AsSpan()[8..12]));
	public override uint RawBootCause => BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt32(errorBytes.AsSpan()[12..16]));
	public override ushort RawSequenceNumber => BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt16(errorBytes.AsSpan()[16..18]));
	public override ushort RawDevicePowerManagement => BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt16(errorBytes.AsSpan()[18..20]));
	public override ushort RawChipTemperature => BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt16(errorBytes.AsSpan()[20..22]));
	public override ushort RawEnvironmentTemperature => BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt16(errorBytes.AsSpan()[22..24]));

	public override string ToString()
	{
		return Enumerable.Range(0, (errorBytes.Length + 4 - 1) / 4)
			.Select(x => Convert.ToHexString([.. errorBytes.Skip(x * 4).Take(4)]))
			.Aggregate((current, next) => current + " " + next);
	}
}
