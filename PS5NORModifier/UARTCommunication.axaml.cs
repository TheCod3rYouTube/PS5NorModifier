using System.IO.Ports;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DialogHostAvalonia;
using PS5Lib;

namespace PS5NORModifier;

public partial class UARTCommunication : UserControl
{
    private readonly UART _uart;
    public UARTCommunication()
    {
        InitializeComponent();
        _uart = new();
    }
    
    internal void RefreshComPorts()
    {
        var mainWindow = MainWindow.Instance;
        string[] ports = SerialPort.GetPortNames();
        if (ports == null || ports.Length == 0)
        {
            mainWindow.ShowError("No available COM ports were detected.");
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
    
    private void RefreshComButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RefreshComPorts();
    }

    private void ClearOutputButton_OnClick(object? sender, RoutedEventArgs e)
    {
        OutputText.Text = "";
    }

    private void SendCustomCommandButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var mainWindow = MainWindow.Instance;
        if (string.IsNullOrEmpty(CustomCommand.Text))
        {
            mainWindow.ShowError("Please enter a command to send via UART.");
            return;
        }

        bool success = _uart.SendArbitraryCommand(CustomCommand.Text, out string result);
        
        if (success)
        {
            OutputText.Text += result + Environment.NewLine;
        }
        else
        {
            mainWindow.ShowError(result);
            mainWindow.SetStatus("An error occurred while sending the command. Please try again.");
        }
    }

    private void ConnectComButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var mainWindow = MainWindow.Instance;
        var selectedPort = (string?)ComPorts.SelectedItem;
        if (string.IsNullOrEmpty(selectedPort))
        {
            mainWindow.ShowError("Please select a COM port from the ports list to establish a connection.");
            mainWindow.SetStatus("Could not connect to UART. Please try again!");
            return;
        }

        try
        {
            _uart.Connect(selectedPort);
            
            DisconnectComButton.IsEnabled = true;
            ConnectComButton.IsEnabled = false;
            mainWindow.SetStatus($"Connected to UART via COM port {selectedPort} at a BAUD rate of 115200.");
        }
        catch (Exception ex)
        {
            mainWindow.ShowError(ex.ToString());
            mainWindow.SetStatus("Could not connect to UART. Please try again!");
        }
    }

    private void DisconnectComButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var mainWindow = MainWindow.Instance;
        try
        {
            _uart.Disconnect();
            
            ConnectComButton.IsEnabled = true;
            DisconnectComButton.IsEnabled = false;
            mainWindow.SetStatus("Disconnected from UART.");
        }
        catch(Exception ex)
        {
            mainWindow.ShowError(ex.ToString());
            mainWindow.SetStatus("An error occurred while disconnecting from UART. Please try again.");
        }
    }
    private async void GetErrorCodesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var mainWindow = MainWindow.Instance;
        for (int i = 0; i <= 10; i++)
        {
            var result = await _uart.GetErrorLog(i);
            
            if (!result.success)
            {
                mainWindow.ShowError(string.Join(Environment.NewLine, result.errors));
                mainWindow.SetStatus($"An error occurred while reading error codes from UART, at page {i}. Please try again.");
                return;
            }
            
            OutputText.Text += result.errors + Environment.NewLine;
        }
    }

    private async void ClearErrorCodesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var mainWindow = MainWindow.Instance;
        DialogHost dialog = mainWindow.Dialog;
        var response = (string?)await DialogHost.Show(new DialogContents(dialog, 
            "This will clear error codes from the console by sending the \"errlog clear\" command." +
            "Are you sure you would like to proceed? This action cannot be undone!",
            "Are you sure?", 
            "No", "Yes"),
            dialog);
        
        if (response != "Yes")
            return;

        bool success = _uart.ClearErrorLog(out string result);
        if (success)
        {
            OutputText.Text += result + Environment.NewLine;
            mainWindow.SetStatus("Error codes cleared successfully.");
        }
        else
        {
            mainWindow.ShowError(result);
            mainWindow.SetStatus("An error occurred while clearing error codes. Please try again.");
        }
    }

    private async void DownloadErrorDBButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var mainWindow = MainWindow.Instance;
        DialogHost dialog = mainWindow.Dialog;
        var result = (string?)await DialogHost.Show(new DialogContents(dialog, 
                "Downloading the error database will overwrite any existing offline database you currently have. Are you sure you would like to do this?",
                "Are you sure?",
                "No", "Yes"),
            dialog);

        if (result != "Yes")
            return;
        
        try
        {
            await UART.DownloadErrorDB();

            await DialogHost.Show(new DialogContents(dialog,
                "The offline database has been updated successfully.", 
                "Offline Database Updated!", 
                "OK"));
        }
        catch (Exception ex)
        {
            mainWindow.ShowError(ex.ToString());
            mainWindow.SetStatus("An error occurred while downloading the offline database. Please try again.");
        }
    }
}