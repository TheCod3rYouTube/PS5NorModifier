using FluentAssertions;
using NUnit.Framework;
using UART_CL_By_TheCod3r;

namespace UART_CL.UnitTests;

public class PS5UARTUtilities_Tests
{
    [Test]
    public void CalculateChecksum_ShouldReturnExpectedChecksum()
    {
        var result =  PS5UARTUtilities.CalculateChecksum("ABC");
        result.Should().Be("ABC:C6");
    }

    [Test]
    public void CalculateChecksum_EmptyString_ShouldReturn00()
    {
        var result =  PS5UARTUtilities.CalculateChecksum("");
        result.Should().Be(":00");
    }

    [Test]
    public void CalculateChecksum_ShouldHandleUnicodeCharacters()
    {
        var result =  PS5UARTUtilities.CalculateChecksum("ÅÄÖ");
        result.Should().StartWith("ÅÄÖ:");
    }

    [Test]
    public void CalculateChecksum_ShouldHandleVeryLongString()
    {
        var longStr = new string('A', 10000);
        var result =  PS5UARTUtilities.CalculateChecksum(longStr);
        result.Should().StartWith(longStr + ":");
    }

    [Test]
    public void HexStringToString_ValidHex_ShouldReturnExpectedString()
    {
        var result =  PS5UARTUtilities.HexStringToString("48656C6C6F");
        result.Should().Be("Hello");
    }

