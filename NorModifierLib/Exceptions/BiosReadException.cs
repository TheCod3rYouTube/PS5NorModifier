namespace NorModifierLib.Exceptions;

public class BiosReadException(string message, Exception innerException) : Exception(message, innerException) { }
