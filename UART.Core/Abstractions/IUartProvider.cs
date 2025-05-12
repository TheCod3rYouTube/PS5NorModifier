namespace UART.Core.Abstractions;

public interface IUartProvider
{
    Task UpdateErrorDatabase();
    string ParseErrorsOffline(string errorCode);
    
    Task<string> ParseErrorsOnline(string errorCode);
}