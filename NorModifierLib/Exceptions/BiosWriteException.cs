namespace NorModifierLib.Exceptions;

public class BiosWriteException(string message, Exception innerException) : Exception(message, innerException) { }
