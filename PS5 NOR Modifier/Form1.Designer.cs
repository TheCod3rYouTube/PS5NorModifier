namespace PS5_NOR_Modifier
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.fileLocationBox = new System.Windows.Forms.TextBox();
            this.browseFileButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.serialNumber = new System.Windows.Forms.Label();
            this.modelInfo = new System.Windows.Forms.Label();
            this.fileSizeInfo = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.label8 = new System.Windows.Forms.Label();
            this.boardVariant = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.convertToDigitalEditionButton = new System.Windows.Forms.Button();
            this.boardVariantSelectionBox = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.serialNumberTextbox = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.boardModelSelectionBox = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.macAddressInfo = new System.Windows.Forms.Label();
            this.LANMacAddressInfo = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.moboSerialInfo = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.wifiMacAddressTextbox = new System.Windows.Forms.TextBox();
            this.lanMacAddressTextbox = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ucUART = new PS5_NOR_Modifier.UserControls.UART.UartUserControl();
            this.label23 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 168);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1352, 90);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::PS5_NOR_Modifier.Properties.Resources.PS5_Nor_Logo;
            this.pictureBox1.Location = new System.Drawing.Point(14, 14);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(201, 150);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(240, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(476, 70);
            this.label2.TabIndex = 2;
            this.label2.Text = "PS5 NOR Modifier";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::PS5_NOR_Modifier.Properties.Resources.Paypal_128;
            this.pictureBox2.Location = new System.Drawing.Point(14, 918);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(77, 76);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(111, 918);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(1227, 60);
            this.label4.TabIndex = 5;
            this.label4.Text = resources.GetString("label4.Text");
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(182, 30);
            this.label5.TabIndex = 6;
            this.label5.Text = "Select NOR Dump";
            // 
            // fileLocationBox
            // 
            this.fileLocationBox.Location = new System.Drawing.Point(9, 40);
            this.fileLocationBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fileLocationBox.Name = "fileLocationBox";
            this.fileLocationBox.Size = new System.Drawing.Size(1226, 35);
            this.fileLocationBox.TabIndex = 7;
            // 
            // browseFileButton
            // 
            this.browseFileButton.Location = new System.Drawing.Point(1246, 38);
            this.browseFileButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.browseFileButton.Name = "browseFileButton";
            this.browseFileButton.Size = new System.Drawing.Size(134, 40);
            this.browseFileButton.TabIndex = 8;
            this.browseFileButton.Text = "Browse";
            this.browseFileButton.UseVisualStyleBackColor = true;
            this.browseFileButton.Click += new System.EventHandler(this.browseFileButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(9, 98);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(154, 30);
            this.label6.TabIndex = 9;
            this.label6.Text = "Dump Results:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 144);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(150, 30);
            this.label7.TabIndex = 10;
            this.label7.Text = "Serial Number:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 318);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(117, 30);
            this.label9.TabIndex = 12;
            this.label9.Text = "PS5 Model:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 378);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(92, 30);
            this.label10.TabIndex = 13;
            this.label10.Text = "File Size:";
            // 
            // serialNumber
            // 
            this.serialNumber.AutoSize = true;
            this.serialNumber.Location = new System.Drawing.Point(213, 144);
            this.serialNumber.Name = "serialNumber";
            this.serialNumber.Size = new System.Drawing.Size(28, 30);
            this.serialNumber.TabIndex = 14;
            this.serialNumber.Text = "...";
            // 
            // modelInfo
            // 
            this.modelInfo.AutoSize = true;
            this.modelInfo.Location = new System.Drawing.Point(213, 318);
            this.modelInfo.Name = "modelInfo";
            this.modelInfo.Size = new System.Drawing.Size(28, 30);
            this.modelInfo.TabIndex = 16;
            this.modelInfo.Text = "...";
            // 
            // fileSizeInfo
            // 
            this.fileSizeInfo.AutoSize = true;
            this.fileSizeInfo.Location = new System.Drawing.Point(213, 378);
            this.fileSizeInfo.Name = "fileSizeInfo";
            this.fileSizeInfo.Size = new System.Drawing.Size(28, 30);
            this.fileSizeInfo.TabIndex = 17;
            this.fileSizeInfo.Text = "...";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 1075);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 17, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1452, 39);
            this.statusStrip1.TabIndex = 18;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(237, 30);
            this.toolStripStatusLabel1.Text = "Status: Waiting for input";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 260);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(142, 30);
            this.label8.TabIndex = 20;
            this.label8.Text = "Board Variant:";
            // 
            // boardVariant
            // 
            this.boardVariant.AutoSize = true;
            this.boardVariant.Location = new System.Drawing.Point(213, 260);
            this.boardVariant.Name = "boardVariant";
            this.boardVariant.Size = new System.Drawing.Size(28, 30);
            this.boardVariant.TabIndex = 21;
            this.boardVariant.Text = "...";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label11.Location = new System.Drawing.Point(674, 98);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(152, 30);
            this.label11.TabIndex = 22;
            this.label11.Text = "Modify Values";
            // 
            // convertToDigitalEditionButton
            // 
            this.convertToDigitalEditionButton.Location = new System.Drawing.Point(1123, 442);
            this.convertToDigitalEditionButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.convertToDigitalEditionButton.Name = "convertToDigitalEditionButton";
            this.convertToDigitalEditionButton.Size = new System.Drawing.Size(257, 92);
            this.convertToDigitalEditionButton.TabIndex = 23;
            this.convertToDigitalEditionButton.Text = "Save New\r\nBIOS Information";
            this.convertToDigitalEditionButton.UseVisualStyleBackColor = true;
            this.convertToDigitalEditionButton.Click += new System.EventHandler(this.convertToDigitalEditionButton_Click);
            // 
            // boardVariantSelectionBox
            // 
            this.boardVariantSelectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.boardVariantSelectionBox.FormattingEnabled = true;
            this.boardVariantSelectionBox.Items.AddRange(new object[] {
            "CFI-1000A",
            "CFI-1000A01",
            "CFI-1000B",
            "CFI-1002A",
            "CFI-1008A",
            "CFI-1014A",
            "CFI-1015A",
            "CFI-1015B",
            "CFI-1016A",
            "CFI-1018A",
            "CFI-1100A01",
            "CFI-1102A",
            "CFI-1108A",
            "CFI-1109A",
            "CFI-1114A",
            "CFI-1115A",
            "CFI-1116A",
            "CFI-1118A",
            "CFI-1208A",
            "CFI-1215A",
            "CFI-1216A",
            "DFI-T1000AA",
            "DFI-D1000AA"});
            this.boardVariantSelectionBox.Location = new System.Drawing.Point(869, 196);
            this.boardVariantSelectionBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.boardVariantSelectionBox.Name = "boardVariantSelectionBox";
            this.boardVariantSelectionBox.Size = new System.Drawing.Size(508, 38);
            this.boardVariantSelectionBox.TabIndex = 29;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(674, 144);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(150, 30);
            this.label12.TabIndex = 30;
            this.label12.Text = "Serial Number:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(674, 200);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(142, 30);
            this.label13.TabIndex = 31;
            this.label13.Text = "Board Variant:";
            // 
            // serialNumberTextbox
            // 
            this.serialNumberTextbox.Location = new System.Drawing.Point(869, 140);
            this.serialNumberTextbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.serialNumberTextbox.Name = "serialNumberTextbox";
            this.serialNumberTextbox.Size = new System.Drawing.Size(508, 35);
            this.serialNumberTextbox.TabIndex = 32;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(674, 258);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(117, 30);
            this.label14.TabIndex = 33;
            this.label14.Text = "PS5 Model:";
            // 
            // boardModelSelectionBox
            // 
            this.boardModelSelectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.boardModelSelectionBox.FormattingEnabled = true;
            this.boardModelSelectionBox.Items.AddRange(new object[] {
            "Digital Edition",
            "Disc Edition"});
            this.boardModelSelectionBox.Location = new System.Drawing.Point(869, 254);
            this.boardModelSelectionBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.boardModelSelectionBox.Name = "boardModelSelectionBox";
            this.boardModelSelectionBox.Size = new System.Drawing.Size(508, 38);
            this.boardModelSelectionBox.TabIndex = 34;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label15.Location = new System.Drawing.Point(111, 990);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(666, 38);
            this.label15.TabIndex = 35;
            this.label15.Text = "This project is sponsored by www.consolefix.shop";
            this.label15.Click += new System.EventHandler(this.label15_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 442);
            this.label16.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(184, 30);
            this.label16.TabIndex = 36;
            this.label16.Text = "WiFi Mac Address:";
            // 
            // macAddressInfo
            // 
            this.macAddressInfo.AutoSize = true;
            this.macAddressInfo.Location = new System.Drawing.Point(213, 442);
            this.macAddressInfo.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.macAddressInfo.Name = "macAddressInfo";
            this.macAddressInfo.Size = new System.Drawing.Size(28, 30);
            this.macAddressInfo.TabIndex = 37;
            this.macAddressInfo.Text = "...";
            // 
            // LANMacAddressInfo
            // 
            this.LANMacAddressInfo.AutoSize = true;
            this.LANMacAddressInfo.Location = new System.Drawing.Point(213, 504);
            this.LANMacAddressInfo.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.LANMacAddressInfo.Name = "LANMacAddressInfo";
            this.LANMacAddressInfo.Size = new System.Drawing.Size(28, 30);
            this.LANMacAddressInfo.TabIndex = 39;
            this.LANMacAddressInfo.Text = "...";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(9, 504);
            this.label18.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(185, 30);
            this.label18.TabIndex = 38;
            this.label18.Text = "LAN Mac Address:";
            // 
            // moboSerialInfo
            // 
            this.moboSerialInfo.AutoSize = true;
            this.moboSerialInfo.Location = new System.Drawing.Point(213, 202);
            this.moboSerialInfo.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.moboSerialInfo.Name = "moboSerialInfo";
            this.moboSerialInfo.Size = new System.Drawing.Size(28, 30);
            this.moboSerialInfo.TabIndex = 41;
            this.moboSerialInfo.Text = "...";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(9, 202);
            this.label19.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(196, 30);
            this.label19.TabIndex = 40;
            this.label19.Text = "Motherboard Serial:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(674, 318);
            this.label17.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(190, 30);
            this.label17.TabIndex = 42;
            this.label17.Text = "WiFi MAC Address:";
            // 
            // wifiMacAddressTextbox
            // 
            this.wifiMacAddressTextbox.Enabled = false;
            this.wifiMacAddressTextbox.Location = new System.Drawing.Point(869, 312);
            this.wifiMacAddressTextbox.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.wifiMacAddressTextbox.Name = "wifiMacAddressTextbox";
            this.wifiMacAddressTextbox.Size = new System.Drawing.Size(508, 35);
            this.wifiMacAddressTextbox.TabIndex = 43;
            // 
            // lanMacAddressTextbox
            // 
            this.lanMacAddressTextbox.Enabled = false;
            this.lanMacAddressTextbox.Location = new System.Drawing.Point(869, 370);
            this.lanMacAddressTextbox.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.lanMacAddressTextbox.Name = "lanMacAddressTextbox";
            this.lanMacAddressTextbox.Size = new System.Drawing.Size(508, 35);
            this.lanMacAddressTextbox.TabIndex = 44;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(674, 376);
            this.label20.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(185, 30);
            this.label20.TabIndex = 45;
            this.label20.Text = "LAN Mac Address:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(21, 264);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1407, 622);
            this.tabControl1.TabIndex = 46;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label20);
            this.tabPage1.Controls.Add(this.fileLocationBox);
            this.tabPage1.Controls.Add(this.lanMacAddressTextbox);
            this.tabPage1.Controls.Add(this.browseFileButton);
            this.tabPage1.Controls.Add(this.wifiMacAddressTextbox);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label17);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.moboSerialInfo);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.label19);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.LANMacAddressInfo);
            this.tabPage1.Controls.Add(this.serialNumber);
            this.tabPage1.Controls.Add(this.label18);
            this.tabPage1.Controls.Add(this.modelInfo);
            this.tabPage1.Controls.Add(this.macAddressInfo);
            this.tabPage1.Controls.Add(this.fileSizeInfo);
            this.tabPage1.Controls.Add(this.label16);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.boardVariant);
            this.tabPage1.Controls.Add(this.boardModelSelectionBox);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.convertToDigitalEditionButton);
            this.tabPage1.Controls.Add(this.serialNumberTextbox);
            this.tabPage1.Controls.Add(this.boardVariantSelectionBox);
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Location = new System.Drawing.Point(4, 39);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tabPage1.Size = new System.Drawing.Size(1399, 579);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "NOR Modifier";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ucUART);
            this.tabPage2.Location = new System.Drawing.Point(4, 39);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tabPage2.Size = new System.Drawing.Size(1399, 579);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "UART Communication";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ucUART
            // 
            this.ucUART.Location = new System.Drawing.Point(0, 0);
            this.ucUART.Name = "ucUART";
            this.ucUART.Size = new System.Drawing.Size(1391, 549);
            this.ucUART.TabIndex = 0;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.label23.Location = new System.Drawing.Point(331, 96);
            this.label23.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(383, 30);
            this.label23.TabIndex = 47;
            this.label23.Text = "and UART stuff too... BwE can SUCK IT!";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1452, 1114);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "PS5 NOR Modifier";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private PictureBox pictureBox1;
        private Label label2;
        private PictureBox pictureBox2;
        private Label label4;
        private Label label5;
        private TextBox fileLocationBox;
        private Button browseFileButton;
        private Label label6;
        private Label label7;
        private Label label9;
        private Label label10;
        private Label serialNumber;
        private Label modelInfo;
        private Label fileSizeInfo;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private Label label8;
        private Label boardVariant;
        private Label label11;
        private Button convertToDigitalEditionButton;
        private ComboBox boardVariantSelectionBox;
        private Label label12;
        private Label label13;
        private TextBox serialNumberTextbox;
        private Label label14;
        private ComboBox boardModelSelectionBox;
        private Label label15;
        private Label label16;
        private Label macAddressInfo;
        private Label LANMacAddressInfo;
        private Label label18;
        private Label moboSerialInfo;
        private Label label19;
        private Label label17;
        private TextBox wifiMacAddressTextbox;
        private TextBox lanMacAddressTextbox;
        private Label label20;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label23;
        private UserControls.UART.UartUserControl ucUART;
    }
}