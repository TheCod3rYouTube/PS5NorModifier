using UART_CL_By_TheCod3r.Enumerators;

namespace UART_CL_By_TheCod3r.Data;

/// <summary>
/// Represents the properties extracted from a BIOS file.
/// </summary>
public class BiosInfo
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
	public IEnumerable<BiosError> Errors { get; set; } = [];
}
