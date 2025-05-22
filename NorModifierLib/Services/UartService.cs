using Microsoft.Extensions.Logging;
using NorModifierLib.Data;
using NorModifierLib.Exceptions;
using NorModifierLib.Interfaces;

namespace NorModifierLib.Services;

public class UartService(ILogger<UartService> logger) : IUartService
{
	/// <summary>
	/// Clears the error codes from the device
	/// </summary>
	/// <param name="serialPort">An ISerialPort object that facilitates serial communication with the device</param>
	public async Task ClearErrorsAsync(ISerialPort serialPort)
	{
		var command = "errlog clear";
		var transmitCommand = Helpers.CreateTransmittableCommand(command);

		await serialPort.WriteLineAsync(transmitCommand);

		logger.LogInformation("Transmitted command: {Command}", transmitCommand);
		logger.LogInformation("Cleared error codes");
	}

	/// <summary>
	/// Gets the error codes from the device
	/// </summary>
	/// <param name="serialPort">An ISerialPort object that facilitates serial communication with the device</param>
	/// <returns>The errors read from the UART</returns>
	public async Task<IEnumerable<UartError>> GetErrorsAsync(ISerialPort serialPort)
	{
		var retVal = new List<UartError>();
		for (var i = 0; i <= 255; i++)
		{
			var command = $"errlog {i}";
			var transmitCommand = Helpers.CreateTransmittableCommand(command);

			await serialPort.WriteLineAsync(transmitCommand);
			var response = await serialPort.ReadLineAsync();

			logger.LogInformation("Transmitted command: {Command}", transmitCommand);

			// End of the error list
			if (string.Equals("NG", response[..2], StringComparison.InvariantCultureIgnoreCase))
			{
				logger.LogInformation("Received NG response, end of the error list.");
				break;
			}

			// Unknown response
			if (!string.Equals("OK", response[..2], StringComparison.InvariantCultureIgnoreCase))
			{
				logger.LogError("Unexpected response received. Expected response to begin with 'OK', but got: {Response}.", response);
				throw new UartResponseInvalidException($"Unexpected response received. Expected response to begin with 'OK', but got: {response}.");
			}

			// Invalid response
			if (response.Length != 70)
			{
				logger.LogError("Invalid response length. Expected 70 characters, but got: {Length}. Response: {Response}.", response.Length, response);
				throw new UartResponseInvalidException($"Invalid response length. Expected 70 characters, but got: {response.Length}. Response {response}");
			}

			var responseChecksum = response[^2..];
			var calculatedChecksum = Helpers.CalculateChecksum(response[..^3]);

			// Response validation checksum did not match
			if (!string.Equals(responseChecksum, calculatedChecksum, StringComparison.InvariantCultureIgnoreCase))
			{
				logger.LogError("Invalid response checksum: Expected: {Expected}, but got {Calculated}. Response: {Response}.", responseChecksum, calculatedChecksum, response);
				throw new UartResponseInvalidException($"Invalid response checksum: Expected: {responseChecksum}, but got {calculatedChecksum}. Response: {response}.");
			}

			logger.LogInformation("Received response: {Response}.", response);
			var errorBytes = Convert.FromHexString(response[12..^3].Replace(" ", string.Empty));
			retVal.Add(new(errorBytes));
		}

		return retVal;
	}
}
