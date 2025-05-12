using System.Text;
using System.IO.Ports;
using UART.Core.Extensions;
using UART.Core.Abstractions;
using UART.Core.Configuration;
using UART.Core.Models;

namespace PS5_NOR_Modifier
{
    public partial class Form1 : Form
    {
        private readonly IUartProvider _uartProvider;
        private readonly INotificationHandler _notificationHandler;
        static SerialPort UARTSerial = new SerialPort();
        
        public Form1(IUartProvider uartProvider, INotificationHandler notificationHandler)
        {
            _uartProvider = uartProvider;
            _notificationHandler = notificationHandler;
            InitializeComponent();
        }

        private void DisplayErrorMessage(string errmsg)
        {
            _notificationHandler.HandleMessage(new Notification
            {
                Message = errmsg,
                Title = "An Error Has Occurred",
                Type = NotificationType.Warning,
            });
        }

        // Upon first launch, we need to get a list of COM ports available for UART
        private void Form1_Load(object sender, EventArgs e)
            => UpdatePorts();

        // Declare offsets to detect console version
        long offsetOne = 0x1c7010;
        long offsetTwo = 0x1c7030;
        long WiFiMacOffset = 0x1C73C0;
        string? WiFiMacValue = null;
        long LANMacOffset = 0x1C4020;
        string? LANMacValue = null;
        string? offsetOneValue = null;
        string? offsetTwoValue = null;
        long serialOffset = 0x1c7210;
        string? serialValue = null;
        long variantOffset = 0x1c7226;
        string? variantValue = null;
        long moboSerialOffset = 0x1C7200;
        string? moboSerialValue = null;

        /// <summary>
        /// We need to be able to send the error code we received from the console and fetch an XML result back from the server
        /// Once we have a result from the server, parse the XML data and output it in an easy to understand format for the user
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        async Task<string> ParseErrorsAsync(string errorCode)
        {
            // If the user has opted to parse errors with an offline database, run the parse offline function
            if (chkUseOffline.Checked)
            {
                return _uartProvider.ParseErrorsOffline(errorCode);
            }
            
            // The user wants to use the online version. Proceed at will
            return await _uartProvider.ParseErrorsOnline(errorCode);
        }

        private void ResetAppFields()
        {
            fileLocationBox.Text = "";
            serialNumber.Text = "...";
            boardVariant.Text = "...";
            modelInfo.Text = "...";
            fileSizeInfo.Text = "...";
            serialNumberTextbox.Text = "";
            toolStripStatusLabel1.Text = "Status: Waiting for input";
        }

        #region Donations

        /// <summary>
        /// If you modify this app, please leave my credits in, otherwise a little kitten will cry!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label4_Click(object sender, EventArgs e)
        {
            Constants.TipUrl.OpenUrl();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Constants.TipUrl.OpenUrl();
        }


        #endregion

        private void browseFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialogBox = new OpenFileDialog();
            fileDialogBox.Title = "Open NOR BIN File";
            fileDialogBox.Filter = "PS5 BIN Files|*.bin";

            if (fileDialogBox.ShowDialog() != DialogResult.OK)
                return;
            
            if(fileDialogBox.CheckFileExists == false)
            {
                DisplayErrorMessage("The file you selected could not be found. Please check the file exists and is a valid BIN file.");
                return;
            }
            
            if(!fileDialogBox.SafeFileName.EndsWith(".bin"))
            {
                DisplayErrorMessage("The file you selected is not a valid. Please ensure the file you are choosing is a correct BIN file and try again.");
                return;
            }
            
            // Let's load simple information first, before loading BIN specific data
            fileLocationBox.Text = "";
            // Get the path selected and print it into the path box
            string selectedPath = fileDialogBox.FileName;
            toolStripStatusLabel1.Text = "Status: Selected file " + selectedPath;
            fileLocationBox.Text = selectedPath;

            // Get file length and show in bytes and MB
            long length = new FileInfo(selectedPath).Length;
            fileSizeInfo.Text = length + " bytes (" + length / 1024 / 1024 + "MB)";

            #region Extract PS5 Version

            offsetOneValue = fileDialogBox.FileName.ExtractValueFromFile(offsetOne, 12, x => x.Replace("-", null));
            //bug: original code is using offsetOne, is this a bug?
            offsetTwoValue = fileDialogBox.FileName.ExtractValueFromFile(offsetOne, 12, x => x.Replace("-", null));
            
