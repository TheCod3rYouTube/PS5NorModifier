using Microsoft.JSInterop;
using NorModifierLib.Interfaces;
using NorModifierWeb.Exceptions;

namespace NorModifierWeb.Data;

/// <summary>
/// A service for communicating with the Web Serial API.
/// </summary>
/// <param name="logger">ILogger interface to receive log data.</param>
/// <param name="jsRuntime">JavaScript runtime interface for the browser session.</param>
public sealed class WebSerialPort(ILogger<WebSerialPort> logger, IJSRuntime jsRuntime) : ISerialPort
{
	/// <summary>
	/// Checks if the Web Serial API is supported in the current browser.
	/// </summary>
	/// <returns>A Task<bool> representing the JavaScript promise for the Web Serial API support check. True if the browser supports Web Serial and false otherwise.</returns>
	public async Task<bool> IsSupported() => await jsRuntime.InvokeAsync<bool>("serialIsSupported");

	/// <summary>
	/// Requests a web serial port from the user and opens it with a baud rate of 115200.
	/// </summary>
	/// <returns>A Task<bool> representing the JavaScript promise for the Web Serial port open operation. True if the port was successfully opened and false otherwise.</returns>
	public async Task<bool> OpenAsync()
	{
		if(!await jsRuntime.InvokeAsync<bool>("serialRequestPort"))
		{
			logger.LogInformation("Web serial port request was denied by the user");
			return false;
		}

		if(!await jsRuntime.InvokeAsync<bool>("serialOpen", 115200))
		{
			logger.LogInformation("Web serial port open failed");
			return false;
		}

		logger.LogInformation("Web serial port opened successfully");
		return true;
	}

	/// <summary>
	/// Closes the web serial port.
	/// </summary>
	/// <returns>A Task representing the JavaScript promise for the Web Serial port close operation.</returns>
	public async Task CloseAsync()
	{
		await jsRuntime.InvokeVoidAsync("serialClose");
		logger.LogInformation("Closed web serial port");
	}

	/// <summary>
	/// Reads a line from the web serial port.
	/// </summary>
	/// <returns>A Task<string> representing the JavaScript promise for the Web Serial port read line operation. </returns>
	public async Task<string> ReadLineAsync()
	{
		var line = await jsRuntime.InvokeAsync<string>("serialRead");
		logger.LogInformation("Read line from web serial port: {Line}", line);
		return line;
	}

	/// <summary>
	/// Writes a line to the web serial port. The line is terminated with a newline character.
	/// </summary>
	/// <param name="data">The data to be written to the serial port.</param>
	/// <returns>A Task representing the JavaScript promise for the Wweb Serial port write operation.</returns>
	/// <exception cref="WebSerialWriteException">Thrown when the write operation fails for any reason.</exception>
	public async Task WriteLineAsync(string data)
	{
		if (!await jsRuntime.InvokeAsync<bool>("serialWrite", $"{data}\n"))
		{
			logger.LogError("Web serial port write failed");
			throw new WebSerialWriteException("Web serial port write failed");
		}

		logger.LogInformation("Wrote line to web serial port: {Data}", data);
	}
}
