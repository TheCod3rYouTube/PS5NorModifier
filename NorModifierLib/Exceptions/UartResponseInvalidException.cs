namespace NorModifierLib.Exceptions;

/// <summary>
/// Exception thrown when the response from the UART device is invalid.
/// </summary>
/// <param name="message">The error message.</param>
/// <summary>
public class UartResponseInvalidException(string message) : Exception(message) { }
