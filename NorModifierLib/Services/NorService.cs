using System.IO.Hashing;
using System.Text;
using Microsoft.Extensions.Logging;
using NorModifierLib.Data;
using NorModifierLib.Enumerators;
using NorModifierLib.Exceptions;
using NorModifierLib.Interfaces;

namespace NorModifierLib.Services;

/// <summary>
/// Service for reading and modifying NOR files.
/// </summary>
/// <param name="logger">ILogger interface to receive log data</param>
public class NorService(ILogger<NorService> logger) : INorService
{
	private const uint _knownHeaderChecksum = 0xdbff5c9d;
	private const long _editionOffsetOne = 0x1c7010;
	private const long _editionOffsetTwo = 0x1c7030;
	private const long _serialOffset = 0x1c7210;
	private const long _modelOffset = 0x1c7230;
	private const long _moboSerialOffset = 0x1C7200;
	private const long _wifiMacOffset = 0x1C73C0;
	private const long _lanMacOffset = 0x1C4020;
	private const long _logStartOffset = 0x1CE100;
	private const long _logEndOffset = 0x1CEC70;
	private const int _logEntrySize = 32 * 8; // uint32 * 8

	/// <summary>
	/// Reads the NOR file and extracts properties such as edition, region, console serial number, motherboard serial number, model, WiFi MAC address, and LAN MAC address.
	/// </summary>
	/// <param name="filePath">The path to the NOR dump file</param>
	/// <returns>A NORInfo containing the NOR properties</returns>
	/// <exception cref="FileNotFoundException">Thrown if the dump file cannot be found</exception>
	/// <exception cref="NorReadException">Thrown if a property cannot be read from the dump file</exception>
	/// <exception cref="InvalidDataException">Thrown if the dump header checksum validation fails</exception>
	public NorInfo ReadNor(string filePath)
	{
		if (!File.Exists(filePath))
		{
			logger.LogError("NOR file does not exist at provided path.");
			throw new FileNotFoundException("NOR file does not exist at provided path.", filePath);
		}

		BinaryReader reader;
		try
		{
			reader = new BinaryReader(new FileStream(filePath, FileMode.Open));
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "NOR file could not be opened.");

			throw new NorReadException("NOR file could not be opened.", ex);
		}

		// Read the header and validate that this is a NOR file\
		uint headerChecksum;
		try
		{
			reader.BaseStream.Position = 0;
			var bytes = reader.ReadBytes(32);
			headerChecksum = Crc32.HashToUInt32(bytes);

			logger.LogInformation("NOR header data: {Header}:{Checksum}", bytes, headerChecksum);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "NOR header extraction failed.");

			reader.Close();
			reader.Dispose();

