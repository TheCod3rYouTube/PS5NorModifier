namespace NorModifierLib.Exceptions;

public class NorWriteException(string message, Exception innerException) : Exception(message, innerException) { }
