namespace NorModifierLib.Exceptions;

/// <summary>
/// Exception thrown when there is an error reading the NOR dump.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="innerException">The underlying exception that triggered this exception.</param>
public class NorReadException(string message, Exception innerException) : Exception(message, innerException) { }
