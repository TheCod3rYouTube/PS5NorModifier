using System;
using System.IO.Ports;
using System.Xml;
using PS5_NOR_Modifier.UserControls.Events;

namespace PS5_NOR_Modifier.UserControls.UART
{
    public partial class UartUserControl : UserControl
    {
        public event EventHandler<StatusUpdateEventArgs>? statusUpdateEvent;

        private SerialPort _UARTSerial;

        // We want this app to work offline, so let's declare where the local "offline" database will be stored
        private const string LOCAL_DATABASE_FILE = "errorDB.xml";

        public UartUserControl()
        {
            InitializeComponent();

            _UARTSerial = new SerialPort();
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (this.ParentForm != null)
            {
                this.ParentForm.FormClosing += new FormClosingEventHandler(ParentForm_FormClosing);
            }
        }

        // If the app is closed before UART is terminated, we need to at least try to close the COM port gracefully first
        void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_UARTSerial.IsOpen == true)
            {
                try
                {
                    _UARTSerial.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task DownloadDatabaseAsync()
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
                    string xmlData = await client.GetStringAsync(url);

                    // Create an XmlDocument instance and load the XML data
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlData);

                    // Save the XML data to a file
                    xmlDoc.Save(LOCAL_DATABASE_FILE);

                    MessageBox.Show("The most recent offline database has been updated successfully.", "Offline Database Updated!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        static string CalculateChecksum(string str)
        {
            int sum = 0;
            foreach (char c in str)
            {
                sum += (int)c;
            }
            return str + ":" + (sum & 0xFF).ToString("X2");
        }

        string ParseErrorsOffline(string errorCode)
        {
            string results = "";

            try
            {
                // Check if the XML file exists
                if (File.Exists(LOCAL_DATABASE_FILE))
                {
                    // Load the XML file
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(LOCAL_DATABASE_FILE);

                    // Get the root node
                    XmlNode? root = xmlDoc.DocumentElement;
                    if (root is null) return results;

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
                                string errorCodeValue = errorCodeNode.SelectSingleNode("ErrorCode")?.InnerText ?? "";
                                string description = errorCodeNode.SelectSingleNode("Description")?.InnerText ?? "";

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
                    else
                    {
                        results = "Error: Invalid XML database file. Please reconfigure the application, redownload the offline database, or uncheck the option to use the offline database.";
                    }
                }
                else
                {
                    results = "Error: Local XML file not found.";
                }
            }
            catch (Exception ex)
            {
                results = "Error: " + ex.Message;
            }

            return results;
        }

        /// <summary>
        /// We need to be able to send the error code we received from the console and fetch an XML result back from the server
        /// Once we have a result from the server, parse the XML data and output it in an easy to understand format for the user
        /// </summary>
        /// <param name="ErrorCode"></param>
        /// <returns></returns>
        async Task<string> ParseErrorsAsync(string ErrorCode)
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

                string results = "";

                try
                {
                    string response = "";
                    // Create a WebClient instance to send the request
                    using (HttpClient client = new())
                    {
                        // Send the request and retrieve the response as a string
                        response = await client.GetStringAsync(url);
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
                                string errorCode = errorCodeNode.SelectSingleNode("ErrorCode")?.InnerText ?? "";
                                string description = errorCodeNode.SelectSingleNode("Description")?.InnerText ?? "";

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

        private void btnConnectCom_Click(object sender, EventArgs e)
        {
            // Let's try and connect to the UART reader
            btnConnectCom.Enabled = false;

            if (comboComPorts.Text != String.Empty)
            {
                try
                {
                    // Set port to selected port
                    _UARTSerial.PortName = comboComPorts.Text;
                    // Set the BAUD rate to 115200
                    _UARTSerial.BaudRate = 115200;
                    // Enable RTS
                    _UARTSerial.RtsEnable = true;
                    // Open the COM port
                    _UARTSerial.Open();
                    btnDisconnectCom.Enabled = true;

                    UpdateStatus("Connected to UART via COM port " + comboComPorts.Text + " at a BAUD rate of 115200.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnConnectCom.Enabled = true;
                    btnDisconnectCom.Enabled = false;

                    UpdateStatus("Could not connect to UART. Please try again!");
                }
            }
            else
            {
                MessageBox.Show("Please select a COM port from the ports list to establish a connection.", "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnConnectCom.Enabled = true;
                btnDisconnectCom.Enabled = false;

                UpdateStatus("Could not connect to UART. Please try again!");
            }
        }

        private void UpdateStatus(string newStatus)
        {
            if (statusUpdateEvent != null)
            {
                statusUpdateEvent(this, new StatusUpdateEventArgs(newStatus));
            }
        }

        private void btnDisconnectCom_Click(object sender, EventArgs e)
        {
            // Let's close the COM port
            try
            {
                if (_UARTSerial.IsOpen == true)
                {
                    _UARTSerial.Close();
                    btnConnectCom.Enabled = true;
                    btnDisconnectCom.Enabled = false;

                    UpdateStatus("Disconnected from UART...");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Error);

                UpdateStatus("An error occurred while disconnecting from UART. Please try again...");
            }
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

        /// <summary>
        /// Read error codes from UART
        /// </summary>
        private async void button1_Click(object sender, EventArgs e)
        {
            // Let's read the error codes from UART
            txtUARTOutput.Text = "";

            if (_UARTSerial.IsOpen == true)
            {
                try
                {

                    List<string> UARTLines = new();

                    for (var i = 0; i <= 10; i++)
                    {
                        var command = $"errlog {i}";
                        var checksum = CalculateChecksum(command);
                        _UARTSerial.WriteLine(checksum);
                        do
                        {
                            var line = _UARTSerial.ReadLine();
                            if (!string.Equals($"{command}:{checksum:X2}", line, StringComparison.InvariantCultureIgnoreCase))
                            {
                                UARTLines.Add(line);
                            }
                        } while (_UARTSerial.BytesToRead != 0);

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
                                    string errorResult = await ParseErrorsAsync(errorCode);
                                    if (!txtUARTOutput.Text.Contains(errorResult))
                                    {
                                        txtUARTOutput.AppendText(errorResult + Environment.NewLine);
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    UpdateStatus("An error occurred while reading error codes from UART. Please try again...");
                }
            }
            else
            {
                MessageBox.Show("Please connect to UART before attempting to read the error codes.", "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// The user can clear the error codes from the console if required but let's make sure they actually want to do
        /// that by showing a confirmation dialog first. If the click yes, send the UART command and wipe the codes from
        /// the console. This action cannot be undone!
        /// </summary>
        private void btnClearErrorCodes_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("This will clear error codes from the console by sending the \"errlog clear\" command. Are you sure you would like to proceed? This action cannot be undone!", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Let's read the error codes from UART
                txtUARTOutput.Text = "";

                if (_UARTSerial.IsOpen == true)
                {
                    try
                    {

                        List<string> UARTLines = new();

                        var command = "errlog clear";
                        var checksum = CalculateChecksum(command);
                        _UARTSerial.WriteLine(checksum);
                        do
                        {
                            var line = _UARTSerial.ReadLine();
                            if (!string.Equals($"{command}:{checksum:X2}", line, StringComparison.InvariantCultureIgnoreCase))
                            {
                                UARTLines.Add(line);
                            }
                        } while (_UARTSerial.BytesToRead != 0);

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

                        UpdateStatus("An error occurred while attempting to send a UART command. Please try again...");
                    }
                }
                else
                {
                    MessageBox.Show("Please connect to UART before attempting to send commands.", "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                // Do nothing. The user cancelled the request
            }
        }

        /// <summary>
        /// When the user clicks on the download error database button, show a confirmation first and then if they click yes,
        /// continue to download the latest database from the update server
        /// </summary>
        private async void btnDownloadDatabase_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Downloading the error database will overwrite any existing offline database you currently have. Are you sure you would like to do this?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Check if user wants to proceed
            if (result == DialogResult.Yes)
            {
                // Call the function to download and save the XML data
                await DownloadDatabaseAsync();
            }
            else
            {
                // Do nothing. The user cancelled the request// The user cancelled
            }
        }

        /// <summary>
        /// Sometimes the user might want to send a custom command. Let them do that here!
        /// </summary>
        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            if (txtCustomCommand.Text != "")
            {
                // Let's read the error codes from UART
                txtUARTOutput.Text = "";

                if (_UARTSerial.IsOpen == true)
                {
                    try
                    {

                        List<string> UARTLines = new();

                        var checksum = CalculateChecksum(txtCustomCommand.Text);
                        _UARTSerial.WriteLine(checksum);
                        do
                        {
                            var line = _UARTSerial.ReadLine();
                            if (!string.Equals($"{txtCustomCommand.Text}:{checksum:X2}", line, StringComparison.InvariantCultureIgnoreCase))
                            {
                                UARTLines.Add(line);
                            }
                        } while (_UARTSerial.BytesToRead != 0);

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

                        UpdateStatus("An error occurred while reading error codes from UART. Please try again...");
                    }
                }
                else
                {
                    MessageBox.Show("Please connect to UART before attempting to send commands.", "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please enter a command to send via UART.", "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Clear the UART output window
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            txtUARTOutput.Text = String.Empty;
        }

        /// <summary>
        /// If the user presses the enter key while using the custom command box, handle it by programmatically pressing the
        /// send button. This is more of a convenience thing really!
        /// </summary>
        private void txtCustomCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSendCommand.PerformClick();
            }
        }

        private void UartUserControl_Load(object sender, EventArgs e)
        {
            // Upon first launch, we need to get a list of COM ports available for UART
            comboComPorts.Items.Clear();
            
            string[] ports = SerialPort.GetPortNames();
            
            if (ports.Length > 0)
            {
                comboComPorts.Items.AddRange(ports);
                comboComPorts.SelectedIndex = 0;
                btnConnectCom.Enabled = true;
                btnDisconnectCom.Enabled = false;
            }
        }
    }
}
