using NorModifierLib.Interfaces;

namespace UART_CL_By_TheCod3r.Data;

public class SerialPort(string portName) : ISerialPort, IDisposable
{
	private bool disposedValue;

	public string PortName => portName;

	private System.IO.Ports.SerialPort _serialPort = default!;

	public void Open()
	{
		_serialPort = new(portName)
		{
			BaudRate = 115200,
			RtsEnable = true
		};
		_serialPort.Open();
	}

	public void Close()
	{
		_serialPort.Close();
	}

	// This is a hacky workaround as the Web Serial APIs are async and this is not
	public async Task<string> ReadLineAsync()
	{
		return _serialPort.ReadLine();
	}

	public async Task WriteLineAsync(string data)
	{
		_serialPort.WriteLine(data);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				_serialPort.Dispose();
			}

			disposedValue = true;
		}
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
