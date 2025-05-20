using System.IO.Ports;
using Microsoft.Extensions.Logging;
using NorModifierLib.Data;

namespace NorModifierLib.Services;

public class UartService(ILogger<UartService> logger)
{
	/// <summary>
	/// Clears the error codes from the device
	/// </summary>
	/// <param name="serialPort">The name or path to the serial port device</param>
	public void ClearErrors(string serialPort)
	{
		logger.LogInformation("Attempting to clear errors on serial port: {Path}", serialPort);

		SerialPort port;
		try
		{
			port = new SerialPort(serialPort)
			{
				BaudRate = 115200,
				RtsEnable = true
			};
			port.Open();

			logger.LogInformation("Successfully opened serial port: {Path}", serialPort);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to open serial port: {Path}", serialPort);
			throw;
		}

		var command = "errlog clear";
		var transmitCommand = Helpers.CreateTransmittableCommand(command);

		port.WriteLine(transmitCommand);

		logger.LogInformation("Transmitted command: {Command}", transmitCommand);
		logger.LogInformation("Cleared error codes");

		port.Close();
		port.Dispose();
	}

	/// <summary>
	/// Gets the error codes from the device
	/// </summary>
	/// <param name="serialPort">The name or path to the serial port device</param>
	/// <returns>The errors read from the UART</returns>
	public IEnumerable<ErrorCode> GetErrors(string serialPort)
	{
		logger.LogInformation("Attempting to get errors on serial port: {Path}", serialPort);

		SerialPort port;
		try
		{
			port = new SerialPort(serialPort)
			{
				BaudRate = 115200,
				RtsEnable = true
			};
			port.Open();

			logger.LogInformation("Successfully opened serial port: {Path}", serialPort);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to open serial port: {Path}", serialPort);
			throw;
		}

		var retVal = new List<ErrorCode>();
		for (var i = 0; i <= 255; i++)
		{
			var command = $"errlog {i}";
			var transmitCommand = Helpers.CreateTransmittableCommand(command);

			port.WriteLine(transmitCommand);
			var line = port.ReadLine();

			logger.LogInformation("Transmitted command: {Command}", transmitCommand);

			// End of the error list
			if (string.Equals("NG", line[..2], StringComparison.InvariantCultureIgnoreCase))
			{
				logger.LogInformation("Received NG response, end of the error list.");
				break;
			}

			// Unknown response
			if (!string.Equals("OK", line[..2], StringComparison.InvariantCultureIgnoreCase))
			{
				logger.LogError("Unexpected response received. Expected 'OK', but got: {Response}", line);
				throw new InvalidDataException(line);
			}

			// Invalid response
			if (line.Length != 70)
			{
				logger.LogError("Invalid response length. Expected 70 characters, but got: {Length}", line.Length);
				throw new InvalidDataException(line);
			}

			var error = new ErrorCode(line);

			// Response validation checksum did not match
			if (!error.ChecksumValid)
			{
				logger.LogError("Invalid response checksum: {Response}", line);
				throw new InvalidDataException(line);
			}

			logger.LogInformation("Received response: {Response}", line);
			retVal.Add(error);
		}

		port.Close();
		port.Dispose();

		return retVal;
	}
}
