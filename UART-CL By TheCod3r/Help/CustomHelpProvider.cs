using Spectre.Console.Cli.Help;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;
using Spectre.Console;

namespace UART_CL_By_TheCod3r.Help;

/// <summary>
/// Custom help provider for the application that generates the footer
/// </summary>
/// <param name="settings"></param>
public class CustomHelpProvider(ICommandAppSettings settings) : HelpProvider(settings)
{
	public override IEnumerable<IRenderable> GetFooter(ICommandModel model, ICommandInfo? command)
	{
		var style = settings?.HelpProviderStyles?.Usage?.Header ?? null;

		return [
			Text.NewLine,
			new Markup("INFO:", style),
			Text.NewLine,
			new Markup("    UARL-CL is designed with simplicity in mind. This command line application makes it quick and easy to obtain error codes from your PlayStation 5 console."),
			Text.NewLine,
			Text.NewLine,
			new Markup("    UART stands for Universal Asynchronous Receiver-Transmitter. UART allows you to send and receive commands to any compatible serial communications device."),
			Text.NewLine,
			Text.NewLine,
			new Markup("    The PlayStation 5 has UART functionality built in. Unfortunately Sony don't make it easy to understand what is happening with the machine when you request " +
						"error codes, which is why this application exists. UART-CL is a command-line spin off to the PS5 NOR and UART tool for Windows, which allows you to communicate " +
						"via serial to your PlayStation 5. You can grab error codes from the system at the click of a button (well, a few clicks) and the software will automatically " +
						"check the error codes received and attempt to convert them into plain text."),
			Text.NewLine,
			Text.NewLine,
			new Markup("    This is done by splitting the error codes up into useful sections and then comparing those error codes against a database of codes collected by the repair " +
						"community. If the code exists in the database, the application will automatically grab the error details and output them for you on the screen so you can figure " +
						"out your next move."),
			Text.NewLine,
			Text.NewLine,
			new Markup("    [blue]Where does the database come from?[/]"),
			Text.NewLine,
			new Markup("    The database is downloaded on first launch from uartcodes.com/xml.php. The download page is hosted by \"TheCod3r\" for free and is simply a PHP script which " +
						"fetches all known error codes from the uartcodes.com database and converts them into an XML document. An XML document makes it quick and easy to work with and " +
						"it's a more elegant solution to provide a database which doesn't rely on an internet connection. It can also be updated whenever you like, free of charge."),
			Text.NewLine,
			Text.NewLine,
			new Markup("    [blue]How do I use this to fix my PS5?[/]"),
			Text.NewLine,
			new Markup("    You'll need a compatible serial communication (UART) device first of all. Most devices that have a transmit, receive, and ground pin and that can provide 3.3v" +
						"instead of 5v should work, and you can buy one for a few bucks on eBay, Amazon or AliExpress."),
			Text.NewLine,
			Text.NewLine,
			new Markup("    Once you have a compatible device, you'll need to:"),
			Text.NewLine,
			new Markup("    - Solder the transmit pin on the device to the receive pin on the PS5."),
			Text.NewLine,
			new Markup("    - Solder the receive pin on the device to the transmit pin on the PS5."),
			Text.NewLine,
			new Markup("    - Solder ground on the device to ground on the PS5."),
			Text.NewLine,
			new Markup("    - Connect the PS5 power chord to the PS5 power supply (do not turn on the console)"),
			Text.NewLine,
			new Markup("    - Use this software and select either option 1, 2 or 3 to run commands."),
			Text.NewLine,
			new Markup("    - Choose your device from the list of available devices."),
			Text.NewLine,
			new Markup("    - Let the software do the rest. Then working out a plan for the actual repair is up to you. We can't do everything ;)"),
			Text.NewLine,
			Text.NewLine,
			new Markup("    As a personal note from myself (TheCod3r). I want to thank you for trusting my software. I'm an electronics technician primarily and I write code for fun " +
						"(ironic since my name is TheCod3r, I know!). I would also like to thank the following people for inspiring me:"),
			Text.NewLine,
			new Markup("    Louis Rossmann, FightToRepair.org, Jessa Jones (iPad Rehab), Andy-Man (PS5 Wee Tools), my YouTube viewers, my Patreon supporters and my mom!"),
			Text.NewLine,
			Text.NewLine,
			new Markup("    #FuckBwE"),
			Text.NewLine,
			new Markup("    Be sure to use the hashtag. It really pisses him off!"),
			Text.NewLine,
			Text.NewLine,
			new Markup("    If you want to say thanks, you can buy me a coffee."),
			Text.NewLine,
			new Markup("    [link]https://www.streamelements.com/thecod3r/tip[/]"),
		];
	}
}