            if(offsetOneValue?.Contains("22020101")??false)
            {
                modelInfo.Text = "Disc Edition";
            }
            else
            {
                if(offsetTwoValue?.Contains("22030101") ?? false)
                {
                    modelInfo.Text = "Digital Edition";
                }
                else
                {
                    modelInfo.Text = "Unknown";
                }
            }

            #endregion

            #region Extract Motherboard Serial Number

            moboSerialValue = fileDialogBox.FileName.ExtractValueFromFile(moboSerialOffset, 12, x => x.Replace("-", null));
            
            if(moboSerialValue != null)
            {
                moboSerialInfo.Text = moboSerialValue.HexStringToString();
            }
            else
            {
                moboSerialInfo.Text = "Unknown";
            }

            #endregion

            #region Extract Board Serial Number

            serialValue = fileDialogBox.FileName.ExtractValueFromFile(serialOffset, 17, x => x.Replace("-", null));

            if (serialValue != null)
            {
                var hexString = serialValue.HexStringToString();
                serialNumber.Text = hexString;
                serialNumberTextbox.Text = hexString;

            }
            else
            {
                serialNumber.Text = "Unknown";
            }

            #endregion

            #region Extract WiFi Mac Address

            WiFiMacValue = fileDialogBox.FileName.ExtractValueFromFile(WiFiMacOffset, 6);

            if (WiFiMacValue != null)
            {
                macAddressInfo.Text = WiFiMacValue;
                wifiMacAddressTextbox.Text = WiFiMacValue;
            }
            else
            {
                macAddressInfo.Text = "Unknown";
                wifiMacAddressTextbox.Text = "";
            }

            #endregion

            #region Extract LAN Mac Address

            LANMacValue = fileDialogBox.FileName.ExtractValueFromFile(LANMacOffset, 6);

            if (LANMacValue != null)
            {
                LANMacAddressInfo.Text = LANMacValue;
                lanMacAddressTextbox.Text = LANMacValue;
            }
            else
            {
                LANMacAddressInfo.Text = "Unknown";
                lanMacAddressTextbox.Text = "";
            }

            #endregion

            #region Extract Board Variant

            variantValue = fileDialogBox.FileName.ExtractValueFromFile(variantOffset, 19, x => x.Replace("-", null).Replace("FF", null));

            if (variantValue != null)
            {
                boardVariant.Text = variantValue.HexStringToString();
            }
            else
            {
                boardVariant.Text = "Unknown";
            }

            boardVariant.Text += boardVariant.Text switch {
                _ when boardVariant.Text.EndsWith("00A") || boardVariant.Text.EndsWith("00B") => " - Japan",
                _ when boardVariant.Text.EndsWith("01A") || boardVariant.Text.EndsWith("01B") ||
                       boardVariant.Text.EndsWith("15A") || boardVariant.Text.EndsWith("15B") => " - US, Canada, (North America)",
                _ when boardVariant.Text.EndsWith("02A") || boardVariant.Text.EndsWith("02B") => " - Australia / New Zealand, (Oceania)",
                _ when boardVariant.Text.EndsWith("03A") || boardVariant.Text.EndsWith("03B") => " - United Kingdom / Ireland",
                _ when boardVariant.Text.EndsWith("04A") || boardVariant.Text.EndsWith("04B") => " - Europe / Middle East / Africa",
                _ when boardVariant.Text.EndsWith("05A") || boardVariant.Text.EndsWith("05B") => " - South Korea",
                _ when boardVariant.Text.EndsWith("06A") || boardVariant.Text.EndsWith("06B") => " - Southeast Asia / Hong Kong",
                _ when boardVariant.Text.EndsWith("07A") || boardVariant.Text.EndsWith("07B") => " - Taiwan",
                _ when boardVariant.Text.EndsWith("08A") || boardVariant.Text.EndsWith("08B") => " - Russia, Ukraine, India, Central Asia",
                _ when boardVariant.Text.EndsWith("09A") || boardVariant.Text.EndsWith("09B") => " - Mainland China",
                _ when boardVariant.Text.EndsWith("11A") || boardVariant.Text.EndsWith("11B") ||
                       boardVariant.Text.EndsWith("14A") || boardVariant.Text.EndsWith("14B") 
                    => " - Mexico, Central America, South America",
                _ when boardVariant.Text.EndsWith("16A") || boardVariant.Text.EndsWith("16B") => " - Europe / Middle East / Africa",
                _ when boardVariant.Text.EndsWith("18A") || boardVariant.Text.EndsWith("18B") => " - Singapore, Korea, Asia",
                _=> " - Unknown Region"
            };
            #endregion
        }

