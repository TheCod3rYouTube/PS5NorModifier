using System.IO.Ports;
using Avalonia.Interactivity;

using System.Linq.Expressions;
using System.Xml;
using DialogHostAvalonia;

namespace PS5NORModifier;

public partial class MainWindow
{
    private readonly SerialPort _uartSerial = new();
    
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
            List<string> uartLines = [];

            string checksum = CalculateChecksum(CustomCommand.Text);
            _uartSerial.WriteLine(checksum);
            do
            {
                string? line = _uartSerial.ReadLine();
                if (!string.Equals($"{CustomCommand.Text}:{checksum}", line, StringComparison.InvariantCultureIgnoreCase))
                {
                    uartLines.Add(line);
                }
            } while (_uartSerial.BytesToRead != 0);

            foreach (string l in uartLines)
            {
                string[] split = l.Split(' ');
                if (split.Length == 0) continue;
                
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

    private void ConnectComButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var selectedPort = (string?)ComPorts.SelectedItem;
        if (string.IsNullOrEmpty(selectedPort))
        {
            ShowError("Please select a COM port from the ports list to establish a connection.");
            StatusLabel.Content = "Could not connect to UART. Please try again!";
            return;
        }

        try
        {
            _uartSerial.PortName = selectedPort;
            _uartSerial.BaudRate = 115200;
            _uartSerial.RtsEnable = true;
            _uartSerial.Open();
            
            DisconnectComButton.IsEnabled = true;
            ConnectComButton.IsEnabled = false;
            StatusLabel.Content = $"Connected to UART via COM port {selectedPort} at a BAUD rate of 115200.";
        }
        catch (Exception ex)
        {
            ShowError(ex.ToString());
            StatusLabel.Content = "Could not connect to UART. Please try again!";
        }
    }

    private void DisconnectComButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (!_uartSerial.IsOpen)
                return;
            
