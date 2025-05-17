using System.IO.Ports;
using FluentAssertions;
using NUnit.Framework;
using UART_CL_By_TheCod3r;
using UART_CL_By_TheCod3r.UARTMenu;
using UART_CL_By_TheCod3r.Utilities;

namespace UART_CL.UnitTests.UARTMenu;

public class UARTMenuHelper_Tests
{
    private List<string> _output = null!;
    private Action<string> _writeLine = null!;
    private Func<string> _readLine = null!;
    private Queue<string> _readLineInputs = null!;

    [SetUp]
    public void SetUp()
    {
        _output = new List<string>();
        _writeLine = s => _output.Add(s);
        _readLineInputs = new Queue<string>();
        _readLine = () => _readLineInputs.Count > 0 ? _readLineInputs.Dequeue() : "";
    }

    [Test]
    public void ClearUARTCodes_ShouldWriteClearCommand_AndDisplayUARTOutput()
    {
        string? writtenCommand = null;
        bool wasClosed = false;
        var uartOutput = new List<string> { "OK DEVICE XYZ123", "CLEARED ERRORS" };

        var result = UARTMenuHelper.ClearUARTCodes(
            getAvailablePorts: () => new[] { "COM3" },
            promptPortSelection: (_, _) => 1,
            configureSerialPort: _ => new SerialPort(),
            openSerialPort: (_, _, _) => { }, // skip real open
            readUARTLines: _ => uartOutput,
            writeLineToPort: (_, cmd) => writtenCommand = cmd,
            closePort: _ => wasClosed = true,
            readLine: _readLine,
            writeLine: _writeLine
        );

        result.Should().BeTrue();
        writtenCommand.Should().Be(PS5UARTUtilities.CalculateChecksum("errlog clear"));
        _output.Should().Contain("OK DEVICE XYZ123");
        _output.Should().Contain("CLEARED ERRORS");
        _output.Should().Contain("Press Enter to continue...");
        wasClosed.Should().BeTrue();
    }

    [Test]
    public void ClearUARTCodes_ShouldContinue_WhenUserPressesEnter()
    {
        bool continuePressed = false;

        var result = UARTMenuHelper.ClearUARTCodes(
            getAvailablePorts: () => new[] { "COM3" },
            promptPortSelection: (_, _) => 1,
            configureSerialPort: _ => new SerialPort(),
            openSerialPort: (_, _, _) => { },
            readUARTLines: _ => new List<string> { "Test Line" },
            writeLineToPort: (_, _) => { }, // ignore writing command for this test
            closePort: _ => { }, // ignore closing port for this test
            readLine: () =>
            {
                continuePressed = true;
                return "";
            },
            writeLine: _writeLine
        );

        result.Should().BeTrue();
        continuePressed.Should().BeTrue();
        _output.Should().Contain("Test Line");
        _output.Should().Contain("Press Enter to continue...");
    }

    [Test]
    public void ClearUARTCodes_ShouldShowNoDevicesMessage_WhenNoPortsAvailable()
    {
        var result = UARTMenuHelper.ClearUARTCodes(
            getAvailablePorts: () => Array.Empty<string>(),
            readLine: _readLine,
            writeLine: _writeLine
        );

        result.Should().BeTrue();
        _output.Should().Contain("No communication devices were found on this system.");
    }

    [Test]
    public void ClearUARTCodes_ShouldHandleException_AndPrintErrorMessage()
    {
        bool readLineCalledAfterError = false;
        var errorMessagePrinted = false;

        var result = UARTMenuHelper.ClearUARTCodes(
            getAvailablePorts: () => new[] { "COM3" },
            promptPortSelection: (_, _) => 1,
            configureSerialPort: _ => throw new InvalidOperationException("Test exception"),
            readLine: () =>
            {
                readLineCalledAfterError = true;
                return "";
            },
            writeLine: s =>
            {
                if (s.Contains("Error"))
                    errorMessagePrinted = true;
                _output.Add(s);
            }
        );

        result.Should().BeTrue();
        errorMessagePrinted.Should().BeTrue();
        readLineCalledAfterError.Should().BeTrue();
    }

    [Test]
    public void GetErrorCodesFromPS5_ShouldPrintNoDevicesMessage_WhenNoPortsAvailable()
    {
        var result = UARTMenuHelper.GetErrorCodesFromPS5(
            getAvailablePorts: () => Array.Empty<string>(),
            readLine: _readLine,
            writeLine: _writeLine
        );

        result.Should().BeTrue();
        _output.Should().Contain("No communication devices were found on this system.");
        _output.Should().Contain("Please insert a UART compatible device and try again.");
    }

