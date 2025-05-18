using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Xml;

namespace PS5_NOR_Modifier;

public sealed partial class MainForm : Form
{
    // We want this app to work offline, so let's declare where the local "offline" database will be stored
    private const string localDatabaseFile = "errorDB.xml";

    private static readonly SerialPort UARTSerial = new ();

    private readonly CancellationTokenSource errorsCTSource = new ();

    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        // Upon first launch, we need to get a list of COM ports available for UART
        string[] ports = SerialPort.GetPortNames();
        comboComPorts.Items.Clear();
        comboComPorts.Items.AddRange(ports);
        comboComPorts.SelectedIndex = 0;
        btnConnectCom.Enabled = true;
        btnDisconnectCom.Enabled = false;
    }

    private async Task DownloadDatabaseAsync(CancellationToken cancellationToken)
    {
        // Define the URL
        string url = "http://uartcodes.com/xml.php"; // Update with your URL

        // Define the file path to save the XML

        try
        {
            // Create a WebClient instance
            using (HttpClient client = new())
            {
                // Download the XML data from the URL
                string xmlData = await client.GetStringAsync(url, cancellationToken)
                    .ConfigureAwait(false);

                // Create an XmlDocument instance and load the XML data
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlData);

                // Save the XML data to a file
                xmlDoc.Save(localDatabaseFile);

                MessageBox.Show("The most recent offline database has been updated successfully.", "Offline Database Updated!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    /// <summary>
    /// We need to be able to send the error code we received from the console and fetch an XML result back from the server
    /// Once we have a result from the server, parse the XML data and output it in an easy to understand format for the user
    /// </summary>
    /// <param name="ErrorCode"></param>
    /// <returns></returns>
    private async Task<string> ParseErrorsAsync(string ErrorCode, CancellationToken cancellationToken)
    {
        // If the user has opted to parse errors with an offline database, run the parse offline function
        if (chkUseOffline.Checked == true)
        {
            return ParseErrorsOffline(ErrorCode);
        }
        else
        {
            // The user wants to use the online version. Proceed at will

            // Define the URL with the error code parameter
            string url = "http://uartcodes.com/xml.php?errorCode=" + ErrorCode;

            string results = string.Empty;

            try
            {
                string response = string.Empty;
                // Create a WebClient instance to send the request
                using (HttpClient client = new())
                {
                    // Send the request and retrieve the response as a string
                    response = await client.GetStringAsync(url, cancellationToken)
                        .ConfigureAwait(false);
                }
                // Load the XML response into an XmlDocument
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response);


                // Get the root node
                XmlNode? root = xmlDoc.DocumentElement;
                if (root is null)
                {
                    throw new Exception("Error reading the file");
                }

                // Check if the root node is <errorCodes>
                if (root.Name == "errorCodes")
                {
                    // Loop through each errorCode node
                    foreach (XmlNode errorCodeNode in root.ChildNodes)
                    {
                        // Check if the node is <errorCode>
                        if (errorCodeNode.Name == "errorCode")
                        {
                            // Get ErrorCode and Description
                            string errorCode = errorCodeNode.SelectSingleNode("ErrorCode")?.InnerText ?? string.Empty;
                            string description = errorCodeNode.SelectSingleNode("Description")?.InnerText ?? string.Empty;

                            // Output the results
                            results = "Error code: "
                                + errorCode
                                + Environment.NewLine
                                + "Description: "
                                + description;
                        }
                    }
                }
                else
                {
                    results = "Error code: "
                                + ErrorCode
                                + Environment.NewLine
                                + "An error occurred while fetching a result for this error. Please try again!";
                }
            }
            catch (Exception ex)
            {
                results = "Error code: "
                    + ErrorCode
                    + Environment.NewLine
                    + ex.Message;
            }
            return results;
        }
    }

    private string ParseErrorsOffline(string errorCode)
    {
        string results = string.Empty;

        try
        {
            // Check if the XML file exists
            if (!File.Exists(localDatabaseFile))
            {
                results = "Error: Local XML file not found.";
                return results;
            }

            // Load the XML file
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(localDatabaseFile);

            // Get the root node
            XmlNode? root = xmlDoc.DocumentElement;
            if (root is null) return results;

            // Check if the root node is <errorCodes>
            if (root.Name != "errorCodes")
            {
                results = "Error: Invalid XML database file. Please reconfigure the application, redownload the offline database, or uncheck the option to use the offline database.";
                return results;
            }

            // Loop through each errorCode node
            foreach (XmlNode errorCodeNode in root.ChildNodes)
            {
                // Check if the node is <errorCode>
                if (errorCodeNode.Name == "errorCode")
                {
                    // Get ErrorCode and Description
                    string errorCodeValue = errorCodeNode.SelectSingleNode("ErrorCode")?.InnerText ?? string.Empty;
                    string description = errorCodeNode.SelectSingleNode("Description")?.InnerText ?? string.Empty;

                    // Check if the current error code matches the requested error code
                    if (errorCodeValue == errorCode)
                    {
                        // Output the results
                        results = "Error code: " + errorCodeValue + Environment.NewLine + "Description: " + description;
                        break; // Exit the loop after finding the matching error code
                    }
                }
            }

        }
        catch (Exception ex)
        {
            results = "Error: " + ex.Message;
        }

        return results;
    }

    /// <summary>
    /// Lauinches a URL in a new window using the default browser...
    /// </summary>
    /// <param name="url">The URL you want to launch</param>
    private void OpenUrl(string url)
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

    private void ResetAppFields()
    {
        fileLocationBox.Text = string.Empty;
        serialNumber.Text = "...";
        boardVariant.Text = "...";
        modelInfo.Text = "...";
        fileSizeInfo.Text = "...";
        serialNumberTextbox.Text = string.Empty;
        toolStripStatusLabel.Text = "Status: Waiting for input";
    }

    #region Donations

    /// <summary>
    /// If you modify this app, please leave my credits in, otherwise a little kitten will cry!
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void donateInfoLabel_Click(object sender, EventArgs e)
    {
        OpenUrl("https://www.streamelements.com/thecod3r/tip");
    }

    private void donateImageButton_Click(object sender, EventArgs e)
    {
        OpenUrl("https://www.streamelements.com/thecod3r/tip");
    }


    #endregion


    private void browseFileButton_Click(object sender, EventArgs e)
    {
        Utilities.TryCatchErrors(() =>
        {
            OpenFileDialog fileDialogBox = new OpenFileDialog();
            fileDialogBox.Title = "Open NOR BIN File";
            fileDialogBox.Filter = "PS5 BIN Files|*.bin";

            if (fileDialogBox.ShowDialog() != DialogResult.OK)
                return;

            if (!fileDialogBox.CheckFileExists)
                throw new Exception("The file you selected could not be found. Please check the file exists and is a valid BIN file.");

            if (!fileDialogBox.SafeFileName.EndsWith(".bin"))
                throw new Exception("The file you selected is not a valid. Please ensure the file you are choosing is a correct BIN file and try again.");

            // Let's load simple information first, before loading BIN specific data
            fileLocationBox.Text = string.Empty;

            // Get the path selected and print it into the path box
            string selectedPath = fileDialogBox.FileName;
            toolStripStatusLabel.Text = "Status: Selected file " + selectedPath;
            fileLocationBox.Text = selectedPath;

            // Get file length and show in bytes and MB
            long length = new FileInfo(selectedPath).Length;
            fileSizeInfo.Text = length.ToString() + " bytes (" + length / 1024 / 1024 + "MB)";

            var binaryReader = new BinaryReader(new FileStream(fileDialogBox.FileName, FileMode.Open));

            modelInfo.Text = Utilities.ExtractPS5Version(binaryReader);
            moboSerialInfo.Text = Utilities.ExtractMotherboardSerialNumber(binaryReader);

            string breakfastSerial = Utilities.ExtractBoardSerialNumber(binaryReader);
            serialNumber.Text = breakfastSerial;
            serialNumberTextbox.Text = breakfastSerial;

            string macMillersAddress = Utilities.ExtractWiFiMacAddress(binaryReader);
            macAddressInfo.Text = macMillersAddress;
            wifiMacAddressTextbox.Text = macMillersAddress != "Unknown" ? macMillersAddress : string.Empty;

            string lanAddress = Utilities.ExtractLANMacAddress(binaryReader);
            LANMacAddressInfo.Text = lanAddress;
            lanMacAddressTextbox.Text = lanAddress != "Unknown" ? lanAddress : string.Empty;

            boardVariant.Text = Utilities.ExtractBoardVariant(binaryReader, closeReader: true);
        });
    }

    private void convertToDigitalEditionButton_Click(object sender, EventArgs e)
    {
        Utilities.TryCatchErrors(() =>
        {
            string fileNameToLookFor = string.Empty;

            if (modelInfo.Text == string.Empty || modelInfo.Text == "...")
                // No valid BIN file seems to have been selected
                throw new Exception("Please select a valid BIOS file first...");


            if (boardModelSelectionBox.Text == string.Empty)
                throw new Exception("Please select a valid board model before saving new BIOS information!");


            if (boardVariantSelectionBox.Text == string.Empty)
                throw new Exception("Please select a valid board variant before saving new BIOS information!");

            SaveFileDialog saveBox = new SaveFileDialog();
            saveBox.Title = "Save NOR BIN File";
            saveBox.Filter = "PS5 BIN Files|*.bin";

            if (saveBox.ShowDialog() != DialogResult.OK)
                throw new Exception("Save operation cancelled!");

            // First create a copy of the old BIOS file
            byte[] existingFile = File.ReadAllBytes(fileLocationBox.Text);
            string newFile = saveBox.FileName;

            File.WriteAllBytes(newFile, existingFile);

            fileNameToLookFor = saveBox.FileName;

            if (!Utilities.PlaystationModelLookup.Keys.Contains(modelInfo.Text))
                throw new Exception("Unknown Console Model Type!");

            if (!Utilities.PlaystationModelLookup.Keys.Contains(boardModelSelectionBox.Text))
                throw new Exception("Unknown Console Model Type!");

            Utilities.SetNewModelInfo(fileNameToLookFor, Utilities.PlaystationModelLookup[modelInfo.Text], Utilities.PlaystationModelLookup[boardModelSelectionBox.Text]);
            Utilities.SetBoardVariant(fileNameToLookFor, boardVariant.Text, boardModelSelectionBox.Text);
            Utilities.ChangeSerialNumber(fileNameToLookFor, serialNumber.Text, serialNumberInputLabel.Text);

            if (!File.Exists(fileNameToLookFor))
                throw new Exception("Failed to write new information to file!");

            // Reset everything and show message
            ResetAppFields();
            MessageBox.Show
                (
                    "A new BIOS file was successfully created. Please load the new BIOS file to verify the information you entered before installing onto your motherboard. " + 
                    "Remember this software was created by TheCod3r with nothing but love. " +
                    "Why not show some love back by dropping me a small donation to say thanks ;).", 
                    "All done!", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information
                );
        });
    }

    private void sponsorLabel_Click(object sender, EventArgs e)
    {
        OpenUrl("https://www.consolefix.shop");
    }

    private void btnRefreshPorts_Click(object sender, EventArgs e)
    {
        // When the "refresh ports" button is pressed, we need to refresh the list of available COM ports for UART
        string[] ports = SerialPort.GetPortNames();
        comboComPorts.Items.Clear();
        comboComPorts.Items.AddRange(ports);
        comboComPorts.SelectedIndex = 0;
        btnConnectCom.Enabled = true;
        btnDisconnectCom.Enabled = false;
    }

    private void btnConnectCom_Click(object sender, EventArgs e)
    {
        // Let's close the COM port
        Utilities.TryCatchErrors(() =>
        {
            // Let's try and connect to the UART reader
            btnConnectCom.Enabled = false;

            if (comboComPorts.Text == string.Empty)
                throw new Exception("Please select a COM port from the ports list to establish a connection.");

            // Set port to selected port
            UARTSerial.PortName = comboComPorts.Text;
            // Set the BAUD rate to 115200
            UARTSerial.BaudRate = 115200;
            // Enable RTS
            UARTSerial.RtsEnable = true;

            // Open the COM port
            UARTSerial.Open();
            btnDisconnectCom.Enabled = true;
            toolStripStatusLabel.Text = "Connected to UART via COM port " + comboComPorts.Text + " at a BAUD rate of 115200.";
        },

        (_) => 
        {
            btnConnectCom.Enabled = true;
            btnDisconnectCom.Enabled = false;
            toolStripStatusLabel.Text = "Could not connect to UART. Please try again!";
        });

    }

    private void btnDisconnectCom_Click(object sender, EventArgs e)
    {
        // Let's close the COM port
        Utilities.TryCatchErrors(() =>
        {
            if (!UARTSerial.IsOpen)
                throw new Exception("Please connect to UART before attempting to read the error codes.");

            UARTSerial.Close();
            btnConnectCom.Enabled = true;
            btnDisconnectCom.Enabled = false;
            toolStripStatusLabel.Text = "Disconnected from UART...";
        },

        (_) => toolStripStatusLabel.Text = "An error occurred while disconnecting from UART. Please try again...");
    }

    /// <summary>
    /// Read error codes from UART
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnGetErrorCodes_Click(object sender, EventArgs e)
    {
        _ = Utilities.TryCatchErrorsAsync
            (
                GetErrorCodesAsync(errorsCTSource.Token),
                (Exception ex) => toolStripStatusLabel.Text = "An error occurred while reading error codes from UART. Please try again..."
            );
    }

    private async Task GetErrorCodesAsync(CancellationToken cancellationToken)
    {
        // Let's read the error codes from UART
        txtUARTOutput.Text = string.Empty;

        if (!UARTSerial.IsOpen)
            throw new Exception("Please connect to UART before attempting to read the error codes.");

        List<string> UARTLines = new();

        for (var i = 0; i <= 10; i++)
        {
            var command = $"errlog {i}";
            var checksum = Utilities.CalculateChecksum(command);
            UARTSerial.WriteLine(checksum);
            do
            {
                var line = UARTSerial.ReadLine();
                if (!string.Equals($"{command}:{checksum:X2}", line, StringComparison.InvariantCultureIgnoreCase))
                {
                    UARTLines.Add(line);
                }
            } while (UARTSerial.BytesToRead != 0);

            foreach (var l in UARTLines)
            {
                var split = l.Split(' ');
                if (!split.Any()) continue;
                switch (split[0])
                {
                    case "NG":
                        break;
                    case "OK":
                        var errorCode = split[2];
                        // Now that the error code has been isolated from the rest of the junk sent by the system
                        // let's check it against the database. The error server will need to return XML results
                        string errorResult = await ParseErrorsAsync(errorCode, cancellationToken)
                            .ConfigureAwait(true); // Explicitly return to UI thread context. 

                        if (!txtUARTOutput.Text.Contains(errorResult))
                        {
                            txtUARTOutput.AppendText(errorResult + Environment.NewLine);
                        }
                        break;
                }
            }
        }
    }

    // If the app is closed before UART is terminated, we need to at least try to close the COM port gracefully first
    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        errorsCTSource.Cancel();

        if (!UARTSerial.IsOpen)
            return;

        Utilities.TryCatchErrors(() => 
                    UARTSerial.Close());
    }

    /// <summary>
    /// Clear the UART output window
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClearOutput_Click(object sender, EventArgs e)
    {
        txtUARTOutput.Text = string.Empty;
    }

    /// <summary>
    /// When the user clicks on the download error database button, show a confirmation first and then if they click yes,
    /// continue to download the latest database from the update server
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDownloadDatabase_Click(object sender, EventArgs e)
    {
        DialogResult result = MessageBox.Show("Downloading the error database will overwrite any existing offline database you currently have. Are you sure you would like to do this?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        // Check if user wants to proceed
        if (result != DialogResult.Yes)
            return;

        // Call the function to download and save the XML data
        _ = DownloadDatabaseAsync(errorsCTSource.Token)
            .ConfigureAwait(false);
        
    }

    /// <summary>
    /// The user can clear the error codes from the console if required but let's make sure they actually want to do
    /// that by showing a confirmation dialog first. If the click yes, send the UART command and wipe the codes from
    /// the console. This action cannot be undone!
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClearErrorCodes_Click(object sender, EventArgs e)
    {
        DialogResult result = MessageBox.Show("This will clear error codes from the console by sending the \"errlog clear\" command. Are you sure you would like to proceed? This action cannot be undone!", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
            return;
        
        // Let's read the error codes from UART
        txtUARTOutput.Text = string.Empty;

        if (!UARTSerial.IsOpen)
        {
            MessageBox.Show("Please connect to UART before attempting to send commands.", "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {

            List<string> UARTLines = new();

            var command = "errlog clear";
            var checksum = Utilities.CalculateChecksum(command);
            UARTSerial.WriteLine(checksum);
            do
            {
                var line = UARTSerial.ReadLine();
                if (!string.Equals($"{command}:{checksum:X2}", line, StringComparison.InvariantCultureIgnoreCase))
                {
                    UARTLines.Add(line);
                }
            } while (UARTSerial.BytesToRead != 0);

            foreach (var l in UARTLines)
            {
                var split = l.Split(' ');
                if (!split.Any()) continue;
                switch (split[0])
                {
                    case "NG":
                        if (!txtUARTOutput.Text.Contains("FAIL"))
                        {
                            txtUARTOutput.AppendText("Response: FAIL" + Environment.NewLine + "Information: An error occurred while clearing the error logs from the system. Please try again...");
                        }
                        break;
                    case "OK":
                        if (!txtUARTOutput.Text.Contains("SUCCESS"))
                        {
                            txtUARTOutput.AppendText("Response: SUCCESS" + Environment.NewLine + "Information: All error codes cleared successfully");
                        }
                        break;
                }
            }
        }

        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            toolStripStatusLabel.Text = "An error occurred while attempting to send a UART command. Please try again...";
        }
    }

    /// <summary>
    /// Sometimes the user might want to send a custom command. Let them do that here!
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSendCommand_Click(object sender, EventArgs e)
    {
        if (txtCustomCommand.Text == string.Empty)
            MessageBox.Show("Please connect to UART before attempting to send commands.", "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        // Let's read the error codes from UART
        txtUARTOutput.Text = string.Empty;

        if (!UARTSerial.IsOpen)
        {
            MessageBox.Show("Please enter a command to send via UART.", "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return;
        }

        try
        {

            List<string> UARTLines = new();

            var checksum = Utilities.CalculateChecksum(txtCustomCommand.Text);
            UARTSerial.WriteLine(checksum);
            do
            {

                var line = UARTSerial.ReadLine();
                if (!string.Equals($"{txtCustomCommand.Text}:{checksum:X2}", line, StringComparison.InvariantCultureIgnoreCase))
                {
                    UARTLines.Add(line);
                }

            } while (UARTSerial.BytesToRead != 0);

            foreach (var l in UARTLines)
            {
                var split = l.Split(' ');
                if (!split.Any()) continue;
                switch (split[0])
                {
                    case "NG":
                        txtUARTOutput.Text = "ERROR: " + l;
                        break;
                    case "OK":
                        txtUARTOutput.Text = "SUCCESS: " + l;
                        break;
                }
            }
        }

        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            toolStripStatusLabel.Text = "An error occurred while reading error codes from UART. Please try again...";
        }
    }

    /// <summary>
    /// If the user presses the enter key while using the custom command box, handle it by programmatically pressing the
    /// send button. This is more of a convenience thing really!
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtCustomCommand_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            btnSendCommand.PerformClick();
        }
    }
}