            _uartSerial.Close();
            ConnectComButton.IsEnabled = true;
            DisconnectComButton.IsEnabled = false;
            StatusLabel.Content = "Disconnected from UART.";
        }
        catch(Exception ex)
        {
            ShowError(ex.ToString());
            StatusLabel.Content = "An error occurred while disconnecting from UART. Please try again.";
        }
    }
    private async void GetErrorCodesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_uartSerial.IsOpen != true)
        {
            ShowError("Please connect to UART before attempting to read the error codes.");
            return;
        }
        
        try
        {
            List<string> uartLines = [];

            for (var i = 0; i <= 10; i++)
            {
                var command = $"errlog {i}";
                string checksum = CalculateChecksum(command);
                _uartSerial.WriteLine(checksum);
                do
                {
                    string? line = _uartSerial.ReadLine();
                    if (!string.Equals($"{command}:{checksum}", line, StringComparison.InvariantCultureIgnoreCase))
                    {
                        uartLines.Add(line);
                    }
                } while (_uartSerial.BytesToRead != 0);

                foreach (string[] split in uartLines.Select(l => l.Split(' ')).Where(split => split.Length != 0))
                {
                    switch (split[0])
                    {
                        case "NG":
                            break;
                        case "OK":
                            string errorCode = split[2];
                            string errorResult = UseOfflineDB.IsChecked == true
                                ? ParseErrorsOffline(errorCode)
                                : await ParseErrorsOnline(errorCode);
                            
                            OutputText.Text += errorResult + Environment.NewLine;
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.ToString());
            StatusLabel.Content = "An error occurred while reading error codes from UART. Please try again.";
        }
    }
    
    private static async Task<string> ParseErrorsOnline(string error)
    {
        string url = "https://uartcodes.com/xml.php?errorCode=" + error;

        try
        {
            string response;
            using (HttpClient client = new())
            {
                response = await client.GetStringAsync(url);
            }
            
            XmlDocument xmlDoc = new();
            xmlDoc.LoadXml(response);
            
            XmlNode? description = xmlDoc.SelectSingleNode("errorCodes/errorCode/Description");
            if (description == null)
            {
                return $"Error code: {error}\n" +
                       "An error occurred while fetching a result for this error. Please try again!";
            }
            
            return $"Error code: {error}\n" +
                   $"Description: {description.InnerText}";
        }
        catch (Exception ex)
        {
            return $"Error code: {error}\n" +
                   $"{ex}";
        }
    }
    
    private static string ParseErrorsOffline(string errorCode)
    {
        try
        {
            if (!File.Exists("errorDB.xml"))
                return "Error: Local XML file not found.";
            
            XmlDocument xmlDoc = new();
            xmlDoc.Load("errorDB.xml");

            XmlNode? errorNode = xmlDoc.SelectSingleNode($"//errorCode[ErrorCode='{errorCode}']");
            XmlNode? descriptionNode = errorNode?.SelectSingleNode("Description");
            
            if (descriptionNode == null)
                return "Error: Invalid XML database file. Please re-download the offline database or use the online database.";
            
            string description = descriptionNode.InnerText;
            return $"Error code: {errorCode}\n" +
                   $"Description: {description}";
        }
        catch (Exception ex)
        {
            return $"Error: {ex}\n" +
                   "Is the database present and valid?";
        }
    }

    private static string CalculateChecksum(string str)
    {
        int sum = str.Aggregate(0, (current, c) => current + c);
        return str + ":" + (sum & 0xFF).ToString("X2");
    }

    private async void ClearErrorCodesButton_OnClick(object? sender, RoutedEventArgs e)
    {
        //DialogResult result = MessageBox.Show(, , MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        var result = (string?)await DialogHost.Show(new DialogContents(Dialog, 
            "This will clear error codes from the console by sending the \"errlog clear\" command." +
            "Are you sure you would like to proceed? This action cannot be undone!",
            "Are you sure?", 
            "No", "Yes"),
            Dialog);
        
        if (result != "Yes")
            return;

        if (!_uartSerial.IsOpen)
        {
            ShowError("Please connect to UART before attempting to send commands.");
            return;
        }

        try
        {
            List<string> uartLines = [];

            const string command = "errlog clear";
            
            string checksum = CalculateChecksum(command);
            
            _uartSerial.WriteLine(checksum);
            do
            {
                string? line = _uartSerial.ReadLine();
                if (!string.Equals($"{command}:{checksum}", line, StringComparison.InvariantCultureIgnoreCase))
                {
                    uartLines.Add(line);
                }
            } while (_uartSerial.BytesToRead != 0);

            foreach (string[] split in uartLines.Select(l => l.Split(' ')).Where(split => split.Length != 0))
            {
                switch (split[0])
                {
                    case "NG":
                        OutputText.Text += "Response: FAIL" + Environment.NewLine +
                                           "Information: An error occurred while clearing the error logs from the system. Please try again...";
                        
                        break;
                    case "OK":
                        OutputText.Text += "Response: SUCCESS" + Environment.NewLine +
                                           "Information: All error codes cleared successfully";

                        break;
                }
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.ToString());
            StatusLabel.Content = "An error occurred while attempting to send a UART command. Please try again.";
        }
    }

    private async void DownloadErrorDBButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var result = (string?)await DialogHost.Show(new DialogContents(Dialog, 
                "Downloading the error database will overwrite any existing offline database you currently have. Are you sure you would like to do this?",
                "Are you sure?",
                "No", "Yes"),
            Dialog);

        if (result != "Yes")
            return;
        
        const string url = "https://uartcodes.com/xml.php";

        try
        {
            string xmlData;
            using (HttpClient client = new())
            {
                xmlData = await client.GetStringAsync(url);
            }
            
            XmlDocument xmlDoc = new();
            xmlDoc.LoadXml(xmlData);

            xmlDoc.Save("errorDB.xml");

            await DialogHost.Show(new DialogContents(Dialog,
                "The offline database has been updated successfully.", 
                "Offline Database Updated!", 
                "OK"));
        }
        catch (Exception ex)
        {
            ShowError(ex.ToString());
            StatusLabel.Content = "An error occurred while downloading the offline database. Please try again.";
        }
    }
}