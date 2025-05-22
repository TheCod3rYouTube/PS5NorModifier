namespace NorModifierLib.Interfaces;

public interface ISerialPort
{
	public Task WriteLineAsync(string data);
	public Task<string> ReadLineAsync();
}
