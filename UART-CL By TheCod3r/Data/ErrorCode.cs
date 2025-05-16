namespace UART_CL_By_TheCod3r.Data
{
	/// <summary>
	/// Represents the properties of an error code.
	/// </summary>
	/// <param name="errorCode">The raw string of the error as read from the device.</param>
	public class ErrorCode(string errorCode)
    {
		//OK 00000000 80C00140 0000008D FFFF0005 00000100 2157 0016 46E4 1A80:27
		public string FirstPart => errorCode[3..11];
		public string SecondPart => errorCode[12..20];
		public string ThirdPart => errorCode[21..29];
		public string FourthPart => errorCode[30..38];
		public string FifthPart => errorCode[39..47];
		public string SixthPart => errorCode[48..52];
		public string SeventhPart => errorCode[53..57];
		public string EighthPart => errorCode[58..62];
		public string NinthPart => errorCode[63..67];
		public string Checksum => errorCode[^2..];

		/// <summary>
		/// Validates the checksum of the error code as retrieved from the device against the calculated checksum.
		/// </summary>
		public bool ChecksumValid => Checksum == Uart.CalculateChecksum(errorCode[..^3]);
	}
}