    [Test]
    public void GetErrorCodesFromPS5_ShouldPrintNoError_WhenCodeStartsWithFFFFFF()
    {
        var result = UARTMenuHelper.GetErrorCodesFromPS5(
            getAvailablePorts: () => new[] { "COM1" },
            promptPortSelection: (_, _) => 1,
            configureSerialPorts: _ => new SerialPort(), // Will not be opened, just passed in
            openSerialPort: (_, _, _) => { }, // stub to do nothing
            collectErrorLines: _ => new List<string> { "OK DEVICE FFFFFFF1" },
            readLine: _readLine,
            writeLine: _writeLine
        );

        result.Should().BeTrue();
        _output.Should().Contain("No error displayed");
        _output.Should().Contain("Press Enter to continue...");
    }

    [Test]
    public void GetErrorCodesFromPS5_ShouldParseError_WhenValidCodeProvided()
    {
        string? parsed = null;

        var result = UARTMenuHelper.GetErrorCodesFromPS5(
            getAvailablePorts: () => new[] { "COM3" },
            promptPortSelection: (_, _) => 1,
            configureSerialPorts: _ => new SerialPort(), // won't be opened
            openSerialPort: (_, _, _) => { }, // stub to do nothing
            collectErrorLines: _ => new List<string> { "OK DEVICE ABCD1234" },
            parseErrors: code =>
            {
                parsed = $"Error parsed: {code}";
                return parsed;
            },
            readLine: _readLine,
            writeLine: _writeLine
        );

        result.Should().BeTrue();
        parsed.Should().Be("Error parsed: ABCD1234");
        _output.Should().Contain("Error parsed: ABCD1234");
        _output.Should().Contain("Press Enter to continue...");
    }

    [Test]
    public void GetErrorCodesFromPS5_ShouldIgnoreNGLine()
    {
        var result = UARTMenuHelper.GetErrorCodesFromPS5(
            getAvailablePorts: () => new[] { "COM4" },
            promptPortSelection: (_, _) => 1,
            configureSerialPorts: _ => new SerialPort(),
            collectErrorLines: _ => new List<string> { "NG DEVICE 00000001" },
            readLine: _readLine,
            writeLine: _writeLine
        );

        result.Should().BeTrue();
        _output.Should().Contain("Press Enter to continue...");
    }

    [Test]
    public void GetErrorCodesFromPS5_ShouldIgnoreEmptyLines()
    {
        var result = UARTMenuHelper.GetErrorCodesFromPS5(
            getAvailablePorts: () => new[] { "COM2" },
            promptPortSelection: (_, _) => 1,
            configureSerialPorts: _ => new SerialPort(),
            collectErrorLines: _ => new List<string> { "", "   " },
            readLine: _readLine,
            writeLine: _writeLine
        );

        result.Should().BeTrue();
        _output.Should().Contain("Press Enter to continue...");
    }

    [Test]
    public void GetErrorCodesFromPS5_ShouldFallbackToDefaultParser_IfNoneProvided()
    {
        var result = UARTMenuHelper.GetErrorCodesFromPS5(
            getAvailablePorts: () => new[] { "COM5" },
            promptPortSelection: (_, _) => 1,
            configureSerialPorts: _ => new SerialPort(),
            collectErrorLines: _ => new List<string> { "OK DEVICE 12345678" },
            readLine: _readLine,
            writeLine: _writeLine
        );

        result.Should().BeTrue();
        _output.Should().Contain("Press Enter to continue...");
    }

    [Test]
    public void GetErrorCodesFromPS5_ShouldPrintErrorDetails_WhenExceptionOccurs()
    {
        var result = UARTMenuHelper.GetErrorCodesFromPS5(
            getAvailablePorts: () => new[] { "COM6" },
            promptPortSelection: (_, _) => 1,
            configureSerialPorts: _ => throw new InvalidOperationException("Port failed"),
            readLine: _readLine,
            writeLine: _writeLine
        );

        result.Should().BeTrue();
        _output.Should().Contain("An error occurred while connecting to your selected device.");
        _output.Should().Contain("Error details:");
        _output.Should().Contain("Port failed");
    }

    [Test]
    public void RunCustomUARTCommand_ShouldReturnTrue_AndShowNoDevices_WhenNoPortsAvailable()
    {
        var result = UARTMenuHelper.RunCustomUARTCommand(
            getAvailablePorts: () => Array.Empty<string>(),
            writeLine: _writeLine,
            readLine: _readLine
        );

        result.Should().BeTrue();
        _output.Should().Contain("No communication devices were found on this system.");
    }

