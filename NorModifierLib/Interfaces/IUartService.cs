using NorModifierLib.Data;

namespace NorModifierLib.Interfaces;

public interface IUartService
{
	public Task ClearErrorsAsync(ISerialPort serialPort);
	public Task<IEnumerable<UartError>> GetErrorsAsync(ISerialPort serialPort);
}