        private void convertToDigitalEditionButton_Click(object sender, EventArgs e)
        {

            string fileNameToLookFor = "";
            bool errorShownAlready = false;

            if (modelInfo.Text == "" || modelInfo.Text == "...")
            {
                // No valid BIN file seems to have been selected
                DisplayErrorMessage("Please select a valid BIOS file first...");
                return;
            }
            
            if(boardModelSelectionBox.Text == "")
            {
                DisplayErrorMessage("Please select a valid board model before saving new BIOS information!");
                return;
            }
            
            if(boardVariantSelectionBox.Text == "")
            {
                DisplayErrorMessage("Please select a valid board variant before saving new BIOS information!");
                return;
            }
            
            SaveFileDialog saveBox = new SaveFileDialog();
            saveBox.Title = "Save NOR BIN File";
            saveBox.Filter = "PS5 BIN Files|*.bin";

            if (saveBox.ShowDialog() != DialogResult.OK)
            {
                DisplayErrorMessage("Save operation cancelled!");
                return;
            }
            
            // First create a copy of the old BIOS file
            byte[] existingFile = File.ReadAllBytes(fileLocationBox.Text);
            string newFile = saveBox.FileName;

            File.WriteAllBytes(newFile, existingFile);

            fileNameToLookFor = saveBox.FileName;

            #region Set the new model info

            try
            {
                GetBiosDetails(out byte[]? find, out byte[]? replace);

                if (find == null || replace == null || find.Length != replace.Length)
                {
                    DisplayErrorMessage("The length of the old hex value does not match the length of the new hex value!");
                    errorShownAlready = true;
                }
                else
                {
                    byte[] bytes = File.ReadAllBytes(newFile);
                    foreach (int index in bytes.PatternAt(find))
                    {
                        for (int i = index, replaceIndex = 0;
                             i < bytes.Length && replaceIndex < replace.Length;
                             i++, replaceIndex++)
                        {
                            bytes[i] = replace[replaceIndex];
                        }

                        File.WriteAllBytes(newFile, bytes);
                    }
                }
            }
            catch (Exception)
            {
                DisplayErrorMessage("An error occurred while saving your BIOS file");
                errorShownAlready = true;
            }
            
            #endregion

            #region Set the new board variant

            try
            {
                byte[] oldVariant = Encoding.UTF8.GetBytes(boardVariant.Text);
                string oldVariantHex = Convert.ToHexString(oldVariant);

                byte[] newVariantSelection = Encoding.UTF8.GetBytes(boardVariantSelectionBox.Text);
                string newVariantHex = Convert.ToHexString(newVariantSelection);

                byte[] find = oldVariantHex.ConvertHexStringToByteArray("0x|[ ,]", string.Empty);
                byte[] replace = newVariantHex.ConvertHexStringToByteArray("0x|[ ,]", string.Empty);

                byte[] bytes = File.ReadAllBytes(newFile);
                foreach (int index in bytes.PatternAt(find))
                {
                    for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace.Length; i++, replaceIndex++)
                    {
                        bytes[i] = replace[replaceIndex];
                    }
                    File.WriteAllBytes(newFile, bytes);
                }

            }
            catch(ArgumentException ex)
            {
                DisplayErrorMessage(ex.Message.ToString());
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

                byte[] find = oldSerialHex.ConvertHexStringToByteArray("0x|[ ,]", string.Empty);
                byte[] replace = newSerialHex.ConvertHexStringToByteArray("0x|[ ,]", string.Empty);

                byte[] bytes = File.ReadAllBytes(newFile);
                foreach (int index in bytes.PatternAt(find))
                {
                    for (int i = index, replaceIndex = 0; i < bytes.Length && replaceIndex < replace.Length; i++, replaceIndex++)
                    {
                        bytes[i] = replace[replaceIndex];
                    }
                    File.WriteAllBytes(newFile, bytes);
                }

            }
            catch (ArgumentException ex)
            {
                DisplayErrorMessage(ex.Message);
                errorShownAlready = true;
            }

            #endregion
            