    [Test]
    public void HexStringToString_InvalidLength_ShouldThrow()
    {
        var act = () =>  PS5UARTUtilities.HexStringToString("ABC");
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void HexStringToString_ShouldReturnEmptyString_WhenInputIsEmpty()
    {
        var result =  PS5UARTUtilities.HexStringToString("");
        result.Should().Be("");
    }

    [Test]
    public void HexStringToString_ShouldParseLowercaseHex()
    {
        var result =  PS5UARTUtilities.HexStringToString("6869");
        result.Should().Be("hi");
    }

    [Test]
    public void ConvertHexStringToByteArray_ValidHex_ShouldReturnExpectedBytes()
    {
        var result =  PS5UARTUtilities.ConvertHexStringToByteArray("4142");
        result.Should().Equal(0x41, 0x42);
    }

    [Test]
    public void ConvertHexStringToByteArray_OddLength_ShouldThrow()
    {
        var act = () =>  PS5UARTUtilities.ConvertHexStringToByteArray("123");
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void ConvertHexStringToByteArray_ShouldReturnEmptyArray_WhenInputIsEmpty()
    {
        var result =  PS5UARTUtilities.ConvertHexStringToByteArray("");
        result.Should().BeEmpty();
    }

    [Test]
    public void ConvertHexStringToByteArray_ShouldParseLowercase()
    {
        var result =  PS5UARTUtilities.ConvertHexStringToByteArray("ff");
        result.Should().BeEquivalentTo(new byte[] { 255 });
    }

    [Test]
    public void ConvertHexStringToByteArray_ShouldThrowFormatException_WhenHexIsInvalid()
    {
        var act = () =>  PS5UARTUtilities.ConvertHexStringToByteArray("ZZ");
        act.Should().Throw<FormatException>();
    }

    [Test]
    public void PatternAt_FindsPatternOnce()
    {
        var result =  PS5UARTUtilities.PatternAt(
            new byte[] { 1, 2, 3, 4, 5 },
            new byte[] { 3, 4 }
        ).ToList();

        result.Should().Equal(2);
    }

    [Test]
    public void PatternAt_FindsPatternMultipleTimes()
    {
        var result =  PS5UARTUtilities.PatternAt(
            new byte[] { 1, 2, 3, 4, 3, 4 },
            new byte[] { 3, 4 }
        ).ToList();

        result.Should().Equal(2, 4);
    }

    [Test]
    public void PatternAt_NoMatch_ShouldReturnEmpty()
    {
        var result =  PS5UARTUtilities.PatternAt(
            new byte[] { 1, 2, 3 },
            new byte[] { 4, 5 }
        ).ToList();

        result.Should().BeEmpty();
    }

    [Test]
    public void ParseErrors_ValidCode_ShouldReturnDescription()
    {
        File.WriteAllText("errorDB.xml",
            @"<errorCodes><errorCode><ErrorCode>1234</ErrorCode><Description>Test error</Description></errorCode></errorCodes>");
        var result =  PS5UARTUtilities.ParseErrors("1234");
        result.Should().Contain("Description: Test error");
        File.Delete("errorDB.xml");
    }

    [Test]
    public void ParseErrors_InvalidCode_ShouldReturnNoResult()
    {
        File.WriteAllText("errorDB.xml", @"<errorCodes></errorCodes>");
        var result =  PS5UARTUtilities.ParseErrors("9999");
        result.Should().Contain("No result found");
        File.Delete("errorDB.xml");
    }

    [Test]
    public void ParseErrors_MissingFile_ShouldReturnError()
    {
        if (File.Exists("errorDB.xml"))
            File.Delete("errorDB.xml");

        var result =  PS5UARTUtilities.ParseErrors("1234");
        result.Should().Contain("Local XML file not found");
    }

    [Test]
    public void PatternAt_ShouldReturnEmpty_WhenPatternIsEmpty()
    {
        var result =  PS5UARTUtilities.PatternAt(new byte[] { 1, 2, 3 }, Array.Empty<byte>());
        result.Should().BeEmpty();
    }

    [Test]
    public void PatternAt_ShouldReturnEmpty_WhenPatternIsLongerThanSource()
    {
        var result =  PS5UARTUtilities.PatternAt(new byte[] { 1, 2 }, new byte[] { 1, 2, 3 });
        result.Should().BeEmpty();
    }

    [Test]
    public void PatternAt_ShouldReturnZero_WhenSourceEqualsPattern()
    {
        var result =  PS5UARTUtilities.PatternAt(new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 });
        result.Should().ContainSingle().Which.Should().Be(0);
    }

    [Test]
    public void ParseErrors_ShouldHandleMalformedXml()
    {
        File.WriteAllText("errorDB.xml", "<invalid></xml>");
        var result =  PS5UARTUtilities.ParseErrors("123");
        result.Should().StartWith("Error:");
    }

    [Test]
    public void ParseErrors_ShouldHandleMissingRootElement()
    {
        File.WriteAllText("errorDB.xml", "<root></root>");
        var result =  PS5UARTUtilities.ParseErrors("123");
        result.Should().Contain("Invalid XML database file");
    }

    [Test]
    public void ParseErrors_ShouldReturnFirstMatchOnly_WhenMultipleMatchesExist()
    {
        var xml = @"
            <errorCodes>
                <errorCode><ErrorCode>100</ErrorCode><Description>First</Description></errorCode>
                <errorCode><ErrorCode>100</ErrorCode><Description>Second</Description></errorCode>
            </errorCodes>";
        File.WriteAllText("errorDB.xml", xml);
        var result =  PS5UARTUtilities.ParseErrors("100");
        result.Should().Contain("First").And.NotContain("Second");
    }

    [Test]
    public void ParseErrors_ShouldHandleNullErrorCode()
    {
        var xml = @"
            <errorCodes>
                <errorCode><ErrorCode></ErrorCode><Description>No code</Description></errorCode>
            </errorCodes>";
        File.WriteAllText("errorDB.xml", xml);
        var result =  PS5UARTUtilities.ParseErrors(null);
        result.Should().Contain("No error code given.");
    }

    [Test]
    public void GetFriendlyName_ShouldHandleNullInput()
    {
        var name =  PS5UARTUtilities.GetFriendlyName(null);
        name.Should().Be("Unknown Port Name");
    }

    [Test]
    public void GetFriendlyName_ShouldHandleSpecialCharacters()
    {
        var name =  PS5UARTUtilities.GetFriendlyName("COM😊");
        name.Should().Be("Unknown Port Name");
    }

    [Test]
    public void GetFriendlyName_ShouldReturnNonNullValue()
    {
        var result =  PS5UARTUtilities.GetFriendlyName("COM1");
        result.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void DownloadDatabase_WithInvalidUrl_ShouldReturnFalse()
    {
        var invalidUrl = "http://invalid.url/fakefile.txt";
        var tempFile = Path.GetTempFileName();

        var result = PS5UARTUtilities.DownloadDatabase(invalidUrl, tempFile);

        result.Should().BeFalse();

        if (File.Exists(tempFile))
            File.Delete(tempFile);
    }

    [Test]
    public void DownloadDatabase_WithFileUrl_ShouldReturnTrue()
    {: Create a test source file
        var sourceFile = Path.GetTempFileName();
        File.WriteAllText(sourceFile, "test content");

        var destinationFile = Path.GetTempFileName();
        File.Delete(destinationFile); // Ensure it's empty before test

        var fileUrl = new Uri(sourceFile).AbsoluteUri;

        // Act
        var result = PS5UARTUtilities.DownloadDatabase(fileUrl, destinationFile);

        // Assert
        result.Should().BeTrue();
        File.Exists(destinationFile).Should().BeTrue();
        File.ReadAllText(destinationFile).Should().Be("test content");

        // Cleanup
        File.Delete(sourceFile);
        File.Delete(destinationFile);
    }
}