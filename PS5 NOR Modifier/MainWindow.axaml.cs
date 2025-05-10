using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using DialogHostAvalonia;
using Label = Avalonia.Controls.Label;

namespace PS5_NOR_Modifier;

public partial class MainWindow : Window
{
    private readonly Dictionary<string, string> Regions = new()
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
    
    private readonly SerialPort _uartSerial = new();
    private readonly NORData _norData = new();
    
    public MainWindow()
    {
        InitializeComponent();
        RefreshComPorts();
    }

    private void RefreshComPorts()
    {
        string[] ports = SerialPort.GetPortNames();
        if (ports == null || ports.Length == 0)
        {
            ShowError("No available COM ports were detected.");
            return;
        }
        
        ComPorts.Items.Clear();
        foreach (string port in ports)
        {
            ComPorts.Items.Add(port);
        }
        ComPorts.SelectedIndex = 0;
        ConnectComButton.IsEnabled = true;
        DisconnectComButton.IsEnabled = false;
    }

    private void SponsorLink_OnClick(object? sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://www.consolefix.shop")
        {
            UseShellExecute = true
        });
    }

    private void ClearOutputButton_OnClick(object? sender, RoutedEventArgs e)
    {
        OutputText.Text = "";
    }

    private void SendCustomCommandButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(CustomCommand.Text))
        {
            ShowError("Please enter a command to send via UART.");
            return;
        }

        if (!_uartSerial.IsOpen)
        {
            ShowError("Please connect to UART before attempting to send commands.");
            return;
        }
        
        try
        {
            List<string> UARTLines = [];

            string checksum = CalculateChecksum(CustomCommand.Text);
            _uartSerial.WriteLine(checksum);
            do
            {
                string? line = _uartSerial.ReadLine();
                if (!string.Equals($"{CustomCommand.Text}:{checksum}", line, StringComparison.InvariantCultureIgnoreCase))
                {
                    UARTLines.Add(line);
                }
            } while (_uartSerial.BytesToRead != 0);

            foreach (string l in UARTLines)
            {
                string[] split = l.Split(' ');
                if (!split.Any()) continue;
                
                OutputText.Text = split[0] switch
                {
                    "NG" => "ERROR: " + l,
                    "OK" => "SUCCESS: " + l,
                    _ => OutputText.Text
                };
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.ToString());
            StatusLabel.Content = "An error occurred while reading error codes from UART. Please try again...";
        }
    }

    private static string CalculateChecksum(string str)
    {
        int sum = str.Aggregate(0, (current, c) => current + c);
        return str + ":" + (sum & 0xFF).ToString("X2");
    }

    private void ShowError(string error)
    {
        DialogHost.Show(new DialogContents(Dialog, error, "An error occurred.", "OK"), Dialog);
    }

    private void RefreshComButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RefreshComPorts();
    }

    
    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);

        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new()
        {
            Title = "Open NOR BIN File",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new("PS5 BIN Files")
                {
                    Patterns = new List<string> { "*.bin" }
                }
            }
        });
        
        if (files.Count == 0)
            return;
        
        IStorageFile file = files[0];
        string norPath = file.Path.AbsolutePath;
        if (!File.Exists(norPath))
        {
            ShowError("The file you selected could not be found. Please check the file exists and is a valid BIN file.");
            return;
        }

        if(!file.Name.EndsWith(".bin", StringComparison.InvariantCultureIgnoreCase))
        {
            ShowError("You did not select a .bin file. Please ensure the file you are choosing is a correct BIN file and try again.");
            return;
        }

        StatusLabel.Content = "Status: Selected file " + norPath;
        NORDumpPath.Text = norPath;

        long length = new FileInfo(norPath).Length;
        FileSizeOut.Content = length + " bytes (" + length / 1024 / 1024 + "MB)";

        #region Extract PS5 Version

        SetData(norPath, Offsets.One, 12, ref _norData.OffsetOne, out _);
        SetData(norPath, Offsets.Two, 12, ref _norData.OffsetTwo, out _);
                        
        if(_norData.OffsetOne?.Contains("22020101") ?? false)
        {
            PS5ModelOut.Content = "Disc Edition";
        }
        else
        {
            if(_norData.OffsetTwo?.Contains("22030101") ?? false)
            {
                PS5ModelOut.Content = "Digital Edition";
            }
            else
            {
                PS5ModelOut.Content = "Unknown";
            }
        }

        #endregion

        #region Extract Motherboard Serial Number

        SetData(norPath, Offsets.MoboSerial, 16, ref _norData.MoboSerial, out string moboSerialText);

        MotherboardSerialOut.Content = _norData.MoboSerial != null
            ? moboSerialText
            : "Unknown";

        #endregion

        #region Extract Board Serial Number

        SetData(norPath, Offsets.Serial, 17, ref _norData.Serial, out string serialText);
        
        if (_norData.Serial != null)
        {
            SerialNumberOut.Content = serialText;
            SerialNumberIn.Text = serialText;
        }
        else
        {
            SerialNumberOut.Content = "Unknown";
            SerialNumberIn.Text = "";
        }

        #endregion

        #region Extract WiFi Mac Address

        SetData(norPath, Offsets.WiFiMAC, 6, ref _norData.WiFiMAC, out _);
        if (_norData.WiFiMAC != null)
            _norData.WiFiMAC = string.Join("", _norData.WiFiMAC.Select((c, i) => i % 2 == 0 ? $"{c}" : $"{c}-"))[..^1];

        if (_norData.WiFiMAC != null)
        {
            WiFiMACAddressOut.Content = _norData.WiFiMAC;
            WiFiMACAddressIn.Text = _norData.WiFiMAC;
        }
        else
        {
            WiFiMACAddressOut.Content = "Unknown";
            WiFiMACAddressIn.Text = "";
        }

        #endregion

        #region Extract LAN Mac Address

        SetData(norPath, Offsets.LANMAC, 6, ref _norData.LANMAC, out _);
        if (_norData.LANMAC != null)
            _norData.LANMAC = string.Join("", _norData.LANMAC.Select((c, i) => i % 2 == 0 ? $"{c}" : $"{c}-"))[..^1];

        if (_norData.LANMAC != null)
        {
            LANMACAddressOut.Content = _norData.LANMAC;
            LANMACAddressIn.Text = _norData.LANMAC;
        }
        else
        {
            LANMACAddressOut.Content = "Unknown";
            LANMACAddressIn.Text = "";
        }

        #endregion

        #region Extract Board Variant
        
        SetData(norPath, Offsets.Variant, 19, ref _norData.Variant, out string variantText);
        if (_norData.Variant != null)
            _norData.Variant = _norData.Variant.Replace("FF", null);

        variantText += " - " + Regions.GetValueOrDefault(variantText[^3..^1], "Unknown Region");

        BoardVariantOut.Content = _norData.Variant != null ? variantText : "Unknown";

        #endregion

        return;
        
        void SetData(string path, long offset, int bytes, ref string? dataValue, out string outputText)
        {
            try
            {
                BinaryReader reader = new(new FileStream(path, FileMode.Open));
                reader.BaseStream.Position = offset;

                byte[] dataBytes = reader.ReadBytes(bytes);
                dataValue = BitConverter.ToString(dataBytes).Replace("-", null);
                outputText = Encoding.ASCII.GetString(dataBytes);

                reader.Close();
            }
            catch
            {
                dataValue = null;
                outputText = "";
            }
        }
    }
}