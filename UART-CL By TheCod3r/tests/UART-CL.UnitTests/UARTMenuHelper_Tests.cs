using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UART_CL_By_TheCod3r;

namespace UART_CL.UnitTests;

public class UARTMenuHelper_Tests
{
    [Test]
    public void ClearUARTCodes_UsesInjectedSerialRead_ReturnsTrue()
    {
        var fakeLines = new List<string> { "UART OK", "UART Done" };
        var fakePorts = new[] { "COM1" };
        List<string> fakeSerialRead(SerialPort _) => fakeLines;

        var result = UARTMenuHelper.ClearUARTCodes(true, fakePorts,
            serialPortFactory: _ => new SerialPort(), promptPortSelection: _ => 1, fakeSerialRead);

        result.Should().BeTrue();
    }

    [Test]
    public void ClearUARTCodes_ReturnsTrue_WhenNoPortsFound()
    {
        var result = UARTMenuHelper.ClearUARTCodes(
            isTest: true,
            fakePorts: Array.Empty<string>()
        );

        result.Should().BeTrue();
    }

    [Test]
    public void ClearUARTCodes_PrintsUARTLines()
    {
        var testPorts = new[] { "COM1" };

        var result = UARTMenuHelper.ClearUARTCodes(
            isTest: true,
            fakePorts: testPorts,
            promptPortSelection: _ => 1,
            serialPortFactory: _ => new SerialPort(),
            customSerialRead: sp => new List<string> { "TEST LINE" },
            printErrorMessage: _ => { }
        );

        result.Should().BeTrue();
    }

    [Test]
    public void ClearUARTCodes_UsesCustomSerialFactory_AndRead()
    {
        var testPorts = new[] { "COM2" };
        var usedPort = string.Empty;
        var readLines = new List<string>();

        var result = UARTMenuHelper.ClearUARTCodes(
            isTest: true,
            fakePorts: testPorts,
            promptPortSelection: _ => 1,
            serialPortFactory: port =>
            {
                usedPort = port;
                return new SerialPort();
            },
            customSerialRead: _ =>
            {
                readLines.Add("Line1");
                readLines.Add("Line2");
                return readLines;
            },
            printErrorMessage: _ => { }
        );

        result.Should().BeTrue();
        usedPort.Should().Be("COM2");
        readLines.Should().HaveCount(2);
    }

    [Test]
    public void ClearUARTCodes_HandlesException_AndReturnsTrue()
    {
        var testPorts = new[] { "COM3" };
        var exceptionHandled = false;

        var result = UARTMenuHelper.ClearUARTCodes(
            isTest: true,
            fakePorts: testPorts,
            promptPortSelection: _ => 1,
            serialPortFactory: _ => throw new Exception("Factory failed"),
            printErrorMessage: ex => exceptionHandled = ex.Message.Contains("Factory failed")
        );

        result.Should().BeTrue();
        exceptionHandled.Should().BeTrue();
    }

    [Test]
    public void ClearUARTCodes_UsesPromptPortSelectionCorrectly()
    {
        var selectedIndex = -1;
        var testPorts = new[] { "COM4", "COM5" };

        var result = UARTMenuHelper.ClearUARTCodes(
            isTest: true,
            fakePorts: testPorts,
            promptPortSelection: len =>
            {
                selectedIndex = len;
                return 2;
            },
            serialPortFactory: port =>
            {
                port.Should().Be("COM5");
                return new SerialPort();
            },
            customSerialRead: _ => new List<string>(),
            printErrorMessage: _ => { }
        );

        result.Should().BeTrue();
        selectedIndex.Should().Be(2);
    }

    [Test]
    public void GetErrorCodesFromPS5_ReturnsTrue_WhenNoPortsFound()
    {
        var result = UARTMenuHelper.GetErrorCodesFromPS5(
            isTest: true,
            mockPorts: Array.Empty<string>()
        );

        result.Should().BeTrue();
    }

    [Test]
    public void GetErrorCodesFromPS5_CallsParseErrors_ForEachValidErrorCode()
    {
        var testPorts = new[] { "COM1" };
        var capturedErrors = new List<string>();

        var result = UARTMenuHelper.GetErrorCodesFromPS5(
            isTest: true,
            mockPorts: testPorts,
            promptPortSelection: _ => 1,
            serialPortFactory: _ => new SerialPort(),
            collectErrorLines: _ => new List<string>
            {
                "OK 00 TESTERR1",
                "OK 00 FFFFFF00",
                "NG ERROR1",
                ""
            },
            parseErrors: code =>
            {
                capturedErrors.Add(code);
                return $"Error Parsed: {code}";
            },
            waitForUser: () => { }
        );

        result.Should().BeTrue();
        capturedErrors.Should().ContainSingle().Which.Should().Be("TESTERR1");
    }

    [Test]
    public void GetErrorCodesFromPS5_HandlesException_AndReturnsTrue()
    {
        var testPorts = new[] { "COM1" };
        var exceptionLogged = false;

        var result = UARTMenuHelper.GetErrorCodesFromPS5(
            isTest: true,
            mockPorts: testPorts,
            promptPortSelection: _ => 1,
            serialPortFactory: _ => throw new Exception("Port config failed"),
            printErrorMessage: ex => exceptionLogged = ex.Message.Contains("Port config failed"),
            waitForUser: () => { }
        );

        result.Should().BeTrue();
        exceptionLogged.Should().BeTrue();
    }

