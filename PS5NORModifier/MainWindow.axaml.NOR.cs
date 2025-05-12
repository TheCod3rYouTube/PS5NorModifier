using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using UARTLib;

namespace PS5NORModifier;

public partial class MainWindow
{
    private NORData? _norData;
    
    private readonly string[] _models =
    [
        "Disc Edition",
        "Digital Edition"
    ];
    
    private readonly List<string> _boardVariants =
    [
        "CFI-1000A",
        "CFI-1000A01",
        "CFI-1000B",
        "CFI-1002A",
        "CFI-1008A",
        "CFI-1014A",
        "CFI-1015A",
        "CFI-1015B",
        "CFI-1016A",
        "CFI-1018A",
        "CFI-1100A01",
        "CFI-1102A",
        "CFI-1108A",
        "CFI-1109A",
        "CFI-1114A",
        "CFI-1115A",
        "CFI-1116A",
        "CFI-1118A",
        "CFI-1208A",
        "CFI-1215A",
        "CFI-1216A",
        "DFI-T1000AA",
        "DFI-D1000AA"
    ];
    
    private async void BrowseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new()
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
        string norPath =  Uri.UnescapeDataString(file.Path.AbsolutePath);
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
        FileSizeOut.Content = length + " bytes (" + length / 1024 / 1024 + " MiB)";

        _norData = new(norPath);

        Editions edition = _norData.Edition;
        PS5ModelOut.Content = edition + " Edition";
        PS5ModelIn.SelectedItem = edition == Editions.Unknown ? null : edition + " Edition";

        string serial = _norData.Serial;
        SerialNumberOut.Content = serial;
        SerialNumberIn.Text = serial;

        string wifiMac = _norData.WiFiMAC;
        WiFiMACAddressOut.Content = wifiMac;
        WiFiMACAddressIn.Text = wifiMac;

        string lanMac = _norData.LANMAC;
        LANMACAddressOut.Content = lanMac;
        LANMACAddressIn.Text = lanMac;
        
        MotherboardSerialOut.Content = _norData.MoboSerial;
        
        BoardVariantOut.Content = _norData.Variant;
        BoardVariantIn.SelectedValue = _norData.VariantCode;
    }

    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!File.Exists(_norData?.Path))
        {
            ShowError("Please select a valid BIOS file first.");
            return;
        }
        if (PS5ModelIn.SelectedValue == null)
        {
            ShowError("Please select a valid board model before saving new BIOS information!");
            return;
        }
        if (BoardVariantIn.SelectedValue == null)
        {
            ShowError("Please select a valid board variant before saving new BIOS information!");
            return;
        }
        if (string.IsNullOrEmpty(SerialNumberIn.Text))
        {
            ShowError("Please enter a valid serial number before saving new BIOS information!");
            return;
        }

        IStorageFile? file = await StorageProvider.SaveFilePickerAsync(new()
        {
            Title = "Save NOR BIN File",
            SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(NORDumpPath.Text),
            SuggestedFileName = "bios.bin",
            FileTypeChoices =
            [
                new("PS5 BIN Files")
                {
                    Patterns = [ "*.bin" ]
                }
            ]
        });

        if (file == null)
            return;

        string filePath = Uri.UnescapeDataString(file.Path.AbsolutePath);

        _norData.Edition = (Editions)PS5ModelIn.SelectedIndex;
        _norData.VariantCode = BoardVariantIn.SelectedValue.ToString() ?? null;
        _norData.Serial = SerialNumberIn.Text;
        
        _norData.Save(filePath);
    }
}