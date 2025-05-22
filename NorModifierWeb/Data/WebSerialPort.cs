using Microsoft.JSInterop;
using NorModifierLib.Interfaces;
using NorModifierWeb.Exceptions;

namespace NorModifierWeb.Data;

public class WebSerialPort(ILogger<WebSerialPort> logger, IJSRuntime jsRuntime) : ISerialPort
{
	public async Task<bool> IsSupported() => await jsRuntime.InvokeAsync<bool>("serialIsSupported");

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

	public async Task CloseAsync()
	{
		await jsRuntime.InvokeVoidAsync("serialClose");
		logger.LogInformation("Closed web serial port");
	}

	public async Task<string> ReadLineAsync()
	{
		var line = await jsRuntime.InvokeAsync<string>("serialRead");
		logger.LogInformation("Read line from web serial port: {Line}", line);
		return line;
	}

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
