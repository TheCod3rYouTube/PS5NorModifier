using System.Text;
using FluentAssertions;
using NUnit.Framework;
using UART_CL_By_TheCod3r.SubMenu;

namespace UART_CL.UnitTests.SubMenu;

public class SubMenuHelper_Tests
{
    private Dictionary<string, string> _regionMap = null!;
    private List<string> _output = null!;
    private Action<string> _writeLine = null!;
    private Func<string> _readLine = null!;

    [SetUp]
    public void SetUp()
    {
        _regionMap = new Dictionary<string, string> { { "US1", "United States" } };
        _output = new List<string>();
        _writeLine = s => _output.Add(s);
        _readLine = () => ""; // simulate Enter key
    }

    [TearDown]
    public void DeleteTempFile()
    {
        try
        {
            File.Delete(Path.GetTempFileName());
        }
        catch
        {
            // Do nothing
        }
    }

    [Test]
    public void ViewBIOSInformation_ShouldDisplayCorrectParsedValues()
    {
        var memory = new byte[0x1C8000];
        Array.Copy(new byte[] { 0x22, 0x02, 0x01, 0x01, 0xAB, 0xCD, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0, memory, 0x1C7010, 12);
        Array.Copy(Encoding.ASCII.GetBytes("PS5-US1".PadLeft(19)), 0, memory, 0x1C7226, 19);
        Array.Copy(Encoding.ASCII.GetBytes("C12345678901234"), 0, memory, 0x1C7210, 15);
        Array.Copy(Encoding.ASCII.GetBytes("M98765432109876 "), 0, memory, 0x1C7200, 16);
        Array.Copy(new byte[] { 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF }, 0, memory, 0x1C73C0, 6);
        Array.Copy(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 }, 0, memory, 0x1C4020, 6);

        var tmpPath = CreateTempFile(memory);

        SubMenuHelper.ViewBIOSInformaition(_regionMap, tmpPath, _writeLine, _readLine);

        _output.Should().Contain(s => s.Contains("Disc Edition"));
        _output.Should().Contain(s => s.Contains("PS5-US1 - United States"));
        _output.Should().Contain(s => s.Contains("C12345678901234"));
        _output.Should().Contain(s => s.Contains("M98765432109876"));
        _output.Should().Contain(s => s.Contains("AA-BB-CC-DD-EE-FF"));
        _output.Should().Contain(s => s.Contains("11-22-33-44-55-66"));
    }

    [Test]
    public void ViewBIOSInformation_ShouldHandleMissingOffsetsGracefully()
    {
        var memory = new byte[0x100]; // too short

        // Create temp file with dummy content to avoid FileNotFoundException
        var tmpPath = CreateTempFile(memory);

        var act = () => SubMenuHelper.ViewBIOSInformaition(_regionMap, tmpPath, _writeLine, _readLine);

        act.Should().NotThrow();
        _output.Should().Contain(s => s.Contains("Unknown Model"));
        _output.Should().Contain(s => s.Contains("Unknown S/N"));
        _output.Should().Contain(s => s.Contains("Unknown Mac Address"));
    }

    [Test]
    public void ViewBIOSInformation_ShouldDefaultToConsoleIO_WhenNoDelegatesProvided()
    {
        // Just ensure it doesn’t throw
        var memory = new byte[0x1C8000];
        Array.Copy(Encoding.ASCII.GetBytes("22020101abcd"), 0, memory, 0x1C7010, 12);
        var tmpPath = CreateTempFile(memory);

        var act = () => SubMenuHelper.ViewBIOSInformaition(_regionMap, tmpPath, _writeLine, _readLine);

        act.Should().NotThrow();
        _output.Should().Contain(s => s.Contains("Unknown Model"));
    }

