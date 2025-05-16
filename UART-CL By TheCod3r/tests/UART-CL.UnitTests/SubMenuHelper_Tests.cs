using System.Text;
using FluentAssertions;
using NUnit.Framework;
using UART_CL_By_TheCod3r;

namespace UART_CL.UnitTests;

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

    [Test]
    public void RunSubMenu_ShouldShowNoPathErrorMessage_WhenViewBIOSWithoutFile()
    {
        var inputs = new Queue<string>(new[] { "2", "", "X" });
        _readLine = () => inputs.Dequeue();

        SubMenuHelper.RunSubMenu(
            "AppTitle",
            _regionMap,
            _readLine,
            _writeLine,
            loadDumpFile: (r, w, f1, f2, f3, a) => null!,
            setConsoleTitle: _ => { },
            pathToDump: ""
        );

        _output.Should().Contain("You must select a .bin file to read before proceeding. " +
            "Please select a valid .bin file and try again.");
        _output.Should().Contain("Press Enter to continue...");
    }

    [Test]
    public void RunSubMenu_ShouldExit_WhenUserChooses_X()
    {
        var inputs = new Queue<string>(new[] { "X" });
        _readLine = () => inputs.Dequeue();

        var titleSet = "";
        void SetTitle(string title) => titleSet = title;

        SubMenuHelper.RunSubMenu(
            "AppTitle",
            _regionMap,
            _readLine,
            _writeLine,
            loadDumpFile: (r, w, f1, f2, f3, a) => null!,
            setConsoleTitle: SetTitle,
            pathToDump: ""
        );

        titleSet.Should().Be("AppTitle");
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

        var readerFactory = new Func<string, BinaryReader>(_ => CreateMockReader(memory));

        SubMenuHelper.ViewBIOSInformaition(_regionMap, "fake.bin", readerFactory, _writeLine, _readLine, memory.Length);

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
        var tmpPath = Path.GetTempFileName();
        File.WriteAllBytes(tmpPath, memory);

        var readerFactory = new Func<string, BinaryReader>(_ => CreateMockReader(memory));

        try
        {
            var act = () => SubMenuHelper.ViewBIOSInformaition(_regionMap, tmpPath, readerFactory, _writeLine, _readLine);

            act.Should().NotThrow();
            _output.Should().Contain(s => s.Contains("Unknown Model"));
            _output.Should().Contain(s => s.Contains("Unknown S/N"));
            _output.Should().Contain(s => s.Contains("Unknown Mac Address"));
        }
        finally
        {
            File.Delete(tmpPath);
        }
    }

    [Test]
    public void ViewBIOSInformation_ShouldDefaultToConsoleIO_WhenNoDelegatesProvided()
    {
        // Just ensure it doesn’t throw
        var memory = new byte[0x1C8000];
        Array.Copy(Encoding.ASCII.GetBytes("22020101abcd"), 0, memory, 0x1C7010, 12);
        var tmpPath = Path.GetTempFileName();
        File.WriteAllBytes(tmpPath, memory);

        try
        {
            SubMenuHelper.ViewBIOSInformaition(_regionMap, tmpPath, readLine: _readLine);
        }
        finally
        {
            File.Delete(tmpPath);
        }
    }

    [TestCase("22030101abcd", "Digital Edition")]
    [TestCase("22010101abcd", "Slim Edition")]
    public void ViewBIOSInformation_ShouldDetectDigitalAndSlimVariants(string consoleCode, string consoleType)
    {
        var memory = new byte[0x1C8000];

        var consoleCodeBytes = HexStringToBytes(consoleCode);
        Array.Copy(consoleCodeBytes, 0, memory, 0x1C7010, consoleCodeBytes.Length);

        var readerFactory = new Func<string, BinaryReader>(_ => CreateMockReader(memory));
        var tmpPath = Path.GetTempFileName();
        File.WriteAllBytes(tmpPath, memory);

        try
        {
            SubMenuHelper.ViewBIOSInformaition(_regionMap, tmpPath, readerFactory, _writeLine, _readLine);

            _output.Should().Contain(s => s.Contains(consoleType));
        }
        finally
        {
            File.Delete(tmpPath);
        }
    }

    [Test]
    public void LoadDumpFile_ValidPath_ReturnsPath()
    {
        var inputs = new Queue<string>(new[] { "/valid/path/file.bin" });

        string path = SubMenuHelper.LoadDumpFile(
            readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            fileExists: _ => true,
            getExtension: _ => ".bin",
            getFileSize: _ => 1048576,
            sleep: _ => { });

        path.Should().Be("/valid/path/file.bin");
        _output.Should().Contain("Selected file: /valid/path/file.bin - File Size: 1048576 bytes (1MB)");
    }

    [Test]
    public void LoadDumpFile_ExitTyped_ReturnsExit()
    {
        var inputs = new Queue<string>(new[] { "exit" });

        var path = SubMenuHelper.LoadDumpFile(
            readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            fileExists: _ => false,
            getExtension: _ => "",
            getFileSize: _ => 0,
            sleep: _ => { });

        path.Should().Be("exit");
    }

    [Test]
    public void LoadDumpFile_BlankInput_ShowsError()
    {
        var inputs = new Queue<string>(new[] { "   ", "exit" });

        SubMenuHelper.LoadDumpFile(
            readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            fileExists: _ => false,
            getExtension: _ => "",
            getFileSize: _ => 0,
            sleep: _ => { });

        _output.Should().Contain("Invalid input. File path cannot be blank.");
    }

    [Test]
    public void LoadDumpFile_FileNotFound_ShowsError()
    {
        var inputs = new Queue<string>(new[] { "/fake/path/file.bin", "exit" });

        SubMenuHelper.LoadDumpFile(
            readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            fileExists: _ => false,
            getExtension: _ => ".bin",
            getFileSize: _ => 0,
            sleep: _ => { });

        _output.Should().Contain("The file path you entered does not exist. Please enter the path to a valid .bin file.");
    }

    [Test]
    public void LoadDumpFile_InvalidExtension_ShowsError()
    {
        var inputs = new Queue<string>(new[] { "/fake/path/file.txt", "exit" });

        SubMenuHelper.LoadDumpFile(
            readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            fileExists: _ => true,
            getExtension: _ => ".txt",
            getFileSize: _ => 0,
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

        var tmpPath = Path.GetTempFileName();
        File.WriteAllBytes(tmpPath, memory);

        var confirmCalled = false;
        var convertCalled = false;

        SubMenuHelper.ConvertToDigital(
            tmpPath,
            confirmPrompt: type => { confirmCalled = true; return "yes"; },
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (f1, r1, f2, r2, type, path) => {
                convertCalled = true;
                type.Should().Be("digital edition");
                path.Should().Be(tmpPath);
            });

        confirmCalled.Should().BeTrue();
        convertCalled.Should().BeTrue();

        File.Delete(tmpPath);
    }

    [Test]
    public void ConvertToDigital_ShouldSkipIfAlreadyDigital()
    {
        var memory = new byte[0x1C8000];
        // Set digital value at offsetOne
        var offsetOneBytes = HexStringToBytes("22030101");
        Array.Copy(offsetOneBytes, 0, memory, 0x1C7010, 4);

        var tmpPath = Path.GetTempFileName();
        File.WriteAllBytes(tmpPath, memory);

        var convertCalled = false;

        SubMenuHelper.ConvertToDigital(
            tmpPath,
            confirmPrompt: _ => "yes",
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => convertCalled = true);

        _output.Should().Contain(s => s.Contains("already a digital edition"));
        convertCalled.Should().BeFalse();

        File.Delete(tmpPath);
    }

    [Test]
    public void ConvertToDigital_ShouldNotConvert_WhenUserDeclines()
    {
        var memory = new byte[0x1C8000];
        var tmpPath = Path.GetTempFileName();
        File.WriteAllBytes(tmpPath, memory);

        var convertCalled = false;

        SubMenuHelper.ConvertToDigital(
            tmpPath,
            confirmPrompt: _ => "no",
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => convertCalled = true);

        convertCalled.Should().BeFalse();
        _output.Should().BeEmpty();

        File.Delete(tmpPath);
    }

    [Test]
    public void ConvertToDigital_ShouldRetryOnInvalidInput()
    {
        var memory = new byte[0x1C8000];
        var offsetOneBytes = HexStringToBytes("22010101");
        var offsetTwoBytes = HexStringToBytes("22020101");
        Array.Copy(offsetOneBytes, 0, memory, 0x1C7010, 4);
        Array.Copy(offsetTwoBytes, 0, memory, 0x1C7030, 4);

        var tmpPath = Path.GetTempFileName();
        File.WriteAllBytes(tmpPath, memory);

        var promptResponses = new Queue<string>(new[] { "maybe", "yes" });
        var convertCalled = false;

        SubMenuHelper.ConvertToDigital(
            tmpPath,
            confirmPrompt: _ => promptResponses.Dequeue(),
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => convertCalled = true);

        _output.Should().Contain(o => o.Contains("Invalid input"));
        convertCalled.Should().BeTrue();

        File.Delete(tmpPath);
    }

    [Test]
    public void ConvertToDigital_ShouldHandleReadErrorGracefully()
    {
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".bin");

        SubMenuHelper.ConvertToDigital(
            nonExistentPath,
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

        var tmpPath = Path.GetTempFileName();
        File.WriteAllBytes(tmpPath, memory);


        SubMenuHelper.ConvertToDigital(
            tmpPath,
            confirmPrompt: _ => "yes",
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => throw new IOException("Disk full"));

        _output.Should().Contain(s => s.Contains("Error updating the binary file"));
        _output.Should().Contain(s => s.Contains("Disk full"));

        File.Delete(tmpPath);
    }

    [Test]
    public void ConvertToDisc_ShouldDoNothing_IfAlreadyDiscEdition()
    {
        var memory = new byte[0x1C8000];
        Array.Copy(HexStringToBytes("22020101"), 0, memory, 0x1C7010, 4);
        Array.Copy(HexStringToBytes("22030101"), 0, memory, 0x1C7030, 4);

        var tmpPath = Path.GetTempFileName();
        File.WriteAllBytes(tmpPath, memory);

        var confirmResponses = new Queue<string>(new[] { "yes" });

        SubMenuHelper.ConvertToDisc(
            tmpPath,
            confirmPrompt: _ => confirmResponses.Dequeue(),
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => Assert.Fail("Should not be called")
        );

        _output.Should().Contain(s => s.Contains("already a disc edition"));
        File.Delete(tmpPath);
    }

    [Test]
    public void ConvertToDisc_ShouldRetryOnInvalidInput()
    {
        var memory = new byte[0x1C8000];
        Array.Copy(HexStringToBytes("22010101"), 0, memory, 0x1C7010, 4);
        Array.Copy(HexStringToBytes("22030101"), 0, memory, 0x1C7030, 4);

        var tmpPath = Path.GetTempFileName();
        File.WriteAllBytes(tmpPath, memory);

        var promptResponses = new Queue<string>(new[] { "maybe", "yes" });
        var convertCalled = false;

        SubMenuHelper.ConvertToDisc(
            tmpPath,
            confirmPrompt: _ => promptResponses.Dequeue(),
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => convertCalled = true);

        _output.Should().Contain(s => s.Contains("Invalid input"));
        convertCalled.Should().BeTrue();

        File.Delete(tmpPath);
    }

    [Test]
    public void ConvertToDisc_ShouldCallConvert_WhenNotAlreadyDiscEdition()
    {
        var memory = new byte[0x1C8000];
        Array.Copy(HexStringToBytes("22010101"), 0, memory, 0x1C7010, 4);
        Array.Copy(HexStringToBytes("22030101"), 0, memory, 0x1C7030, 4);

        var tmpPath = Path.GetTempFileName();
        File.WriteAllBytes(tmpPath, memory);

        var confirmResponses = new Queue<string>(new[] { "yes" });
        var convertCalled = false;

        SubMenuHelper.ConvertToDisc(
            tmpPath,
            confirmPrompt: _ => confirmResponses.Dequeue(),
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => convertCalled = true
        );

        convertCalled.Should().BeTrue();
        File.Delete(tmpPath);
    }

    [Test]
    public void ConvertToDisc_ShouldExit_WhenUserCancels()
    {
        var memory = new byte[0x1C8000];
        var tmpPath = Path.GetTempFileName();
        File.WriteAllBytes(tmpPath, memory);

        var confirmResponses = new Queue<string>(new[] { "no" });

        SubMenuHelper.ConvertToDisc(
            tmpPath,
            confirmPrompt: _ => confirmResponses.Dequeue(),
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => Assert.Fail("Should not be called")
        );

        _output.Should().NotContain(s => s.Contains("Invalid input"));
        File.Delete(tmpPath);
    }

    [Test]
    public void ConvertToDisc_ShouldHandleExceptionReadingFile()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".bin"); // invalid path (file doesn't exist)
        var confirmResponses = new Queue<string>(new[] { "yes" });

        SubMenuHelper.ConvertToDisc(
            path,
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
        var path = Path.GetTempFileName();
        File.WriteAllBytes(path, memory);

        SubMenuHelper.ConvertToSlim(path,
            confirmPrompt: _ => "yes",
            writeLine: _writeLine,
            readLine: _readLine);

        _output.Should().Contain(s => s.Contains("already a slim edition"));
        File.Delete(path);
    }

    [Test]
    public void ConvertToSlim_ShouldConvert_WhenNotAlreadySlim()
    {
        var memory = new byte[0x1C8000];
        Array.Copy(HexStringToBytes("22020101"), 0, memory, 0x1C7010, 4);
        Array.Copy(HexStringToBytes("22030101"), 0, memory, 0x1C7030, 4);
        var path = Path.GetTempFileName();
        File.WriteAllBytes(path, memory);

        var convertCalled = false;
        SubMenuHelper.ConvertToSlim(path,
            confirmPrompt: _ => "yes",
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => convertCalled = true);

        convertCalled.Should().BeTrue();
        File.Delete(path);
    }

    [Test]
    public void ConvertToSlim_ShouldRetryOnInvalidInput()
    {
        var memory = new byte[0x1C8000];
        Array.Copy(HexStringToBytes("22020101"), 0, memory, 0x1C7010, 4);
        Array.Copy(HexStringToBytes("22030101"), 0, memory, 0x1C7030, 4);
        var path = Path.GetTempFileName();
        File.WriteAllBytes(path, memory);

        var prompResponses = new Queue<string>(new[] { "maybe", "yes" });
        var convertCalled = false;

        SubMenuHelper.ConvertToSlim(path,
            confirmPrompt: _ => prompResponses.Dequeue(),
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => convertCalled = true);

        _output.Should().Contain(s => s.Contains("Invalid input"));
        convertCalled.Should().BeTrue();
        File.Delete(path);
    }

    [Test]
    public void ConvertToSlim_ShouldExit_WhenUserCancels()
    {
        var memory = new byte[0x1C8000];
        Array.Copy(HexStringToBytes("22020101"), 0, memory, 0x1C7010, 4);
        Array.Copy(HexStringToBytes("22030101"), 0, memory, 0x1C7030, 4);
        var path = Path.GetTempFileName();
        File.WriteAllBytes(path, memory);

        SubMenuHelper.ConvertToSlim(path,
            confirmPrompt: _ => "no",
            writeLine: _writeLine,
            readLine: _readLine,
            convertConsoleType: (_, _, _, _, _, _) => Assert.Fail("Should not have been called"));

        _output.Should().BeEmpty();
        File.Delete(path);
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

        // Act
        SubMenuHelper.ChangeSerialNumber(
            tempFile,
            readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            updateSerialNumber: (path, jobDone, oldSerial, rl, wl) =>
            {
                // This simulates UpdateSerialNumber behavior
                var newSerial = rl();
                if (newSerial == "exit")
                    return true;
                return false;
            });

        // Assert
        _output.Should().BeEmpty(); // No writeLine calls in our dummy updateSerialNumber
        File.Delete(tempFile);
    }

    [Test]
    public void ChangeSerialNumber_ShouldInvokeUpdateSerialNumber_MultipleTimesUntilDone()
    {
        var inputs = new Queue<string>(new[] { "wrong", "exit" });
        var tempFile = TestFileWithSerialWithContent("OLD_SERIAL_1234567");
        int callCount = 0;

        // Act
        SubMenuHelper.ChangeSerialNumber(
            tempFile,
            readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            updateSerialNumber: (path, jobDone, oldSerial, rl, wl) =>
            {
                callCount++;
                var input = rl();
                if (input == "exit") return true;
                return false;
            });

        // Assert
        callCount.Should().Be(2);
        File.Delete(tempFile);
    }

    [Test]
    public void ChangeSerialNumber_ShouldReadOldSerial_FromFile()
    {
        string oldSerialFromFile = null!;
        var tempFile = TestFileWithSerialWithContent("OLD_SERIAL_1234567");

        // Act
        SubMenuHelper.ChangeSerialNumber(
            tempFile,
            readLine: () => "exit",
            writeLine: s => { },
            updateSerialNumber: (path, jobDone, oldSerial, rl, wl) =>
            {
                oldSerialFromFile = oldSerial;
                return true; // end loop
            });

        // Assert
        oldSerialFromFile.Should().StartWith("OLD_SERIAL_");
        File.Delete(tempFile);
    }

    [Test]
    public void ChangeSerialNumber_ShouldSetOldSerialToNull_IfFileReadFails()
    {
        var oldSerialFromFile = "not_null";
        var invalidFilePath = "Z:\\nonexistent_file.bin";

        // Act
        SubMenuHelper.ChangeSerialNumber(
            invalidFilePath,
            readLine: () => "exit",
            writeLine: s => { },
            updateSerialNumber: (path, jobDone, oldSerial, rl, wl) =>
            {
                oldSerialFromFile = oldSerial;
                return true; // exit loop
            });

        // Assert
        oldSerialFromFile.Should().BeNull();
    }

    [Test]
    public void ChangeMotherboardSerialNumber_ShouldCallUpdate_WhenSerialIsValid()
    {
        var tempFile = CreateTestFileWithMotherboardSerial("0011223344556677");
        string? receivedPath = null, receivedSerial = null;
        var updateCalled = false;

        SubMenuHelper.ChangeMotherboardSerialNumber(
            pathToDump: tempFile,
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
        File.Delete(tempFile);
    }

    [Test]
    public void ChangeMotherboardSerialNumber_ShouldNotCallUpdate_WhenSerialIsNull()
    {
        var path = Path.GetTempFileName(); // empty file
        var updateCalled = false;

        SubMenuHelper.ChangeMotherboardSerialNumber(
            pathToDump: path,
            readLine: () => { return "ignored"; },
            writeLine: _ => { },
            updateMotherboardSerialNumber: (_, _, _, _) => updateCalled = true);

        updateCalled.Should().BeFalse();
        File.Delete(path);
    }

    [Test]
    public void ChangeMotherboardSerialNumber_ShouldNotCallUpdate_WhenSerialIsEmpty()
    {
        var path = Path.GetTempFileName();
        File.WriteAllBytes(path, Array.Empty<byte>());
        var updateCalled = false;

        SubMenuHelper.ChangeMotherboardSerialNumber(
            pathToDump: path,
            readLine: () => "ignored",
            writeLine: _ => { },
            updateMotherboardSerialNumber: (_, _, _, _) => updateCalled = true);

        updateCalled.Should().BeFalse();
        File.Delete(path);
    }

    [Test]
    public void ChangeMotherboardSerialNumber_ShouldShowError_WhenSerialIsInvalid()
    {
        var path = Path.GetTempFileName();
        File.WriteAllBytes(path, new byte[10]); // too short for offset
        var readCalled = false;

        SubMenuHelper.ChangeMotherboardSerialNumber(
            pathToDump: path,
            readLine: () => { readCalled = true; return "ignored"; },
            writeLine: _writeLine,
            updateMotherboardSerialNumber: (_, _, _, _) => throw new Exception("Should not be called"));

        _output.Should().Contain(l => l.Contains("Could not parse"));
        _output.Should().Contain("Press Enter to continue...");
        readCalled.Should().BeTrue();
        File.Delete(path);
    }

    [Test]
    public void ChangeConsoleModel_ShouldNotCallUpdate_WhenVariantOrConsoleModelIsInvalid()
    {
        var path = Path.GetTempFileName();
        File.WriteAllBytes(path, Array.Empty<byte>()); // empty file

        var updateCalled = false;

        SubMenuHelper.ChangeConsoleModel(
            pathToDump: path,
            readLine: () => "ignored",
            writeLine: _ => { },
            updateModelOrSerial: (_, _, _) => updateCalled = true);

        updateCalled.Should().BeFalse();

        File.Delete(path);
    }

    [Test]
    public void ChangeConsoleModel_ShouldCallUpdate_WhenValidModelEntered()
    {
        var path = Path.GetTempFileName();
        var bin = Enumerable.Repeat((byte)0x20, 0x1C7226 + 19).ToArray();
        Encoding.ASCII.GetBytes("CFI-1016A     ").CopyTo(bin, 0x1C7226);
        File.WriteAllBytes(path, bin);

        string? oldModel = null;
        string? newModel = null;

        var inputs = new Queue<string>(new[] { "CFI-2016B", "" }); // second "" is for "Press Enter to continue..."

        SubMenuHelper.ChangeConsoleModel(
            pathToDump: path,
            readLine: () => inputs.Dequeue(),
            writeLine: _ => { },
            updateModelOrSerial: (_, oldM, newM) =>
            {
                oldModel = oldM;
                newModel = newM;
            });

        oldModel.Should().StartWith("CFI-");
        newModel.Should().Be("CFI-2016B");

        File.Delete(path);
    }

    [Test]
    public void ChangeConsoleModel_ShouldNotUpdate_WhenUserTypesExit()
    {
        var path = Path.GetTempFileName();
        var bin = Enumerable.Repeat((byte)0x20, 0x1C7226 + 19).ToArray();
        Encoding.ASCII.GetBytes("CFI-1016A     ").CopyTo(bin, 0x1C7226);
        File.WriteAllBytes(path, bin);

        var updateCalled = false;

        var callCount = 0;
        var readLine = () =>
        {
            callCount++;
            return callCount == 1 ? "exit" : "";
        };

        SubMenuHelper.ChangeConsoleModel(
            pathToDump: path,
            readLine: readLine,
            writeLine: _ => { },
            updateModelOrSerial: (_, _, _) => updateCalled = true);

        updateCalled.Should().BeFalse();
        File.Delete(path);
    }

    [Test]
    public void ChangeConsoleModel_ShouldRejectInvalidModel_EmptyInput()
    {
        var path = Path.GetTempFileName();
        var bin = Enumerable.Repeat((byte)0x20, 0x1C7226 + 19).ToArray();
        Encoding.ASCII.GetBytes("CFI-1016A     ").CopyTo(bin, 0x1C7226);
        File.WriteAllBytes(path, bin);

        var inputs = new Queue<string>(new[] { "", "exit" });

        SubMenuHelper.ChangeConsoleModel(
            pathToDump: path,
            readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            updateModelOrSerial: (_, _, _) => { });

        _output.Should().Contain(m => m.Contains("valid model number"));
        File.Delete(path);
    }

    [Test]
    public void ChangeConsoleModel_ShouldRejectInvalidModel_TooShortOrNoCFI()
    {
        var path = Path.GetTempFileName();
        var bin = Enumerable.Repeat((byte)0x20, 0x1C7226 + 19).ToArray();
        Encoding.ASCII.GetBytes("CFI-1016A     ").CopyTo(bin, 0x1C7226);
        File.WriteAllBytes(path, bin);

        var inputs = new Queue<string>(new[] { "12345678", "XYZ-0000Z", "exit" });

        SubMenuHelper.ChangeConsoleModel(
            pathToDump: path,
            readLine: () => inputs.Dequeue(),
            writeLine: _writeLine,
            updateModelOrSerial: (_, _, _) => { });

        _output.Should().Contain(m => m.Contains("model you entered is invalid"));
        File.Delete(path);
    }

    [Test]
    public void ChangeConsoleModel_ShouldShowErrorMessage_WhenUpdateThrows()
    {
        var path = Path.GetTempFileName();
        var bin = Enumerable.Repeat((byte)0x20, 0x1C7226 + 19).ToArray();
        Encoding.ASCII.GetBytes("CFI-1016A     ").CopyTo(bin, 0x1C7226);
        File.WriteAllBytes(path, bin);

        SubMenuHelper.ChangeConsoleModel(
            pathToDump: path,
            readLine: () => "CFI-9999Z",
            writeLine: _writeLine,
            updateModelOrSerial: (_, _, _) => throw new ArgumentException("Simulated failure"));

        _output.Should().Contain(m => m.Contains("error occurred"));
        File.Delete(path);
    }

    private static string CreateTestFileWithMotherboardSerial(string serial)
    {
        var path = Path.GetTempFileName();
        var data = new byte[0x1C7200 + 16];
        Encoding.ASCII.GetBytes(serial).CopyTo(data, 0x1C7200);
        File.WriteAllBytes(path, data);
        return path;
    }

    private static BinaryReader CreateMockReader(byte[] data)
    {
        var memStream = new MemoryStream(data);
        return new BinaryReader(memStream);
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
