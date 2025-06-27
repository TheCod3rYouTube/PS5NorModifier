using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Text;

namespace UART_CL_By_TheCod3r;

public static class Utilities
{
    /// <summary>
    /// Handles checksum calculations for UART commands.
    /// </summary>
    public static class Checksum
    {
        /// <summary>
        /// Calculates the 8-bit checksum for a given string.
        /// </summary>
        /// <param name="data">The input string.</param>
        /// <returns>The calculated checksum value.</returns>
        public static int Calculate(string data)
        {
            // Sum the ASCII values of all characters.
            int sum = data.Sum(c => (int)c);
            // Return the least significant byte of the sum.
            return sum & 0xFF;
        }

        /// <summary>
        /// Appends a calculated checksum to a string in the required format "string:XX".
        /// </summary>
        /// <param name="data">The input string.</param>
        /// <returns>The string with its checksum appended.</returns>
        public static string Append(string data)
        {
            int checksumValue = Calculate(data);
            return $"{data}:{checksumValue:X2}";
        }
    }

    /// <summary>
    /// Provides utility methods for Hex, Byte, and String conversions.
    /// </summary>
    public static class Hex
    {
        public static string BytesToString(byte[] bytes) => BitConverter.ToString(bytes).Replace("-", "");

        public static byte[] ConvertStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("Hex string must have an even number of digits.", nameof(hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int i = 0; i < data.Length; i++)
            {
                string byteValue = hexString.Substring(i * 2, 2);
                data[i] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return data;
        }

        public static IEnumerable<int> FindPattern(byte[] source, byte[] pattern)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                {
                    yield return i;
                }
            }
        }
    }

    /// <summary>
    /// Interacts with the underlying operating system.
    /// </summary>
    public static class System
    {
        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening URL: {ex.Message}");
            }
        }

        public static string GetPortFriendlyName(string portName)
        {
            // ManagementObjectSearcher is Windows-specific.
            if (!OperatingSystem.IsWindows())
            {
                return "Friendly name not available on this OS";
            }

            try
            {
                using var searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%({portName})%'");
                foreach (var port in searcher.Get())
                {
                    return port["Name"]?.ToString() ?? portName;
                }
            }
            catch (Exception)
            {
                // Fallback if WMI query fails
                return "Unknown Port Name";
            }
            return portName;
        }
    }

    /// <summary>
    /// Maps PS5 model suffixes to their corresponding regions.
    /// </summary>
    public static class RegionMapper
    {
        private static readonly Dictionary<string, string> SuffixToRegion = new()
        {
            { "00", "Japan" },
            { "01", "North America" },
            { "02", "Australia / New Zealand" },
            { "03", "United Kingdom / Ireland" },
            { "04", "Europe / Middle East / Africa" },
            { "05", "South Korea" },
            { "06", "Southeast Asia / Hong Kong" },
            { "07", "Taiwan" },
            { "08", "Russia / Ukraine / India / Central Asia" },
            { "09", "Mainland China" },
            { "11", "Mexico / Central & South America" },
            { "14", "Mexico / Central & South America" },
            { "15", "North America" },
            { "16", "Europe / Middle East / Africa" },
            { "18", "Singapore / Korea / Asia" }
        };

        public static string GetRegionFromModel(string model)
        {
            if (model == null || model.Length < 7) return "Unknown Region";

            // CFI-12xxA -> extracts "12"
            string suffix = model.Substring(6, 2);

            return SuffixToRegion.TryGetValue(suffix, out var region) ? region : "Unknown Region";
        }
    }
}
