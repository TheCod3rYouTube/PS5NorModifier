using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO.Ports;
using System;
using System.Threading;
using System.Collections.Generic;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net;
using System.Xml;
using System.Security.Policy;

namespace PS5_NOR_Modifier
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
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

        private void throwError(string errmsg)
        {
            MessageBox.Show(errmsg, "An Error Has Occurred", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // We want this app to work offline, so let's declare where the local "offline" database will be stored
        string localDatabaseFile = "errorDB.xml";

        static SerialPort UARTSerial = new SerialPort();

        /// <summary>
        /// With thanks to  @jjxtra on Github. The code has already been created and there's no need to reinvent the wheel is there?
        /// </summary>
        #region Hex Code

        private static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                {
                    yield return i;
                }
            }
        }

        private static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            // Upon first launch, we need to get a list of COM ports available for UART
            string[] ports = SerialPort.GetPortNames();
            comboComPorts.Items.Clear();
            comboComPorts.Items.AddRange(ports);
            comboComPorts.SelectedIndex = 0;
            btnConnectCom.Enabled = true;
            btnDisconnectCom.Enabled = false;
        }

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

        string ParseErrorsOffline(string errorCode)
        {
            string results = "";

            try
            {
                // Check if the XML file exists
                if (File.Exists(localDatabaseFile))
                {
                    // Load the XML file
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(localDatabaseFile);

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

        string HexStringToString(string hexString)
        {
            if (hexString == null || (hexString.Length & 1) == 1)
            {
                throw new ArgumentException();
            }
            var sb = new StringBuilder();
            for (var i = 0; i < hexString.Length; i += 2)
            {
                var hexChar = hexString.Substring(i, 2);
                sb.Append((char)Convert.ToByte(hexChar, 16));
            }
            return sb.ToString();
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
            OpenUrl("https://www.streamelements.com/thecod3r/tip");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            OpenUrl("https://www.streamelements.com/thecod3r/tip");
        }


        #endregion

        private void browseFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialogBox = new OpenFileDialog();
            fileDialogBox.Title = "Open NOR BIN File";
            fileDialogBox.Filter = "PS5 BIN Files|*.bin";

            if (fileDialogBox.ShowDialog() == DialogResult.OK)
            {
                if (fileDialogBox.CheckFileExists == false)
                {
                    throwError("The file you selected could not be found. Please check the file exists and is a valid BIN file.");
                }
                else
                {
                    if (!fileDialogBox.SafeFileName.EndsWith(".bin"))
                    {
                        throwError("The file you selected is not a valid. Please ensure the file you are choosing is a correct BIN file and try again.");
                    }
                    else
                    {
                        // Let's load simple information first, before loading BIN specific data
                        fileLocationBox.Text = "";
                        // Get the path selected and print it into the path box
                        string selectedPath = fileDialogBox.FileName;
                        toolStripStatusLabel1.Text = "Status: Selected file " + selectedPath;
                        fileLocationBox.Text = selectedPath;

                        // Get file length and show in bytes and MB
                        long length = new System.IO.FileInfo(selectedPath).Length;
                        fileSizeInfo.Text = length.ToString() + " bytes (" + length / 1024 / 1024 + "MB)";

                        #region Extract PS5 Version

                        try
                        {
                            BinaryReader reader = new BinaryReader(new FileStream(fileDialogBox.FileName, FileMode.Open));
                            //Set the position of the reader
                            reader.BaseStream.Position = offsetOne;
                            //Read the offset
                            offsetOneValue = BitConverter.ToString(reader.ReadBytes(12)).Replace("-", null);
                            reader.Close();
                        }
                        catch
                        {
                            // Obviously this value is invalid, so null the value and move on
                            offsetOneValue = null;
                        }

                        try
                        {
                            BinaryReader reader = new BinaryReader(new FileStream(fileDialogBox.FileName, FileMode.Open));
                            //Set the position of the reader
                            reader.BaseStream.Position = offsetOne;
                            //Read the offset
                            offsetTwoValue = BitConverter.ToString(reader.ReadBytes(12)).Replace("-", null);
                            reader.Close();
                        }
                        catch
                        {
                            // Obviously this value is invalid, so null the value and move on
                            offsetTwoValue = null;
                        }


                        if (offsetOneValue?.Contains("22020101") ?? false)
                        {
                            modelInfo.Text = "Disc Edition";
                        }
                        else
                        {
                            if (offsetTwoValue?.Contains("22030101") ?? false)
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

                        try
                        {
                            BinaryReader reader = new BinaryReader(new FileStream(fileDialogBox.FileName, FileMode.Open));
                            //Set the position of the reader
                            reader.BaseStream.Position = moboSerialOffset;
                            //Read the offset
                            moboSerialValue = BitConverter.ToString(reader.ReadBytes(16)).Replace("-", null);
                            reader.Close();
                        }
                        catch
                        {
                            // Obviously this value is invalid, so null the value and move on
                            moboSerialValue = null;
                        }



                        if (moboSerialValue != null)
                        {
                            moboSerialInfo.Text = HexStringToString(moboSerialValue);
                        }
                        else
                        {
                            moboSerialInfo.Text = "Unknown";
                        }

                        #endregion

                        #region Extract Board Serial Number

                        try
                        {
                            BinaryReader reader = new BinaryReader(new FileStream(fileDialogBox.FileName, FileMode.Open));
                            //Set the position of the reader
                            reader.BaseStream.Position = serialOffset;
                            //Read the offset
                            serialValue = BitConverter.ToString(reader.ReadBytes(17)).Replace("-", null);
                            reader.Close();
                        }
                        catch
                        {
                            // Obviously this value is invalid, so null the value and move on
                            serialValue = null;
                        }



                        if (serialValue != null)
                        {
                            serialNumber.Text = HexStringToString(serialValue);
                            serialNumberTextbox.Text = HexStringToString(serialValue);

                        }
                        else
                        {
                            serialNumber.Text = "Unknown";
                        }

                        #endregion

                        #region Extract WiFi Mac Address

                        try
                        {
                            BinaryReader reader = new BinaryReader(new FileStream(fileDialogBox.FileName, FileMode.Open));
                            //Set the position of the reader
                            reader.BaseStream.Position = WiFiMacOffset;
                            //Read the offset
                            WiFiMacValue = BitConverter.ToString(reader.ReadBytes(6));
                            reader.Close();
                        }
                        catch
                        {
                            // Obviously this value is invalid, so null the value and move on
                            WiFiMacValue = null;
                        }

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

                        try
                        {
                            BinaryReader reader = new BinaryReader(new FileStream(fileDialogBox.FileName, FileMode.Open));
                            //Set the position of the reader
                            reader.BaseStream.Position = LANMacOffset;
                            //Read the offset
                            LANMacValue = BitConverter.ToString(reader.ReadBytes(6));
                            reader.Close();
                        }
                        catch
                        {
                            // Obviously this value is invalid, so null the value and move on
                            LANMacValue = null;
                        }

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

                        try
                        {
                            BinaryReader reader = new BinaryReader(new FileStream(fileDialogBox.FileName, FileMode.Open));
                            //Set the position of the reader
                            reader.BaseStream.Position = variantOffset;
                            //Read the offset
                            variantValue = BitConverter.ToString(reader.ReadBytes(19)).Replace("-", null).Replace("FF", null);
                            reader.Close();
                        }
                        catch
                        {
                            // Obviously this value is invalid, so null the value and move on
                            variantValue = null;
                        }



                        if (variantValue != null)
                        {
                            boardVariant.Text = HexStringToString(variantValue);
                        }
                        else
                        {
                            boardVariant.Text = "Unknown";
                        }

                        boardVariant.Text += boardVariant.Text switch
                        {
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
                            _ => " - Unknown Region"
                        };
                        #endregion
                    }
                }
            }
        }

        private void convertToDigitalEditionButton_Click(object sender, EventArgs e)
        {

            string fileNameToLookFor = "";
            bool errorShownAlready = false;

            if (modelInfo.Text == "" || modelInfo.Text == "...")
            {
                // No valid BIN file seems to have been selected
                throwError("Please select a valid BIOS file first...");
                errorShownAlready = true;
            }
            else
            {
                if (boardModelSelectionBox.Text == "")
                {
                    throwError("Please select a valid board model before saving new BIOS information!");
                    errorShownAlready = true;
                }
                else
                {
                    if (boardVariantSelectionBox.Text == "")
                    {
                        throwError("Please select a valid board variant before saving new BIOS information!");
                        errorShownAlready = true;
                    }
                    else
                    {
                        SaveFileDialog saveBox = new SaveFileDialog();
                        saveBox.Title = "Save NOR BIN File";
                        saveBox.Filter = "PS5 BIN Files|*.bin";

                        if (saveBox.ShowDialog() == DialogResult.OK)
                        {
                            // First create a copy of the old BIOS file
                            byte[] existingFile = File.ReadAllBytes(fileLocationBox.Text);
                            string newFile = saveBox.FileName;

                            File.WriteAllBytes(newFile, existingFile);

                            fileNameToLookFor = saveBox.FileName;

                            #region Set the new model info
                            if (modelInfo.Text == "Disc Edition")
                            {
                                try
                                {

                                    if (boardModelSelectionBox.Text == "Digital Edition")

                                    {

                                        byte[] find = ConvertHexStringToByteArray(Regex.Replace("22020101", "0x|[ ,]", string.Empty).Normalize().Trim());
                                        byte[] replace = ConvertHexStringToByteArray(Regex.Replace("22030101", "0x|[ ,]", string.Empty).Normalize().Trim());
                                        if (find.Length != replace.Length)
                                        {
                                            throwError("The length of the old hex value does not match the length of the new hex value!");
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
                                    throwError("An error occurred while saving your BIOS file");
                                    errorShownAlready = true;
                                }
                            }
                            else
                            {
                                if (modelInfo.Text == "Digital Edition")
                                {
                                    try
                                    {

                                        if (boardModelSelectionBox.Text == "Disc Edition")

                                        {

                                            byte[] find = ConvertHexStringToByteArray(Regex.Replace("22030101", "0x|[ ,]", string.Empty).Normalize().Trim());
                                            byte[] replace = ConvertHexStringToByteArray(Regex.Replace("22020101", "0x|[ ,]", string.Empty).Normalize().Trim());
                                            if (find.Length != replace.Length)
                                            {
                                                throwError("The length of the old hex value does not match the length of the new hex value!");
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
                                        throwError("An error occurred while saving your BIOS file");
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
                            catch (System.ArgumentException ex)
                            {
                                throwError(ex.Message.ToString());
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
                                throwError(ex.Message.ToString());
                                errorShownAlready = true;
                            }

                            #endregion
                        }
                        else
                        {
                            throwError("Save operation cancelled!");
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
            }

        }

        private void label15_Click(object sender, EventArgs e)
        {
            OpenUrl("https://www.consolefix.shop");
        }

        private void label1_Click(object sender, EventArgs e)
        {

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
            // Let's try and connect to the UART reader
            btnConnectCom.Enabled = false;

            if (comboComPorts.Text != "")
            {

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
                    toolStripStatusLabel1.Text = "Connected to UART via COM port " + comboComPorts.Text + " at a BAUD rate of 115200.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnConnectCom.Enabled = true;
                    btnDisconnectCom.Enabled = false;
                    toolStripStatusLabel1.Text = "Could not connect to UART. Please try again!";
                }

            }
            else
            {
                MessageBox.Show("Please select a COM port from the ports list to establish a connection.", "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (UARTSerial.IsOpen == true)
                {
                    UARTSerial.Close();
                    btnConnectCom.Enabled = true;
                    btnDisconnectCom.Enabled = false;
                    toolStripStatusLabel1.Text = "Disconnected from UART...";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (UARTSerial.IsOpen == true)
            {
                try
                {

                    List<string> UARTLines = new();

                    for (var i = 0; i <= 10; i++)
                    {
                        var command = $"errlog {i}";
                        var checksum = CalculateChecksum(command);
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
                    MessageBox.Show(ex.Message, "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    toolStripStatusLabel1.Text = "An error occurred while reading error codes from UART. Please try again...";
                }
            }
            else
            {
                MessageBox.Show("Please connect to UART before attempting to read the error codes.", "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // If the app is closed before UART is terminated, we need to at least try to close the COM port gracefully first
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (UARTSerial.IsOpen == true)
            {
                try
                {
                    UARTSerial.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "An error occurred...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
        /// The user can clear the error codes from the console if required but let's make sure they actually want to do
        /// that by showing a confirmation dialog first. If the click yes, send the UART command and wipe the codes from
        /// the console. This action cannot be undone!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearErrorCodes_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("This will clear error codes from the console by sending the \"errlog clear\" command. Are you sure you would like to proceed? This action cannot be undone!", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Let's read the error codes from UART
                txtUARTOutput.Text = "";

                if (UARTSerial.IsOpen == true)
                {
                    try
                    {

                        List<string> UARTLines = new();

                        var command = "errlog clear";
                        var checksum = CalculateChecksum(command);
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
                        toolStripStatusLabel1.Text = "An error occurred while attempting to send a UART command. Please try again...";
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
        /// Sometimes the user might want to send a custom command. Let them do that here!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            if (txtCustomCommand.Text != "")
            {
                // Let's read the error codes from UART
                txtUARTOutput.Text = "";

                if (UARTSerial.IsOpen == true)
                {
                    try
                    {

                        List<string> UARTLines = new();

                        var checksum = CalculateChecksum(txtCustomCommand.Text);
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
                        toolStripStatusLabel1.Text = "An error occurred while reading error codes from UART. Please try again...";
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


        private void chooseNorFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialogBox = new OpenFileDialog();
            fileDialogBox.Title = "Open NOR BIN File";
            fileDialogBox.Filter = "PS5 BIN Files|*.bin";

            if (fileDialogBox.ShowDialog() == DialogResult.OK)
            {
                if (fileDialogBox.CheckFileExists == false)
                {
                    throwError("The file you selected could not be found. Please check the file exists and is a valid BIN file.");
                }
                else
                {
                    if (!fileDialogBox.SafeFileName.EndsWith(".bin"))
                    {
                        throwError("The file you selected is not a valid. Please ensure the file you are choosing is a correct BIN file and try again.");
                    }
                    else
                    {
                        norFileInputTextBox.Text = "";
                        string selectedPath = fileDialogBox.FileName;
                        toolStripStatusLabel1.Text = "Status: Selected file " + selectedPath;
                        norFileInputTextBox.Text = selectedPath;
                        txtNorDecodeOutput.Text = "NOR Decode Tool | Made by Dony | Migrated by EagLeZz 'Marv'";

                        long length = new System.IO.FileInfo(selectedPath).Length;
                        fileSizeInfo.Text = length.ToString() + " bytes (" + length / 1024 / 1024 + "MB)";
                    }
                }
            }
        }

        private void fileLocationBox_TextChanged(object sender, EventArgs e)
        {

        }

        private List<string> ExtractHexData(string filePath, long startOffset, long endOffset)
        {
            var hexData = new List<string>();

            try
            {
                byte[] data = File.ReadAllBytes(filePath);
                if (data.Length < endOffset)
                {
                    throw new Exception("File size is smaller than the specified end offset.");
                }

                byte[] extractedData = data.Skip((int)startOffset).Take((int)(endOffset - startOffset)).ToArray();

                if (extractedData.Length % 4 != 0)
                {
                    throw new Exception("Data length is not a multiple of 4 bytes. Please check the offsets.");
                }

                for (int i = 0; i < extractedData.Length; i += 4)
                {
                    uint value = BitConverter.ToUInt32(extractedData, i);
                    hexData.Add(value.ToString("X8"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error extracting data: " + ex.Message);
            }

            return hexData;
        }

        private string FormatDataWithLineNumbers(List<string> hexData, string filePath)
        {
            StringBuilder formattedData = new StringBuilder();
            


            for (int i = 0; i < hexData.Count; i += 8)
            {
                var row = hexData.Skip(i).Take(8).ToArray();

                if (row.Length == 8)
                {
                    string formattedRow = $"{(i / 8):00} ";
                    formattedRow += string.Join(" ", row);
                    formattedRow += " FFFF FFFF";

                    formattedData.AppendLine(formattedRow);
                }
            }

            return formattedData.ToString();
        }


        private void norDecodeButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(norFileInputTextBox.Text))
            {
                txtNorDecodeOutput.Text = "Select a valid NOR File before Decoding.";
                return;
            }

            string selectedFile = norFileInputTextBox.Text;

            long startOffset = 0x1CE100;
            long endOffset = 0x1CEC70;

            List<string> hexData = ExtractHexData(selectedFile, startOffset, endOffset);

            if (hexData.Count == 0)
            {
                txtNorDecodeOutput.Text = "No Datas Extracted. Check Offsets.";
                return;
            }

            string formattedData = FormatDataWithLineNumbers(hexData, selectedFile);

            txtNorDecodeOutput.Text =
                "== Emc Error Log ==\r\n" +
                "No  Code       Rtc        PowState   UpCause    SeqNo   DevPm  T(SoC)  T(Env) Padding(0) Padding(1)\r\n";

            txtNorDecodeOutput.Text += formattedData;

            AppendNorDecodeResult(formattedData);
        }

        private void AppendNorDecodeResult(string formattedData)
        {
            string norDecodedResult = "\r\n== NOR Decode Result ==\r\n";

            foreach (var line in formattedData.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = line.Split(' ');

                // Make sure there are enough parts in the line to extract values
                if (parts.Length >= 10)
                {
                    // Extract the Code, SeqNo, and Temperatures
                    string code = parts[1];
                    string seqNoHex = parts[5];  // Full sequence number is here

                    // Make sure the seqNoHex is long enough to perform the substring operation
                    string seqNo = seqNoHex.Length > 4 ? seqNoHex.Substring(4) : seqNoHex;  // If length is less than 5, use the full seqNoHex

                    string tempSoCHex = parts[6];
                    string tempEnvHex = parts[7];
                    string tempSoC = tempSoCHex.Length > 4 ? tempSoCHex.Substring(4) : tempSoCHex;  // If length is less than 5, use the full tempSoCHex
                    string tempEnv = tempEnvHex.Length > 4 ? tempEnvHex.Substring(4) : tempEnvHex;  // If length is less than 5, use the full tempEnvHex

                    // Append formatted details to the result
                    norDecodedResult += $"Code: {code} ({GetCodeDescription(code)}),\r\n";
                    norDecodedResult += $"SeqNo: {seqNo} ({GetSeqDescription(seqNo)}),\r\n";
                    norDecodedResult += $"T(SoC): {ConvertToCelsius(tempSoC)}°C,\r\n";
                    norDecodedResult += $"T(Env): {ConvertToCelsius(tempEnv)}°C,\r\n";
                    norDecodedResult += $"PowState: {GetPowStateDescription(0)},\r\n";
                    norDecodedResult += $"UPCAUSE: {GetUpCauseFlags(0)},\r\n";
                    norDecodedResult += $"devpm: {GetDevPmFlags(0)}\r\n\r\n";
                }
            }

            // Append the final result to the output
            txtNorDecodeOutput.AppendText(norDecodedResult);
        }

        private string GetCodeDescription(string code)
        {
            var codeDatabase = new Dictionary<string, string>
    {
        { "80000001", "Thermal Sensor Fail - NaN SOC" },
        { "80000004", "AC/DC Power Fail" },
        { "80000005", "Main SoC CPU Power Fail" },
        { "80000006", "Main SoC GFX Power Fail" },
        { "80000007", "Main SoC Thrm High Temperature Abnormality" },
        { "80000008", "Drive Dead Notify Timeout" },
        { "80000009", "AC In Detect(12v)" },
        { "8000000A", "VRM HOT Fatal" },
        { "8000000B", "Unexpected Thermal Shutdown in state that Fatal OFF is not allowed" },
        { "8000000C", "MSoC Temperature Alert" },
        { "80000024", "MEMIO(2) Init FAIL(SoC) (?)" },
        { "80800024", "MEMIO(2) Init FAIL(SoC) (?)" },
        { "80050000", "VRM CPU (2)" },
        { "80060000", "VRM GPU(6)" },
        { "80810001", "FORCE_Fatal_Off - PSQ Error" },
        { "80810002", "PSQ NVS Access Error" },
        { "80810013", "PSQ ScCmd DRAM Init Error" },
        { "80810014", "PSQ ScCmd Link Up Failure" },
        { "80830000", "Power Group 2 Init Fail (?)" },
        { "80870001", "Titania RAM Protect Error" },
        { "80870002", "Titania RAM Parity Error" },
        { "80870003", "Titania Boot Failed : Couldn't read Chip Revision." },
        { "80870004", "Titania Boot Failed : Couldn't read error information." },
        { "80870005", "Titania Boot Failed : State Error" },
        { "808D0000", "Thermal Shutdown : Main SoC" },
        { "808D0001", "Thermal Shutdown : Local Sensor 1" },
        { "808D0002", "Thermal Shutdown : Local Sensor 2" },
        { "808D0003", "Thermal Shutdown : Local Sensor 3" },
        { "808E0000", "EAP_Fail (SSD_CON)" },
        { "808E0001", "EAP_Fail (SSD_CON)" },
        { "808E0002", "EAP_Fail (SSD_CON)" },
        { "808E0003", "EAP_Fail (SSD_CON)" },
        { "808E0004", "EAP_Fail (SSD_CON)" },
        { "808E0005", "EAP_Fail (SSD_CON) - Sig 1" },
        { "808E0006", "EAP_Fail (SSD_CON)" },
        { "808E0007", "EAP_Fail (SSD_CON)" },
        { "808F0001", "SMCU (SSD_CON > EMC) (?)" },
        { "808F0002", "SMCU (SSD_CON > EMC) (?)" },
        { "808F0003", "SMCU (SSD_CON > EMC) (?)" },
        { "808F00FF", "SMCU (SSD_CON > EMC) (?)" },
        { "80C00114", "WatchDog For SoC" },
        { "80C00115", "WatchDog For EAP" },
        { "80C0012C", "BD Drive Detached" },
        { "80C0012D", "EMC Watch Dog Timer Error" },
        { "80C0012E", "ADC Error (Button)" },
        { "80C0012F", "ADC Error (BD Drive)" },
        { "80C00130", "ADC Error (AC In Det)" },
        { "80C00131", "USB Over Current" },
        { "80C00132", "FAN Storage Access Failed" },
        { "80C00133", "USB-BT FW Header Invalid Header" },
        { "80C00134", "USB-BT BT Command Error" },
        { "80C00135", "USB-BT Memory Malloc Failed" },
        { "80C00136", "USB-BT Device Not Found" },
        { "80C00137", "USB-BT MISC Error" },
        { "80C00138", "Titania Interrupt HW Error" },
        { "80C00139", "BD Drive Eject Assert Delayed" },
        { "80801101", "RAM GDDR6 1" },
        { "80801102", "RAM GDDR6 2" },
        { "80801103", "RAM GDDR6 1 2" },
        { "80801104", "RAM GDDR6 3" },
        { "80801105", "RAM GDDR6 1 3" },
        { "80801106", "RAM GDDR6 2 3" },
        { "80801107", "RAM GDDR6 1 2 3" },
        { "80801108", "RAM GDDR6 4" },
        { "80801109", "RAM GDDR6 1 4" },
        { "8080110A", "RAM GDDR6 2 4" },
        { "8080110B", "RAM GDDR6 1 2 4" },
        { "8080110C", "RAM GDDR6 3 4" },
        { "8080110D", "RAM GDDR6 1 3 4" },
        { "8080110E", "RAM GDDR6 2 3 4" },
        { "8080110F", "RAM GDDR6 1 2 3 4" },
        { "80801110", "RAM GDDR6 5" },
        { "80801111", "RAM GDDR6 1 5" },
        { "80801112", "RAM GDDR6 2 5" },
        { "80801113", "RAM GDDR6 1 2 5" },
        { "80801114", "RAM GDDR6 3 5" },
        { "80801115", "RAM GDDR6 1 3 5" },
        { "80801116", "RAM GDDR6 2 3 5" },
        { "80801117", "RAM GDDR6 1 2 3 5" },
        { "80801118", "RAM GDDR6 4 5" },
        { "80801119", "RAM GDDR6 1 4 5" },
        { "8080111A", "RAM GDDR6 2 4 5" },
        { "8080111B", "RAM GDDR6 1 2 4 5" },
        { "8080111C", "RAM GDDR6 3 4 5" },
        { "8080111D", "RAM GDDR6 1 3 4 5" },
        { "8080111E", "RAM GDDR6 2 3 4 5" },
        { "8080111F", "RAM GDDR6 1 2 3 4 5" },
        { "FFFFFFFF", "No Error" },
    };

            if (codeDatabase.ContainsKey(code))
            {
                return codeDatabase[code];
            }
            var regexDatabase = new List<KeyValuePair<Regex, string>>()
    {
        new KeyValuePair<Regex, string>(new Regex(@"8005[0-9A-Fa-f]{4}"), "VRM CPU (2) (?)"),
        new KeyValuePair<Regex, string>(new Regex(@"8006[0-9A-Fa-f]{4}"), "VRM GPU(6) (?)"),
        new KeyValuePair<Regex, string>(new Regex(@"8080[0-9A-Fa-f]{4}"), "Fatal Shutdown by OS request"),
        new KeyValuePair<Regex, string>(new Regex(@"8080[0-9A-Fa-f]{4}"), "Fatal_OFF by BigOs - Failed to Start OS Kernel"),
        new KeyValuePair<Regex, string>(new Regex(@"808710[0-9A-Fa-f]{2}"), "Titania ScCmd Response Error"),
        new KeyValuePair<Regex, string>(new Regex(@"8088[0-9A-Fa-f]{1}[A-Z]{3}"), "Titania Boot EAP Error"),
        new KeyValuePair<Regex, string>(new Regex(@"8089[0-9A-Fa-f]{1}[A-Z]{3}"), "Titania Boot EFC Error"),
        new KeyValuePair<Regex, string>(new Regex(@"808A[0-9A-Fa-f]{4}"), "Titania Temperature Error"),
        new KeyValuePair<Regex, string>(new Regex(@"808B[0-9A-Fa-f]{3}[A-Z]{1}"), "Titania Watch Dog Timer"),
        new KeyValuePair<Regex, string>(new Regex(@"808C[0-9A-Fa-f]{4}"), "USB Type-C Error"),
        new KeyValuePair<Regex, string>(new Regex(@"8090[0-9A-Fa-f]{4}"), "Fatal Shutdown - OS CRASH"),
        new KeyValuePair<Regex, string>(new Regex(@"8091[0-9A-Fa-f]{4}"), "SSD PMIC Error"),
        new KeyValuePair<Regex, string>(new Regex(@"C001[0-9A-Fa-f]{4}"), "Main SoC Access Error (I2C)"),
        new KeyValuePair<Regex, string>(new Regex(@"C002[0-9A-Fa-f]{4}"), "Main SoC Access Error (SB-TSI I2C)"),
        new KeyValuePair<Regex, string>(new Regex(@"C003[0-9A-Fa-f]{4}"), "Main SoC Access Error (SB-RMI)"),
        new KeyValuePair<Regex, string>(new Regex(@"C00B[0-9A-Fa-f]{4}"), "Serial Flash Access Error"),
        new KeyValuePair<Regex, string>(new Regex(@"C00C[0-9A-Fa-f]{4}"), "VRM Controller Access Error"),
        new KeyValuePair<Regex, string>(new Regex(@"C00D[0-9A-Fa-f]{4}"), "PMIC (Subsystem) Access Error"),
        new KeyValuePair<Regex, string>(new Regex(@"C010[0-9A-Fa-f]{4}"), "Flash Controller Access Error"),
        new KeyValuePair<Regex, string>(new Regex(@"C011[0-9A-Fa-f]{4}"), "Potentiometer Access Error"),
        new KeyValuePair<Regex, string>(new Regex(@"C015[0-9A-Fa-f]{4}"), "PCIe Access Error"),
        new KeyValuePair<Regex, string>(new Regex(@"C016[0-9A-Fa-f]{4}"), "PMIC (SSD) Access Error"),
        new KeyValuePair<Regex, string>(new Regex(@"C081[0-9A-Fa-f]{4}"), "HDMI Tx Access Error"),
        new KeyValuePair<Regex, string>(new Regex(@"C090[0-9A-Fa-f]{4}"), "USB Type-C PD Controller Access Error"),
        new KeyValuePair<Regex, string>(new Regex(@"C091[0-9A-Fa-f]{4}"), "USB Type-C USB/DP Mux Access Error"),
        new KeyValuePair<Regex, string>(new Regex(@"C092[0-9A-Fa-f]{4}"), "USB Type-C Redriver Access Error"),
        new KeyValuePair<Regex, string>(new Regex(@"C0FE[0-9A-Fa-f]{4}"), "Dummy"),
    };

            foreach (var entry in regexDatabase)
            {
                if (entry.Key.IsMatch(code))
                {
                    return entry.Value;
                }
            }

            return "Unknown Code";
        }


        private string GetSeqDescription(string seqNo)
        {
            var seqDatabase = new Dictionary<string, string>
{
    { "2002", "EmcBootup" },
    { "2067", "EmcBootup" },
    { "2064", "EmcBootup, FATAL OFF" },
    { "218E", "EmcBootup" },
    { "2003", "Subsystem Peripheral Initialize" },
    { "2005", "Subsystem Peripheral Initialize" },
    { "2004", "Subsystem Peripheral Initialize" },
    { "2008", "aEmcTimerIniti" },
    { "2009", "aEmcTimerIniti" },
    { "200A", "aEmcTimerIniti" },
    { "200B", "aEmcTimerIniti" },
    { "200C", "aPowerGroup2On 1" },
    { "2109", "aPowerGroup2On 1" },
    { "200D", "aPowerGroup2On 1" },
    { "2011", "aPowerGroup2On 1" },
    { "200E", "aPowerGroup2On 1, Subsystem PG2 reset" },
    { "200F", "aPowerGroup2On 1" },
    { "2010", "aPowerGroup2On 1, Subsystem PG2 reset" },
    { "202E", "aPowerGroup2On 1, Subsystem PG2 reset" },
    { "2006", "aPowerGroup2On 1, Subsystem PG2 reset" },
    { "21AF", "aPowerGroup2On 1" },
    { "21B1", "aPowerGroup2On 1" },
    { "2014", "aPowerGroup2Off, Flash Controller OFF EFC, Flash Controller OFF EAP, Flash Controller STOP EFC, Flash Controller STOP EAP, FATAL OFF" },
    { "202F", "aPowerGroup2Off, FATAL OFF" },
    { "2015", "aPowerGroup2Off, FATAL OFF" },
    { "2016", "aPowerGroup2Off, Subsystem PG2 reset, FATAL OFF" },
    { "202B", "aPowerGroup2Off, FATAL OFF" },
    { "2017", "aPowerGroup2Off, FATAL OFF" },
    { "210A", "aPowerGroup2Off, FATAL OFF" },
    { "2018", "aPowerGroup2Off, FATAL OFF" },
    { "2019", "aPowerGroup2Off" },
    { "201A", "aSbPcieInitiali" },
    { "2030", "aSbPcieInitiali, aSbPcieInitiali 1, FATAL OFF" },
    { "2031", "aSbPcieInitiali, aSbPcieInitiali 1, FATAL OFF" },
    { "2066", "aSbPcieInitiali 1" },
    { "208D", "aEfcBootModeSet, EAP Boot Mode Set" },
    { "210B", "aEfcBootModeSet, EAP Boot Mode Set" },
    { "210C", "aEfcBootModeSet, EAP Boot Mode Set" },
    { "210D", "aEfcBootModeSet" },
    { "201D", "Flash Controller ON EFC, Flash Controller ON EAP" },
    { "2027", "Flash Controller ON EFC, Flash Controller ON EAP, Flash Controller Soft reset" },
    { "2110", "Flash Controller ON EFC, Flash Controller ON EAP" },
    { "2033", "Flash Controller ON EFC, Flash Controller ON EAP, Flash Controller Soft reset" },
    { "2089", "Flash Controller ON EFC, Flash Controller ON EAP, Flash Controller Soft reset" },
    { "2035", "Flash Controller ON EFC, Flash Controller ON EAP, Flash Controller Soft reset, FC NAND Close Not urgent, FC NAND Close Urgent" },
    { "201C", "Subsystem PCIe USP Enable" },
    { "2029", "Subsystem PCIe DSP Enable, Subsystem PCIe DSP Enable BT DL" },
    { "2107", "Subsystem PCIe DSP Enable, Dev WLAN BT PCIE RESET NEGATE, Dev WLAN BT PCIE RESET ASSERT NEGATE" },
    { "2159", "Flash Controller Initialization EFC, Flash Controller Initialization EAP" },
    { "2045", "Flash Controller Initialization EFC, Flash Controller Initialization EAP" },
    { "2038", "Flash Controller Initialization EFC" },
    { "2043", "Flash Controller Initialization EFC, Flash Controller Initialization EAP" },
    { "2041", "Flash Controller Initialization EFC, Flash Controller Initialization EAP" },
    { "2047", "Flash Controller Initialization EAP" },
    { "204C", "Flash Controller OFF EFC, Flash Controller STOP EFC" },
    { "2108", "Flash Controller OFF EFC, Flash Controller OFF EAP, Flash Controller STOP EFC, Flash Controller STOP EAP, FATAL OFF, Dev WLAN BT PCIE RESET ASSERT, Dev WLAN BT PCIE RESET ASSERT NEGATE" },
    { "206D", "Flash Controller OFF EFC, Flash Controller OFF EAP, Flash Controller STOP EFC, Flash Controller STOP EAP, FATAL OFF" },
    { "2034", "Flash Controller OFF EFC, Flash Controller OFF EAP, FATAL OFF" },
    { "208A", "Flash Controller OFF EFC, Flash Controller OFF EAP, FATAL OFF" },
    { "210F", "Flash Controller OFF EFC, Flash Controller OFF EAP, FATAL OFF" },
    { "2028", "Flash Controller OFF EFC, Flash Controller OFF EAP, Flash Controller STOP EFC, Flash Controller STOP EAP, FATAL OFF" },
    { "201E", "Flash Controller OFF EFC, Flash Controller OFF EAP, FATAL OFF" },
    { "2046", "Flash Controller OFF EAP, Flash Controller STOP EFC, Flash Controller STOP EAP" },
    { "2048", "Flash Controller STOP EFC, Flash Controller STOP EAP" },
    { "204D", "Flash Controller STOP EAP" },
    { "2049", "Flash Controller SRAM Keep Enable" },
    { "2111", "ACDC 12V ON" },
    { "2113", "ACDC 12V ON" },
    { "2052", "ACDC 12V ON" },
    { "2085", "ACDC 12V ON" },
    { "2054", "ACDC 12V ON" },
    { "2087", "ACDC 12V ON" },
    { "216F", "USB VBUS On, USB VBUS Off, Dev USB VBUS On" },
    { "211B", "USB VBUS On, Dev USB VBUS On" },
    { "211D", "BD Drive Power On, Dev BD Drive Power On" },
    { "203A", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
    { "203D", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
    { "2126", "Main SoC Power ON Cold Boot, FATAL OFF" },
    { "2128", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
    { "212A", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
    { "2135", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF, Dev VBURN OFF" },
    { "211F", "Main SoC Power ON Cold Boot, GDDR6 USB Power On" },
    { "2189", "Main SoC Power ON Cold Boot, GDDR6 USB Power On" },
    { "218B", "Main SoC Power ON Cold Boot, GDDR6 USB Power On" },
    { "21B6", "Main SoC Power ON Cold Boot, GDDR6 USB Power On" },
    { "21B8", "Main SoC Power ON Cold Boot, GDDR6 USB Power On" },
    { "21BA", "Main SoC Power ON Cold Boot, GDDR6 USB Power On" },
    { "2023", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
    { "2125", "Main SoC Power ON Cold Boot, GDDR6 USB Power On" },
    { "2167", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
    { "21C1", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
    { "21C3", "Main SoC Power ON Cold Boot" },
    { "2121", "Main SoC Power ON Cold Boot" },
    { "21C5", "Main SoC Power ON Cold Boot" },
    { "2175", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
    { "2133", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
    { "2141", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
    { "205F", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
    { "218D", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
    { "21BE", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
    { "21C0", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
    { "21C4", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
    { "2123", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
    { "2136", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
    { "2137", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
    { "216D", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit" },
    { "2060", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
    { "2061", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
    { "2025", "Main SoC Power ON Cold Boot, Main SoC Power ON S3 Exit, Main SoC Power Off, FATAL OFF" },
    { "2127", "Main SoC Reset Release, Cold reset WA" },
    { "204A", "Main SoC Reset Release" },
    { "2129", "Main SoC Reset Release, Cold reset WA" },
    { "21A3", "Main SoC Reset Release, USB VBUS On 2, Dev USBA1 VBUS On" },
    { "21A5", "Main SoC Reset Release, USB VBUS On 2, Dev USBA2 VBUS On" },
    { "21A7", "Main SoC Reset Release, USB VBUS On 2, Dev USBA3 VBUS On" },
    { "21A9", "Main SoC Reset Release, USB VBUS On 2, Dev USBA1 VBUS On" },
    { "21AB", "Main SoC Reset Release, USB VBUS On 2, Dev USBA2 VBUS On" },
    { "21AD", "Main SoC Reset Release, USB VBUS On 2, Dev USBA3 VBUS On" },
    { "212F", "Main SoC Reset Release, Main SoC Power Off, FATAL OFF" },
    { "2169", "Main SoC Reset Release, Main SoC Power Off, FATAL OFF" },
    { "2161", "Main SoC Reset Release, Main SoC Power Off, FATAL OFF" },
    { "21B3", "Main SoC Reset Release, Main SoC Power Off, FATAL OFF" },
    { "21B5", "Main SoC Reset Release" },
    { "213C", "Main SoC Reset Release, Main SoC Power Off, FATAL OFF, Cold reset WA" },
    { "213D", "Main SoC Reset Release, Main SoC Power Off, FATAL OFF, Cold reset WA" },
    { "213F", "Main SoC Reset Release, Main SoC Power Off, FATAL OFF, Cold reset WA" },
    { "2050", "Main SoC Reset Release, Main SoC Power Off, FATAL OFF, Cold reset WA" },
    { "2083", "Main SoC Reset Release" },
    { "2187", "Main SoC Reset Release" },
    { "2195", "Main SoC Reset Release" },
    { "2197", "Main SoC Reset Release" },
    { "2155", "Main SoC Reset Release" },
    { "205C", "Main SoC Reset Release, Main SoC Power Off, FATAL OFF, Cold reset WA" },
    { "217F", "Main SoC Reset Release, Cold reset WA" },
    { "212B", "MSOC Reset Moni High, Main SoC Power Off, FATAL OFF" },
    { "2157", "MSOC Reset Moni High, Main SoC Power Off, FATAL OFF" },
    { "208F", "Main SoC Power Off, FATAL OFF" },
    { "2040", "Main SoC Power Off, FATAL OFF, FC NAND Close Not urgent" },
    { "2156", "Main SoC Power Off, FATAL OFF" },
    { "2196", "Main SoC Thermal Moni Stop, Main SoC Power Off, FATAL OFF" },
    { "2198", "Main SoC Thermal Moni Stop, Main SoC Power Off, FATAL OFF" },
    { "2188", "Main SoC Thermal Moni Stop, Main SoC Power Off, FATAL OFF" },
    { "2084", "Main SoC Thermal Moni Stop, Main SoC Power Off, FATAL OFF" },
    { "2051", "Main SoC Thermal Moni Stop, Main SoC Power Off, FATAL OFF, Cold reset WA" },
    { "211E", "BD Drive Power Off, FATAL OFF, Dev BD Drive Power Off" },
    { "211C", "USB VBUS Off, FATAL OFF" },
    { "2114", "ACDC 12V Off, FATAL OFF" },
    { "2112", "ACDC 12V Off, FATAL OFF" },
    { "207A", "ACDC 12V Off" },
    { "2086", "ACDC 12V Off, FATAL OFF" },
    { "2053", "ACDC 12V Off, FATAL OFF" },
    { "2088", "ACDC 12V Off, FATAL OFF" },
    { "2055", "ACDC 12V Off, FATAL OFF" },
    { "204B", "FC NAND Close Not urgent, FC NAND Close Urgent, FATAL OFF" },
    { "2042", "FC NAND Close Not urgent, FC NAND Close Urgent" },
    { "2044", "FC NAND Close Not urgent, FC NAND Close Urgent" },
    { "2024", "FATAL OFF" },
    { "2152", "USB OC Moni de assert, FATAL OFF" },
    { "2122", "FATAL OFF" },
    { "21AA", "FATAL OFF, USB OC Moni de assert 2, Dev USBA1 VBUS Off" },
    { "21AC", "FATAL OFF, USB OC Moni de assert 2, Dev USBA2 VBUS Off" },
    { "21AE", "FATAL OFF, USB OC Moni de assert 2, Dev USBA3 VBUS Off" },
    { "21A4", "FATAL OFF, USB VBUS Off 2, Dev USBA1 VBUS Off" },
    { "21A6", "FATAL OFF, USB VBUS Off 2, Dev USBA2 VBUS Off" },
    { "21A8", "FATAL OFF, USB VBUS Off 2, Dev USBA3 VBUS Off" },
    { "218C", "FATAL OFF" },
    { "218A", "FATAL OFF" },
    { "2120", "FATAL OFF" },
    { "2118", "FATAL OFF, Dev HDMI 5V Power Off" },
    { "2073", "FATAL OFF, HDMI CECStop" },
    { "2075", "FATAL OFF, HDMI CECStop, HDMIStop" },
    { "2079", "FATAL OFF, HDMI CECStop" },
    { "2071", "FATAL OFF, HDMI CECStop" },
    { "204F", "FATAL OFF, HDMI CECStop" },
    { "2022", "FATAL OFF, HDMI CECStop" },
    { "2116", "FATAL OFF, HDMI CECStop" },
    { "208C", "FATAL OFF" },
    { "2165", "FATAL OFF" },
    { "2164", "FATAL OFF" },
    { "216C", "FATAL OFF" },
    { "21B2", "FATAL OFF" },
    { "21B0", "FATAL OFF" },
    { "2012", "Stop SFlash DMA, FATAL OFF" },
    { "2091", "Local Temp.3 OFF, FATAL OFF" },
    { "2057", "Local Temp.3 OFF, FATAL OFF" },
    { "217E", "Fan Servo Parameter Reset, FATAL OFF" },
    { "2105", "WLAN Module Reset, FATAL OFF, WM Reset, Dev WLAN BT RESET ASSERT, Dev WLAN BT RESET ASSERT NEGATE" },
    { "2092", "FATAL OFF" },
    { "212D", "EAP Reset Moni de assert" },
    { "212E", "EAP Reset Moni Assert, FATAL OFF" },
    { "205D", "EAP Reset Moni Assert, Main SoC Power Off, FATAL OFF" },
    { "213B", "EAP Reset Moni Assert, Main SoC Power Off, FATAL OFF" },
    { "205E", "FAN CONTROL Parameter Reset" },
    { "2065", "EMC SoC Handshake ST" },
    { "2151", "USB OC Moni Assert" },
    { "2068", "HDMI Standby, HDMIStop" },
    { "2106", "WLAN Module USB Enable, WLAN Module Reset, WM Reset, Dev WLAN BT RESET NEGATE, Dev WLAN BT RESET ASSERT NEGATE" },
    { "217B", "WLAN Module Reset, BT WAKE Disabled, WM Reset, Dev WLAN BT RESET ASSERT, Dev WLAN BT RESET ASSERT NEGATE" },
    { "215A", "1GbE NIC Reset de assert" },
    { "215B", "1GbE NIC Reset assert" },
    { "2115", "HDMI CECStart, CECStart" },
    { "2021", "HDMI CECStart" },
    { "204E", "HDMI CECStart" },
    { "2070", "HDMI CECStart, CECStop" },
    { "2078", "HDMI CECStart, CECStop" },
    { "206E", "HDMI CECStart, CECStart" },
    { "2074", "HDMI CECStart" },
    { "2072", "HDMI CECStart" },
    { "2077", "HDMIStop, CECStop" },
    { "215F", "MDCDC ON" },
    { "2160", "MDCDC Off" },
    { "208E", "Titania2 GPIO Glitch Issue WA" },
    { "216E", "Check AC IN DETECT" },
    { "2170", "Check BD DETECT" },
    { "2173", "GPI SW Open" },
    { "2174", "GPI SW Close" },
    { "2102", "Devkit IO Expander Initialize" },
    { "2177", "Salina PMIC Register Initialize" },
    { "2178", "Disable AC IN DETECT" },
    { "2179", "BT WAKE Enabled" },
    { "2094", "Stop PCIePLL NoSS part" },
    { "217A", "Titania PMIC Register Initialize" },
    { "203B", "Setup FC for BTFW DL" },
    { "2039", "Setup FC for BTFW DL" },
    { "217C", "BTFW Download" },
    { "2095", "Telstar ROM Boot Wait" },
    { "201B", "Stop PCIePLL SS NOSS part, FATAL OFF" },
    { "2082", "Stop PCIePLL SS part" },
    { "2013", "Stop Subsystem PG2 Bus Error Detection(DDR4 BufferOverflow)" },
    { "2056", "Local Temp.3 ON" },
    { "2090", "Local Temp.3 ON" },
    { "2180", "FAN Control Start at Restmode during US" },
    { "2181", "FAN Control Start at Restmode during US" },
    { "2182", "FAN Control Start at Restmode during US" },
    { "2193", "FAN Control Start at Restmode during US" },
    { "2183", "FAN Control Stop at Restmode during USB" },
    { "2184", "FAN Control Stop at Restmode during USB" },
    { "2185", "FAN Control Stop at Restmode during USB" },
    { "2194", "FAN Control Stop at Restmode during USB" },
    { "2186", "Read Titania PMIC Registe" },
    { "219B", "I2C Open" },
    { "219C", "I2C Open" },
    { "219D", "I2C Open" },
    { "219E", "I2C Open" },
    { "2199", "I2C Open" },
    { "219A", "I2C Open" },
    { "21A0", "Drive FAN Control Stop" },
    { "219F", "Drive FAN Control Stop" },
    { "21A1", "Drive FAN Control Start" },
    { "21A2", "Drive FAN Control Start" },
    { "2117", "Dev HDMI 5V Power On" },
    { "2134", "Dev VBURN ON" },
    { "FFFF", "Unknown SeqNo" }
};

            return seqDatabase.ContainsKey(seqNo) ? seqDatabase[seqNo] : "Unknown Sequence";
        }

        private string ConvertToCelsius(string hexValue)
        {
            try
            {
                int tempValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                float tempCelsius = tempValue / 256.0f;
                return tempCelsius.ToString("F2");
            }
            catch
            {
                return "0.0";
            }
        }

        private string GetPowStateDescription(int powState)
        {
            var powStateMap = new Dictionary<int, string>
    {
        { 0x00, "ACIN_L Before Standby" },
        { 0x01, "STANDBY" },
        { 0x02, "PG2_ON" },
        { 0x03, "EFC_ON" },
        { 0x04, "EAP_ON" },
        { 0x05, "SOC_ON" },
        { 0x06, "ERROR_DET" },
        { 0x07, "FATAL_ERRO" },
        { 0x08, "NEVER_BOOT" },
        { 0x09, "FORCE_OFF" },
        { 0x0A, "FORCE_OFF BT Firmware Download" }
    };

            return powStateMap.ContainsKey(powState) ? powStateMap[powState] : "Unknown PowState";
        }

        private string GetUpCauseFlags(int upcauseValue)
        {
            var bootCauseMap = new Dictionary<int, string>
            {
                { 0x1A, "DEV UART" },
                { 0x13, "BT (Bluetooth)" },
                { 0x12, "CEC (HDMI-CEC)" },
                { 0x11, "EAP (EAP's order)" },
                { 0x10, "Main SoC" },
                { 0x0A, "Eject Button" },
                { 0x09, "Disc Loaded" },
                { 0x08, "Power Button" },
                { 0x00, "Boot-Up at power-on" }
            };


            return bootCauseMap.ContainsKey(upcauseValue) ? bootCauseMap[upcauseValue] : "Unknown Boot Cause";
        }
        private string GetDevPmFlags(int devpmValue)
        {
            var devPmMap = new Dictionary<int, string>
            {
                { 0x04, "HDMI(5V)" },
                { 0x03, "BD DRIVE" },
                { 0x02, "HDMI(CEC)" },
                { 0x01, "USB" },
                { 0x00, "WLAN" }
            };
            return devPmMap.ContainsKey(devpmValue) ? devPmMap[devpmValue] : "Unknown Device Power Management";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.Title = "Save Decoded Output";
            saveFileDialog.FileName = "Decoded_Output.txt";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, txtNorDecodeOutput.Text);
                    MessageBox.Show($"File saved successfully at: {saveFileDialog.FileName}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}