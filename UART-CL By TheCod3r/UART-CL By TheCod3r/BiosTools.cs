using System.Text;
using System.Text.RegularExpressions;

namespace UART_CL_By_TheCod3r;

/// <summary>
/// Handles all operations on PS5 BIOS dump files (.bin).
/// </summary>
public class BiosTools
{
    // Define constants for BIOS file offsets
    private static class BiosOffsets
    {
        public const long ModelVersion1 = 0x1C7010;
        public const long ModelVersion2 = 0x1C7030;
        public const long ConsoleSerial = 0x1C7210;
        public const long MotherboardSerial = 0x1C7200;
        public const long ConsoleModel = 0x1C7226;
        public const long WiFiMac = 0x1C73C0;
        public const long LanMac = 0x1C4020;
    }

    // Define constants for model signature hex strings
    private static class ModelSignatures
    {
        public const string Disc = "22020101";
        public const string Digital = "22030101";
        public const string Slim = "22010101";
    }

    public enum ConsoleEdition { Disc, Digital, Slim }

    public string? LoadedDumpPath { get; private set; }

    /// <summary>
    /// Prompts the user to select and validate a BIOS dump file.
    /// </summary>
    public void LoadDumpFile()
    {
        Console.WriteLine("\nEnter the full path to your BIOS dump file (.bin). Type 'exit' to cancel.");
        while (true)
        {
            Console.Write("File path: ");
            string? userInput = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                Console.WriteLine("Invalid input. File path cannot be blank.");
                continue;
            }
            if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                LoadedDumpPath = null;
                return;
            }
            if (!File.Exists(userInput))
            {
                Console.WriteLine("File not found. Please enter a valid path.");
                continue;
            }
            if (Path.GetExtension(userInput).ToLower() != ".bin")
            {
                Console.WriteLine("Invalid file type. Only .bin files are supported.");
                continue;
            }

