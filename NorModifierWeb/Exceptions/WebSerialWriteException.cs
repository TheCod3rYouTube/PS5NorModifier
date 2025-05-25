namespace NorModifierWeb.Exceptions;

/// <summary>
/// Exception thrown when there is an error writing to the web serial port.
/// </summary>
/// <param name="message">The error message.</param>
public class WebSerialWriteException(string message) : Exception(message) { }
