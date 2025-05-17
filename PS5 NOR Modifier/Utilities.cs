using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PS5_NOR_Modifier;

public enum PlaystationFiveModel
{
    DiscEdition,
    DigitalEdition
}

public static class Utilities
{
    // Declare offsets to detect console version
    private const long ps5VersionOffsetOne = 0x1c7010;
    private const long ps5VersionOffsetTwo = 0x1c7030;
    private const int ps5VersionOffsetLength = 12;

    private const long WiFiMacOffset = 0x1C73C0;
    private const long LANMacOffset = 0x1C4020;
    private const int macAddressLength = 6;

    private const long serialOffset = 0x1c7210;
    private const long moboSerialOffset = 0x1C7200;
    public const int serialLength = 17;

    private const long variantOffset = 0x1c7226;
    private const int variantOffsetLength = 19;

    public static readonly Dictionary<string, PlaystationFiveModel> PlaystationModelLookup = new ()
    {
        { "Disc Edition", PlaystationFiveModel.DigitalEdition },
        { "Digital Edition", PlaystationFiveModel.DiscEdition }
    };

    public static void TryCatchErrors(in Action action, in Action<Exception>? catchAction = null)
    {
        try 
        { 
            action(); 
        } 

        catch (Exception ex) 
        {
            ThrowError(ex.Message);
            catchAction?.Invoke(ex);
        }
    }

    public static void ThrowError(in string errmsg)
    {
        MessageBox.Show(errmsg, "An Error Has Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /// <summary>
    /// With thanks to  @jjxtra on Github. The code has already been created and there's no need to reinvent the wheel is there?
    /// </summary>
    #region Hex Code

    public static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
    {
        for (int i = 0; i < source.Length; i++)
        {
            if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
            {
                yield return i;
            }
        }
    }

    public static byte[] ConvertHexStringToByteArray(in string hexString)
    {
        if (hexString.Length % 2 != 0)
        {
            throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
        }

        byte[] data = new byte[hexString.Length / 2];
        for (int index = 0; index < data.Length; index++)
        {
            string byteValue = hexString.Substring(index * 2, 2);
            data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        return data;
    }

    #endregion

    public static string HexStringToString(in string hexString)
    {
        if (hexString == null || (hexString.Length & 1) == 1)
        {
            throw new ArgumentException();
        }
        var sb = new StringBuilder();
        for (var i = 0; i < hexString.Length; i += 2)
        {
            var hexChar = hexString.Substring(i, 2);
            sb.Append((char)Convert.ToByte(hexChar, 16));
        }
        return sb.ToString();
    }

    public static string CalculateChecksum(in string str)
    {
        int sum = 0;
        foreach (char c in str)
        {
            sum += (int)c;
        }
        return str + ":" + (sum & 0xFF).ToString("X2");
    }

    public static void ChangeSerialNumber(in string fileName, in string currentSerialNumber, in string newSerialNumber)
    {
        byte[] oldSerial = Encoding.UTF8.GetBytes(currentSerialNumber);
        string oldSerialHex = Convert.ToHexString(oldSerial);

        byte[] newSerial = Encoding.UTF8.GetBytes(newSerialNumber);
        string newSerialHex = Convert.ToHexString(newSerial);

        byte[] find = Utilities.ConvertHexStringToByteArray(Regex.Replace(oldSerialHex, "0x|[ ,]", string.Empty).Normalize().Trim());
        byte[] replace = Utilities.ConvertHexStringToByteArray(Regex.Replace(newSerialHex, "0x|[ ,]", string.Empty).Normalize().Trim());

        byte[] bytes = File.ReadAllBytes(fileName);
        foreach (int index in Utilities.PatternAt(bytes, find))
        {
            for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace.Length; i++, replaceIndex++)
            {
                bytes[i] = replace[replaceIndex];
            }
            File.WriteAllBytes(fileName, bytes);
        }
    }

    public static void SetBoardVariant(in string fileName, in string currentBoardVariant, in string newBoardVariant)
    {
        if (currentBoardVariant == newBoardVariant)
            return;

        byte[] oldVariant = Encoding.UTF8.GetBytes(currentBoardVariant);
        string oldVariantHex = Convert.ToHexString(oldVariant);

        byte[] newVariantSelection = Encoding.UTF8.GetBytes(newBoardVariant);
        string newVariantHex = Convert.ToHexString(newVariantSelection);

        byte[] find = Utilities.ConvertHexStringToByteArray(Regex.Replace(oldVariantHex, "0x|[ ,]", string.Empty).Normalize().Trim());
        byte[] replace = Utilities.ConvertHexStringToByteArray(Regex.Replace(newVariantHex, "0x|[ ,]", string.Empty).Normalize().Trim());

        byte[] bytes = File.ReadAllBytes(fileName);
        foreach (int index in Utilities.PatternAt(bytes, find))
        {
            for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace.Length; i++, replaceIndex++)
            {
                bytes[i] = replace[replaceIndex];
            }
            File.WriteAllBytes(fileName, bytes);
        }
    }

    public static void SetNewModelInfo(string fileName, in PlaystationFiveModel currentModel, in PlaystationFiveModel newModel)
    {
        if (currentModel == newModel)
            return;

        string find = string.Empty;
        string replace = string.Empty;

        if (currentModel == PlaystationFiveModel.DiscEdition)
        {
            find = "22020101";
            replace = "22030101";
        }

        else if (currentModel == PlaystationFiveModel.DigitalEdition)
        {
            find  = "22030101";
            replace = "22020101";
        }

        Utilities.TryCatchErrors(() =>
        {
            byte[] findBytes = Utilities.ConvertHexStringToByteArray(Regex.Replace(find, "0x|[ ,]", string.Empty).Normalize().Trim());
            byte[] replaceBytes = Utilities.ConvertHexStringToByteArray(Regex.Replace(replace, "0x|[ ,]", string.Empty).Normalize().Trim());
            if (find.Length != replace.Length)
                throw new Exception("The length of the old hex value does not match the length of the new hex value!");

            byte[] bytes = File.ReadAllBytes(fileName);
            foreach (int index in Utilities.PatternAt(bytes, findBytes))
            {
                for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace.Length; i++, replaceIndex++)
                {
                    bytes[i] = replaceBytes[replaceIndex];
                }
                File.WriteAllBytes(fileName, bytes);
            }

        }, 
        (_) => ThrowError("An error occurred while saving your BIOS file"));
    }

