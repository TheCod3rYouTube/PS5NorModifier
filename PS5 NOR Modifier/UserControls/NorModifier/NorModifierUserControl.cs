using System;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using PS5_NOR_Modifier.UserControls.Events;

namespace PS5_NOR_Modifier.UserControls.NorModifier
{
    public partial class NorModifierUserControl : UserControl
    {
        public event EventHandler<StatusUpdateEventArgs>? statusUpdateEvent;

        private const string NO_VALUE = "Unknown";

        private const long WIFI_OFFSET = 0x1C73C0;
        private const long SN_OFFSET = 0x1c7210;
        private const long MB_SN_OFFSET = 0x1C7200;
        private const long LAN_MAC_OFFSET = 0x1C4020;
        private const long MODEL_OFFSET = 0x1C7011;
        private const long REGION_OFFSET = 0x1C7236;

        private readonly Dictionary<string, string> _regions = new Dictionary<string, string>()
        {
            { "00", "Japan" },
            { "01", "US, Canada, (North America)" },
            { "15", "US, Canada, (North America)" },
            { "02", "Australia / New Zealand, (Oceania)" },
            { "03", "United Kingdom / Ireland" },
            { "04", "Europe / Middle East / Africa" },
            { "05", "South Korea" },
            { "06", "Southeast Asia / Hong Kong" },
            { "07", "Taiwan" },
            { "08", "Russia, Ukraine, India, Central Asia" },
            { "09", "Mainland China" },
            { "11", "Mexico, Central America, South America" },
            { "14", "Mexico, Central America, South America" },
            { "16", "Europe / Middle East / Africa" },
            { "18", "Singapore, Korea, Asia" },
        };

        public NorModifierUserControl()
        {
            InitializeComponent();
        }

        private void throwError(string errmsg)
        {
            MessageBox.Show(errmsg, "An Error Has Occurred", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void UpdateStatus(string newStatus)
        {
            if (statusUpdateEvent != null)
            {
                statusUpdateEvent(this, new StatusUpdateEventArgs(newStatus));
            }
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

        private void ResetAppFields()
        {
            fileLocationBox.Text = "";
            serialNumber.Text = "...";
            boardVariant.Text = "...";
            modelInfo.Text = "...";
            fileSizeInfo.Text = "...";
            serialNumberTextbox.Text = "";
            UpdateStatus( "Status: Waiting for input");
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

        private byte[]? ReadStreamBytes(BinaryReader reader, long offset, int length)
        {
            byte[]? result = null;

            try
            {
                reader.BaseStream.Position = offset;
                result = reader.ReadBytes(length);
            }
            catch 
            {
                //Hmm, something is wrong, but continue
            }

            return result;
        }

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
                    if (!fileDialogBox.SafeFileName.ToLower().EndsWith(".bin"))
                    {
                        throwError("The file you selected is not a valid. Please ensure the file you are choosing is a correct BIN file and try again.");
                    }
                    else
                    {
                        // Let's load simple information first, before loading BIN specific data
                        fileLocationBox.Text = "";
                        // Get the path selected and print it into the path box
                        string selectedPath = fileDialogBox.FileName;
                        UpdateStatus("Status: Selected file " + selectedPath);
                        fileLocationBox.Text = selectedPath;

                        // Get file length and show in bytes and MB
                        long length = new FileInfo(selectedPath).Length;
                        fileSizeInfo.Text = length.ToString() + " bytes (" + length / 1024 / 1024 + "MB)";

                        using (BinaryReader reader = new BinaryReader(new FileStream(fileDialogBox.FileName, FileMode.Open)))
                        {
                            //Reading Motherboard Serial
                            byte[]? rawBytes = ReadStreamBytes(reader, MB_SN_OFFSET, 16);

                            moboSerialInfo.Text = NO_VALUE;

                            if (rawBytes != null)
                            {
                                string mbSerialValue = BitConverter.ToString(rawBytes).Replace("-", null);
                                moboSerialInfo.Text = HexStringToString(mbSerialValue);
                            }

                            //Reading serial number
                            rawBytes = ReadStreamBytes(reader, SN_OFFSET, 17);

                            serialNumber.Text = NO_VALUE;
                            serialNumberTextbox.Text = NO_VALUE;

                            if (rawBytes != null)
                            {
                                string sn = BitConverter.ToString(rawBytes).Replace("-", null);
                                string readableSn = HexStringToString(sn);
                                serialNumber.Text = readableSn;
                                serialNumberTextbox.Text = readableSn;
                            }

                            //Reading WIFI
                            rawBytes = ReadStreamBytes(reader, WIFI_OFFSET, 6);

                            macAddressInfo.Text = NO_VALUE;
                            wifiMacAddressTextbox.Text = NO_VALUE;

                            if (rawBytes != null)
                            {
                                string wifi = BitConverter.ToString(rawBytes);
                                macAddressInfo.Text = wifi;
                                wifiMacAddressTextbox.Text = wifi;
                            }

                            //Reading LAN
                            rawBytes = ReadStreamBytes(reader, LAN_MAC_OFFSET, 6);

                            LANMacAddressInfo.Text = NO_VALUE;
                            lanMacAddressTextbox.Text = NO_VALUE;

                            if (rawBytes != null)
                            {
                                string lan = BitConverter.ToString(rawBytes);
                                LANMacAddressInfo.Text = lan;
                                lanMacAddressTextbox.Text = lan;
                            }

                            //Region
                            rawBytes = ReadStreamBytes(reader, REGION_OFFSET, 2);

                            boardVariant.Text = NO_VALUE;

                            if (rawBytes != null)
                            {
                                string regionHex = BitConverter.ToString(rawBytes).Replace("-", null);
                                string regionReadable = HexStringToString(regionHex);

                                if (_regions.Keys.Contains(regionReadable))
                                {
                                    boardVariant.Text = String.Format("{0} - {1}", regionReadable, _regions[regionReadable]);
                                }
                            }

                            //Reading Model
                            rawBytes = ReadStreamBytes(reader, MODEL_OFFSET, 1);

                            if (rawBytes != null)
                            {
                                string model = BitConverter.ToString(rawBytes).Replace("-", null);

                                string modelText = "Unknown";

                                if (model == "01")
                                {
                                    modelText = "Slim";
                                }
                                else if (model == "02")
                                {
                                    modelText = "Disk Edition";
                                }
                                else if (model == "03")
                                {
                                    modelText = "Digital Edition";
                                }

                                modelInfo.Text = modelText;
                            }
                        }
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

        private void NorModifierUserControl_Load(object sender, EventArgs e)
        {
            ResetAppFields();
        }
    }
}
