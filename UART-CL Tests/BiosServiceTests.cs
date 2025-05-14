using Microsoft.Extensions.Logging;
using NSubstitute;
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

		parser.SetConsoleSerial(biosInfo, "AJ22516081");

		var modifiedBiosInfo = parser.ReadBios(copyFilePath);

		// Assert
		Assert.NotNull(biosInfo);
		Assert.Equal(expected: "AJ22516080", biosInfo.ConsoleSerialNumber);
		Assert.NotNull(modifiedBiosInfo);
		Assert.Equal(expected: "AJ22516081", modifiedBiosInfo.ConsoleSerialNumber);
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

		parser.SetMotherboardSerial(biosInfo, "179SC205704400A1");

		var modifiedBiosInfo = parser.ReadBios(copyFilePath);

		// Assert
		Assert.NotNull(biosInfo);
		Assert.Equal(expected: "179SC205704400A0", biosInfo.MotherboardSerialNumber);
		Assert.NotNull(modifiedBiosInfo);
		Assert.Equal(expected: "179SC205704400A1", modifiedBiosInfo.MotherboardSerialNumber);
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

		parser.SetModel(biosInfo, "CFI-1015B");

		var modifiedBiosInfo = parser.ReadBios(copyFilePath);

		// Assert
		Assert.NotNull(biosInfo);
		Assert.Equal(expected: "CFI-1015A", biosInfo.Model);
		Assert.NotNull(modifiedBiosInfo);
		Assert.Equal(expected: "CFI-1015B", modifiedBiosInfo.Model);
	}
}