    public static string? TryReadValueAtFileStreamOffset(in BinaryReader fileBinaryReader, in long offset, in int length, in bool closeReader = false)
    {
        string? value = null;

        try
        {
            if (!fileBinaryReader.BaseStream.CanRead)
                throw new Exception("fileBinaryReader is unable to read file stream!");

            fileBinaryReader.BaseStream.Position = offset;
            value = BitConverter.ToString(fileBinaryReader.ReadBytes(length));

            if (closeReader)
                fileBinaryReader.Close();
        }

        catch
        {
            return value;
        }

        return value;
    }

    public static string ExtractPS5Version(in BinaryReader reader, in bool closeReader = false)
    {
        // File stream offset one
        string? ps5VersionOffsetOneValue = TryReadValueAtFileStreamOffset(reader, ps5VersionOffsetOne, ps5VersionOffsetLength)?.Replace("-", null);

        // File stream offset two
        string? ps5VersionOffsetTwoValue = TryReadValueAtFileStreamOffset(reader, ps5VersionOffsetTwo, ps5VersionOffsetLength, closeReader)?.Replace("-", null);

        if (ps5VersionOffsetOneValue?.Contains("22020101") ?? false)
            return "Disc Edition";

        if (ps5VersionOffsetTwoValue?.Contains("22030101") ?? false)
            return "Digital Edition";

        return "Unknown";
    }

    public static string ExtractMotherboardSerialNumber(in BinaryReader reader, in bool closeReader = false)
    {
        string? moboSerialValue = TryReadValueAtFileStreamOffset(reader, moboSerialOffset, serialLength, closeReader)?.Replace("-", null);

        if (moboSerialValue is not null)
            return HexStringToString(moboSerialValue);

        return "Unknown";
    }

    public static string ExtractBoardSerialNumber(in BinaryReader reader, in bool closeReader = false)
    {
        string? serialValue = TryReadValueAtFileStreamOffset(reader, serialOffset, serialLength, closeReader)?.Replace("-", null);

        if (serialValue is not null)
            return HexStringToString(serialValue);


        return "Unknown";
    }

    public static string ExtractWiFiMacAddress(in BinaryReader reader, in bool closeReader = false)
    {
        string? WiFiMacValue = TryReadValueAtFileStreamOffset(reader, WiFiMacOffset, macAddressLength, closeReader);

        if (WiFiMacValue is not null)
            return WiFiMacValue;

        return "Unknown";
    }

    public static string ExtractLANMacAddress(in BinaryReader reader, in bool closeReader = false)
    {
        string? LANMacValue = TryReadValueAtFileStreamOffset(reader, WiFiMacOffset, macAddressLength, closeReader);

        if (LANMacValue is not null)
            return LANMacValue;

        return "Unknown";
    }

    public static string ExtractBoardVariant(in BinaryReader reader, in bool closeReader = false)
    {
        string? variantValue = TryReadValueAtFileStreamOffset(reader, variantOffset, variantOffsetLength, closeReader)?.Replace("-", null).Replace("FF", null);

        if (variantValue is not null)
            variantValue = HexStringToString(variantValue);
        else
            variantValue = "Unknown";

        return variantValue switch
        {
            _ when variantValue.EndsWith("00A") || variantValue.EndsWith("00B") => " - Japan",

            _ when variantValue.EndsWith("01A") || variantValue.EndsWith("01B") || 
                   variantValue.EndsWith("15A") || variantValue.EndsWith("15B") => " - US, Canada, (North America)",

            _ when variantValue.EndsWith("02A") || variantValue.EndsWith("02B") => " - Australia / New Zealand, (Oceania)",
            _ when variantValue.EndsWith("03A") || variantValue.EndsWith("03B") => " - United Kingdom / Ireland",
            _ when variantValue.EndsWith("04A") || variantValue.EndsWith("04B") => " - Europe / Middle East / Africa",
            _ when variantValue.EndsWith("05A") || variantValue.EndsWith("05B") => " - South Korea",
            _ when variantValue.EndsWith("06A") || variantValue.EndsWith("06B") => " - Southeast Asia / Hong Kong",
            _ when variantValue.EndsWith("07A") || variantValue.EndsWith("07B") => " - Taiwan",
            _ when variantValue.EndsWith("08A") || variantValue.EndsWith("08B") => " - Russia, Ukraine, India, Central Asia",
            _ when variantValue.EndsWith("09A") || variantValue.EndsWith("09B") => " - Mainland China",

            _ when variantValue.EndsWith("11A") || variantValue.EndsWith("11B") || variantValue.EndsWith("14A") || 
                   variantValue.EndsWith("14B") => " - Mexico, Central America, South America",

            _ when variantValue.EndsWith("16A") || variantValue.EndsWith("16B") => " - Europe / Middle East / Africa",
            _ when variantValue.EndsWith("18A") || variantValue.EndsWith("18B") => " - Singapore, Korea, Asia",
            _ => " - Unknown Region"
        };
    }
}
