using System.Text;

namespace PS5_NOR_Modifier;

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
    
    private byte[] _data = File.ReadAllBytes(path);
    // TODO: These two need better names
    private string? One => GetData(Offsets.One, 4, false);

    public string Path => path;
    public Editions Edition
    {
        get
        {
            return One switch
            {
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
            
            Array.Copy(replace, 0, _data, Offsets.One, replace.Length);
        }
    }
    public string WiFiMAC
    {
        get
        {
            string? val = GetData(Offsets.WiFiMAC, 6, false);
            return val == null
                ? "Unknown"
                : string.Join("", val.Select((c, i) => i % 2 == 0 ? $"{c}" : $"{c}-"))[..^1];
        }
    }
    public string LANMAC 
    {
        get
        {
            string? val = GetData(Offsets.LANMAC, 6, false);
            return val == null
                ? "Unknown"
                : string.Join("", val.Select((c, i) => i % 2 == 0 ? $"{c}" : $"{c}-"))[..^1];
        }
    }

    public string Serial
    {
        get => GetData(Offsets.Serial, 16, true) ?? "Unknown";
        set
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            
            Array.Copy(new byte[16], 0, _data, Offsets.Serial, bytes.Length);
            Array.Copy(bytes, 0, _data, Offsets.Serial, bytes.Length);
        }
    }
    public string? VariantCode
    {
        get 
        {
            try
            {
                byte[] variantBytes = _data[Offsets.Variant..(Offsets.Variant + 19)].Where(b => b != 0xFF).ToArray();
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
            Array.Copy(bytes, 0, _data, Offsets.Variant + 10, bytes.Length);
        }
    }
    public string? Variant
    {
        get 
        {
            string? variant = VariantCode;
            return variant == null
                ? null
                : $"{variant} - {Regions.GetValueOrDefault(variant[^3..^1], "Unknown Region")}";
        }
    }
    public string MoboSerial => GetData(Offsets.MoboSerial, 16, true) ?? "Unknown";
        
    private string? GetData(int offset, int length, bool useString)
    {
        try
        {
            byte[] bytes = _data[offset..(offset + length)];
            return useString
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
    
    public enum Editions
    {
        Disc,
        Digital,
        Unknown,
    }
}