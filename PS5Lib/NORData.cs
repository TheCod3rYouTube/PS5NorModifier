using System.Text;

namespace PS5Lib;

public class NORData(string path)
{
    private static readonly Dictionary<string, string> Regions = new()
    {
        { "00", "Japan" },
        { "01", "US, Canada, (North America)" },
        { "15", "US, Canada, (North America)" },
        { "02", "Australia / New Zealand, (Oceania)" },
        { "03", "United Kingdom / Ireland" },
        { "04", "Europe / Middle East / Africa" },
        { "05", "South Korea" },
        { "06", "Southeast Asia / Hong Kong" },
        { "07", "Taiwan" },
        { "08", "Russia, Ukraine, India, Central Asia" },
        { "09", "Mainland China" },
        { "11", "Mexico, Central America, South America" },
        { "14", "Mexico, Central America, South America" },
        { "16", "Europe / Middle East / Africa" },
        { "18", "Singapore, Korea, Asia" }
    };

    private enum DataFormat
    {
        String,
        Hex
    }

    private readonly byte[] _data = File.ReadAllBytes(path);
    public string Path => path;
    
    private string? NORFormatModelCode => GetData(Offsets.Model, 4, DataFormat.Hex);

    /// <summary>
    /// Gets or sets the edition of the PS5.
    /// </summary>
    public Editions Edition
    {
        get
        {
            return NORFormatModelCode switch
            {
                "22010101" => Editions.Slim,
                "22020101" => Editions.Disc,
                "22030101" => Editions.Digital,
                _ => Editions.Unknown
            };
        }
        set
        {
            if (value == Editions.Unknown)
                return;

            byte[] replace = [0x22, (byte)(value == Editions.Disc ? 0x02 : 0x03), 0x01, 0x01];
            
            Array.Copy(replace, 0, _data, Offsets.Model, replace.Length);
        }
    }
    
    /// <summary>
    /// Gets the Wi-Fi MAC address of the PS5.
    /// </summary>
    public string WiFiMAC
    {
        get
        {
            string? val = GetData(Offsets.WiFiMAC, 6, DataFormat.Hex);
            return val == null
                ? "Unknown"
                : string.Join("", val.Select((c, i) => i % 2 == 0 ? $"{c}" : $"{c}-"))[..^1];
        }
    }
    
    /// <summary>
    /// Gets the MAC address of the PS5's first Ethernet port.
    /// </summary>
    public string Ethernet1MAC 
    {
        get
        {
            string? val = GetData(Offsets.Ethernet1MAC, 6, DataFormat.Hex);
            return val == null
                ? "Unknown"
                : string.Join("", val.Select((c, i) => i % 2 == 0 ? $"{c}" : $"{c}-"))[..^1];
        }
    }
    
    /// <summary>
    /// Gets the MAC address of the PS5's second Ethernet port, if present. Otherwise, FF-FF-FF-FF-FF-FF.
    /// This should be Ethernet1Mac + 1.
    /// </summary>
    public string Ethernet2MAC 
    {
        get
        {
            string? val = GetData(Offsets.Ethernet2MAC, 6, DataFormat.Hex);
            return val == null
                ? "Unknown"
                : string.Join("", val.Select((c, i) => i % 2 == 0 ? $"{c}" : $"{c}-"))[..^1];
        }
    }

    /// <summary>
    /// Gets or sets the product serial number of the PS5.
    /// </summary>
    public string Serial
    {
        get => GetData(Offsets.Serial, 16, DataFormat.String) ?? "Unknown";
        set
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            
            Array.Copy(new byte[16], 0, _data, Offsets.Serial, bytes.Length);
            Array.Copy(bytes, 0, _data, Offsets.Serial, bytes.Length);
        }
    }
    
    /// <summary>
    /// Gets or sets the SKU identifier of the PS5.
    /// </summary>
    public string? SKUModel
    {
        get 
        {
            try
            {
                byte[] variantBytes = _data[Offsets.SKUModel..(Offsets.SKUModel + 19)].Where(b => b != 0xFF).ToArray();
                return Encoding.UTF8.GetString(variantBytes).Split(' ')[0];
            }
            catch
            {
                return null;
            }
        }
        set
        {
            if (value == null)
                return;

            // this is stupid, because i'm almost certain that this doesn't work for all models, but it's what the old
            // code did (or, more accurately, what it *would have* done, had it actually *worked*), and i don't know how
            // to make it right, so i'm leaving it as is for now.
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            Array.Copy(bytes, 0, _data, Offsets.SKUModel + 10, bytes.Length);
        }
    }
    
    /// <summary>
    /// Gets the SKU of the PS5, plus the region, e.g. "CFI-1015A - US, Canada, (North America)".
    /// </summary>
    public string? SKUInfo
    {
        get 
        {
            string? variant = SKUModel;
            return variant == null
                ? null
                : $"{variant} - {Regions.GetValueOrDefault(variant[^3..^1], "Unknown Region")}";
        }
    }
    
    /// <summary>
    /// Gets or sets the serial number of the PS5's motherboard.
    /// </summary>
    public string MoboSerial
    {
        get => GetData(Offsets.MoboSerial, 16, DataFormat.String) ?? "Unknown";
        set
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            
            Array.Copy(new byte[16], 0, _data, Offsets.MoboSerial, bytes.Length);
            Array.Copy(bytes, 0, _data, Offsets.MoboSerial, bytes.Length);
        }
    }
    
    private string? GetData(int offset, int length, DataFormat format)
    {
        try
        {
            byte[] bytes = _data[offset..(offset + length)];
            return format == DataFormat.String
                ? Encoding.UTF8.GetString(bytes)
                : BitConverter.ToString(bytes).Replace("-", null);
        }
        catch
        {
            return null;
        }
    }

    // ReSharper disable once ParameterHidesPrimaryConstructorParameter
    public void Save(string path)
    {
        using FileStream stream = new(path, FileMode.Create);
        stream.Write(_data, 0, _data.Length);
        stream.Close();
    }
}