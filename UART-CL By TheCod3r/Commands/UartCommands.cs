using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using NorModifierLib.Data;
using NorModifierLib.Services;
using UART_CL_By_TheCod3r.Data;

namespace UART_CL_By_TheCod3r.Commands;

public class UartCommands(ILogger<UartCommands> logger, UartService uartService, ErrorCodeService errorCodeService) : AsyncCommand<UartCommands.Settings>
{
	public class Settings : CommandSettings
	{
		[CommandArgument(0, "<SERIALPORT>")]
		[Description("The serial port to use for UART commands")]
		public string Path { get; set; } = string.Empty;

		[CommandOption("-c|--clear")]
		[Description("Clear error codes from the device")]
		public bool? Clear { get; set; }
	}

	public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
	{
		logger.LogInformation("Executing UART command with settings: {Settings}", settings);

		SerialPort serialPort;
		try
		{
			serialPort = new SerialPort(settings.Path);
			serialPort.Open();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to open serial port: {Path}", settings.Path);
			AnsiConsole.MarkupLineInterpolated($"[red]Failed to open serial port: {settings.Path}. See log for details.[/]");
			return -1;
		}

		// Clear errors flag specified
		if (settings.Clear is true)
		{
			try
			{
				await uartService.ClearErrorsAsync(serialPort);
				logger.LogInformation("Cleared errors.");
				AnsiConsole.MarkupLine("[green]Cleared errors.[/]");
			}
			catch
			{
				logger.LogError("Failed to clear errors.");
				AnsiConsole.MarkupLine("[red]Failed to clear errors. See log for details.[/]");
				return -1;
			}
		}

		// No option/flag specified, default behavior is list errors
		IEnumerable<UartError> errors;
		try
		{
			errors = await uartService.GetErrorsAsync(serialPort);
		}
		catch
		{
			logger.LogError("Failed to get errors.");
			AnsiConsole.MarkupLine("[red]Failed to get errors. See log for details.[/]");
			return -1;
		}

		// Display top 5 log entries
		var logTable = new Table()
		{
			Title = new TableTitle("Error Log"),
		};
		var redStyle = new Style(Color.Red);
		var blueStyle = new Style(Color.Blue);
		var greenStyle = new Style(Color.Green);

		logTable.AddColumn(new TableColumn("Error").Centered());
		logTable.AddColumn(new TableColumn("RTC").Centered());
		logTable.AddColumn(new TableColumn("Power State").Centered());
		logTable.AddColumn(new TableColumn("Boot Cause").Centered());
		logTable.AddColumn(new TableColumn("Device Power").Centered());
		logTable.AddColumn(new TableColumn("Sequence Number").Centered());
		logTable.AddColumn(new TableColumn("Env Temp").Centered());
		logTable.AddColumn(new TableColumn("SoC Temp").Centered());

		if (!errors.Any())
		{
			logTable.AddRow(new Text("No Errors", greenStyle));
		}

		foreach (var error in errors)
		{
			logTable.AddRow([new Text($"{error.RawCode:X8}", blueStyle).Centered(),
					new Text($"{error.Rtc:X8}", blueStyle).Centered(),
					new Text($"{error.RawPowerState:X8}", blueStyle).Centered(),
					new Text($"{error.RawBootCause:X8}", blueStyle).Centered(),
					new Text($"{error.RawDevicePowerManagement:X4}", blueStyle).Centered(),
					new Text($"{error.RawSequenceNumber:X4}", blueStyle).Centered(),
					new Text($"{error.RawEnvironmentTemperature:X4}", blueStyle).Centered(),
					new Text($"{error.RawChipTemperature:X4}", blueStyle).Centered(),
				]);
			logTable.AddRow([
				new Text(error.Code, greenStyle),
					new Text(string.Empty),
					new Text($"{error.PowerStateA}-{error.PowerStateB}", greenStyle),
					new Text(error.BootCause, greenStyle),
					new Markup($"[{(error.HdmiPower ? """green""" : """red""")}]HDMI[/] " +
						$"[{(error.BddPower ? """green""" : """red""")}]BDD[/] " +
						$"[{(error.HdmiCecPower ? """green""" : """red""")}]HDMI-CEC[/] " +
						$"[{(error.UsbPower ? """green""" : """red""")}]USB[/] " +
						$"[{(error.WifiPower ? """green""" : """red""")}]WiFi[/]"),
					new Text(error.SequenceNumber.Replace(", ", Environment.NewLine), greenStyle),
					new Text(error.EnvironmentTemperature, greenStyle),
					new Text(error.ChipTemperature, greenStyle),
				]);
			logTable.AddEmptyRow();
		}

		AnsiConsole.Write(logTable);

		return 0;
	}
}
