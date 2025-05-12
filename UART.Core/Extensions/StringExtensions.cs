using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace UART.Core.Extensions;

public static class StringExtensions
{
    public static string HexStringToString(this string hexString)
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

    /// <summary>
    /// Lauinches a URL in a new window using the default browser...
    /// </summary>
    /// <param name="url">The URL you want to launch</param>
    public static void OpenUrl(this string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }

    public static byte[] ConvertHexStringToByteArray(this string hexString, [StringSyntax(StringSyntaxAttribute.Regex)]string pattern, string replacement = "")
        => Regex.Replace(hexString, pattern, replacement)
            .Normalize()
            .Trim()
            .ConvertHexStringToByteArray();

    public static byte[] ConvertHexStringToByteArray(this string hexString)
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

    public static string CalculateChecksum(this string str)
    {
        int sum = 0;
        foreach (char c in str)
        {
            sum += c;
        }
        return str + ":" + (sum & 0xFF).ToString("X2");
    }

    //TODO: move into class that implements IDisposable and allow methods to extract specific values
    public static string? ExtractValueFromFile(this string fileName, long offset, int count, Func<string, string>? postProcess = null, string? defaultValue = null)
    {
        try
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                //Set the position of the reader
                reader.BaseStream.Position = offset;
                
                // read the offset
                var result = BitConverter.ToString(reader.ReadBytes(count));
                
                if (postProcess != null)
                    return postProcess(result);

                return result;
            }
        }
        catch
        {
            return defaultValue;
        }
    }
}