    [TestCase("22030101abcd", "Digital Edition")]
    [TestCase("22010101abcd", "Slim Edition")]
    public void ViewBIOSInformation_ShouldDetectDigitalAndSlimVariants(string consoleCode, string consoleType)
    {
        var memory = new byte[0x1C8000];

        var consoleCodeBytes = HexStringToBytes(consoleCode);
        Array.Copy(consoleCodeBytes, 0, memory, 0x1C7010, consoleCodeBytes.Length);

        var tmpPath = CreateTempFile(memory);

        SubMenuHelper.ViewBIOSInformaition(_regionMap, tmpPath, _writeLine, _readLine);

        _output.Should().Contain(s => s.Contains(consoleType));
    }

    [Test]
    public void LoadDumpFile_ValidPath_ReturnsPath()
    {
        var memory = new byte[0x1C8000];

        Array.Copy(Encoding.ASCII.GetBytes("C12345678901234"), 0, memory, 0x1C7210, 15);

        var tmpPath = CreateTempFile(memory);
        var fileSize = new FileInfo(tmpPath).Length;

        var inputs = new Queue<string>(new[] { tmpPath });

        var path = SubMenuHelper.LoadDumpFile(readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            fileExists: _ => true,
            getExtension: _ => ".bin",
            sleep: _ => { });

        path.Should().Be(tmpPath);
        _output.Should().Contain($"Selected file: {tmpPath} - File Size: {fileSize} bytes (1MB)");
    }

    [Test]
    public void LoadDumpFile_ExitTyped_ReturnsNullPath()
    {
        var inputs = new Queue<string>(new[] { "exit" });

        var path = SubMenuHelper.LoadDumpFile(readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            fileExists: _ => false,
            getExtension: _ => "",
            sleep: _ => { });

        path.Should().BeNullOrEmpty();
    }

    [Test]
    public void LoadDumpFile_BlankInput_ShowsError()
    {
        var inputs = new Queue<string>(new[] { "   ", "exit" });

        SubMenuHelper.LoadDumpFile(readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            fileExists: _ => false,
            getExtension: _ => "",
            sleep: _ => { });

        _output.Should().Contain("Invalid input. File path cannot be blank.");
    }

    [Test]
    public void LoadDumpFile_FileNotFound_ShowsError()
    {
        var inputs = new Queue<string>(new[] { "/fake/path/file.bin", "exit" });

        SubMenuHelper.LoadDumpFile(readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            fileExists: _ => false,
            getExtension: _ => ".bin",
            sleep: _ => { });

        _output.Should().Contain("The file path you entered does not exist. Please enter the path to a valid .bin file.");
    }

    [Test]
    public void LoadDumpFile_InvalidExtension_ShowsError()
    {
        var inputs = new Queue<string>(new[] { "/fake/path/file.txt", "exit" });

        SubMenuHelper.LoadDumpFile(readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            fileExists: _ => true,
            getExtension: _ => ".txt",
            sleep: _ => { });

        _output.Should().Contain("The file you provided is not a .bin file. Please enter a valid .bin file path.");
    }

    [Test]
    public void ConvertToDigital_ShouldConvert_IfNotAlreadyDigital()
    {
        var memory = new byte[0x1C8000];
        // Set original values that are NOT digital (22010101 at offsetOne, 22020101 at offsetTwo)
        var offsetOneBytes = HexStringToBytes("22010101");
        var offsetTwoBytes = HexStringToBytes("22020101");

        Array.Copy(offsetOneBytes, 0, memory, 0x1C7010, 4);
        Array.Copy(offsetTwoBytes, 0, memory, 0x1C7030, 4);

        var tmpPath = CreateTempFile(memory);

        var confirmCalled = false;
        var convertCalled = false;

        SubMenuHelper.ConvertToDigital(tmpPath,
            confirmPrompt: type => { confirmCalled = true; return "yes"; },
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (f1, r1, f2, r2, type, path) =>
            {
                convertCalled = true;
                type.Should().Be("digital edition");
                path.Should().Be(tmpPath);
            });

        confirmCalled.Should().BeTrue();
        convertCalled.Should().BeTrue();
    }