    [Test]
    public void RunCustomUARTCommand_ShouldPromptForCommands_AndSendThemUntilExit()
    {
        var fakeSerialPort = new SerialPort("COM3");
        var sentCommands = new List<string>();
        var uartOutputs = new Queue<List<string>>();
        var portClosed = false;

        // Simulate UART output lines for each command
        uartOutputs.Enqueue(new List<string> { "Response to CMD1" });
        uartOutputs.Enqueue(new List<string> { "Response to CMD2" });

        // Commands user will enter in sequence, ending with "exit"
        _readLineInputs.Enqueue("CMD1");
        _readLineInputs.Enqueue("");
        _readLineInputs.Enqueue("CMD2");
        _readLineInputs.Enqueue("");
        _readLineInputs.Enqueue("");         // empty command triggers warning
        _readLineInputs.Enqueue("exit");

        var result = UARTMenuHelper.RunCustomUARTCommand(
            getAvailablePorts: () => new[] { "COM3" },
            promptPortSelection: (count, rl) => 1,
            configureSerialPort: portName =>
            {
                portName.Should().Be("COM3");
                return fakeSerialPort;
            },
            openSerialPort: (sp, portName, writeLine) => { /* skip real open */ },
            readUARTLines: sp =>
            {
                uartOutputs.TryDequeue(out var lines);
                return lines ?? new List<string>();
            },
            readLine: _readLine,
            writeLine: _writeLine,
            writeLineToPort: (sp, command) => sentCommands.Add(command),
            closePort: sp => portClosed = true
        );

        result.Should().BeTrue();
        sentCommands.Should().HaveCount(2);
        sentCommands[0].Should().Be(PS5UARTUtilities.CalculateChecksum("CMD1"));
        sentCommands[1].Should().Be(PS5UARTUtilities.CalculateChecksum("CMD2"));

        _output.Should().Contain("Please enter a valid command.");
        _output.Should().Contain("Response to CMD1");
        _output.Should().Contain("Response to CMD2");
        portClosed.Should().BeTrue();
    }

    [Test]
    public void RunCustomUARTCommand_ShouldSendChecksumCommand_AndDisplayUARTOutput()
    {
        var sentChecksums = new List<string>();
        var uartOutput = new List<string> { "OK DEVICE", "RESULT 123" };
        var portClosed = false;

        _readLineInputs.Enqueue("TESTCOMMAND");
        _readLineInputs.Enqueue("");
        _readLineInputs.Enqueue("exit");
        _readLineInputs.Enqueue("");

        var result = UARTMenuHelper.RunCustomUARTCommand(
            getAvailablePorts: () => new[] { "COM3" },
            promptPortSelection: (count, rl) => 1,
            configureSerialPort: portName => new SerialPort(),
            openSerialPort: (sp, portName, writeLine) => { },
            readUARTLines: sp => uartOutput,
            readLine: _readLine,
            writeLine: _writeLine,
            writeLineToPort: (sp, command) => sentChecksums.Add(command),
            closePort: sp => portClosed = true
        );

        result.Should().BeTrue();
        sentChecksums.Should().ContainSingle().Which.Should().Be(PS5UARTUtilities.CalculateChecksum("TESTCOMMAND"));
        _output.Should().Contain("OK DEVICE");
        _output.Should().Contain("RESULT 123");
        portClosed.Should().BeTrue();
    }

    [Test]
    public void RunCustomUARTCommand_ShouldHandleExceptions_AndPrintError()
    {
        var fakeSerialPort = new SerialPort("COM3");
        bool writeLineCalled = false;

        var result = UARTMenuHelper.RunCustomUARTCommand(
            getAvailablePorts: () => new[] { "COM3" },
            promptPortSelection: (count, rl) => 1,
            configureSerialPort: portName => fakeSerialPort,
            openSerialPort: (sp, portName, writeLine) => throw new InvalidOperationException("Open failed"),
            readLine: _readLine,
            writeLine: s =>
            {
                writeLineCalled = true;
                _output.Add(s);
            }
        );

        result.Should().BeTrue();
        writeLineCalled.Should().BeTrue();
        _output.Should().Contain("An error occurred while connecting to your selected device.");
        _output.Should().Contain("Open failed");
    }

    [Test]
    public void RunCustomUARTCommand_ShouldUsePromptPortSelection_Correctly()
    {
        var promptCalled = false;
        var fakeSerialPort = new SerialPort("COM1");
        var ports = new[] { "COM1", "COM2" };

        _readLineInputs.Enqueue("exit");

        var result = UARTMenuHelper.RunCustomUARTCommand(
            getAvailablePorts: () => ports,
            promptPortSelection: (count, rl) =>
            {
                promptCalled = true;
                count.Should().Be(2);
                return 2; // selects "COM2"
            },
            configureSerialPort: portName =>
            {
                portName.Should().Be("COM2");
                return fakeSerialPort;
            },
            openSerialPort: (sp, portName, writeLine) => { },
            readUARTLines: sp => new List<string>(),
            readLine: _readLine,
            writeLine: _writeLine,
            closePort: sp => { }
        );

        result.Should().BeTrue();
        promptCalled.Should().BeTrue();
    }
}
