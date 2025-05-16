using Spectre.Console;
using Spectre.Console.Cli;
using UART_CL_By_TheCod3r.Enumerators;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using UART_CL_By_TheCod3r.Services;
using UART_CL_By_TheCod3r.Data;

namespace UART_CL_By_TheCod3r.Commands;

public class BiosCommands(ILogger<BiosCommands> logger, BiosService biosService) : Command<BiosCommands.Settings>
{
	public class Settings : CommandSettings
	{
		[CommandArgument(0, "<FILENAME>")]
		[Description("Path to the BIOS file")]
		public string Path { get; set; } = string.Empty;

		[CommandOption("-e|--edition <EDITION>")]
		[Description("Change the edition of the BIOS (Disc, Digital, or Slim)")]
		public Edition? Edition { get; set; }

		[CommandOption("-s|--serial <SERIAL>")]
		[Description("Change the console serial number")]
		public string? ConsoleSerial { get; set; }

		[CommandOption("-m|--motherboardSerial <MOTHERBOARDSERIAL>")]
		[Description("Change the motherboard serial number")]
		public string? MotherboardSerial { get; set; }

		[CommandOption("-o|--model <MODEL>")]
		[Description("Change the model")]
		public string? Model { get; set; }
	}

	public override int Execute(CommandContext context, Settings settings)
	{
		AnsiConsole.WriteLine();
		logger.LogInformation("Executing BIOS command with settings: {Settings}", settings);

		BiosInfo biosInfo;
		try
		{
			biosInfo = biosService.ReadBios(settings.Path);
		}
		catch
		{
			logger.LogError("Failed to read BIOS file.");
			AnsiConsole.MarkupLine("[red]Failed to read BIOS file. See log for details.[/]");
			return -1;
		}

		var table = new Table()
		{
			Title = new TableTitle("BIOS Properties"),
		};
		var redStyle = new Style(Color.Red);
		var blueStyle = new Style(Color.Blue);
		var greenStyle = new Style(Color.Green);

		// No properties are going to be changed, so just display the current BIOS properties
		if (settings.Edition is null &&
			settings.ConsoleSerial is null &&
			settings.MotherboardSerial is null &&
			settings.Model is null)
		{
			logger.LogInformation("Displaying BIOS properties without modification.");

			table.AddColumn(new TableColumn("Property").Centered());
			table.AddColumn(new TableColumn("Value").Centered());

			table.AddRow(new Text[] { 
				new("PS5 Version", redStyle), 
				new(biosInfo.Edition.ToString(), blueStyle), 
			});
			table.AddRow(new Text[] { 
				new("Model", redStyle), 
				new(biosInfo.Model, blueStyle), 
			});
			table.AddRow(new Text[] { 
				new("Serial", redStyle), 
				new(biosInfo.ConsoleSerialNumber, blueStyle), 
			});
			table.AddRow(new Text[] { 
				new("Motherboard Serial", redStyle), 
				new(biosInfo.MotherboardSerialNumber, blueStyle), 
			});
			table.AddRow(new Text[] { 
				new("WiFi MAC Addr", redStyle), 
				new(biosInfo.WiFiMac, blueStyle), 
			});
			table.AddRow(new Text[] { 
				new("LAN MAC Addr", redStyle), 
				new(biosInfo.LanMac, blueStyle), 
			});

			AnsiConsole.Write(table);
			AnsiConsole.WriteLine();

			// Display top 5 log entries
			var logTable = new Table()
			{
				Title = new TableTitle("Error Log"),
			};

			logTable.AddColumn(new TableColumn("Error").Centered());
			logTable.AddColumn(new TableColumn("RTC").Centered());
			logTable.AddColumn(new TableColumn("Power State").Centered());
			logTable.AddColumn(new TableColumn("Boot Cause").Centered());
			logTable.AddColumn(new TableColumn("Device Power").Centered());
			logTable.AddColumn(new TableColumn("Sequence Number").Centered());
			logTable.AddColumn(new TableColumn("Env Temp").Centered());
			logTable.AddColumn(new TableColumn("SoC Temp").Centered());

			var errors = biosInfo.Errors.Take(5);

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

		if (settings.Edition is Edition edition)
		{
			logger.LogInformation("Setting edition to {Edition}", edition);

			try
			{
				biosService.SetEdition(biosInfo, edition);
			}
			catch
			{
				logger.LogError("Failed to set edition.");
				AnsiConsole.MarkupLine("[red]Failed to set edition. See log for details.[/]");
				return -1;
			}

			AnsiConsole.MarkupLine("[green]Successfully set edition.[/]");
		}

		if (settings.ConsoleSerial is string serial)
		{
			logger.LogInformation("Setting console serial to {Serial}", serial);

			try
			{
				biosService.SetConsoleSerial(biosInfo, serial);
			}
			catch
			{
				logger.LogError("Failed to set console serial.");
				AnsiConsole.MarkupLine("[red]Failed to set console serial. See log for details.[/]");
				return -1;
			}

			AnsiConsole.MarkupLine("[green]Successfully set console serial.[/]");
		}

		if (settings.MotherboardSerial is string motherboardSerial)
		{
			logger.LogInformation("Setting motherboard serial to {MotherboardSerial}", motherboardSerial);

			try
			{
				biosService.SetMotherboardSerial(biosInfo, motherboardSerial);
			}
			catch
			{
				logger.LogError("Failed to set motherboard serial.");
				AnsiConsole.MarkupLine("[red]Failed to set motherboard serial. See log for details.[/]");
				return -1;
			}

			AnsiConsole.MarkupLine("[green]Successfully set motherboard serial.[/]");
		}

		if (settings.Model is string model)
		{
			logger.LogInformation("Setting model to {Model}", model);

			try
			{
				biosService.SetModel(biosInfo, model);
			}
			catch
			{
				logger.LogError("Failed to set model.");
				AnsiConsole.MarkupLine("[red]Failed to set model. See log for details.[/]");
				return -1;
			}

			AnsiConsole.MarkupLine("[green]Successfully set model.[/]");
		}

		BiosInfo modifiedBiosInfo;
		try
		{
			modifiedBiosInfo = biosService.ReadBios(settings.Path);
		}
		catch
		{
			logger.LogError("Failed to read modified BIOS file.");
			AnsiConsole.MarkupLine("[red]Failed to read modified BIOS file. See log for details.[/]");
			return -1;
		}

		table.AddColumn(new TableColumn("Property").Centered());
		table.AddColumn(new TableColumn("Original Value").Centered());
		table.AddColumn(new TableColumn("Modified Value").Centered());

		table.AddRow(new Text[] {
				new("PS5 Version", redStyle), 
				new(biosInfo.Edition.ToString(), blueStyle), 
				new(modifiedBiosInfo.Edition.ToString(), greenStyle), 
			});
		table.AddRow(new Text[] {
				new("Model", redStyle), 
				new(biosInfo.Model, blueStyle),
				new(modifiedBiosInfo.Model, greenStyle), 
			});
		table.AddRow(new Text[] {
				new("Serial", redStyle),
				new(biosInfo.ConsoleSerialNumber, blueStyle), 
				new(modifiedBiosInfo.ConsoleSerialNumber, greenStyle), 
			});
		table.AddRow(new Text[] {
				new("Motherboard Serial", redStyle), 
				new(biosInfo.MotherboardSerialNumber, blueStyle),
				new(modifiedBiosInfo.MotherboardSerialNumber, greenStyle), 
			});
		table.AddRow(new Text[] {
				new("WiFi MAC Addr", redStyle), 
				new(biosInfo.WiFiMac, blueStyle),
				new(modifiedBiosInfo.WiFiMac, greenStyle), 
			});
		table.AddRow(new Text[] {
				new("LAN MAC Addr", redStyle), 
				new(biosInfo.LanMac, blueStyle),
				new(modifiedBiosInfo.LanMac, greenStyle), 
			});

		AnsiConsole.Write(table);

		return 0;
	}
}
