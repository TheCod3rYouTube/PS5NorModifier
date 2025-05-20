using Microsoft.Extensions.Logging;
using NSubstitute;
using NorModifierLib.Enumerators;
using NorModifierLib.Services;

namespace UART_CL_Tests;

public class NorServiceTests
{
	[Fact]
	public void ParseNor_Test()
	{
		// Arrange
		var filePath = @"Z:\nor.bin";
		var logger = Substitute.For<ILogger<NorService>>();

		// Act
		var service = new NorService(logger);
		var norInfo = service.ReadNor(filePath);

		// Assert
		Assert.NotNull(norInfo);
		Assert.NotEmpty(norInfo.Errors);

		var norError = norInfo.Errors.First();
		Assert.Equal("Unknown Error Code", norError.Code);
		Assert.Equal(416761713u, norError.Rtc);
		Assert.Equal("HstOsOFF", norError.PowerStateA);
		Assert.Equal("SOC_ON", norError.PowerStateB);
		Assert.Equal("Power Button", norError.BootCause);
		Assert.Equal("MSOC Reset Moni High, Main SoC Power Off, FATAL OFF", norError.SequenceNumber);

		Assert.True(norError.HdmiPower);
		Assert.True(norError.BddPower);
		Assert.True(norError.HdmiCecPower);
		Assert.True(norError.UsbPower);
		Assert.False(norError.WifiPower);

		Assert.Equal("35.72°C", norError.EnvironmentTemperature);
		Assert.Equal("54.98°C", norError.ChipTemperature);
	}

	[Fact]
	public void ModifyEdition_Test()
	{
		// Arrange
		var filePath = @"Z:\nor.bin";
		var copyFilePath = @"Z:\nor_copy.bin";
		File.Copy(filePath, copyFilePath, true); // Copy the file to ensure it exists
		var logger = Substitute.For<ILogger<NorService>>();

		// Act
		var service = new NorService(logger);
		var norInfo = service.ReadNor(copyFilePath);

		service.SetEdition(norInfo, Edition.Digital);

		var modifiedNorInfo = service.ReadNor(copyFilePath);

		// Assert
		Assert.NotNull(norInfo);
		Assert.Equal(expected: Edition.Disc, norInfo.Edition);
		Assert.NotNull(modifiedNorInfo);
		Assert.Equal(expected: Edition.Digital, modifiedNorInfo.Edition);
	}

	[Fact]
	public void ModifyConsoleSerial_Test()
	{
		// Arrange
		var filePath = @"Z:\nor.bin";
		var copyFilePath = @"Z:\nor_copy.bin";
		File.Copy(filePath, copyFilePath, true); // Copy the file to ensure it exists
		var logger = Substitute.For<ILogger<NorService>>();

		// Act
		var service = new NorService(logger);
		var norInfo = service.ReadNor(copyFilePath);

		service.SetConsoleSerial(norInfo, "F22301EL011425029");

		var modifiedNorInfo = service.ReadNor(copyFilePath);

		// Assert
		Assert.NotNull(norInfo);
		Assert.Equal(expected: "F22301EL011425028", norInfo.ConsoleSerialNumber);
		Assert.NotNull(modifiedNorInfo);
		Assert.Equal(expected: "F22301EL011425029", modifiedNorInfo.ConsoleSerialNumber);
	}

	[Fact]
	public void ModifyMotherboardSerial_Test()
	{
		// Arrange
		var filePath = @"Z:\nor.bin";
		var copyFilePath = @"Z:\nor_copy.bin";
		File.Copy(filePath, copyFilePath, true); // Copy the file to ensure it exists
		var logger = Substitute.For<ILogger<NorService>>();

		// Act
		var service = new NorService(logger);
		var norInfo = service.ReadNor(copyFilePath);

		service.SetMotherboardSerial(norInfo, "40026H00810660A1");

		var modifiedNorInfo = service.ReadNor(copyFilePath);

		// Assert
		Assert.NotNull(norInfo);
		Assert.Equal(expected: "40026H00810660A0", norInfo.MotherboardSerialNumber);
		Assert.NotNull(modifiedNorInfo);
		Assert.Equal(expected: "40026H00810660A1", modifiedNorInfo.MotherboardSerialNumber);
	}

	[Fact]
	public void ModifyModelNumber_Test()
	{
		// Arrange
		var filePath = @"Z:\nor.bin";
		var copyFilePath = @"Z:\nor_copy.bin";
		File.Copy(filePath, copyFilePath, true); // Copy the file to ensure it exists
		var logger = Substitute.For<ILogger<NorService>>();

		// Act
		var service = new NorService(logger);
		var norInfo = service.ReadNor(copyFilePath);

		service.SetModel(norInfo, "CFI-1118B");

		var modifiedNorInfo = service.ReadNor(copyFilePath);

		// Assert
		Assert.NotNull(norInfo);
		Assert.Equal(expected: "CFI-1118A", norInfo.Model);
		Assert.NotNull(modifiedNorInfo);
		Assert.Equal(expected: "CFI-1118B", modifiedNorInfo.Model);
	}
}
