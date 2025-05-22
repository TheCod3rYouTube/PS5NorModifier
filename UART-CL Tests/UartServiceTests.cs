using Microsoft.Extensions.Logging;
using NSubstitute;
using NorModifierLib.Services;
using NorModifierLib.Interfaces;
using NorModifierLib.Data;
using NorModifierLib.Exceptions;

namespace UART_CL_Tests;

public class UartServiceTests
{
	[Fact]
	public async Task ClearErrorsAsync_Test()
	{
		// Arrange
		var logger = Substitute.For<ILogger<UartService>>();
		var serialPort = Substitute.For<ISerialPort>();
		var uartService = new UartService(logger);

		// Act
		await uartService.ClearErrorsAsync(serialPort);

		// Assert
		await serialPort.Received(1).WriteLineAsync(Arg.Is<string>(x => x == Helpers.CreateTransmittableCommand("errlog clear")));
	}

	[Fact]
	public async Task GetErrorsAsync_Test()
	{
		// Arrange
		var logger = Substitute.For<ILogger<UartService>>();
		var serialPort = Substitute.For<ISerialPort>();
		var uartService = new UartService(logger);

		var errorOneResponse = "OK 00000000 80C00140 0000008D FFFF0005 00000100 2157 0016 46E4 1A80:27";
		var errorTwoResponse = "NG";
		serialPort.ReadLineAsync().Returns(x => errorOneResponse, x => errorTwoResponse);

		// Act
		var errors = await uartService.GetErrorsAsync(serialPort);

		// Assert
		await serialPort.Received(1).WriteLineAsync(Arg.Is<string>(x => x == Helpers.CreateTransmittableCommand("errlog 0")));
		await serialPort.Received(1).WriteLineAsync(Arg.Is<string>(x => x == Helpers.CreateTransmittableCommand("errlog 1")));

		Assert.NotNull(errors);
		Assert.Single(errors);

		var error = errors.First();
		Assert.Equal(0x80C00140, error.RawCode);
		Assert.Equal<uint>(0x0000008D, error.Rtc);
		Assert.Equal(0xFFFF0005, error.RawPowerState);
		Assert.Equal<uint>(0x00000100, error.RawBootCause);
		Assert.Equal(0x2157, error.RawSequenceNumber);
		Assert.Equal(0x0016, error.RawDevicePowerManagement);
		Assert.Equal(0x46E4, error.RawChipTemperature);
		Assert.Equal(0x1A80, error.RawEnvironmentTemperature);

		Assert.Equal("Power Button", errors.First().BootCause);
	}

	[Fact]
	public async Task GetErrorsAsync_ThrowsUartResponseInvalidException()
	{
		// Arrange
		var logger = Substitute.For<ILogger<UartService>>();
		var serialPort = Substitute.For<ISerialPort>();
		var uartService = new UartService(logger);

		var responseOne = "OK 00000000 80C00140 0000008D FFFF0005 00000100 2157 0016 46E4 1A80:28";
		var responseTwo = "OK 00000000 80C00140 0000008D FFFF0005 00000100 2157 0016 46E4 1A80";
		var responseThree = "KO 00000000 80C00140 0000008D FFFF0005 00000100 2157 0016 46E4 1A80:27";
		serialPort.ReadLineAsync().Returns(x => responseOne, x => responseTwo, x => responseThree);

		// Act & Assert
		await Assert.ThrowsAsync<UartResponseInvalidException>(() => uartService.GetErrorsAsync(serialPort));
		await Assert.ThrowsAsync<UartResponseInvalidException>(() => uartService.GetErrorsAsync(serialPort));
		await Assert.ThrowsAsync<UartResponseInvalidException>(() => uartService.GetErrorsAsync(serialPort));
	}
}
