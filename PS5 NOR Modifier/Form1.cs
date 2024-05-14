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
            if (ports.Length > 0)
            {
                comboComPorts.Items.Clear();
                comboComPorts.Items.AddRange(ports);
                comboComPorts.SelectedIndex = 0;
                btnConnectCom.Enabled = true;
                btnDisconnectCom.Enabled = false;
            }
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
                    if (root is null) {
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
                                string description = errorCodeNode.SelectSingleNode("Description")?.InnerText??"";

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
                                string errorCodeValue = errorCodeNode.SelectSingleNode("ErrorCode")?.InnerText??"";
                                string description = errorCodeNode.SelectSingleNode("Description")?.InnerText??"";

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
                if(fileDialogBox.CheckFileExists == false)
                {
                    throwError("The file you selected could not be found. Please check the file exists and is a valid BIN file.");
                }
                else
                {
                    if(!fileDialogBox.SafeFileName.EndsWith(".bin"))
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



                        if(moboSerialValue != null)
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
                if(boardModelSelectionBox.Text == "")
                {
                    throwError("Please select a valid board model before saving new BIOS information!");
                    errorShownAlready = true;
                }
                else
                {
                    if(boardVariantSelectionBox.Text == "")
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
                            catch(System.ArgumentException ex)
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

            if(File.Exists(fileNameToLookFor) && errorShownAlready == false)
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
            if (ports.Length > 0)
            {
                comboComPorts.Items.Clear();
                comboComPorts.Items.AddRange(ports);
                comboComPorts.SelectedIndex = 0;
                btnConnectCom.Enabled = true;
                btnDisconnectCom.Enabled = false;
            }
            else MessageBox.Show("Not existing COM ports were found!", "COM Ports", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if(UARTSerial.IsOpen == true)
            {
                try
                {
                    UARTSerial.Close();
                }
                catch(Exception ex)
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

            if(result == DialogResult.Yes)
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
            if(e.KeyChar == (char)Keys.Enter)
            {
                btnSendCommand.PerformClick();
            }
        }
    }
}