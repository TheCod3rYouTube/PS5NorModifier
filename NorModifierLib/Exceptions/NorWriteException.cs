namespace NorModifierLib.Exceptions;

/// <summary>
/// Exception thrown when there is an error wrtiting the NOR dump.
/// </summary>
/// <param name="message">The error message.</param>
/// <param name="innerException">The underlying exception that triggered this exception.</param>
public class NorWriteException(string message, Exception innerException) : Exception(message, innerException) { }
