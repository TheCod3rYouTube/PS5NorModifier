namespace PS5_NOR_Modifier.UserControls.NorModifier
{
    partial class NorModifierUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label5 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.fileLocationBox = new System.Windows.Forms.TextBox();
            this.lanMacAddressTextbox = new System.Windows.Forms.TextBox();
            this.browseFileButton = new System.Windows.Forms.Button();
            this.wifiMacAddressTextbox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.moboSerialInfo = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.LANMacAddressInfo = new System.Windows.Forms.Label();
            this.serialNumber = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.modelInfo = new System.Windows.Forms.Label();
            this.macAddressInfo = new System.Windows.Forms.Label();
            this.fileSizeInfo = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.boardVariant = new System.Windows.Forms.Label();
            this.boardModelSelectionBox = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.convertToDigitalEditionButton = new System.Windows.Forms.Button();
            this.serialNumberTextbox = new System.Windows.Forms.TextBox();
            this.boardVariantSelectionBox = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(182, 30);
            this.label5.TabIndex = 46;
            this.label5.Text = "Select NOR Dump";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(670, 378);
            this.label20.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(185, 30);
            this.label20.TabIndex = 75;
            this.label20.Text = "LAN Mac Address:";
            // 
            // fileLocationBox
            // 
            this.fileLocationBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileLocationBox.Location = new System.Drawing.Point(5, 42);
            this.fileLocationBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fileLocationBox.Name = "fileLocationBox";
            this.fileLocationBox.Size = new System.Drawing.Size(1226, 35);
            this.fileLocationBox.TabIndex = 47;
            // 
            // lanMacAddressTextbox
            // 
            this.lanMacAddressTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lanMacAddressTextbox.Enabled = false;
            this.lanMacAddressTextbox.Location = new System.Drawing.Point(865, 372);
            this.lanMacAddressTextbox.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.lanMacAddressTextbox.Name = "lanMacAddressTextbox";
            this.lanMacAddressTextbox.Size = new System.Drawing.Size(508, 35);
            this.lanMacAddressTextbox.TabIndex = 74;
            // 
            // browseFileButton
            // 
            this.browseFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseFileButton.Location = new System.Drawing.Point(1242, 40);
            this.browseFileButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.browseFileButton.Name = "browseFileButton";
            this.browseFileButton.Size = new System.Drawing.Size(134, 40);
            this.browseFileButton.TabIndex = 48;
            this.browseFileButton.Text = "Browse";
            this.browseFileButton.UseVisualStyleBackColor = true;
            this.browseFileButton.Click += new System.EventHandler(this.browseFileButton_Click);
            // 
            // wifiMacAddressTextbox
            // 
            this.wifiMacAddressTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wifiMacAddressTextbox.Enabled = false;
            this.wifiMacAddressTextbox.Location = new System.Drawing.Point(865, 314);
            this.wifiMacAddressTextbox.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.wifiMacAddressTextbox.Name = "wifiMacAddressTextbox";
            this.wifiMacAddressTextbox.Size = new System.Drawing.Size(508, 35);
            this.wifiMacAddressTextbox.TabIndex = 73;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(5, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(154, 30);
            this.label6.TabIndex = 49;
            this.label6.Text = "Dump Results:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(670, 320);
            this.label17.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(190, 30);
            this.label17.TabIndex = 72;
            this.label17.Text = "WiFi MAC Address:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 146);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(150, 30);
            this.label7.TabIndex = 50;
            this.label7.Text = "Serial Number:";
            // 
            // moboSerialInfo
            // 
            this.moboSerialInfo.AutoSize = true;
            this.moboSerialInfo.Location = new System.Drawing.Point(209, 204);
            this.moboSerialInfo.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.moboSerialInfo.Name = "moboSerialInfo";
            this.moboSerialInfo.Size = new System.Drawing.Size(28, 30);
            this.moboSerialInfo.TabIndex = 71;
            this.moboSerialInfo.Text = "...";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 320);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(117, 30);
            this.label9.TabIndex = 51;
            this.label9.Text = "PS5 Model:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(5, 204);
            this.label19.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(196, 30);
            this.label19.TabIndex = 70;
            this.label19.Text = "Motherboard Serial:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(5, 380);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(92, 30);
            this.label10.TabIndex = 52;
            this.label10.Text = "File Size:";
            // 
            // LANMacAddressInfo
            // 
            this.LANMacAddressInfo.AutoSize = true;
            this.LANMacAddressInfo.Location = new System.Drawing.Point(209, 506);
            this.LANMacAddressInfo.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.LANMacAddressInfo.Name = "LANMacAddressInfo";
            this.LANMacAddressInfo.Size = new System.Drawing.Size(28, 30);
            this.LANMacAddressInfo.TabIndex = 69;
            this.LANMacAddressInfo.Text = "...";
            // 
            // serialNumber
            // 
            this.serialNumber.AutoSize = true;
            this.serialNumber.Location = new System.Drawing.Point(209, 146);
            this.serialNumber.Name = "serialNumber";
            this.serialNumber.Size = new System.Drawing.Size(28, 30);
            this.serialNumber.TabIndex = 53;
            this.serialNumber.Text = "...";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(5, 506);
            this.label18.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(185, 30);
            this.label18.TabIndex = 68;
            this.label18.Text = "LAN Mac Address:";
            // 
            // modelInfo
            // 
            this.modelInfo.AutoSize = true;
            this.modelInfo.Location = new System.Drawing.Point(209, 320);
            this.modelInfo.Name = "modelInfo";
            this.modelInfo.Size = new System.Drawing.Size(28, 30);
            this.modelInfo.TabIndex = 54;
            this.modelInfo.Text = "...";
            // 
            // macAddressInfo
            // 
            this.macAddressInfo.AutoSize = true;
            this.macAddressInfo.Location = new System.Drawing.Point(209, 444);
            this.macAddressInfo.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.macAddressInfo.Name = "macAddressInfo";
            this.macAddressInfo.Size = new System.Drawing.Size(28, 30);
            this.macAddressInfo.TabIndex = 67;
            this.macAddressInfo.Text = "...";
            // 
            // fileSizeInfo
            // 
            this.fileSizeInfo.AutoSize = true;
            this.fileSizeInfo.Location = new System.Drawing.Point(209, 380);
            this.fileSizeInfo.Name = "fileSizeInfo";
            this.fileSizeInfo.Size = new System.Drawing.Size(28, 30);
            this.fileSizeInfo.TabIndex = 55;
            this.fileSizeInfo.Text = "...";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(5, 444);
            this.label16.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(184, 30);
            this.label16.TabIndex = 66;
            this.label16.Text = "WiFi Mac Address:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 262);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(142, 30);
            this.label8.TabIndex = 56;
            this.label8.Text = "Board Variant:";
            // 
            // boardVariant
            // 
            this.boardVariant.AutoSize = true;
            this.boardVariant.Location = new System.Drawing.Point(209, 262);
            this.boardVariant.Name = "boardVariant";
            this.boardVariant.Size = new System.Drawing.Size(28, 30);
            this.boardVariant.TabIndex = 57;
            this.boardVariant.Text = "...";
            // 
            // boardModelSelectionBox
            // 
            this.boardModelSelectionBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boardModelSelectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.boardModelSelectionBox.FormattingEnabled = true;
            this.boardModelSelectionBox.Items.AddRange(new object[] {
            "Slim",
            "Digital Edition",
            "Disc Edition"});
            this.boardModelSelectionBox.Location = new System.Drawing.Point(865, 256);
            this.boardModelSelectionBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.boardModelSelectionBox.Name = "boardModelSelectionBox";
            this.boardModelSelectionBox.Size = new System.Drawing.Size(508, 38);
            this.boardModelSelectionBox.TabIndex = 65;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label11.Location = new System.Drawing.Point(670, 100);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(152, 30);
            this.label11.TabIndex = 58;
            this.label11.Text = "Modify Values";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(670, 260);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(117, 30);
            this.label14.TabIndex = 64;
            this.label14.Text = "PS5 Model:";
            // 
            // convertToDigitalEditionButton
            // 
            this.convertToDigitalEditionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.convertToDigitalEditionButton.Location = new System.Drawing.Point(1119, 444);
            this.convertToDigitalEditionButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.convertToDigitalEditionButton.Name = "convertToDigitalEditionButton";
            this.convertToDigitalEditionButton.Size = new System.Drawing.Size(257, 92);
            this.convertToDigitalEditionButton.TabIndex = 59;
            this.convertToDigitalEditionButton.Text = "Save New\r\nBIOS Information";
            this.convertToDigitalEditionButton.UseVisualStyleBackColor = true;
            this.convertToDigitalEditionButton.Click += new System.EventHandler(this.convertToDigitalEditionButton_Click);
            // 
            // serialNumberTextbox
            // 
            this.serialNumberTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serialNumberTextbox.Location = new System.Drawing.Point(865, 142);
            this.serialNumberTextbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.serialNumberTextbox.Name = "serialNumberTextbox";
            this.serialNumberTextbox.Size = new System.Drawing.Size(508, 35);
            this.serialNumberTextbox.TabIndex = 63;
            // 
            // boardVariantSelectionBox
            // 
            this.boardVariantSelectionBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boardVariantSelectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.boardVariantSelectionBox.FormattingEnabled = true;
            this.boardVariantSelectionBox.Items.AddRange(new object[] {
            "CFI-1000A",
            "CFI-1000A01",
            "CFI-1000B",
            "CFI-1001A",
            "CFI-1001B",
            "CFI-1002A",
            "CFI-1002B",
            "CFI-1003A",
            "CFI-1003B",
            "CFI-1004A",
            "CFI-1004B",
            "CFI-1005A",
            "CFI-1005B",
            "CFI-1006A",
            "CFI-1006B",
            "CFI-1007A",
            "CFI-1007B",
            "CFI-1008A",
            "CFI-1008B",
            "CFI-1009A",
            "CFI-1009B",
            "CFI-1011A",
            "CFI-1011B",
            "CFI-1014A",
            "CFI-1014B",
            "CFI-1015A",
            "CFI-1015B",
            "CFI-1016A",
            "CFI-1016B",
            "CFI-1018A",
            "CFI-1018B",
            "CFI-1100A",
            "CFI-1100A01",
            "CFI-1100B",
            "CFI-1101A",
            "CFI-1101B",
            "CFI-1102A",
            "CFI-1102B",
            "CFI-1103A",
            "CFI-1103B",
            "CFI-1104A",
            "CFI-1104B",
            "CFI-1105A",
            "CFI-1105B",
            "CFI-1106A",
            "CFI-1106B",
            "CFI-1107A",
            "CFI-1107B",
            "CFI-1108A",
            "CFI-1108B",
            "CFI-1109A",
            "CFI-1109B",
            "CFI-1111A",
            "CFI-1111B",
            "CFI-1114A",
            "CFI-1114B",
            "CFI-1115A",
            "CFI-1115B",
            "CFI-1116A",
            "CFI-1116B",
            "CFI-1118A",
            "CFI-1118B",
            "CFI-1200A",
            "CFI-1200A01",
            "CFI-1200B",
            "CFI-1201A",
            "CFI-1201B",
            "CFI-1202A",
            "CFI-1202B",
            "CFI-1203A",
            "CFI-1203B",
            "CFI-1204A",
            "CFI-1204B",
            "CFI-1205A",
            "CFI-1205B",
            "CFI-1206A",
            "CFI-1206B",
            "CFI-1207A",
            "CFI-1207B",
            "CFI-1208A",
            "CFI-1208B",
            "CFI-1209A",
            "CFI-1209B",
            "CFI-1211A",
            "CFI-1211B",
            "CFI-1214A",
            "CFI-1214B",
            "CFI-1215A",
            "CFI-1215B",
            "CFI-1216A",
            "CFI-1216B",
            "CFI-1218A",
            "CFI-1218B",
            "CFI-2000A",
            "CFI-2000A01",
            "CFI-2000B",
            "CFI-2001A",
            "CFI-2001B",
            "CFI-2002A",
            "CFI-2002B",
            "CFI-2003A",
            "CFI-2003B",
            "CFI-2004A",
            "CFI-2004B",
            "CFI-2005A",
            "CFI-2005B",
            "CFI-2006A",
            "CFI-2006B",
            "CFI-2007A",
            "CFI-2007B",
            "CFI-2008A",
            "CFI-2008B",
            "CFI-2009A",
            "CFI-2009B",
            "CFI-2011A",
            "CFI-2011B",
            "CFI-2014A",
            "CFI-2014B",
            "CFI-2015A",
            "CFI-2015B",
            "CFI-2016A",
            "CFI-2016B",
            "CFI-2018A",
            "CFI-2018B",
            "DFI-T1000AA",
            "DFI-D1000AA"});
            this.boardVariantSelectionBox.Location = new System.Drawing.Point(865, 198);
            this.boardVariantSelectionBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.boardVariantSelectionBox.Name = "boardVariantSelectionBox";
            this.boardVariantSelectionBox.Size = new System.Drawing.Size(508, 38);
            this.boardVariantSelectionBox.TabIndex = 60;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(670, 202);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(142, 30);
            this.label13.TabIndex = 62;
            this.label13.Text = "Board Variant:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(670, 146);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(150, 30);
            this.label12.TabIndex = 61;
            this.label12.Text = "Serial Number:";
            // 
            // NorModifierUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.fileLocationBox);
            this.Controls.Add(this.lanMacAddressTextbox);
            this.Controls.Add(this.browseFileButton);
            this.Controls.Add(this.wifiMacAddressTextbox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.moboSerialInfo);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.LANMacAddressInfo);
            this.Controls.Add(this.serialNumber);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.modelInfo);
            this.Controls.Add(this.macAddressInfo);
            this.Controls.Add(this.fileSizeInfo);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.boardVariant);
            this.Controls.Add(this.boardModelSelectionBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.convertToDigitalEditionButton);
            this.Controls.Add(this.serialNumberTextbox);
            this.Controls.Add(this.boardVariantSelectionBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Name = "NorModifierUserControl";
            this.Size = new System.Drawing.Size(1382, 543);
            this.Load += new System.EventHandler(this.NorModifierUserControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label5;
        private Label label20;
        private TextBox fileLocationBox;
        private TextBox lanMacAddressTextbox;
        private Button browseFileButton;
        private TextBox wifiMacAddressTextbox;
        private Label label6;
        private Label label17;
        private Label label7;
        private Label moboSerialInfo;
        private Label label9;
        private Label label19;
        private Label label10;
        private Label LANMacAddressInfo;
        private Label serialNumber;
        private Label label18;
        private Label modelInfo;
        private Label macAddressInfo;
        private Label fileSizeInfo;
        private Label label16;
        private Label label8;
        private Label boardVariant;
        private ComboBox boardModelSelectionBox;
        private Label label11;
        private Label label14;
        private Button convertToDigitalEditionButton;
        private TextBox serialNumberTextbox;
        private ComboBox boardVariantSelectionBox;
        private Label label13;
        private Label label12;
    }
}