            if(File.Exists(fileNameToLookFor) && !errorShownAlready)
            {
                // Reset everything and show message
                ResetAppFields();
                _notificationHandler.HandleMessage(new Notification
                {
                    Message = "A new BIOS file was successfully created. Please load the new BIOS file to verify the information you entered before installing onto your motherboard. Remember this software was created by TheCod3r with nothing but love. Why not show some love back by dropping me a small donation to say thanks ;).", 
                    Title = "All done!",
                    Type = NotificationType.Information,
                });
            }
        }

        private void GetBiosDetails(out byte[]? find, out byte[]? replace)
        {
            find = null;
            replace = null;

            if (modelInfo.Text == "Disc Edition" && boardModelSelectionBox.Text == "Digital Edition")
            {
                find = "22020101".ConvertHexStringToByteArray("0x|[ ,]", string.Empty);
                replace = "22030101".ConvertHexStringToByteArray("0x|[ ,]", string.Empty);
                return;
            }

            if (modelInfo.Text == "Digital Edition" && boardModelSelectionBox.Text == "Disc Edition")
            {
                find = "22030101".ConvertHexStringToByteArray("0x|[ ,]", string.Empty);
                replace = "22020101".ConvertHexStringToByteArray("0x|[ ,]", string.Empty);
            }
        }

        private void label15_Click(object sender, EventArgs e)
        {
            Constants.ShopUrl.OpenUrl();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        // When the "refresh ports" button is pressed, we need to refresh the list of available COM ports for UART
        private void btnRefreshPorts_Click(object sender, EventArgs e)
            => UpdatePorts();

        private void UpdatePorts()
        {
            string[] ports = SerialPort.GetPortNames();
            comboComPorts.Items.Clear();
            if (ports.Length > 0)
            {
                comboComPorts.Items.AddRange(ports);
                comboComPorts.SelectedIndex = 0;
            }
            btnConnectCom.Enabled = true;
            btnDisconnectCom.Enabled = false;
        }

        private void btnConnectCom_Click(object sender, EventArgs e)
        {
            // Let's try and connect to the UART reader
            btnConnectCom.Enabled = false;

            if (string.IsNullOrWhiteSpace(comboComPorts.Text))
            {
                _notificationHandler.HandleMessage(new Notification
                {
                    Message = "Please select a COM port from the ports list to establish a connection.",
                    Title = "An error occurred...",
                    Type = NotificationType.Error,
                });
                btnConnectCom.Enabled = true;
                btnDisconnectCom.Enabled = false;
                toolStripStatusLabel1.Text = "Could not connect to UART. Please try again!";
                return;
            }
            
            try
            {
                // Set port to selected port
                UARTSerial.PortName = comboComPorts.Text;
                // Set the BAUD rate to 115200
                UARTSerial.BaudRate = 115200;
                // Enable RTS
                UARTSerial.RtsEnable = true;
                // Open the COM port
                UARTSerial.Open();
                btnDisconnectCom.Enabled = true;
                toolStripStatusLabel1.Text = $"Connected to UART via COM port {comboComPorts.Text} at a BAUD rate of 115200.";
            }
            catch (Exception ex)
            {
                _notificationHandler.HandleMessage(new Notification
                {
                    Message = ex.Message,
                    Title = "An error occurred...",
                    Type = NotificationType.Error,
                });
                btnConnectCom.Enabled = true;
                btnDisconnectCom.Enabled = false;
                toolStripStatusLabel1.Text = "Could not connect to UART. Please try again!";
            }
        }

        private void btnDisconnectCom_Click(object sender, EventArgs e)
        {
            // Let's close the COM port
            try
            {
                if(UARTSerial.IsOpen == true)
                {
                    UARTSerial.Close();
                    btnConnectCom.Enabled = true;
                    btnDisconnectCom.Enabled = false;
                    toolStripStatusLabel1.Text = "Disconnected from UART...";
                }
            }
            catch(Exception ex)
            {
                _notificationHandler.HandleMessage(new Notification
                {
                    Message = ex.Message,
                    Title = "An error occurred...",
                    Type = NotificationType.Error
                });
                toolStripStatusLabel1.Text = "An error occurred while disconnecting from UART. Please try again...";
            }
        }

        /// <summary>
        /// Read error codes from UART
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button1_Click(object sender, EventArgs e)
        {
            // Let's read the error codes from UART
            txtUARTOutput.Text = "";

            if (!UARTSerial.IsOpen)
            {
                _notificationHandler.HandleMessage(new Notification
                {
                    Message = "Please connect to UART before attempting to read the error codes.",
                    Title = "An error occurred...",
                    Type = NotificationType.Warning
                });
                return;
            }
            
            try
            {
                List<string> UARTLines = new();

                for (var i = 0; i <= 10; i++)
                {
                    var command = $"errlog {i}";
                    var checksum = command.CalculateChecksum();
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
                _notificationHandler.HandleMessage(new Notification
                {
                    Message = ex.Message,
                    Title = "An error occurred...",
                    Type = NotificationType.Error
                });
                toolStripStatusLabel1.Text = "An error occurred while reading error codes from UART. Please try again...";
            }
        }

        // If the app is closed before UART is terminated, we need to at least try to close the COM port gracefully first
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!UARTSerial.IsOpen)
                return;
            
            try
            {
                UARTSerial.Close();
            }
            catch(Exception ex)
            {
                _notificationHandler.HandleMessage(new Notification
                {
                    Message = ex.Message,
                    Title = "An error occurred...",
                    Type = NotificationType.Error
                });
            }
        }

        /// <summary>
        /// Clear the UART output window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            txtUARTOutput.Text = "";
        }

        /// <summary>
        /// When the user clicks on the download error database button, show a confirmation first and then if they click yes,
        /// continue to download the latest database from the update server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnDownloadDatabase_Click(object sender, EventArgs e)
        {
            // Check if user wants to proceed
            await _notificationHandler.HandleQuestion(
                new AsyncQuestion
                {
                    Message =
                        "Downloading the error database will overwrite any existing offline database you currently have. Are you sure you would like to do this?",
                    Title = "Are you sure?",
                    
                    // Call the function to download and save the XML data
                    OnYes = async () => await _uartProvider.UpdateErrorDatabase()
                    // Do nothing. The user cancelled the request// The user cancelled
                });
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
            _notificationHandler.HandleQuestion(new Question
            {
                Message =
                    "This will clear error codes from the console by sending the \"errlog clear\" command. Are you sure you would like to proceed? This action cannot be undone!",
                Title = "Are you sure?",
                OnYes = ClearErrorCodes
            });
        }

        private void ClearErrorCodes()
        {
            // Let's read the error codes from UART
            txtUARTOutput.Text = "";

            if (!UARTSerial.IsOpen)
            {
                _notificationHandler.HandleMessage(new Notification
                {
                    Message = "Please connect to UART before attempting to send commands.",
                    Title = "An error occurred...",
                    Type = NotificationType.Warning
                });
                return;
            }
                
            try
            {
                List<string> UARTLines = new();

                var command = "errlog clear";
                var checksum = command.CalculateChecksum();
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
                _notificationHandler.HandleMessage(new Notification
                {
                    Message = ex.Message,
                    Title = "An error occurred...",
                    Type = NotificationType.Error
                });
                
                toolStripStatusLabel1.Text = "An error occurred while attempting to send a UART command. Please try again...";
            }
        }

        /// <summary>
        /// Sometimes the user might want to send a custom command. Let them do that here!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomCommand.Text))
            {
                _notificationHandler.HandleMessage(new Notification
                {
                    Message = "Please enter a command to send via UART.",
                    Title = "An error occurred...",
                    Type = NotificationType.Warning
                });
                return;
            }

            if (!UARTSerial.IsOpen)
            {
                _notificationHandler.HandleMessage(new Notification
                {
                    Message = "Please connect to UART before attempting to send commands.",
                    Title = "An error occurred...",
                    Type = NotificationType.Warning
                });
                return;
            }
            
            // Let's read the error codes from UART
            txtUARTOutput.Text = "";

            try
            {

                List<string> uartLines = new();

                var checksum = txtCustomCommand.Text.CalculateChecksum();
                UARTSerial.WriteLine(checksum);
                do
                {
                    var line = UARTSerial.ReadLine();
                    if (!string.Equals($"{txtCustomCommand.Text}:{checksum:X2}", line, StringComparison.InvariantCultureIgnoreCase))
                    {
                        uartLines.Add(line);
                    }
                } while (UARTSerial.BytesToRead != 0);

                foreach (var l in uartLines)
                {
                    var split = l.Split(' ');
                    if (!split.Any()) continue;
                    switch (split[0])
                    {
                        case "NG":
                            txtUARTOutput.Text = $"ERROR: {l}";
                            break;
                        case "OK":
                            txtUARTOutput.Text = $"SUCCESS: {l}";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationHandler.HandleMessage(new Notification
                {
                    Message = ex.Message,
                    Title = "An error occurred...",
                    Type = NotificationType.Error
                });
                
                toolStripStatusLabel1.Text = "An error occurred while reading error codes from UART. Please try again...";
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
            if(e.KeyChar == (char)Keys.Enter)
            {
                btnSendCommand.PerformClick();
            }
        }
    }
}