using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using NorModifierLib.Data;
using NorModifierLib.Services;

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

		// Clear errors flag specified
		if (settings.Clear is true)
		{
			try
			{
				uartService.ClearErrors(settings.Path);
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
		IEnumerable<ErrorCode> errors;
		try
		{
			errors = uartService.GetErrors(settings.Path);
		}
		catch
		{
			logger.LogError("Failed to get errors.");
			AnsiConsole.MarkupLine("[red]Failed to get errors. See log for details.[/]");
			return -1;
		}

		if (!errors.Any())
		{
			logger.LogInformation("No errors logged.");
			AnsiConsole.MarkupLine("[green]No errors logged.[/]");
			return 0;
		}

		foreach (var error in errors)
		{
			try
			{
				string errorDescription = await errorCodeService.ParseError(error.SecondPart);
				logger.LogInformation("Retrieved error code description: {ErrorCode} - {Description}", error.SecondPart, errorDescription);
				AnsiConsole.MarkupLineInterpolated($"[green]Found error: {error.SecondPart} - {errorDescription}.");
			}
			catch
			{
				logger.LogError("Failed to parse error code: {ErrorCode}", error);
				AnsiConsole.MarkupLineInterpolated($"[red]Failed to parse error code: {error.SecondPart}. See log for details.[/]");
			}
		}

		return 0;
	}
}
