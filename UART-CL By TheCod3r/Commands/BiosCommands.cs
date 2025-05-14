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
				new("PS5 Version", new Style(Color.Red)), 
				new(biosInfo.Edition.ToString(), new Style(Color.Blue)), 
			});
			table.AddRow(new Text[] { 
				new("Model", new Style(Color.Red)), 
				new(biosInfo.Model, new Style(Color.Blue)), 
			});
			table.AddRow(new Text[] { 
				new("Serial", new Style(Color.Red)), 
				new(biosInfo.ConsoleSerialNumber, new Style(Color.Blue)), 
			});
			table.AddRow(new Text[] { 
				new("Motherboard Serial", new Style(Color.Red)), 
				new(biosInfo.MotherboardSerialNumber, new Style(Color.Blue)), 
			});
			table.AddRow(new Text[] { 
				new("WiFi MAC Addr", new Style(Color.Red)), 
				new(biosInfo.WiFiMac, new Style(Color.Blue)), 
			});
			table.AddRow(new Text[] { 
				new("LAN MAC Addr", new Style(Color.Red)), 
				new(biosInfo.LanMac, new Style(Color.Blue)), 
			});

			AnsiConsole.Write(table);

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
				new("PS5 Version", new Style(Color.Red)), 
				new(biosInfo.Edition.ToString(), new Style(Color.Blue)), 
				new(modifiedBiosInfo.Edition.ToString(), new Style(Color.Green)), 
			});
		table.AddRow(new Text[] {
				new("Model", new Style(Color.Red)), 
				new(biosInfo.Model, new Style(Color.Blue)),
				new(modifiedBiosInfo.Model, new Style(Color.Green)), 
			});
		table.AddRow(new Text[] {
				new("Serial", new Style(Color.Red)),
				new(biosInfo.ConsoleSerialNumber, new Style(Color.Blue)), 
				new(modifiedBiosInfo.ConsoleSerialNumber, new Style(Color.Green)), 
			});
		table.AddRow(new Text[] {
				new("Motherboard Serial", new Style(Color.Red)), 
				new(biosInfo.MotherboardSerialNumber, new Style(Color.Blue)),
				new(modifiedBiosInfo.MotherboardSerialNumber, new Style(Color.Green)), 
			});
		table.AddRow(new Text[] {
				new("WiFi MAC Addr", new Style(Color.Red)), 
				new(biosInfo.WiFiMac, new Style(Color.Blue)),
				new(modifiedBiosInfo.WiFiMac, new Style(Color.Green)), 
			});
		table.AddRow(new Text[] {
				new("LAN MAC Addr", new Style(Color.Red)), 
				new(biosInfo.LanMac, new Style(Color.Blue)),
				new(modifiedBiosInfo.LanMac, new Style(Color.Green)), 
			});

		AnsiConsole.Write(table);

		return 0;
	}
}