            LoadedDumpPath = userInput;
            long fileSize = new FileInfo(LoadedDumpPath).Length;
            Console.WriteLine($"Successfully loaded: {Path.GetFileName(LoadedDumpPath)} ({fileSize / 1024 / 1024}MB)");
            return;
        }
    }

    /// <summary>
    /// Displays all known information from the loaded BIOS file.
    /// </summary>
    public void DisplayBiosInfo()
    {
        if (!IsFileLoaded()) return;

        Console.WriteLine("\n--- BIOS Information ---");
        Console.WriteLine($"File Size: {new FileInfo(LoadedDumpPath!).Length / 1024 / 1024}MB");
        Console.WriteLine($"PS5 Version: {GetPs5Version()}");

        string model = GetStringFromFile(BiosOffsets.ConsoleModel, 19).Replace("\0", "").Replace("ÿ", "");
        string region = Utilities.RegionMapper.GetRegionFromModel(model);
        Console.WriteLine($"Console Model: {model} ({region})");

        Console.WriteLine($"Console S/N: {GetStringFromFile(BiosOffsets.ConsoleSerial, 17).Replace("\0", "")}");
        Console.WriteLine($"Motherboard S/N: {GetStringFromFile(BiosOffsets.MotherboardSerial, 16)}");
        Console.WriteLine($"WiFi MAC: {GetMacAddressFromFile(BiosOffsets.WiFiMac)}");
        Console.WriteLine($"LAN MAC: {GetMacAddressFromFile(BiosOffsets.LanMac)}");
        Console.WriteLine("----------------------");
    }

    /// <summary>
    /// Patches the BIOS file to a specified console edition.
    /// </summary>
    public void ConvertEdition(ConsoleEdition edition)
    {
        if (!IsFileLoaded()) return;

        string targetSignature = edition switch
        {
            ConsoleEdition.Disc => ModelSignatures.Disc,
            ConsoleEdition.Digital => ModelSignatures.Digital,
            ConsoleEdition.Slim => ModelSignatures.Slim,
            _ => throw new ArgumentOutOfRangeException(nameof(edition), "Invalid edition specified.")
        };

        Console.WriteLine($"Are you sure you want to convert to \"{edition}\" edition? Type 'yes' to confirm.");
        if (Console.ReadLine()?.ToLower() != "yes")
        {
            Console.WriteLine("Conversion cancelled.");
            return;
        }

        try
        {
            byte[] bytes = File.ReadAllBytes(LoadedDumpPath!);
            bool modified = false;

            // Find and replace all possible old signatures
            foreach (var oldSig in new[] { ModelSignatures.Disc, ModelSignatures.Digital, ModelSignatures.Slim })
            {
                if (oldSig == targetSignature) continue;

                byte[] findPattern = Utilities.Hex.ConvertStringToByteArray(oldSig);
                byte[] replacePattern = Utilities.Hex.ConvertStringToByteArray(targetSignature);

                foreach (int index in Utilities.Hex.FindPattern(bytes, findPattern))
                {
                    Array.Copy(replacePattern, 0, bytes, index, replacePattern.Length);
                    modified = true;
                }
            }

            if (modified)
            {
                File.WriteAllBytes(LoadedDumpPath!, bytes);
                Console.WriteLine($"Successfully converted BIOS to {edition} edition.");
            }
            else
            {
                Console.WriteLine("BIOS is already set to the target edition. No changes were made.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during conversion: {ex.Message}");
        }
    }

    /// <summary>
    /// Prompts for and changes the console serial number in the BIOS file.
    /// </summary>
    public void ChangeConsoleSerialNumber()
    {
        if (!IsFileLoaded()) return;
        ChangeStringValue("console serial number", BiosOffsets.ConsoleSerial, 17);
    }

    /// <summary>
    /// Prompts for and changes the motherboard serial number in the BIOS file.
    /// </summary>
    public void ChangeMotherboardSerialNumber()
    {
        if (!IsFileLoaded()) return;
        ChangeStringValue("motherboard serial number", BiosOffsets.MotherboardSerial, 16);
    }

    /// <summary>
    /// Prompts for and changes the console model number in the BIOS file.
    /// </summary>
    public void ChangeConsoleModelNumber()
    {
        if (!IsFileLoaded()) return;
        ChangeStringValue("console model number (e.g., CFI-1216A)", BiosOffsets.ConsoleModel, 9, "CFI-");
    }

    #region Private Helper Methods

    private bool IsFileLoaded()
    {
        if (string.IsNullOrEmpty(LoadedDumpPath))
        {
            Console.WriteLine("\nError: No BIOS dump file has been loaded. Please use Option 1 first.");
            return false;
        }
        return true;
    }

    private string GetStringFromFile(long offset, int length)
    {
        try
        {
            using var reader = new BinaryReader(File.Open(LoadedDumpPath!, FileMode.Open, FileAccess.Read, FileShare.Read));
            reader.BaseStream.Position = offset;
            return Encoding.UTF8.GetString(reader.ReadBytes(length));
        }
        catch (Exception) { return "Read Error"; }
    }

    private string GetMacAddressFromFile(long offset)
    {
        try
        {
            using var reader = new BinaryReader(File.Open(LoadedDumpPath!, FileMode.Open, FileAccess.Read, FileShare.Read));
            reader.BaseStream.Position = offset;
            return BitConverter.ToString(reader.ReadBytes(6));
        }
        catch (Exception) { return "Read Error"; }
    }

    private string GetPs5Version()
    {
        string offsetOneValue = Utilities.Hex.BytesToString(ReadBytesFromFile(BiosOffsets.ModelVersion1, 4));
        string offsetTwoValue = Utilities.Hex.BytesToString(ReadBytesFromFile(BiosOffsets.ModelVersion2, 4));

        if (offsetOneValue.Contains(ModelSignatures.Disc) || offsetTwoValue.Contains(ModelSignatures.Disc))
            return "Disc Edition";
        if (offsetOneValue.Contains(ModelSignatures.Digital) || offsetTwoValue.Contains(ModelSignatures.Digital))
            return "Digital Edition";
        if (offsetOneValue.Contains(ModelSignatures.Slim) || offsetTwoValue.Contains(ModelSignatures.Slim))
            return "Slim Edition";

        return "Unknown Model";
    }

    private byte[] ReadBytesFromFile(long offset, int length)
    {
        try
        {
            using var reader = new BinaryReader(File.Open(LoadedDumpPath!, FileMode.Open, FileAccess.Read, FileShare.Read));
            reader.BaseStream.Position = offset;
            return reader.ReadBytes(length);
        }
        catch (Exception) { return Array.Empty<byte>(); }
    }

    private void PatchFile(long offset, byte[] data)
    {
        try
        {
            using var stream = new FileStream(LoadedDumpPath!, FileMode.Open, FileAccess.Write);
            stream.Position = offset;
            stream.Write(data, 0, data.Length);
            Console.WriteLine("Successfully patched the file.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to file: {ex.Message}");
        }
    }

    private void ChangeStringValue(string valueName, long offset, int maxLength, string? prefix = null)
    {
        string currentValue = GetStringFromFile(offset, maxLength).Replace("\0", "");
        Console.WriteLine($"\nCurrent {valueName}: {currentValue}");
        Console.WriteLine($"Enter the new {valueName} (or 'exit' to cancel):");

        string? newValue = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(newValue) || newValue.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Operation cancelled.");
            return;
        }

        if (prefix != null && !newValue.StartsWith(prefix))
        {
            Console.WriteLine($"Invalid format. The value must start with '{prefix}'.");
            return;
        }

        if (newValue.Length > maxLength)
        {
            Console.WriteLine($"The new value is too long. Maximum length is {maxLength} characters.");
            return;
        }

        // Pad with null terminators to the required length
        byte[] newBytes = Encoding.UTF8.GetBytes(newValue.PadRight(maxLength, '\0'));
        PatchFile(offset, newBytes);
    }

    #endregion
}