    [Test]
    public void RunCustomUARTCommand_ReturnsTrue_WhenNoPortsFound()
    {
        var result = UARTMenuHelper.RunCustomUARTCommand(
            isTest: true,
            fakePorts: Array.Empty<string>()
        );

        result.Should().BeTrue();
    }

    [Test]
    public void RunCustomUARTCommand_UsesPromptPortSelectionAndFactory()
    {
        var fakePorts = new[] { "COM1", "COM2" };
        int selectedIndex = -1;
        string usedPort = "";

        var result = UARTMenuHelper.RunCustomUARTCommand(
            isTest: true,
            fakePorts: fakePorts,
            promptPortSelection: len =>
            {
                selectedIndex = len;
                return 2;
            },
            serialPortFactory: port =>
            {
                usedPort = port;
                return new SerialPort();
            },
            consoleReadLine: () => "exit"
        );

        result.Should().BeTrue();
        selectedIndex.Should().Be(2);
        usedPort.Should().Be("COM2");
    }

    [Test]
    public void RunCustomUARTCommand_SendsCustomCommandAndReadsResponse()
    {
        var fakePorts = new[] { "COM3" };
        var capturedOutput = new List<string>();
        var readLines = new List<string> { "UART LINE 1", "UART LINE 2" };
        var inputCalls = 0;

        var result = UARTMenuHelper.RunCustomUARTCommand(
            isTest: true,
            fakePorts: fakePorts,
            promptPortSelection: _ => 1,
            serialPortFactory: _ => new SerialPort(),
            customSerialRead: _ => readLines,
            consoleReadLine: () =>
            {
                inputCalls++;
                return inputCalls == 1 ? "TESTCOMMAND" : "exit";
            },
            consoleWrite: capturedOutput.Add,
            waitForUser: () => { }
        );

        result.Should().BeTrue();
        capturedOutput.Should().ContainInOrder("UART LINE 1", "UART LINE 2");
    }

    [Test]
    public void RunCustomUARTCommand_HandlesEmptyCommand()
    {
        var fakePorts = new[] { "COM4" };
        var output = new List<string>();
        var inputCount = 0;

        var result = UARTMenuHelper.RunCustomUARTCommand(
            isTest: true,
            fakePorts: fakePorts,
            promptPortSelection: _ => 1,
            serialPortFactory: _ => new SerialPort(),
            consoleReadLine: () =>
            {
                inputCount++;
                return inputCount == 1 ? "" : "exit";
            },
            consoleWrite: output.Add,
            waitForUser: () => { }
        );

        result.Should().BeTrue();
        output.Should().Contain("Please enter a valid command.");
    }

    [Test]
    public void RunCustomUARTCommand_ExitsLoopWhenUserTypesExit()
    {
        var fakePorts = new[] { "COM5" };
        var readCalled = false;

        var result = UARTMenuHelper.RunCustomUARTCommand(
            isTest: true,
            fakePorts: fakePorts,
            promptPortSelection: _ => 1,
            serialPortFactory: _ => new SerialPort(),
            consoleReadLine: () => "exit",
            customSerialRead: _ =>
            {
                readCalled = true;
                return new List<string>();
            }
        );

        result.Should().BeTrue();
        readCalled.Should().BeFalse("because 'exit' was typed before sending a command");
    }

    [Test]
    public void RunCustomUARTCommand_HandlesExceptionAndReturnsTrue()
    {
        var fakePorts = new[] { "COM6" };
        var exceptionCaught = false;

        var result = UARTMenuHelper.RunCustomUARTCommand(
            isTest: true,
            fakePorts: fakePorts,
            promptPortSelection: _ => 1,
            serialPortFactory: _ => throw new Exception("Fake failure"),
            consoleReadLine: () => "exit",
            printErrorMessage: ex => exceptionCaught = ex.Message.Contains("Fake failure")
        );

        result.Should().BeTrue();
        exceptionCaught.Should().BeTrue();
    }

    [Test]
    public void RunCustomUARTCommand_CanUseAllInjectedDelegates()
    {
        var fakePorts = new[] { "COM7" };
        var selectedPort = "";
        var capturedLines = new List<string>();
        var readCount = 0;

        var result = UARTMenuHelper.RunCustomUARTCommand(
            isTest: true,
            fakePorts: fakePorts,
            promptPortSelection: _ => 1,
            serialPortFactory: port =>
            {
                selectedPort = port;
                return new SerialPort();
            },
            consoleReadLine: () =>
            {
                readCount++;
                return readCount == 1 ? "TEST123" : "exit";
            },
            customSerialRead: _ => new List<string> { "RESPONSE A", "RESPONSE B" },
            consoleWrite: capturedLines.Add,
            waitForUser: () => { }
        );

        result.Should().BeTrue();
        selectedPort.Should().Be("COM7");
        capturedLines.Should().ContainInOrder("RESPONSE A", "RESPONSE B");
    }
}
