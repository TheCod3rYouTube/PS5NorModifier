using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    
    private readonly SerialPort _uartSerial = new();
    
    public MainWindow()
    {
        InitializeComponent();
        RefreshComPorts();
        BoardVariantIn.ItemsSource = _boardVariants;
        PS5ModelIn.ItemsSource = _models;
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

        string edition = _norData.Edition;
        PS5ModelOut.Content = edition;
        PS5ModelIn.SelectedItem = edition;

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
        BoardVariantIn.SelectedItem = _norData.VariantRaw;
    }

    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        string fileNameToLookFor = "";
        bool errorShownAlready = false;
/*
        if (!File.Exists(NORDumpPath.Text))
        {
            ShowError("Please select a valid BIOS file first.");
            errorShownAlready = true;
        }
        else
        {
            if((string)PS5ModelIn.SelectedValue! == "")
            {
                ShowError("Please select a valid board model before saving new BIOS information!");
                errorShownAlready = true;
            }
            else
            {
                if((string)BoardVariantIn.SelectedValue! == "")
                {
                    ShowError("Please select a valid board variant before saving new BIOS information!");
                    errorShownAlready = true;
                }
                else
                {
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

                    if (file != null)
                    {
                        // First create a copy of the old BIOS file
                        byte[] existingFile = await File.ReadAllBytesAsync(NORDumpPath.Text);
                        string newFile = Uri.UnescapeDataString(file.Path.AbsolutePath);

                        await File.WriteAllBytesAsync(newFile, existingFile);

                        fileNameToLookFor = newFile;

                        #region Set the new model info
                        if ((string)PS5ModelOut.Content! == "Disc Edition")
                        {
                            try
                            {
                                if ((string)PS5ModelIn.SelectedItem! == "Digital Edition")
                                {

                                    byte[] find = ConvertHexStringToByteArray("22020101");
                                    byte[] replace = ConvertHexStringToByteArray("22030101");
                                    if (find.Length != replace.Length)
                                    {
                                        ShowError("The length of the old hex value does not match the length of the new hex value!");
                                        errorShownAlready = true;
                                    }
                                    byte[] bytes = await File.ReadAllBytesAsync(newFile);
                                    foreach (int index in PatternAt(bytes, find))
                                    {
                                        for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace.Length; i++, replaceIndex++)
                                        {
                                            bytes[i] = replace[replaceIndex];
                                        }
                                        await File.WriteAllBytesAsync(newFile, bytes);
                                    }
                                }

                            }
                            catch
                            {
                                ShowError("An error occurred while saving your BIOS file");
                                errorShownAlready = true;
                            }
                        }
                        else
                        {
                            if(modelInfo.Text == "Digital Edition")
                            {
                                try
                                {

                                    if (boardModelSelectionBox.Text == "Disc Edition")

                                    {

                                        byte[] find = ConvertHexStringToByteArray(Regex.Replace("22030101", "0x|[ ,]", string.Empty).Normalize().Trim());
                                        byte[] replace = ConvertHexStringToByteArray(Regex.Replace("22020101", "0x|[ ,]", string.Empty).Normalize().Trim());
                                        if (find.Length != replace.Length)
                                        {
                                            ShowError("The length of the old hex value does not match the length of the new hex value!");
                                            errorShownAlready = true;
                                        }
                                        byte[] bytes = File.ReadAllBytes(newFile);
                                        foreach (int index in PatternAt(bytes, find))
                                        {
                                            for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace.Length; i++, replaceIndex++)
                                            {
                                                bytes[i] = replace[replaceIndex];
                                            }
                                            File.WriteAllBytes(newFile, bytes);
                                        }
                                    }

                                }
                                catch
                                {
                                    ShowError("An error occurred while saving your BIOS file");
                                    errorShownAlready = true;
                                }
                            }
                        }
                        #endregion

                        #region Set the new board variant

                        try
                        {
                            byte[] oldVariant = Encoding.UTF8.GetBytes(boardVariant.Text);
                            string oldVariantHex = Convert.ToHexString(oldVariant);

                            byte[] newVariantSelection = Encoding.UTF8.GetBytes(boardVariantSelectionBox.Text);
                            string newVariantHex = Convert.ToHexString(newVariantSelection);

                            byte[] find = ConvertHexStringToByteArray(Regex.Replace(oldVariantHex, "0x|[ ,]", string.Empty).Normalize().Trim());
                            byte[] replace = ConvertHexStringToByteArray(Regex.Replace(newVariantHex, "0x|[ ,]", string.Empty).Normalize().Trim());

                            byte[] bytes = File.ReadAllBytes(newFile);
                            foreach (int index in PatternAt(bytes, find))
                            {
                                for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace.Length; i++, replaceIndex++)
                                {
                                    bytes[i] = replace[replaceIndex];
                                }
                                File.WriteAllBytes(newFile, bytes);
                            }

                        }
                        catch(System.ArgumentException ex)
                        {
                            ShowError(ex.Message.ToString());
                            errorShownAlready = true;
                        }

                        #endregion

                        #region Change Serial Number

                        try
                        {

                            byte[] oldSerial = Encoding.UTF8.GetBytes(serialNumber.Text);
                            string oldSerialHex = Convert.ToHexString(oldSerial);

                            byte[] newSerial = Encoding.UTF8.GetBytes(serialNumberTextbox.Text);
                            string newSerialHex = Convert.ToHexString(newSerial);

                            byte[] find = ConvertHexStringToByteArray(Regex.Replace(oldSerialHex, "0x|[ ,]", string.Empty).Normalize().Trim());
                            byte[] replace = ConvertHexStringToByteArray(Regex.Replace(newSerialHex, "0x|[ ,]", string.Empty).Normalize().Trim());

                            byte[] bytes = File.ReadAllBytes(newFile);
                            foreach (int index in PatternAt(bytes, find))
                            {
                                for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace.Length; i++, replaceIndex++)
                                {
                                    bytes[i] = replace[replaceIndex];
                                }
                                File.WriteAllBytes(newFile, bytes);
                            }

                        }
                        catch (System.ArgumentException ex)
                        {
                            ShowError(ex.Message.ToString());
                            errorShownAlready = true;
                        }

                        #endregion
                    }
                    else
                    {
                        ShowError("Save operation cancelled!");
                        errorShownAlready = true;
                    }
                }
            }
        }

        if (File.Exists(fileNameToLookFor) && errorShownAlready == false)
        {
            // Reset everything and show message
            ResetAppFields();
            MessageBox.Show("A new BIOS file was successfully created. Please load the new BIOS file to verify the information you entered before installing onto your motherboard. Remember this software was created by TheCod3r with nothing but love. Why not show some love back by dropping me a small donation to say thanks ;).", "All done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }*/
    }
}