    [Test]
    public void ConvertToDigital_ShouldSkipIfAlreadyDigital()
    {
        var memory = new byte[0x1C8000];
        // Set digital value at offsetOne
        var offsetOneBytes = HexStringToBytes("22030101");
        Array.Copy(offsetOneBytes, 0, memory, 0x1C7010, 4);

        var tmpPath = CreateTempFile(memory);

        var convertCalled = false;

        SubMenuHelper.ConvertToDigital(tmpPath,
            confirmPrompt: _ => "yes",
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => convertCalled = true);

        _output.Should().Contain(s => s.Contains("already a digital edition"));
        convertCalled.Should().BeFalse();
    }

    [Test]
    public void ConvertToDigital_ShouldNotConvert_WhenUserDeclines()
    {
        var memory = new byte[0x1C8000];
        var tmpPath = CreateTempFile(memory);

        var convertCalled = false;

        SubMenuHelper.ConvertToDigital(tmpPath,
            confirmPrompt: _ => "no",
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => convertCalled = true);

        convertCalled.Should().BeFalse();
        _output.Should().BeEmpty();
    }

    [Test]
    public void ConvertToDigital_ShouldRetryOnInvalidInput()
    {
        var memory = new byte[0x1C8000];
        var offsetOneBytes = HexStringToBytes("22010101");
        var offsetTwoBytes = HexStringToBytes("22020101");
        Array.Copy(offsetOneBytes, 0, memory, 0x1C7010, 4);
        Array.Copy(offsetTwoBytes, 0, memory, 0x1C7030, 4);

        var tmpPath = CreateTempFile(memory);

        var promptResponses = new Queue<string>(new[] { "maybe", "yes" });
        var convertCalled = false;

        SubMenuHelper.ConvertToDigital(tmpPath,
            confirmPrompt: _ => promptResponses.Dequeue(),
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => convertCalled = true);

        _output.Should().Contain(o => o.Contains("Invalid input"));
        convertCalled.Should().BeTrue();
    }

    [Test]
    public void ConvertToDigital_ShouldHandleReadErrorGracefully()
    {
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".bin");

        SubMenuHelper.ConvertToDigital(nonExistentPath,
            confirmPrompt: _ => "yes",
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => throw new Exception("Should not be called"));

