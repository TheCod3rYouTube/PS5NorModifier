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
    private string? One => GetData(Offsets.One, 12, false);

    private string? Two => GetData(Offsets.Two, 12, false);

    public string Edition
    {
        get
        {
            if (One?.Contains("22020101") ?? false)
                return "Disc Edition";
            
            if (Two?.Contains("22030101") ?? false)
                return "Digital Edition";

            return "Unknown";
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
    public string Serial => GetData(Offsets.Serial, 16, true) ?? "Unknown";
    public string? VariantRaw
    {
        get 
        {
            try
            {
                byte[] variantBytes = _data[Offsets.Variant..(Offsets.Variant + 19)].Where(b => b != 0xFF).ToArray();
                return Encoding.ASCII.GetString(variantBytes);
            }
            catch
            {
                return null;
            }
        }
    }
    public string Variant
    {
        get 
        {
            string? variant = VariantRaw;
            return variant == null
                ? "Unknown"
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
                ? Encoding.ASCII.GetString(bytes)
                : BitConverter.ToString(bytes).Replace("-", null);
        }
        catch
        {
            return null;
        }
    }
}