using NorModifierLib.Data;
using NorModifierLib.Enumerators;

namespace NorModifierLib.Interfaces;

/// <summary>
/// Interface for a service used to read and write NOR dumps.
/// </summary>
public interface INorService
{
	public NorInfo ReadNor(string filePath);
	public void SetEdition(NorInfo NOR, Edition edition);
	public void SetConsoleSerial(NorInfo NOR, string serial);
	public void SetMotherboardSerial(NorInfo NOR, string serial);
	public void SetModel(NorInfo NOR, string model);
}
