using System.IO.Ports;

namespace UART_CL_By_TheCod3r;

/// <summary>
/// Manages all UART (serial port) communication with the PS5.
/// </summary>
public class UartService
{
    private readonly ErrorCodeService _errorCodeService = new();

    public void GetErrorLogsFromPs5()
    {
        ExecuteUartCommand(port =>
        {
            Console.WriteLine("\nFetching error logs...");
            List<string> uniqueErrorLines = new List<string>();

            // The PS5 error log holds a maximum of 10 entries (0-9).
            for (int i = 0; i < 10; i++)
            {
                string command = $"errlog {i}";
                port.WriteLine(Utilities.Checksum.Append(command));

                // Give the device time to respond.
                Thread.Sleep(100);

                string line = port.ReadLine().Trim();

                // Process valid responses that are not echoes of the command
                if (!string.IsNullOrEmpty(line) && !line.Contains("errlog") && !uniqueErrorLines.Contains(line))
                {
                    uniqueErrorLines.Add(line);
                }
            }

            Console.WriteLine("\n--- PS5 Error Log ---");
            if (uniqueErrorLines.Count == 0)
            {
                Console.WriteLine("No error codes found on the device.");
            }
            else
            {
                foreach (var line in uniqueErrorLines)
                {
                    ProcessAndDisplayError(line);
                }
            }
            Console.WriteLine("---------------------");
        });
    }

    public void ClearErrorLogsOnPs5()
    {
        ExecuteUartCommand(port =>
        {
            Console.WriteLine("\nSending command to clear error logs...");
            string command = "errlog clear";
            port.WriteLine(Utilities.Checksum.Append(command));
            Thread.Sleep(100);
            string response = port.ReadExisting();
            Console.WriteLine($"Response from device:\n{response}");
            Console.WriteLine("Error log clear command sent.");
        });
    }

    public void SendCustomCommandToPs5()
    {
        ExecuteUartCommand(port =>
        {
            Console.WriteLine("\nEnter a custom command to send (or 'exit' to quit).");
            while (true)
            {
                Console.Write("CMD> ");
                string? command = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(command) || command.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                port.WriteLine(Utilities.Checksum.Append(command));
                Thread.Sleep(200); // Wait for a response
                string response = port.ReadExisting();
                Console.WriteLine($"RESPONSE:\n{response}");
            }
        });
    }

    private void ProcessAndDisplayError(string line)
    {
        var parts = line.Split(' ');
        if (parts.Length > 2 && parts[0] == "OK")
        {
            string errorCode = parts[2];
            if (errorCode.StartsWith("FFFFFFFF"))
            {
                Console.WriteLine("Log entry is empty.");
            }
            else
            {
                string description = _errorCodeService.GetErrorDescription(errorCode);
                Console.WriteLine(description);
            }
        }
        else
        {
            Console.WriteLine($"Unrecognized response: {line}");
        }
    }

    private void ExecuteUartCommand(Action<SerialPort> uartAction)
    {
        string? portName = SelectComPort();
        if (portName == null) return;

        using (var serialPort = new SerialPort(portName))
        {
            serialPort.BaudRate = 115200;
            serialPort.RtsEnable = true;
            serialPort.ReadTimeout = 500; // 500ms timeout

            try
            {
                serialPort.Open();
                Console.WriteLine($"Successfully opened port {portName}.\n");
                uartAction(serialPort);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error communicating with port {portName}: {ex.Message}");
            }
        }
    }

    private string? SelectComPort()
    {
        string[] ports = SerialPort.GetPortNames();
        if (ports.Length == 0)
        {
            Console.WriteLine("No COM ports found. Please connect a UART device.");
            return null;
        }

        Console.WriteLine("\nAvailable COM ports:");
        for (int i = 0; i < ports.Length; i++)
        {
            string friendlyName = Utilities.System.GetPortFriendlyName(ports[i]);
            Console.WriteLine($"{i + 1}. {ports[i]} ({friendlyName})");
        }

        int selectedIndex;
        do
        {
            Console.Write("Select a port number: ");
        } while (!int.TryParse(Console.ReadLine(), out selectedIndex) || selectedIndex < 1 || selectedIndex > ports.Length);

        return ports[selectedIndex - 1];
    }
}
