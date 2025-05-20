namespace NorModifierLib.Exceptions;

public class NorReadException(string message, Exception innerException) : Exception(message, innerException) { }
