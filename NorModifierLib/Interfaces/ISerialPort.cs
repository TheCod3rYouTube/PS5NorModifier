namespace NorModifierLib.Interfaces;

/// <summary>
/// Interface for a service to communicate with the UART over a serial port.
/// </summary>
public interface ISerialPort
{
	public Task WriteLineAsync(string data);
	public Task<string> ReadLineAsync();
}