        _output.Should().Contain(s => s.Contains("Error reading the binary file"));
        _output.Should().Contain(s => s.Contains("Press Enter to continue..."));
    }

    [Test]
    public void ConvertToDigital_ShouldHandleWriteErrorGracefully()
    {
        var memory = new byte[0x1C8000];
        var offsetOneBytes = HexStringToBytes("22010101");
        var offsetTwoBytes = HexStringToBytes("22020101");
        Array.Copy(offsetOneBytes, 0, memory, 0x1C7010, 4);
        Array.Copy(offsetTwoBytes, 0, memory, 0x1C7030, 4);

        var tmpPath = CreateTempFile(memory);

        SubMenuHelper.ConvertToDigital(tmpPath,
            confirmPrompt: _ => "yes",
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => throw new IOException("Disk full"));

        _output.Should().Contain(s => s.Contains("Error updating the binary file"));
        _output.Should().Contain(s => s.Contains("Disk full"));
    }

    [Test]
    public void ConvertToDisc_ShouldDoNothing_IfAlreadyDiscEdition()
    {
        var memory = new byte[0x1C8000];
        Array.Copy(HexStringToBytes("22020101"), 0, memory, 0x1C7010, 4);
        Array.Copy(HexStringToBytes("22030101"), 0, memory, 0x1C7030, 4);

        var tmpPath = CreateTempFile(memory);

        var confirmResponses = new Queue<string>(new[] { "yes" });

        SubMenuHelper.ConvertToDisc(tmpPath,
            confirmPrompt: _ => confirmResponses.Dequeue(),
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => Assert.Fail("Should not be called")
        );

        _output.Should().Contain(s => s.Contains("already a disc edition"));
    }

    [Test]
    public void ConvertToDisc_ShouldRetryOnInvalidInput()
    {
        var memory = new byte[0x1C8000];
        Array.Copy(HexStringToBytes("22010101"), 0, memory, 0x1C7010, 4);
        Array.Copy(HexStringToBytes("22030101"), 0, memory, 0x1C7030, 4);

        var tmpPath = CreateTempFile(memory);

        var promptResponses = new Queue<string>(new[] { "maybe", "yes" });
        var convertCalled = false;

        SubMenuHelper.ConvertToDisc(tmpPath,
            confirmPrompt: _ => promptResponses.Dequeue(),
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => convertCalled = true);

        _output.Should().Contain(s => s.Contains("Invalid input"));
        convertCalled.Should().BeTrue();
    }

    [Test]
    public void ConvertToDisc_ShouldCallConvert_WhenNotAlreadyDiscEdition()
    {
        var memory = new byte[0x1C8000];
        Array.Copy(HexStringToBytes("22010101"), 0, memory, 0x1C7010, 4);
        Array.Copy(HexStringToBytes("22030101"), 0, memory, 0x1C7030, 4);

        var tmpPath = CreateTempFile(memory);

        var confirmResponses = new Queue<string>(new[] { "yes" });
        var convertCalled = false;

        SubMenuHelper.ConvertToDisc(tmpPath,
            confirmPrompt: _ => confirmResponses.Dequeue(),
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => convertCalled = true
        );

        convertCalled.Should().BeTrue();
    }

    [Test]
    public void ConvertToDisc_ShouldExit_WhenUserCancels()
    {
        var memory = new byte[0x1C8000];
        var tmpPath = CreateTempFile(memory);

        var confirmResponses = new Queue<string>(new[] { "no" });

        SubMenuHelper.ConvertToDisc(tmpPath,
            confirmPrompt: _ => confirmResponses.Dequeue(),
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => Assert.Fail("Should not be called")
        );

        _output.Should().NotContain(s => s.Contains("Invalid input"));
    }

    [Test]
    public void ConvertToDisc_ShouldHandleExceptionReadingFile()
    {
        var tmpPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".bin"); // invalid path (file doesn't exist)
        var confirmResponses = new Queue<string>(new[] { "yes" });

        SubMenuHelper.ConvertToDisc(tmpPath,
            confirmPrompt: _ => confirmResponses.Dequeue(),
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => Assert.Fail("Should not be called")
        );

        _output.Should().Contain(s => s.Contains("Error reading the binary file"));
    }

    [Test]
    public void ConvertToSlim_ShouldDoNothing_IfAlreadySlimEdition()
    {
        var memory = new byte[0x1C8000];
        Array.Copy(HexStringToBytes("22010101"), 0, memory, 0x1C7010, 4);
        Array.Copy(HexStringToBytes("22010101"), 0, memory, 0x1C7030, 4);
        var tmpPath = CreateTempFile(memory);

        SubMenuHelper.ConvertToSlim(tmpPath,
            confirmPrompt: _ => "yes",
            writeLine: _writeLine,
            readLine: _readLine);

        _output.Should().Contain(s => s.Contains("already a slim edition"));
    }

    [Test]
    public void ConvertToSlim_ShouldConvert_WhenNotAlreadySlim()
    {
        var memory = new byte[0x1C8000];
        Array.Copy(HexStringToBytes("22020101"), 0, memory, 0x1C7010, 4);
        Array.Copy(HexStringToBytes("22030101"), 0, memory, 0x1C7030, 4);
        var tmpPath = CreateTempFile(memory);

        var convertCalled = false;
        SubMenuHelper.ConvertToSlim(tmpPath,
            confirmPrompt: _ => "yes",
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => convertCalled = true);

        convertCalled.Should().BeTrue();
    }

    [Test]
    public void ConvertToSlim_ShouldRetryOnInvalidInput()
    {
        var memory = new byte[0x1C8000];
        Array.Copy(HexStringToBytes("22020101"), 0, memory, 0x1C7010, 4);
        Array.Copy(HexStringToBytes("22030101"), 0, memory, 0x1C7030, 4);
        var tmpPath = CreateTempFile(memory);

        var prompResponses = new Queue<string>(new[] { "maybe", "yes" });
        var convertCalled = false;

        SubMenuHelper.ConvertToSlim(tmpPath,
            confirmPrompt: _ => prompResponses.Dequeue(),
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => convertCalled = true);

        _output.Should().Contain(s => s.Contains("Invalid input"));
        convertCalled.Should().BeTrue();
    }

    [Test]
    public void ConvertToSlim_ShouldExit_WhenUserCancels()
    {
        var memory = new byte[0x1C8000];
        Array.Copy(HexStringToBytes("22020101"), 0, memory, 0x1C7010, 4);
        Array.Copy(HexStringToBytes("22030101"), 0, memory, 0x1C7030, 4);
        var tmpPath = CreateTempFile(memory);

        SubMenuHelper.ConvertToSlim(tmpPath,
            confirmPrompt: _ => "no",
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => Assert.Fail("Should not have been called"));

        _output.Should().BeEmpty();
    }

    [Test]
    public void ConvertToSlim_ShouldHandleExceptionReadingFile()
    {
        var invalidPath = "non_existent_file.bin";

        SubMenuHelper.ConvertToSlim(invalidPath,
            confirmPrompt: _ => "yes",
            writeLine: _writeLine,
            readLine: () => string.Empty,
            convertConsoleType: (_, _, _, _, _, _) => Assert.Fail("Should not have been called"));

        _output.Should().Contain(s => s.StartsWith("Error reading the binary file"));
    }

    [Test]
    public void ChangeSerialNumber_ShouldExitImmediately_WhenUserTypesExit()
    {
        var inputs = new Queue<string>(new[] { "exit" });
        var tempFile = TestFileWithSerialWithContent("OLD_SERIAL_1234567");

        SubMenuHelper.ChangeSerialNumber(tempFile,
            readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            updateSerialNumber: (path, jobDone, oldSerial, rl, wl) =>
            {
                var newSerial = rl();
                if (newSerial == "exit")
                    return true;
                return false;
            });

        _output.Should().BeEmpty();
    }

    [Test]
    public void ChangeSerialNumber_ShouldInvokeUpdateSerialNumber_MultipleTimesUntilDone()
    {
        var inputs = new Queue<string>(new[] { "wrong", "exit" });
        var tempFile = TestFileWithSerialWithContent("OLD_SERIAL_1234567");
        int callCount = 0;

        SubMenuHelper.ChangeSerialNumber(tempFile,
            readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            updateSerialNumber: (path, jobDone, oldSerial, rl, wl) =>
            {
                callCount++;
                var input = rl();
                if (input == "exit") return true;
                return false;
            });

        callCount.Should().Be(2);
    }

    [Test]
    public void ChangeSerialNumber_ShouldReadOldSerial_FromFile()
    {
        string oldSerialFromFile = null!;
        var tmpFile = TestFileWithSerialWithContent("OLD_SERIAL_1234567");

        SubMenuHelper.ChangeSerialNumber(tmpFile,
            readLine: () => "exit",
            writeLine: s => { },
            updateSerialNumber: (path, jobDone, oldSerial, rl, wl) =>
            {
                oldSerialFromFile = oldSerial;
                return true;
            });

        oldSerialFromFile.Should().StartWith("OLD_SERIAL_");
    }

    [Test]
    public void ChangeSerialNumber_ShouldSetOldSerialToNull_IfFileReadFails()
    {
        var oldSerialFromFile = "not_null";
        var invalidFilePath = "Z:\\nonexistent_file.bin";

        SubMenuHelper.ChangeSerialNumber(invalidFilePath,
            readLine: () => "exit",
            writeLine: s => { },
            updateSerialNumber: (path, jobDone, oldSerial, rl, wl) =>
            {
                oldSerialFromFile = oldSerial;
                return true;
            });

        oldSerialFromFile.Should().BeNull();
    }

    [Test]
    public void ChangeMotherboardSerialNumber_ShouldCallUpdate_WhenSerialIsValid()
    {
        var tempFile = CreateTestFileWithMotherboardSerial("0011223344556677");
        string? receivedPath = null, receivedSerial = null;
        var updateCalled = false;

        SubMenuHelper.ChangeMotherboardSerialNumber(pathToDump: tempFile,
            readLine: () => "exit",
            writeLine: _ => { },
            updateMotherboardSerialNumber: (p, s, _, _) =>
            {
                updateCalled = true;
                receivedPath = p;
                receivedSerial = s;
            });

        updateCalled.Should().BeTrue();
        receivedPath.Should().Be(tempFile);
        receivedSerial.Should().Be("0011223344556677");
    }

    [Test]
    public void ChangeMotherboardSerialNumber_ShouldNotCallUpdate_WhenSerialIsNull()
    {
        var tmpPath = Path.GetTempFileName(); // empty file
        var updateCalled = false;

        SubMenuHelper.ChangeMotherboardSerialNumber(pathToDump: tmpPath,
            readLine: () => { return "ignored"; },
            writeLine: _ => { },
            updateMotherboardSerialNumber: (_, _, _, _) => updateCalled = true);

        updateCalled.Should().BeFalse();
    }

    [Test]
    public void ChangeMotherboardSerialNumber_ShouldNotCallUpdate_WhenSerialIsEmpty()
    {
        var tmpPath = CreateTempFile(Array.Empty<byte>());
        var updateCalled = false;

        SubMenuHelper.ChangeMotherboardSerialNumber(pathToDump: tmpPath,
            readLine: () => "ignored",
            writeLine: _ => { },
            updateMotherboardSerialNumber: (_, _, _, _) => updateCalled = true);

        updateCalled.Should().BeFalse();
    }

    [Test]
    public void ChangeMotherboardSerialNumber_ShouldShowError_WhenSerialIsInvalid()
    {
        var tmpPath = CreateTempFile(new byte[10]); // too short for offset
        var readCalled = false;

        SubMenuHelper.ChangeMotherboardSerialNumber(pathToDump: tmpPath,
            readLine: () => { readCalled = true; return "ignored"; },
            writeLine: _writeLine,
            updateMotherboardSerialNumber: (_, _, _, _) => throw new Exception("Should not be called"));

        _output.Should().Contain(l => l.Contains("Could not parse"));
        _output.Should().Contain("Press Enter to continue...");
        readCalled.Should().BeTrue();
    }

    [Test]
    public void ChangeConsoleModel_ShouldNotCallUpdate_WhenVariantOrConsoleModelIsInvalid()
    {
        var tmpPath = CreateTempFile(Array.Empty<byte>()); // empty file

        var updateCalled = false;

        SubMenuHelper.ChangeConsoleModel(pathToDump: tmpPath,
            readLine: () => "ignored",
            writeLine: _ => { },
            updateModelOrSerial: (_, _, _) => updateCalled = true);

        updateCalled.Should().BeFalse();
    }

    [Test]
    public void ChangeConsoleModel_ShouldCallUpdate_WhenValidModelEntered()
    {
        var memory = Enumerable.Repeat((byte)0x20, 0x1C7226 + 19).ToArray();
        Encoding.ASCII.GetBytes("CFI-1016A     ").CopyTo(memory, 0x1C7226);
        var tmpPath = CreateTempFile(memory);

        string? oldModel = null;
        string? newModel = null;

        var inputs = new Queue<string>(new[] { "CFI-2016B", "" }); // second "" is for "Press Enter to continue..."

        SubMenuHelper.ChangeConsoleModel(pathToDump: tmpPath,
            readLine: () => inputs.Dequeue(),
            writeLine: _ => { },
            updateModelOrSerial: (_, oldM, newM) =>
            {
                oldModel = oldM;
                newModel = newM;
            });

        oldModel.Should().StartWith("CFI-");
        newModel.Should().Be("CFI-2016B");
    }

    [Test]
    public void ChangeConsoleModel_ShouldNotUpdate_WhenUserTypesExit()
    {
        var memory = Enumerable.Repeat((byte)0x20, 0x1C7226 + 19).ToArray();
        Encoding.ASCII.GetBytes("CFI-1016A     ").CopyTo(memory, 0x1C7226);
        var tmpPath = CreateTempFile(memory);

        var updateCalled = false;

        var callCount = 0;
        var readLine = () =>
        {
            callCount++;
            return callCount == 1 ? "exit" : "";
        };

        SubMenuHelper.ChangeConsoleModel(pathToDump: tmpPath,
            readLine: readLine,
            writeLine: _ => { },
            updateModelOrSerial: (_, _, _) => updateCalled = true);

        updateCalled.Should().BeFalse();
    }

    [Test]
    public void ChangeConsoleModel_ShouldRejectInvalidModel_EmptyInput()
    {
        var memory = Enumerable.Repeat((byte)0x20, 0x1C7226 + 19).ToArray();
        Encoding.ASCII.GetBytes("CFI-1016A     ").CopyTo(memory, 0x1C7226);
        var tmpPath = CreateTempFile(memory);

        var inputs = new Queue<string>(new[] { "", "exit" });

        SubMenuHelper.ChangeConsoleModel(pathToDump: tmpPath,
            readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            updateModelOrSerial: (_, _, _) => { });

        _output.Should().Contain(m => m.Contains("valid model number"));
    }

    [Test]
    public void ChangeConsoleModel_ShouldRejectInvalidModel_TooShortOrNoCFI()
    {
        var memory = Enumerable.Repeat((byte)0x20, 0x1C7226 + 19).ToArray();
        Encoding.ASCII.GetBytes("CFI-1016A     ").CopyTo(memory, 0x1C7226);
        var tmpPath = CreateTempFile(memory);

        var inputs = new Queue<string>(new[] { "12345678", "XYZ-0000Z", "exit" });

        SubMenuHelper.ChangeConsoleModel(pathToDump: tmpPath,
            readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            updateModelOrSerial: (_, _, _) => { });

        _output.Should().Contain(m => m.Contains("model you entered is invalid"));
    }

    [Test]
    public void ChangeConsoleModel_ShouldShowErrorMessage_WhenUpdateThrows()
    {
        var memory = Enumerable.Repeat((byte)0x20, 0x1C7226 + 19).ToArray();
        Encoding.ASCII.GetBytes("CFI-1016A     ").CopyTo(memory, 0x1C7226);
        var tmpPath = CreateTempFile(memory);

        SubMenuHelper.ChangeConsoleModel(pathToDump: tmpPath,
            readLine: () => "CFI-9999Z",
            writeLine: _writeLine,
            updateModelOrSerial: (_, _, _) => throw new ArgumentException("Simulated failure"));

        _output.Should().Contain(m => m.Contains("error occurred"));
    }

    private static string CreateTestFileWithMotherboardSerial(string serial)
    {
        var data = new byte[0x1C7200 + 16];
        Encoding.ASCII.GetBytes(serial).CopyTo(data, 0x1C7200);
        return CreateTempFile(data);
    }

    private static string CreateTempFile(byte[] memory)
    {
        var tmpPath = Path.GetTempFileName();
        File.WriteAllBytes(tmpPath, memory);
        return tmpPath;
    }

    private static byte[] HexStringToBytes(string hex)
    {
        if (hex.Length % 2 != 0)
            throw new ArgumentException("Hex string must have even length");

        var bytes = new byte[hex.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return bytes;
    }

    private static string TestFileWithSerialWithContent(string serial)
    {
        var tempFile = Path.GetTempFileName();

        var serialBytes = Encoding.UTF8.GetBytes(serial.PadRight(17, '\0'));
        var requiredLength = 0x1c7210 + serialBytes.Length;

        var bytes = new byte[requiredLength];  // big enough buffer

        Array.Copy(serialBytes, 0, bytes, 0x1c7210, serialBytes.Length);

        File.WriteAllBytes(tempFile, bytes);
        return tempFile;
    }
}
