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

namespace PS5_NOR_Modifier
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void throwError(string errmsg)
        {
            MessageBox.Show(errmsg, "An Error Has Occurred", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        static SerialPort UARTSerial = new SerialPort();

        internal static byte CalculateChecksum(string data)
        {
            var checksum = 0;
            checksum = Encoding.ASCII.GetBytes(data).Sum(x => x);
            return (byte)((checksum + 256) % 256);
        }

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
        string WiFiMacValue = null;
        long LANMacOffset = 0x1C4020;
        string LANMacValue = null;
        string offsetOneValue = null;
        string offsetTwoValue = null;
        long serialOffset = 0x1c7210;
        string serialValue = null;
        long variantOffset = 0x1c7226;
        string variantValue = null;
        long moboSerialOffset = 0x1C7200;
        string moboSerialValue = null;

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

                        if(offsetOneValue.Contains("22020101"))
                        {
                            modelInfo.Text = "Disc Edition";
                        }
                        else
                        {
                            if(offsetTwoValue.Contains("22030101"))
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

                        if (boardVariant.Text.EndsWith("00A") || boardVariant.Text.EndsWith("00B"))
                        {
                            boardVariant.Text = boardVariant.Text + " - Japan";
                        }
                        else
                        {
                            if (boardVariant.Text.EndsWith("01A") || boardVariant.Text.EndsWith("01B"))
                            {
                                boardVariant.Text = boardVariant.Text + " - US, Canada, (North America)";
                            }
                            else
                            {
                                if (boardVariant.Text.EndsWith("15A") || boardVariant.Text.EndsWith("15B"))
                                {
                                    boardVariant.Text = boardVariant.Text + " - US, Canada, (North America)";
                                }
                                else
                                {
                                    if (boardVariant.Text.EndsWith("02A") || boardVariant.Text.EndsWith("02B"))
                                    {
                                        boardVariant.Text = boardVariant.Text + " - Australia / New Zealand, (Oceania)";
                                    }
                                    else
                                    {
                                        if (boardVariant.Text.EndsWith("03A") || boardVariant.Text.EndsWith("03B"))
                                        {
                                            boardVariant.Text = boardVariant.Text + " - United Kingdom / Ireland";
                                        }
                                        else
                                        {
                                            if (boardVariant.Text.EndsWith("04A") || boardVariant.Text.EndsWith("04B"))
                                            {
                                                boardVariant.Text = boardVariant.Text + " - Europe / Middle East / Africa";
                                            }
                                            else
                                            {
                                                if (boardVariant.Text.EndsWith("05A") || boardVariant.Text.EndsWith("05B"))
                                                {
                                                    boardVariant.Text = boardVariant.Text + " - South Korea";
                                                }
                                                else
                                                {
                                                    if (boardVariant.Text.EndsWith("06A") || boardVariant.Text.EndsWith("06B"))
                                                    {
                                                        boardVariant.Text = boardVariant.Text + " - Southeast Asia / Hong Kong";
                                                    }
                                                    else
                                                    {
                                                        if (boardVariant.Text.EndsWith("07A") || boardVariant.Text.EndsWith("07B"))
                                                        {
                                                            boardVariant.Text = boardVariant.Text + " - Taiwan";
                                                        }
                                                        else
                                                        {
                                                            if (boardVariant.Text.EndsWith("08A") || boardVariant.Text.EndsWith("08B"))
                                                            {
                                                                boardVariant.Text = boardVariant.Text + " - Russia, Ukraine, India, Central Asia";
                                                            }
                                                            else
                                                            {
                                                                if (boardVariant.Text.EndsWith("09A") || boardVariant.Text.EndsWith("09B"))
                                                                {
                                                                    boardVariant.Text = boardVariant.Text + " - Mainland China";
                                                                }
                                                                else
                                                                {
                                                                    if (boardVariant.Text.EndsWith("11A") || boardVariant.Text.EndsWith("11B"))
                                                                    {
                                                                        boardVariant.Text = boardVariant.Text + " - Mexico, Central America, South America";
                                                                    }
                                                                    else
                                                                    {
                                                                        if (boardVariant.Text.EndsWith("14A") || boardVariant.Text.EndsWith("14B"))
                                                                        {
                                                                            boardVariant.Text = boardVariant.Text + " - Mexico, Central America, South America";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (boardVariant.Text.EndsWith("16A") || boardVariant.Text.EndsWith("16B"))
                                                                            {
                                                                                boardVariant.Text = boardVariant.Text + " - Europe / Middle East / Africa";
                                                                            }
                                                                            else
                                                                            {
                                                                                if (boardVariant.Text.EndsWith("18A") || boardVariant.Text.EndsWith("18B"))
                                                                                {
                                                                                    boardVariant.Text = boardVariant.Text + " - Singapore, Korea, Asia";
                                                                                }
                                                                                else
                                                                                {
                                                                                    boardVariant.Text = boardVariant.Text + " - Unknown Region";
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

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

        private void button1_Click(object sender, EventArgs e)
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
                        UARTSerial.WriteLine(command);
                        do
                        {
                            var line = UARTSerial.ReadLine();
                            UARTLines.Add(line);
                        } while (UARTSerial.BytesToRead != 0);

                        foreach(var l in UARTLines)
                        {
                            txtUARTOutput.AppendText(l + Environment.NewLine);
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

        private void button3_Click(object sender, EventArgs e)
        {
            txtUARTOutput.Text = "";
        }
    }
}