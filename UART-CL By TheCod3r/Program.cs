using Microsoft.Extensions.DependencyInjection;
using NReco.Logging.File;
using Spectre.Console.Cli;
using UART_CL_By_TheCod3r.Commands;
using UART_CL_By_TheCod3r.Core;
using UART_CL_By_TheCod3r.Help;
using NorModifierLib.Services;

var logFile = Path.Combine(AppContext.BaseDirectory, "log.txt");

// Configure services - logging, http client, and custom services
var services = new ServiceCollection();

services.AddLogging(builder =>
{
	builder.AddFile(logFile, false);
});

services.AddHttpClient();

services.AddSingleton<ErrorCodeService>();
services.AddSingleton<NorService>();
services.AddSingleton<UartService>();

var registrar = new TypeRegistrar(services);

// Configure and run the application
var app = new CommandApp(registrar);

app.Configure(config =>
{
	config.Settings.ApplicationName = "UART-CL";

	config.AddCommand<NorCommands>("nor")
		.WithDescription("View and modify NOR dumps. Will read and display NOR properties when no options are specified.")
		.WithExample("nor", "./nor.bin")
		.WithExample("nor", "./nor.bin", "--serial AJ11111111");
	config.AddCommand<UartCommands>("uart")
		.WithDescription("Read and clear errors from the console UART. Will read and display errors when no options are specified.")
		.WithExample("uart", "COM3")
		.WithExample("uart", "COM3", "--clear")
		.WithExample("uart", "/dev/ttyUSB0")
		.WithExample("uart", "/dev/ttyUSB0", "--clear");

	config.SetHelpProvider(new CustomHelpProvider(config.Settings));
});

app.Run(args);