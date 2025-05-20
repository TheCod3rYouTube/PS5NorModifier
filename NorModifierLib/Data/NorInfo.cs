using NorModifierLib.Enumerators;

namespace NorModifierLib.Data;

/// <summary>
/// Represents the properties extracted from a NOR file.
/// </summary>
public class NorInfo
{
	public string Path { get; set; } = string.Empty;
	public Edition Edition { get; set; }
	public string Region { get; set; } = string.Empty;
	public string ModelInfo { get; set; } = string.Empty;
	public string ConsoleSerialNumber { get; set; } = string.Empty;
	public string MotherboardSerialNumber { get; set; } = string.Empty;
	public string Model { get; set; } = string.Empty;
	public string WiFiMac { get; set; } = string.Empty;
	public string LanMac { get; set; } = string.Empty;
	public IEnumerable<NorError> Errors { get; set; } = [];
}
