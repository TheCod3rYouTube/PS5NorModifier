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
            this.btnSendCommand = new System.Windows.Forms.Button();
            this.txtCustomCommand = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.chkUseOffline = new System.Windows.Forms.CheckBox();
            this.btnDownloadDatabase = new System.Windows.Forms.Button();
            this.btnRefreshPorts = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.txtUARTOutput = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.btnClearErrorCodes = new System.Windows.Forms.Button();
            this.label21 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.comboComPorts = new System.Windows.Forms.ComboBox();
            this.btnDisconnectCom = new System.Windows.Forms.Button();
            this.btnConnectCom = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
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
            this.label1.Location = new System.Drawing.Point(8, 84);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(768, 45);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::PS5_NOR_Modifier.Properties.Resources.PS5_Nor_Logo;
            this.pictureBox1.Location = new System.Drawing.Point(8, 7);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(117, 75);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(140, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(277, 41);
            this.label2.TabIndex = 2;
            this.label2.Text = "PS5 NOR Modifier";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::PS5_NOR_Modifier.Properties.Resources.Paypal_128;
            this.pictureBox2.Location = new System.Drawing.Point(8, 459);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(45, 38);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(65, 459);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(702, 30);
            this.label4.TabIndex = 5;
            this.label4.Text = resources.GetString("label4.Text");
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 3);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "Select NOR Dump";
            // 
            // fileLocationBox
            // 
            this.fileLocationBox.Location = new System.Drawing.Point(5, 20);
            this.fileLocationBox.Margin = new System.Windows.Forms.Padding(2);
            this.fileLocationBox.Name = "fileLocationBox";
            this.fileLocationBox.Size = new System.Drawing.Size(717, 23);
            this.fileLocationBox.TabIndex = 7;
            // 
            // browseFileButton
            // 
            this.browseFileButton.Location = new System.Drawing.Point(727, 19);
            this.browseFileButton.Margin = new System.Windows.Forms.Padding(2);
            this.browseFileButton.Name = "browseFileButton";
            this.browseFileButton.Size = new System.Drawing.Size(78, 20);
            this.browseFileButton.TabIndex = 8;
            this.browseFileButton.Text = "Browse";
            this.browseFileButton.UseVisualStyleBackColor = true;
            this.browseFileButton.Click += new System.EventHandler(this.browseFileButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(5, 49);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 15);
            this.label6.TabIndex = 9;
            this.label6.Text = "Dump Results:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 72);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 15);
            this.label7.TabIndex = 10;
            this.label7.Text = "Serial Number:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 159);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(66, 15);
            this.label9.TabIndex = 12;
            this.label9.Text = "PS5 Model:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(5, 189);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 15);
            this.label10.TabIndex = 13;
            this.label10.Text = "File Size:";
            // 
            // serialNumber
            // 
            this.serialNumber.AutoSize = true;
            this.serialNumber.Location = new System.Drawing.Point(124, 72);
            this.serialNumber.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.serialNumber.Name = "serialNumber";
            this.serialNumber.Size = new System.Drawing.Size(16, 15);
            this.serialNumber.TabIndex = 14;
            this.serialNumber.Text = "...";
            // 
            // modelInfo
            // 
            this.modelInfo.AutoSize = true;
            this.modelInfo.Location = new System.Drawing.Point(124, 159);
            this.modelInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.modelInfo.Name = "modelInfo";
            this.modelInfo.Size = new System.Drawing.Size(16, 15);
            this.modelInfo.TabIndex = 16;
            this.modelInfo.Text = "...";
            // 
            // fileSizeInfo
            // 
            this.fileSizeInfo.AutoSize = true;
            this.fileSizeInfo.Location = new System.Drawing.Point(124, 189);
            this.fileSizeInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.fileSizeInfo.Name = "fileSizeInfo";
            this.fileSizeInfo.Size = new System.Drawing.Size(16, 15);
            this.fileSizeInfo.TabIndex = 17;
            this.fileSizeInfo.Text = "...";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 535);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(847, 22);
            this.statusStrip1.TabIndex = 18;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(135, 17);
            this.toolStripStatusLabel1.Text = "Status: Waiting for input";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 130);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 15);
            this.label8.TabIndex = 20;
            this.label8.Text = "Board Variant:";
            // 
            // boardVariant
            // 
            this.boardVariant.AutoSize = true;
            this.boardVariant.Location = new System.Drawing.Point(124, 130);
            this.boardVariant.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.boardVariant.Name = "boardVariant";
            this.boardVariant.Size = new System.Drawing.Size(16, 15);
            this.boardVariant.TabIndex = 21;
            this.boardVariant.Text = "...";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label11.Location = new System.Drawing.Point(393, 49);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(84, 15);
            this.label11.TabIndex = 22;
            this.label11.Text = "Modify Values";
            // 
            // convertToDigitalEditionButton
            // 
            this.convertToDigitalEditionButton.Location = new System.Drawing.Point(655, 221);
            this.convertToDigitalEditionButton.Margin = new System.Windows.Forms.Padding(2);
            this.convertToDigitalEditionButton.Name = "convertToDigitalEditionButton";
            this.convertToDigitalEditionButton.Size = new System.Drawing.Size(150, 46);
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
            this.boardVariantSelectionBox.Location = new System.Drawing.Point(507, 98);
            this.boardVariantSelectionBox.Margin = new System.Windows.Forms.Padding(2);
            this.boardVariantSelectionBox.Name = "boardVariantSelectionBox";
            this.boardVariantSelectionBox.Size = new System.Drawing.Size(298, 23);
            this.boardVariantSelectionBox.TabIndex = 29;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(393, 72);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(85, 15);
            this.label12.TabIndex = 30;
            this.label12.Text = "Serial Number:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(393, 100);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(80, 15);
            this.label13.TabIndex = 31;
            this.label13.Text = "Board Variant:";
            // 
            // serialNumberTextbox
            // 
            this.serialNumberTextbox.Location = new System.Drawing.Point(507, 70);
            this.serialNumberTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.serialNumberTextbox.Name = "serialNumberTextbox";
            this.serialNumberTextbox.Size = new System.Drawing.Size(298, 23);
            this.serialNumberTextbox.TabIndex = 32;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(393, 129);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(66, 15);
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
            this.boardModelSelectionBox.Location = new System.Drawing.Point(507, 127);
            this.boardModelSelectionBox.Margin = new System.Windows.Forms.Padding(2);
            this.boardModelSelectionBox.Name = "boardModelSelectionBox";
            this.boardModelSelectionBox.Size = new System.Drawing.Size(298, 23);
            this.boardModelSelectionBox.TabIndex = 34;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label15.Location = new System.Drawing.Point(65, 495);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(387, 21);
            this.label15.TabIndex = 35;
            this.label15.Text = "This project is sponsored by www.consolefix.shop";
            this.label15.Click += new System.EventHandler(this.label15_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(5, 221);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(104, 15);
            this.label16.TabIndex = 36;
            this.label16.Text = "WiFi Mac Address:";
            // 
            // macAddressInfo
            // 
            this.macAddressInfo.AutoSize = true;
            this.macAddressInfo.Location = new System.Drawing.Point(124, 221);
            this.macAddressInfo.Name = "macAddressInfo";
            this.macAddressInfo.Size = new System.Drawing.Size(16, 15);
            this.macAddressInfo.TabIndex = 37;
            this.macAddressInfo.Text = "...";
            // 
            // LANMacAddressInfo
            // 
            this.LANMacAddressInfo.AutoSize = true;
            this.LANMacAddressInfo.Location = new System.Drawing.Point(124, 252);
            this.LANMacAddressInfo.Name = "LANMacAddressInfo";
            this.LANMacAddressInfo.Size = new System.Drawing.Size(16, 15);
            this.LANMacAddressInfo.TabIndex = 39;
            this.LANMacAddressInfo.Text = "...";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(5, 252);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(104, 15);
            this.label18.TabIndex = 38;
            this.label18.Text = "LAN Mac Address:";
            // 
            // moboSerialInfo
            // 
            this.moboSerialInfo.AutoSize = true;
            this.moboSerialInfo.Location = new System.Drawing.Point(124, 101);
            this.moboSerialInfo.Name = "moboSerialInfo";
            this.moboSerialInfo.Size = new System.Drawing.Size(16, 15);
            this.moboSerialInfo.TabIndex = 41;
            this.moboSerialInfo.Text = "...";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(5, 101);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(111, 15);
            this.label19.TabIndex = 40;
            this.label19.Text = "Motherboard Serial:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(393, 159);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(108, 15);
            this.label17.TabIndex = 42;
            this.label17.Text = "WiFi MAC Address:";
            // 
            // wifiMacAddressTextbox
            // 
            this.wifiMacAddressTextbox.Enabled = false;
            this.wifiMacAddressTextbox.Location = new System.Drawing.Point(507, 156);
            this.wifiMacAddressTextbox.Name = "wifiMacAddressTextbox";
            this.wifiMacAddressTextbox.Size = new System.Drawing.Size(298, 23);
            this.wifiMacAddressTextbox.TabIndex = 43;
            // 
            // lanMacAddressTextbox
            // 
            this.lanMacAddressTextbox.Enabled = false;
            this.lanMacAddressTextbox.Location = new System.Drawing.Point(507, 185);
            this.lanMacAddressTextbox.Name = "lanMacAddressTextbox";
            this.lanMacAddressTextbox.Size = new System.Drawing.Size(298, 23);
            this.lanMacAddressTextbox.TabIndex = 44;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(393, 188);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(104, 15);
            this.label20.TabIndex = 45;
            this.label20.Text = "LAN Mac Address:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 132);
            this.tabControl1.Name = "tabControl1";
            //this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(821, 311);
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
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(813, 283);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "NOR Modifier";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label25);
            this.tabPage2.Controls.Add(this.btnSendCommand);
            this.tabPage2.Controls.Add(this.txtCustomCommand);
            this.tabPage2.Controls.Add(this.label24);
            this.tabPage2.Controls.Add(this.chkUseOffline);
            this.tabPage2.Controls.Add(this.btnDownloadDatabase);
            this.tabPage2.Controls.Add(this.btnRefreshPorts);
            this.tabPage2.Controls.Add(this.button3);
            this.tabPage2.Controls.Add(this.txtUARTOutput);
            this.tabPage2.Controls.Add(this.label22);
            this.tabPage2.Controls.Add(this.btnClearErrorCodes);
            this.tabPage2.Controls.Add(this.label21);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.comboComPorts);
            this.tabPage2.Controls.Add(this.btnDisconnectCom);
            this.tabPage2.Controls.Add(this.btnConnectCom);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(813, 283);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "UART Communication";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnSendCommand
            // 
            this.btnSendCommand.Location = new System.Drawing.Point(732, 125);
            this.btnSendCommand.Name = "btnSendCommand";
            this.btnSendCommand.Size = new System.Drawing.Size(75, 23);
            this.btnSendCommand.TabIndex = 16;
            this.btnSendCommand.Text = "Send";
            this.btnSendCommand.UseVisualStyleBackColor = true;
            this.btnSendCommand.Click += new System.EventHandler(this.btnSendCommand_Click);
            // 
            // txtCustomCommand
            // 
            this.txtCustomCommand.Location = new System.Drawing.Point(610, 96);
            this.txtCustomCommand.Name = "txtCustomCommand";
            this.txtCustomCommand.Size = new System.Drawing.Size(197, 23);
            this.txtCustomCommand.TabIndex = 15;
            this.txtCustomCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCustomCommand_KeyPress);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(610, 78);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(186, 15);
            this.label24.TabIndex = 14;
            this.label24.Text = "Send custom command via UART:";
            // 
            // chkUseOffline
            // 
            this.chkUseOffline.AutoSize = true;
            this.chkUseOffline.Location = new System.Drawing.Point(472, 47);
            this.chkUseOffline.Name = "chkUseOffline";
            this.chkUseOffline.Size = new System.Drawing.Size(132, 19);
            this.chkUseOffline.TabIndex = 13;
            this.chkUseOffline.Text = "Use offline database";
            this.chkUseOffline.UseVisualStyleBackColor = true;
            // 
            // btnDownloadDatabase
            // 
            this.btnDownloadDatabase.Location = new System.Drawing.Point(310, 44);
            this.btnDownloadDatabase.Name = "btnDownloadDatabase";
            this.btnDownloadDatabase.Size = new System.Drawing.Size(156, 23);
            this.btnDownloadDatabase.TabIndex = 12;
            this.btnDownloadDatabase.Text = "Download Error Database";
            this.btnDownloadDatabase.UseVisualStyleBackColor = true;
            this.btnDownloadDatabase.Click += new System.EventHandler(this.btnDownloadDatabase_Click);
            // 
            // btnRefreshPorts
            // 
            this.btnRefreshPorts.Location = new System.Drawing.Point(515, 13);
            this.btnRefreshPorts.Name = "btnRefreshPorts";
            this.btnRefreshPorts.Size = new System.Drawing.Size(89, 23);
            this.btnRefreshPorts.TabIndex = 11;
            this.btnRefreshPorts.Text = "Refresh Ports";
            this.btnRefreshPorts.UseVisualStyleBackColor = true;
            this.btnRefreshPorts.Click += new System.EventHandler(this.btnRefreshPorts_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(448, 254);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(156, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "Clear Output Window";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtUARTOutput
            // 
            this.txtUARTOutput.Location = new System.Drawing.Point(73, 78);
            this.txtUARTOutput.Multiline = true;
            this.txtUARTOutput.Name = "txtUARTOutput";
            this.txtUARTOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtUARTOutput.Size = new System.Drawing.Size(531, 170);
            this.txtUARTOutput.TabIndex = 9;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 78);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(48, 15);
            this.label22.TabIndex = 8;
            this.label22.Text = "Output:";
            // 
            // btnClearErrorCodes
            // 
            this.btnClearErrorCodes.Location = new System.Drawing.Point(186, 44);
            this.btnClearErrorCodes.Name = "btnClearErrorCodes";
            this.btnClearErrorCodes.Size = new System.Drawing.Size(118, 23);
            this.btnClearErrorCodes.TabIndex = 7;
            this.btnClearErrorCodes.Text = "Clear Error Codes";
            this.btnClearErrorCodes.UseVisualStyleBackColor = true;
            this.btnClearErrorCodes.Click += new System.EventHandler(this.btnClearErrorCodes_Click);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 48);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(52, 15);
            this.label21.TabIndex = 6;
            this.label21.Text = "Options:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(73, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(107, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Get Error Codes";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboComPorts
            // 
            this.comboComPorts.FormattingEnabled = true;
            this.comboComPorts.Location = new System.Drawing.Point(73, 13);
            this.comboComPorts.Name = "comboComPorts";
            this.comboComPorts.Size = new System.Drawing.Size(274, 23);
            this.comboComPorts.TabIndex = 4;
            // 
            // btnDisconnectCom
            // 
            this.btnDisconnectCom.Location = new System.Drawing.Point(434, 13);
            this.btnDisconnectCom.Name = "btnDisconnectCom";
            this.btnDisconnectCom.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnectCom.TabIndex = 3;
            this.btnDisconnectCom.Text = "Disconnect";
            this.btnDisconnectCom.UseVisualStyleBackColor = true;
            this.btnDisconnectCom.Click += new System.EventHandler(this.btnDisconnectCom_Click);
            // 
            // btnConnectCom
            // 
            this.btnConnectCom.Location = new System.Drawing.Point(353, 12);
            this.btnConnectCom.Name = "btnConnectCom";
            this.btnConnectCom.Size = new System.Drawing.Size(75, 23);
            this.btnConnectCom.TabIndex = 2;
            this.btnConnectCom.Text = "Connect";
            this.btnConnectCom.UseVisualStyleBackColor = true;
            this.btnConnectCom.Click += new System.EventHandler(this.btnConnectCom_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Com Port:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.label23.Location = new System.Drawing.Point(193, 48);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(215, 15);
            this.label23.TabIndex = 47;
            this.label23.Text = "and UART stuff too... BwE can SUCK IT!";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(610, 151);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(198, 105);
            this.label25.TabIndex = 17;
            this.label25.Text = resources.GetString("label25.Text");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(847, 557);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "PS5 NOR Modifier";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
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
        private Button button3;
        private TextBox txtUARTOutput;
        private Label label22;
        private Button btnClearErrorCodes;
        private Label label21;
        private Button button1;
        private ComboBox comboComPorts;
        private Button btnDisconnectCom;
        private Button btnConnectCom;
        private Label label3;
        private Button btnRefreshPorts;
        private Label label23;
        private Button btnDownloadDatabase;
        private CheckBox chkUseOffline;
        private Button btnSendCommand;
        private TextBox txtCustomCommand;
        private Label label24;
        private Label label25;
    }
}