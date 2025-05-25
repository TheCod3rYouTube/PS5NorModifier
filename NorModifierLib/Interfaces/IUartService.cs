using NorModifierLib.Data;

namespace NorModifierLib.Interfaces;

/// <summary>
/// Interface for a service used to read and clear UART errors.
/// </summary>
public interface IUartService
{
	public Task ClearErrorsAsync(ISerialPort serialPort);
	public Task<IEnumerable<UartError>> GetErrorsAsync(ISerialPort serialPort);
}