			throw new NorReadException("NOR header extraction failed.", ex);
		}

		if (_knownHeaderChecksum != headerChecksum)
		{
			logger.LogError("NOR header checksum does not match expected value. Expected: {Expected}, Actual: {Actual}", _knownHeaderChecksum, headerChecksum);

			reader.Close();
			reader.Dispose();

			throw new InvalidDataException($"NOR header checksum does not match expected value. Expected: {_knownHeaderChecksum}, Actual: {headerChecksum}");
		}

		// Get the edition from the NOR
		string editionOne;
		string editionTwo;
		try
		{
			reader.BaseStream.Position = _editionOffsetOne;
			var bytes = reader.ReadBytes(12);
			editionOne = Convert.ToHexString(bytes);

			reader.BaseStream.Position = _editionOffsetTwo;
			bytes = reader.ReadBytes(12);
			editionTwo = Convert.ToHexString(bytes);

			logger.LogInformation("NOR edition data: {EditionOne}-{EditionTwo}", editionOne, editionTwo);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "NOR edition extraction failed.");

			reader.Close();
			reader.Dispose();

			throw new NorReadException("NOR edition extraction failed.", ex);
		}

		Edition edition;
		try
		{
			edition = (editionOne, editionTwo) switch
			{
				var versions when versions.editionOne.Contains("22020101") => Edition.Disc,
				var versions when versions.editionTwo.Contains("22030101") => Edition.Digital,
				var versions when versions.editionOne.Contains("22010101") || versions.editionTwo.Contains("22010101") => Edition.Slim,
				(_, _) => throw new InvalidDataException($"NOR edition offsets did not match any known edition data. Offset One: {editionOne}. Offset Two: {editionTwo}")
			};
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "NOR edition parsing failed.");

			reader.Close();
			reader.Dispose();

			throw new NorReadException("NOR edition parsing failed.", ex);
		}

		logger.LogInformation("Detected edition: {Edition}", edition);

		string model;
		try
		{
			reader.BaseStream.Position = _modelOffset;
			var bytes = reader.ReadBytes(9);
			model = Encoding.ASCII.GetString(bytes);

			logger.LogInformation("NOR variant data: {Variant}", model);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "NOR model extraction failed.");

			reader.Close();
			reader.Dispose();

			throw new NorReadException("NOR model extraction failed.", ex);
		}

		var region = model[^3..] switch
		{
			"00A" => "Japan",
			"00B" => "Japan",
			"01A" => "US, Canada, (North America)",
			"01B" => "US, Canada, (North America)",
			"15A" => "US, Canada, (North America)",
			"15B" => "US, Canada, (North America)",
			"02A" => "Australia / New Zealand, (Oceania)",
			"02B" => "Australia / New Zealand, (Oceania)",
			"03A" => "United Kingdom / Ireland",
			"03B" => "United Kingdom / Ireland",
			"04A" => "Europe / Middle East / Africa",
			"04B" => "Europe / Middle East / Africa",
			"05A" => "South Korea",
			"05B" => "South Korea",
			"06A" => "Southeast Asia / Hong Kong",
			"06B" => "Southeast Asia / Hong Kong",
			"07A" => "Taiwan",
			"07B" => "Taiwan",
			"08A" => "Russia, Ukraine, India, Central Asia",
			"08B" => "Russia, Ukraine, India, Central Asia",
			"09A" => "Mainland China",
			"09B" => "Mainland China",
			"11A" => "Mexico, Central America, South America",
			"11B" => "Mexico, Central America, South America",
			"14A" => "Mexico, Central America, South America",
			"14B" => "Mexico, Central America, South America",
			"16A" => "Europe / Middle East / Africa",
			"16B" => "Europe / Middle East / Africa",
			"18A" => "Singapore, Korea, Asia",
			"18B" => "Singapore, Korea, Asia",
			_ => "Unknown Region"
		};

		logger.LogInformation("Detected region: {Region}", region);

		string serial;
		try
		{
			reader.BaseStream.Position = _serialOffset;
			var bytes = reader.ReadBytes(17);
			serial = Encoding.ASCII.GetString(bytes).TrimEnd('\0');

			logger.LogInformation("Console serial number: {Serial}", serial);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "NOR console serial number extraction failed.");

			reader.Close();
			reader.Dispose();

			throw new NorReadException("NOR console serial number extraction failed.", ex);
		}

		string motherboardSerial;
		try
		{
			reader.BaseStream.Position = _moboSerialOffset;
			var bytes = reader.ReadBytes(16);
			motherboardSerial = Encoding.ASCII.GetString(bytes).TrimEnd('\0');

			logger.LogInformation("Motherboard serial number: {Serial}", motherboardSerial);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "NOR motherboard serial number extraction failed.");

			reader.Close();
			reader.Dispose();

			throw new NorReadException("NOR motherboard serial number extraction failed.", ex);
		}

		string wifiMac;
		try
		{
			reader.BaseStream.Position = _wifiMacOffset;
			var bytes = reader.ReadBytes(6);
			wifiMac = BitConverter.ToString(bytes);

			logger.LogInformation("WiFi MAC address: {Mac}", wifiMac);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "NOR WiFi MAC address extraction failed.");

			reader.Close();
			reader.Dispose();

			throw new NorReadException("NOR WiFi MAC address extraction failed.", ex);
		}

		string lanMac;
		try
		{
			reader.BaseStream.Position = _lanMacOffset;
			var bytes = reader.ReadBytes(6);
			lanMac = BitConverter.ToString(bytes);

			logger.LogInformation("LAN MAC address: {Mac}", lanMac);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "NOR LAN MAC address extraction failed.");

			reader.Close();
			reader.Dispose();

			throw new NorReadException("NOR LAN MAC address extraction failed.", ex);
		}

		var errors = new List<NorError>();
		try
		{
			reader.BaseStream.Position = _logStartOffset;

			for (int i = 0; reader.BaseStream.Position <= _logEndOffset - _logEntrySize; i++)
			{
				var bytes = reader.ReadBytes(_logEntrySize);

				if (BitConverter.ToUInt32(bytes.AsSpan()[0..8]) == 0xFFFFFFFF)
				{
					logger.LogInformation("NOR log entry {i} is empty, ending log read.", i);
					break;
				}

				errors.Add(new NorError(bytes));

				logger.LogInformation("Log entry {i}: {Log}", i, bytes);
			}		
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "NOR log extraction failed.");

			reader.Close();
			reader.Dispose();

			throw new NorReadException("NOR log extraction failed.", ex);
		}

		reader.Close();
		reader.Dispose();

		return new()
		{
			Path = filePath, 
			Edition = edition, 
			Region = region, 
			ConsoleSerialNumber = serial, 
			MotherboardSerialNumber = motherboardSerial, 
			Model = model, 
			WiFiMac = wifiMac, 
			LanMac = lanMac, 
			Errors = errors, 
		};
	}

	/// <summary>
	/// Sets the console edition in the NOR file.
	/// </summary>
	/// <param name="NOR">The NORInfo object for the NOR file.</param>
	/// <param name="edition">The edition to set in the NOR file.</param>
	public void SetEdition(NorInfo NOR, Edition edition)
	{
		var editionBytes = edition.GetBytes();

		using var writer = OpenNOR(NOR.Path);
		try
		{
			writer.BaseStream.Position = _editionOffsetOne;
			writer.Write(editionBytes);
			writer.BaseStream.Position = _editionOffsetTwo;
			writer.Write(editionBytes);

			logger.LogInformation("Wrote console edition: {Edition}", edition);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to write console edition at the provided offsets.");
			throw new NorWriteException("Failed to write console edition at the provided offsets.", ex);
		}
		finally
		{
			writer.Flush();
			writer.Close();
			writer.Dispose();
		}
	}

	/// <summary>
	/// Sets the console serial number in the NOR file.
	/// </summary>
	/// <param name="NOR">The NORInfo object for the NOR file.</param>
	/// <param name="serial">The serial to set in the NOR file.</param>
	public void SetConsoleSerial(NorInfo NOR, string serial)
	{
		var bytes = Encoding.ASCII.GetBytes(serial);
		var paddedBytes = new byte[17];

		if (bytes.Length > 17)
		{
			logger.LogError("Console serial number is too long. Maximum length is 17 bytes.");
			throw new ArgumentException("Console serial number is too long. Maximum length is 17 bytes.", nameof(serial));
		}

		Array.Copy(bytes, paddedBytes, bytes.Length);
		logger.LogInformation("Attempting to write the console serial number: {Serial}-{Bytes}", serial, paddedBytes);

		using var writer = OpenNOR(NOR.Path);
		try
		{
			writer.BaseStream.Position = _serialOffset;
			writer.Write(paddedBytes);

			logger.LogInformation("Wrote console serial number: {Serial}", paddedBytes);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to write console serial number at the provided offset.");
			throw new NorWriteException("Failed to write console serial number at the provided offset.", ex);
		}
		finally
		{
			writer.Flush();
			writer.Close();
			writer.Dispose();
		}
	}

	/// <summary>
	/// Sets the motherboard serial number in the NOR file.
	/// </summary>
	/// <param name="NOR"></param>
	/// <param name="serial"></param>
	/// <exception cref="ArgumentException"></exception>
	public void SetMotherboardSerial(NorInfo NOR, string serial)
	{
		var bytes = Encoding.ASCII.GetBytes(serial);
		var paddedBytes = new byte[16];

		if (bytes.Length > 16)
		{
			logger.LogError("Motherboard serial number is too long. Maximum length is 16 bytes.");
			throw new ArgumentException("Motherboard serial number is too long. Maximum length is 16 bytes.", nameof(serial));
		}

		Array.Copy(bytes, paddedBytes, bytes.Length);
		logger.LogInformation("Attempting to write the motherboard serial number: {Serial}-{Bytes}", serial, paddedBytes);

		using var writer = OpenNOR(NOR.Path);
		try
		{
			writer.BaseStream.Position = _moboSerialOffset;
			writer.Write(paddedBytes);

			logger.LogInformation("Wrote motherboard serial number: {Serial}", paddedBytes);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to write motherboard serial number at the provided offset.");
			throw new NorWriteException("Failed to write motherboard serial number at the provided offset.", ex);
		}
		finally
		{
			writer.Flush();
			writer.Close();
			writer.Dispose();
		}
	}

	/// <summary>
	/// Sets the model in the NOR file.
	/// </summary>
	/// <param name="NOR">The NORInfo object for the NOR file.</param>
	/// <param name="serial">The model to set in the NOR file.</param>
	public void SetModel(NorInfo NOR, string model)
	{
		var bytes = Encoding.ASCII.GetBytes(model);
		var paddedBytes = new byte[9];

		if (bytes.Length > 9)
		{
			logger.LogError("Model number is too long. Maximum length is 9 bytes.");
			throw new ArgumentException("Model number is too long. Maximum length is 9 bytes.", nameof(model));
		}

		Array.Copy(bytes, paddedBytes, bytes.Length);
		logger.LogInformation("Attempting to write the model number: {Model}-{Bytes}", model, paddedBytes);

		using var writer = OpenNOR(NOR.Path);
		try
		{
			writer.BaseStream.Position = _modelOffset;
			writer.Write(paddedBytes);

			logger.LogInformation("Wrote model number: {Model}", paddedBytes);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to write model number at the provided offset.");
			throw new NorWriteException("Failed to write model number at the provided offset.", ex);
		}
		finally
		{
			writer.Flush();
			writer.Close();
			writer.Dispose();
		}
	}

	/// <summary>
	/// Opens the NOR file for reading and writing.
	/// </summary>
	/// <param name="filePath">The path to the dump file</param>
	/// <returns>A BinaryWriter for the dump file</returns>
	/// <exception cref="NorReadException">Thrown when the dump file cannot be read</exception>
	private BinaryWriter OpenNOR(string filePath)
	{
		BinaryWriter writer;
		try
		{
			writer = new BinaryWriter(new FileStream(filePath, FileMode.Open));
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "NOR file could not be opened.");
			throw new NorReadException("NOR file could not be opened.", ex);
		}

		return writer;
	}
}
