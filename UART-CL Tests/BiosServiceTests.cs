using Microsoft.Extensions.Logging;
using NSubstitute;
using UART_CL_By_TheCod3r.Data;
using UART_CL_By_TheCod3r.Enumerators;
using UART_CL_By_TheCod3r.Services;

namespace UART_CL_Tests;

public class BiosServiceTests
{
	[Fact]
	public void ParseBios_Test()
	{
		// Arrange
		var filePath = @"Z:\bios.bin";
		var logger = Substitute.For<ILogger<BiosService>>();

		// Act
		var parser = new BiosService(logger);
		var biosInfo = parser.ReadBios(filePath);

		// Assert
		Assert.NotNull(biosInfo);
		Assert.NotEmpty(biosInfo.Errors);

		var biosError = biosInfo.Errors.First();
		Assert.Equal("Unknown Error Code", biosError.Code);
		Assert.Equal(416761713u, biosError.Rtc);
		Assert.Equal("HstOsOFF", biosError.PowerStateA);
		Assert.Equal("SOC_ON", biosError.PowerStateB);
		Assert.Equal("Power Button", biosError.BootCause);
		Assert.Equal("MSOC Reset Moni High, Main SoC Power Off, FATAL OFF", biosError.SequenceNumber);

		Assert.True(biosError.HdmiPower);
		Assert.True(biosError.BddPower);
		Assert.True(biosError.HdmiCecPower);
		Assert.True(biosError.UsbPower);
		Assert.False(biosError.WifiPower);

		Assert.Equal("35.72°C", biosError.EnvironmentTemperature);
		Assert.Equal("54.98°C", biosError.ChipTemperature);
	}

	[Fact]
	public void ModifyEdition_Test()
	{
		// Arrange
		var filePath = @"Z:\bios.bin";
		var copyFilePath = @"Z:\bios_copy.bin";
		File.Copy(filePath, copyFilePath, true); // Copy the file to ensure it exists
		var logger = Substitute.For<ILogger<BiosService>>();

		// Act
		var parser = new BiosService(logger);
		var biosInfo = parser.ReadBios(copyFilePath);

		parser.SetEdition(biosInfo, Edition.Digital);

		var modifiedBiosInfo = parser.ReadBios(copyFilePath);

		// Assert
		Assert.NotNull(biosInfo);
		Assert.Equal(expected: Edition.Disc, biosInfo.Edition);
		Assert.NotNull(modifiedBiosInfo);
		Assert.Equal(expected: Edition.Digital, modifiedBiosInfo.Edition);
	}

	[Fact]
	public void ModifyConsoleSerial_Test()
	{
		// Arrange
		var filePath = @"Z:\bios.bin";
		var copyFilePath = @"Z:\bios_copy.bin";
		File.Copy(filePath, copyFilePath, true); // Copy the file to ensure it exists
		var logger = Substitute.For<ILogger<BiosService>>();

		// Act
		var parser = new BiosService(logger);
		var biosInfo = parser.ReadBios(copyFilePath);

		parser.SetConsoleSerial(biosInfo, "F22301EL011425029");

		var modifiedBiosInfo = parser.ReadBios(copyFilePath);

		// Assert
		Assert.NotNull(biosInfo);
		Assert.Equal(expected: "F22301EL011425028", biosInfo.ConsoleSerialNumber);
		Assert.NotNull(modifiedBiosInfo);
		Assert.Equal(expected: "F22301EL011425029", modifiedBiosInfo.ConsoleSerialNumber);
	}

	[Fact]
	public void ModifyMotherboardSerial_Test()
	{
		// Arrange
		var filePath = @"Z:\bios.bin";
		var copyFilePath = @"Z:\bios_copy.bin";
		File.Copy(filePath, copyFilePath, true); // Copy the file to ensure it exists
		var logger = Substitute.For<ILogger<BiosService>>();

		// Act
		var parser = new BiosService(logger);
		var biosInfo = parser.ReadBios(copyFilePath);

		parser.SetMotherboardSerial(biosInfo, "40026H00810660A1");

		var modifiedBiosInfo = parser.ReadBios(copyFilePath);

		// Assert
		Assert.NotNull(biosInfo);
		Assert.Equal(expected: "40026H00810660A0", biosInfo.MotherboardSerialNumber);
		Assert.NotNull(modifiedBiosInfo);
		Assert.Equal(expected: "40026H00810660A1", modifiedBiosInfo.MotherboardSerialNumber);
	}

	[Fact]
	public void ModifyModelNumber_Test()
	{
		// Arrange
		var filePath = @"Z:\bios.bin";
		var copyFilePath = @"Z:\bios_copy.bin";
		File.Copy(filePath, copyFilePath, true); // Copy the file to ensure it exists
		var logger = Substitute.For<ILogger<BiosService>>();

		// Act
		var parser = new BiosService(logger);
		var biosInfo = parser.ReadBios(copyFilePath);

		parser.SetModel(biosInfo, "CFI-1118B");

		var modifiedBiosInfo = parser.ReadBios(copyFilePath);

		// Assert
		Assert.NotNull(biosInfo);
		Assert.Equal(expected: "CFI-1118A", biosInfo.Model);
		Assert.NotNull(modifiedBiosInfo);
		Assert.Equal(expected: "CFI-1118B", modifiedBiosInfo.Model);
	}
}
