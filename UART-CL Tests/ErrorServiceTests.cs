using Microsoft.Extensions.Logging;
using RichardSzalay.MockHttp;
using UART_CL_By_TheCod3r.Services;
using NSubstitute;

namespace UART_CL_Tests;

public class ErrorServiceTests
{
	private const string DatabaseUri = "http://uartcodes.com/xml.php";
	private const string ValidResponse = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><errorCodes><errorCode><ErrorCode>00100E3C</ErrorCode><Description>Known Unknown 00100E3C - Report Findings</Description></errorCode><errorCode><ErrorCode>0015442C</ErrorCode><Description>Known Unknown 0015442C - Report Findings</Description></errorCode><errorCode><ErrorCode>015E4E8B</ErrorCode><Description>Known Unknown 015E4E8B - Report Findings</Description></errorCode></errorCodes>";

	[Theory]
	[InlineData(ValidResponse)]
	public async Task ParseError_ValidData_Test(string validResponse)
	{
		// Arrange
		var logger = Substitute.For<ILogger<ErrorCodeService>>();
		var mockHttp = new MockHttpMessageHandler();

		mockHttp.When(DatabaseUri)
			.Respond("application/xml", validResponse);

		var client = mockHttp.ToHttpClient();

		var service = new ErrorCodeService(logger, client);

		// Act
		var errorDescription = await service.ParseError("00100E3C");

		// Assert
		Assert.Equal(expected: "Known Unknown 00100E3C - Report Findings", errorDescription);
	}

	[Theory]
	[ClassData(typeof(InvalidResponses))]
	public async Task ParseError_InvalidData_Test(string invalidResponse)
	{
		// Arrange
		var logger = Substitute.For<ILogger<ErrorCodeService>>();
		var mockHttp = new MockHttpMessageHandler();

		mockHttp.When("http://uartcodes.com/xml.php")
			.Respond("application/xml", invalidResponse);

		var client = mockHttp.ToHttpClient();

		var service = new ErrorCodeService(logger, client);

		// Act & Assert
		await Assert.ThrowsAnyAsync<Exception>(() => service.ParseError("00100E3C"));
	}

	public class InvalidResponses : TheoryData<string>
	{
		public InvalidResponses()
		{
			Add(""); // Blank response
			Add("<?xml version=\"1.0\" encoding=\"UTF-8\"?><errorCodes>"); // Incomplete XML
		}
	}
}
