namespace PS5_NOR_Modifier;

public sealed partial class MainForm : Form
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        infoBoxLabel = new Label();
        pictureBox1 = new PictureBox();
        applicationTitleLabel = new Label();
        donateImageButton = new PictureBox();
        donateInfoLabel = new Label();
        selectNorDumpLabel = new Label();
        fileLocationBox = new TextBox();
        browseFileButton = new Button();
        dumpResultsLabel = new Label();
        serialNumberLabel = new Label();
        ps5ModelLabel = new Label();
        fileSizeLabel = new Label();
        serialNumber = new Label();
        modelInfo = new Label();
        fileSizeInfo = new Label();
        statusStrip = new StatusStrip();
        toolStripStatusLabel = new ToolStripStatusLabel();
        boardVariantLabel = new Label();
        boardVariant = new Label();
        label11 = new Label();
        convertToDigitalEditionButton = new Button();
        boardVariantSelectionBox = new ComboBox();
        serialNumberInputLabel = new Label();
        boardVariantInputLabel = new Label();
        serialNumberTextbox = new TextBox();
        ps5ModelInputLabel = new Label();
        boardModelSelectionBox = new ComboBox();
        sponsorLabel = new Label();
        wifiMacAddressLabel = new Label();
        macAddressInfo = new Label();
        LANMacAddressInfo = new Label();
        lanMacAddress = new Label();
        moboSerialInfo = new Label();
        motherboardSerialLabel = new Label();
        wifiMacAddressInputLabel = new Label();
        wifiMacAddressTextbox = new TextBox();
        lanMacAddressTextbox = new TextBox();
        lanMacAddressInputLabel = new Label();
        consoleOptionsTabControl = new TabControl();
        norModifierTabPage = new TabPage();
        uartCommunicationTabPage = new TabPage();
        uartInfoLabel = new Label();
        btnSendCommand = new Button();
        txtCustomCommand = new TextBox();
        sendUartCommandLabel = new Label();
        chkUseOffline = new CheckBox();
        btnDownloadDatabase = new Button();
        btnRefreshPorts = new Button();
        btnClearOutput = new Button();
        txtUARTOutput = new TextBox();
        label22 = new Label();
        btnClearErrorCodes = new Button();
        label21 = new Label();
        btnGetErrorCodes = new Button();
        comboComPorts = new ComboBox();
        btnDisconnectCom = new Button();
        btnConnectCom = new Button();
        comPortLabel = new Label();
        label23 = new Label();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)donateImageButton).BeginInit();
        statusStrip.SuspendLayout();
        consoleOptionsTabControl.SuspendLayout();
        norModifierTabPage.SuspendLayout();
        uartCommunicationTabPage.SuspendLayout();
        SuspendLayout();
        // 
        // infoBoxLabel
        // 
        infoBoxLabel.AutoSize = true;
        infoBoxLabel.Location = new Point(11, 140);
        infoBoxLabel.Name = "infoBoxLabel";
        infoBoxLabel.Size = new Size(1158, 75);
        infoBoxLabel.TabIndex = 0;
        infoBoxLabel.Text = resources.GetString("infoBoxLabel.Text");
        // 
        // pictureBox1
        // 
        pictureBox1.Image = Properties.Resources.PS5_Nor_Logo;
        pictureBox1.Location = new Point(11, 12);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(167, 125);
        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        pictureBox1.TabIndex = 1;
        pictureBox1.TabStop = false;
        // 
        // applicationTitleLabel
        // 
        applicationTitleLabel.AutoSize = true;
        applicationTitleLabel.Font = new Font("Segoe UI", 22F, FontStyle.Bold, GraphicsUnit.Point);
        applicationTitleLabel.Location = new Point(200, 12);
        applicationTitleLabel.Name = "applicationTitleLabel";
        applicationTitleLabel.Size = new Size(404, 60);
        applicationTitleLabel.TabIndex = 2;
        applicationTitleLabel.Text = "PS5 NOR Modifier";
        // 
        // donateImageButton
        // 
        donateImageButton.Image = Properties.Resources.Paypal_128;
        donateImageButton.Location = new Point(11, 765);
        donateImageButton.Name = "donateImageButton";
        donateImageButton.Size = new Size(64, 63);
        donateImageButton.SizeMode = PictureBoxSizeMode.StretchImage;
        donateImageButton.TabIndex = 4;
        donateImageButton.TabStop = false;
        donateImageButton.Click += donateImageButton_Click;
        // 
        // donateInfoLabel
        // 
        donateInfoLabel.AutoSize = true;
        donateInfoLabel.Location = new Point(93, 765);
        donateInfoLabel.Name = "donateInfoLabel";
        donateInfoLabel.Size = new Size(1053, 50);
        donateInfoLabel.TabIndex = 5;
        donateInfoLabel.Text = resources.GetString("donateInfoLabel.Text");
        donateInfoLabel.Click += donateInfoLabel_Click;
        // 
        // selectNorDumpLabel
        // 
        selectNorDumpLabel.AutoSize = true;
        selectNorDumpLabel.Location = new Point(7, 5);
        selectNorDumpLabel.Name = "selectNorDumpLabel";
        selectNorDumpLabel.Size = new Size(156, 25);
        selectNorDumpLabel.TabIndex = 6;
        selectNorDumpLabel.Text = "Select NOR Dump";
        // 
        // fileLocationBox
        // 
        fileLocationBox.Location = new Point(7, 33);
        fileLocationBox.Name = "fileLocationBox";
        fileLocationBox.Size = new Size(1023, 31);
        fileLocationBox.TabIndex = 7;
        // 
        // browseFileButton
        // 
        browseFileButton.Location = new Point(1039, 32);
        browseFileButton.Name = "browseFileButton";
        browseFileButton.Size = new Size(111, 33);
        browseFileButton.TabIndex = 8;
        browseFileButton.Text = "Browse";
        browseFileButton.UseVisualStyleBackColor = true;
        browseFileButton.Click += browseFileButton_Click;
        // 
        // dumpResultsLabel
        // 
        dumpResultsLabel.AutoSize = true;
        dumpResultsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        dumpResultsLabel.Location = new Point(7, 82);
        dumpResultsLabel.Name = "dumpResultsLabel";
        dumpResultsLabel.Size = new Size(134, 25);
        dumpResultsLabel.TabIndex = 9;
        dumpResultsLabel.Text = "Dump Results:";
        // 
        // serialNumberLabel
        // 
        serialNumberLabel.AutoSize = true;
        serialNumberLabel.Location = new Point(7, 120);
        serialNumberLabel.Name = "serialNumberLabel";
        serialNumberLabel.Size = new Size(128, 25);
        serialNumberLabel.TabIndex = 10;
        serialNumberLabel.Text = "Serial Number:";
        // 
        // ps5ModelLabel
        // 
        ps5ModelLabel.AutoSize = true;
        ps5ModelLabel.Location = new Point(7, 265);
        ps5ModelLabel.Name = "ps5ModelLabel";
        ps5ModelLabel.Size = new Size(102, 25);
        ps5ModelLabel.TabIndex = 12;
        ps5ModelLabel.Text = "PS5 Model:";
        // 
        // fileSizeLabel
        // 
        fileSizeLabel.AutoSize = true;
        fileSizeLabel.Location = new Point(7, 315);
        fileSizeLabel.Name = "fileSizeLabel";
        fileSizeLabel.Size = new Size(78, 25);
        fileSizeLabel.TabIndex = 13;
        fileSizeLabel.Text = "File Size:";
        // 
        // serialNumber
        // 
        serialNumber.AutoSize = true;
        serialNumber.Location = new Point(177, 120);
        serialNumber.Name = "serialNumber";
        serialNumber.Size = new Size(24, 25);
        serialNumber.TabIndex = 14;
        serialNumber.Text = "...";
        // 
        // modelInfo
        // 
        modelInfo.AutoSize = true;
        modelInfo.Location = new Point(177, 265);
        modelInfo.Name = "modelInfo";
        modelInfo.Size = new Size(24, 25);
        modelInfo.TabIndex = 16;
        modelInfo.Text = "...";
        // 
        // fileSizeInfo
        // 
        fileSizeInfo.AutoSize = true;
        fileSizeInfo.Location = new Point(177, 315);
        fileSizeInfo.Name = "fileSizeInfo";
        fileSizeInfo.Size = new Size(24, 25);
        fileSizeInfo.TabIndex = 17;
        fileSizeInfo.Text = "...";
        // 
        // statusStrip
        // 
        statusStrip.ImageScalingSize = new Size(24, 24);
        statusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel });
        statusStrip.Location = new Point(0, 896);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1210, 32);
        statusStrip.TabIndex = 18;
        statusStrip.Text = "statusStrip1";
        // 
        // toolStripStatusLabel
        // 
        toolStripStatusLabel.Name = "toolStripStatusLabel";
        toolStripStatusLabel.Size = new Size(203, 25);
        toolStripStatusLabel.Text = "Status: Waiting for input";
        // 
        // boardVariantLabel
        // 
        boardVariantLabel.AutoSize = true;
        boardVariantLabel.Location = new Point(7, 217);
        boardVariantLabel.Name = "boardVariantLabel";
        boardVariantLabel.Size = new Size(122, 25);
        boardVariantLabel.TabIndex = 20;
        boardVariantLabel.Text = "Board Variant:";
        // 
        // boardVariant
        // 
        boardVariant.AutoSize = true;
        boardVariant.Location = new Point(177, 217);
        boardVariant.Name = "boardVariant";
        boardVariant.Size = new Size(24, 25);
        boardVariant.TabIndex = 21;
        boardVariant.Text = "...";
        // 
        // label11
        // 
        label11.AutoSize = true;
        label11.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        label11.Location = new Point(561, 82);
        label11.Name = "label11";
        label11.Size = new Size(133, 25);
        label11.TabIndex = 22;
        label11.Text = "Modify Values";
        // 
        // convertToDigitalEditionButton
        // 
        convertToDigitalEditionButton.Location = new Point(936, 368);
        convertToDigitalEditionButton.Name = "convertToDigitalEditionButton";
        convertToDigitalEditionButton.Size = new Size(214, 77);
        convertToDigitalEditionButton.TabIndex = 23;
        convertToDigitalEditionButton.Text = "Save New\r\nBIOS Information";
        convertToDigitalEditionButton.UseVisualStyleBackColor = true;
        convertToDigitalEditionButton.Click += convertToDigitalEditionButton_Click;
        // 
        // boardVariantSelectionBox
        // 
        boardVariantSelectionBox.DropDownStyle = ComboBoxStyle.DropDownList;
        boardVariantSelectionBox.FormattingEnabled = true;
        boardVariantSelectionBox.Items.AddRange(new object[] { "CFI-1000A", "CFI-1000A01", "CFI-1000B", "CFI-1002A", "CFI-1008A", "CFI-1014A", "CFI-1015A", "CFI-1015B", "CFI-1016A", "CFI-1018A", "CFI-1100A01", "CFI-1102A", "CFI-1108A", "CFI-1109A", "CFI-1114A", "CFI-1115A", "CFI-1116A", "CFI-1118A", "CFI-1208A", "CFI-1215A", "CFI-1216A", "DFI-T1000AA", "DFI-D1000AA" });
        boardVariantSelectionBox.Location = new Point(724, 163);
        boardVariantSelectionBox.Name = "boardVariantSelectionBox";
        boardVariantSelectionBox.Size = new Size(424, 33);
        boardVariantSelectionBox.TabIndex = 29;
        // 
        // serialNumberInputLabel
        // 
        serialNumberInputLabel.AutoSize = true;
        serialNumberInputLabel.Location = new Point(561, 120);
        serialNumberInputLabel.Name = "serialNumberInputLabel";
        serialNumberInputLabel.Size = new Size(128, 25);
        serialNumberInputLabel.TabIndex = 30;
        serialNumberInputLabel.Text = "Serial Number:";
        // 
        // boardVariantInputLabel
        // 
        boardVariantInputLabel.AutoSize = true;
        boardVariantInputLabel.Location = new Point(561, 167);
        boardVariantInputLabel.Name = "boardVariantInputLabel";
        boardVariantInputLabel.Size = new Size(122, 25);
        boardVariantInputLabel.TabIndex = 31;
        boardVariantInputLabel.Text = "Board Variant:";
        // 
        // serialNumberTextbox
        // 
        serialNumberTextbox.Location = new Point(724, 117);
        serialNumberTextbox.Name = "serialNumberTextbox";
        serialNumberTextbox.Size = new Size(424, 31);
        serialNumberTextbox.TabIndex = 32;
        // 
        // ps5ModelInputLabel
        // 
        ps5ModelInputLabel.AutoSize = true;
        ps5ModelInputLabel.Location = new Point(561, 215);
        ps5ModelInputLabel.Name = "ps5ModelInputLabel";
        ps5ModelInputLabel.Size = new Size(102, 25);
        ps5ModelInputLabel.TabIndex = 33;
        ps5ModelInputLabel.Text = "PS5 Model:";
        // 
        // boardModelSelectionBox
        // 
        boardModelSelectionBox.DropDownStyle = ComboBoxStyle.DropDownList;
        boardModelSelectionBox.FormattingEnabled = true;
        boardModelSelectionBox.Items.AddRange(new object[] { "Digital Edition", "Disc Edition" });
        boardModelSelectionBox.Location = new Point(724, 212);
        boardModelSelectionBox.Name = "boardModelSelectionBox";
        boardModelSelectionBox.Size = new Size(424, 33);
        boardModelSelectionBox.TabIndex = 34;
        // 
        // sponsorLabel
        // 
        sponsorLabel.AutoSize = true;
        sponsorLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
        sponsorLabel.Location = new Point(93, 825);
        sponsorLabel.Name = "sponsorLabel";
        sponsorLabel.Size = new Size(584, 32);
        sponsorLabel.TabIndex = 35;
        sponsorLabel.Text = "This project is sponsored by www.consolefix.shop";
        sponsorLabel.Click += sponsorLabel_Click;
        // 
        // wifiMacAddressLabel
        // 
        wifiMacAddressLabel.AutoSize = true;
        wifiMacAddressLabel.Location = new Point(7, 368);
        wifiMacAddressLabel.Margin = new Padding(4, 0, 4, 0);
        wifiMacAddressLabel.Name = "wifiMacAddressLabel";
        wifiMacAddressLabel.Size = new Size(158, 25);
        wifiMacAddressLabel.TabIndex = 36;
        wifiMacAddressLabel.Text = "WiFi Mac Address:";
        // 
        // macAddressInfo
        // 
        macAddressInfo.AutoSize = true;
        macAddressInfo.Location = new Point(177, 368);
        macAddressInfo.Margin = new Padding(4, 0, 4, 0);
        macAddressInfo.Name = "macAddressInfo";
        macAddressInfo.Size = new Size(24, 25);
        macAddressInfo.TabIndex = 37;
        macAddressInfo.Text = "...";
        // 
        // LANMacAddressInfo
        // 
        LANMacAddressInfo.AutoSize = true;
        LANMacAddressInfo.Location = new Point(177, 420);
        LANMacAddressInfo.Margin = new Padding(4, 0, 4, 0);
        LANMacAddressInfo.Name = "LANMacAddressInfo";
        LANMacAddressInfo.Size = new Size(24, 25);
        LANMacAddressInfo.TabIndex = 39;
        LANMacAddressInfo.Text = "...";
        // 
        // lanMacAddress
        // 
        lanMacAddress.AutoSize = true;
        lanMacAddress.Location = new Point(7, 420);
        lanMacAddress.Margin = new Padding(4, 0, 4, 0);
        lanMacAddress.Name = "lanMacAddress";
        lanMacAddress.Size = new Size(158, 25);
        lanMacAddress.TabIndex = 38;
        lanMacAddress.Text = "LAN Mac Address:";
        // 
        // moboSerialInfo
        // 
        moboSerialInfo.AutoSize = true;
        moboSerialInfo.Location = new Point(177, 168);
        moboSerialInfo.Margin = new Padding(4, 0, 4, 0);
        moboSerialInfo.Name = "moboSerialInfo";
        moboSerialInfo.Size = new Size(24, 25);
        moboSerialInfo.TabIndex = 41;
        moboSerialInfo.Text = "...";
        // 
        // motherboardSerialLabel
        // 
        motherboardSerialLabel.AutoSize = true;
        motherboardSerialLabel.Location = new Point(7, 168);
        motherboardSerialLabel.Margin = new Padding(4, 0, 4, 0);
        motherboardSerialLabel.Name = "motherboardSerialLabel";
        motherboardSerialLabel.Size = new Size(169, 25);
        motherboardSerialLabel.TabIndex = 40;
        motherboardSerialLabel.Text = "Motherboard Serial:";
        // 
        // wifiMacAddressInputLabel
        // 
        wifiMacAddressInputLabel.AutoSize = true;
        wifiMacAddressInputLabel.Location = new Point(561, 265);
        wifiMacAddressInputLabel.Margin = new Padding(4, 0, 4, 0);
        wifiMacAddressInputLabel.Name = "wifiMacAddressInputLabel";
        wifiMacAddressInputLabel.Size = new Size(164, 25);
        wifiMacAddressInputLabel.TabIndex = 42;
        wifiMacAddressInputLabel.Text = "WiFi MAC Address:";
        // 
        // wifiMacAddressTextbox
        // 
        wifiMacAddressTextbox.Enabled = false;
        wifiMacAddressTextbox.Location = new Point(724, 260);
        wifiMacAddressTextbox.Margin = new Padding(4, 5, 4, 5);
        wifiMacAddressTextbox.Name = "wifiMacAddressTextbox";
        wifiMacAddressTextbox.Size = new Size(424, 31);
        wifiMacAddressTextbox.TabIndex = 43;
        // 
        // lanMacAddressTextbox
        // 
        lanMacAddressTextbox.Enabled = false;
        lanMacAddressTextbox.Location = new Point(724, 308);
        lanMacAddressTextbox.Margin = new Padding(4, 5, 4, 5);
        lanMacAddressTextbox.Name = "lanMacAddressTextbox";
        lanMacAddressTextbox.Size = new Size(424, 31);
        lanMacAddressTextbox.TabIndex = 44;
        // 
        // lanMacAddressInputLabel
        // 
        lanMacAddressInputLabel.AutoSize = true;
        lanMacAddressInputLabel.Location = new Point(561, 313);
        lanMacAddressInputLabel.Margin = new Padding(4, 0, 4, 0);
        lanMacAddressInputLabel.Name = "lanMacAddressInputLabel";
        lanMacAddressInputLabel.Size = new Size(158, 25);
        lanMacAddressInputLabel.TabIndex = 45;
        lanMacAddressInputLabel.Text = "LAN Mac Address:";
        // 
        // consoleOptionsTabControl
        // 
        consoleOptionsTabControl.Controls.Add(norModifierTabPage);
        consoleOptionsTabControl.Controls.Add(uartCommunicationTabPage);
        consoleOptionsTabControl.Location = new Point(17, 220);
        consoleOptionsTabControl.Margin = new Padding(4, 5, 4, 5);
        consoleOptionsTabControl.Name = "consoleOptionsTabControl";
        consoleOptionsTabControl.SelectedIndex = 0;
        consoleOptionsTabControl.Size = new Size(1173, 518);
        consoleOptionsTabControl.TabIndex = 46;
        // 
        // norModifierTabPage
        // 
        norModifierTabPage.Controls.Add(selectNorDumpLabel);
        norModifierTabPage.Controls.Add(lanMacAddressInputLabel);
        norModifierTabPage.Controls.Add(fileLocationBox);
        norModifierTabPage.Controls.Add(lanMacAddressTextbox);
        norModifierTabPage.Controls.Add(browseFileButton);
        norModifierTabPage.Controls.Add(wifiMacAddressTextbox);
        norModifierTabPage.Controls.Add(dumpResultsLabel);
        norModifierTabPage.Controls.Add(wifiMacAddressInputLabel);
        norModifierTabPage.Controls.Add(serialNumberLabel);
        norModifierTabPage.Controls.Add(moboSerialInfo);
        norModifierTabPage.Controls.Add(ps5ModelLabel);
        norModifierTabPage.Controls.Add(motherboardSerialLabel);
        norModifierTabPage.Controls.Add(fileSizeLabel);
        norModifierTabPage.Controls.Add(LANMacAddressInfo);
        norModifierTabPage.Controls.Add(serialNumber);
        norModifierTabPage.Controls.Add(lanMacAddress);
        norModifierTabPage.Controls.Add(modelInfo);
        norModifierTabPage.Controls.Add(macAddressInfo);
        norModifierTabPage.Controls.Add(fileSizeInfo);
        norModifierTabPage.Controls.Add(wifiMacAddressLabel);
        norModifierTabPage.Controls.Add(boardVariantLabel);
        norModifierTabPage.Controls.Add(boardVariant);
        norModifierTabPage.Controls.Add(boardModelSelectionBox);
        norModifierTabPage.Controls.Add(label11);
        norModifierTabPage.Controls.Add(ps5ModelInputLabel);
        norModifierTabPage.Controls.Add(convertToDigitalEditionButton);
        norModifierTabPage.Controls.Add(serialNumberTextbox);
        norModifierTabPage.Controls.Add(boardVariantSelectionBox);
        norModifierTabPage.Controls.Add(boardVariantInputLabel);
        norModifierTabPage.Controls.Add(serialNumberInputLabel);
        norModifierTabPage.Location = new Point(4, 34);
        norModifierTabPage.Margin = new Padding(4, 5, 4, 5);
        norModifierTabPage.Name = "norModifierTabPage";
        norModifierTabPage.Padding = new Padding(4, 5, 4, 5);
        norModifierTabPage.Size = new Size(1165, 480);
        norModifierTabPage.TabIndex = 0;
        norModifierTabPage.Text = "NOR Modifier";
        norModifierTabPage.UseVisualStyleBackColor = true;
        // 
        // uartCommunicationTabPage
        // 
        uartCommunicationTabPage.Controls.Add(uartInfoLabel);
        uartCommunicationTabPage.Controls.Add(btnSendCommand);
        uartCommunicationTabPage.Controls.Add(txtCustomCommand);
        uartCommunicationTabPage.Controls.Add(sendUartCommandLabel);
        uartCommunicationTabPage.Controls.Add(chkUseOffline);
        uartCommunicationTabPage.Controls.Add(btnDownloadDatabase);
        uartCommunicationTabPage.Controls.Add(btnRefreshPorts);
        uartCommunicationTabPage.Controls.Add(btnClearOutput);
        uartCommunicationTabPage.Controls.Add(txtUARTOutput);
        uartCommunicationTabPage.Controls.Add(label22);
        uartCommunicationTabPage.Controls.Add(btnClearErrorCodes);
        uartCommunicationTabPage.Controls.Add(label21);
        uartCommunicationTabPage.Controls.Add(btnGetErrorCodes);
        uartCommunicationTabPage.Controls.Add(comboComPorts);
        uartCommunicationTabPage.Controls.Add(btnDisconnectCom);
        uartCommunicationTabPage.Controls.Add(btnConnectCom);
        uartCommunicationTabPage.Controls.Add(comPortLabel);
        uartCommunicationTabPage.Location = new Point(4, 34);
        uartCommunicationTabPage.Margin = new Padding(4, 5, 4, 5);
        uartCommunicationTabPage.Name = "uartCommunicationTabPage";
        uartCommunicationTabPage.Padding = new Padding(4, 5, 4, 5);
        uartCommunicationTabPage.Size = new Size(1165, 480);
        uartCommunicationTabPage.TabIndex = 1;
        uartCommunicationTabPage.Text = "UART Communication";
        uartCommunicationTabPage.UseVisualStyleBackColor = true;
        // 
        // uartInfoLabel
        // 
        uartInfoLabel.AutoSize = true;
        uartInfoLabel.Location = new Point(871, 252);
        uartInfoLabel.Margin = new Padding(4, 0, 4, 0);
        uartInfoLabel.Name = "uartInfoLabel";
        uartInfoLabel.Size = new Size(300, 175);
        uartInfoLabel.TabIndex = 17;
        uartInfoLabel.Text = resources.GetString("uartInfoLabel.Text");
        // 
        // btnSendCommand
        // 
        btnSendCommand.Location = new Point(1046, 208);
        btnSendCommand.Margin = new Padding(4, 5, 4, 5);
        btnSendCommand.Name = "btnSendCommand";
        btnSendCommand.Size = new Size(107, 38);
        btnSendCommand.TabIndex = 16;
        btnSendCommand.Text = "Send";
        btnSendCommand.UseVisualStyleBackColor = true;
        btnSendCommand.Click += btnSendCommand_Click;
        // 
        // txtCustomCommand
        // 
        txtCustomCommand.Location = new Point(871, 160);
        txtCustomCommand.Margin = new Padding(4, 5, 4, 5);
        txtCustomCommand.Name = "txtCustomCommand";
        txtCustomCommand.Size = new Size(280, 31);
        txtCustomCommand.TabIndex = 15;
        txtCustomCommand.KeyPress += txtCustomCommand_KeyPress;
        // 
        // sendUartCommandLabel
        // 
        sendUartCommandLabel.AutoSize = true;
        sendUartCommandLabel.Location = new Point(871, 130);
        sendUartCommandLabel.Margin = new Padding(4, 0, 4, 0);
        sendUartCommandLabel.Name = "sendUartCommandLabel";
        sendUartCommandLabel.Size = new Size(281, 25);
        sendUartCommandLabel.TabIndex = 14;
        sendUartCommandLabel.Text = "Send custom command via UART:";
        // 
        // chkUseOffline
        // 
        chkUseOffline.AutoSize = true;
        chkUseOffline.Location = new Point(674, 78);
        chkUseOffline.Margin = new Padding(4, 5, 4, 5);
        chkUseOffline.Name = "chkUseOffline";
        chkUseOffline.Size = new Size(199, 29);
        chkUseOffline.TabIndex = 13;
        chkUseOffline.Text = "Use offline database";
        chkUseOffline.UseVisualStyleBackColor = true;
        // 
        // btnDownloadDatabase
        // 
        btnDownloadDatabase.Location = new Point(443, 73);
        btnDownloadDatabase.Margin = new Padding(4, 5, 4, 5);
        btnDownloadDatabase.Name = "btnDownloadDatabase";
        btnDownloadDatabase.Size = new Size(223, 38);
        btnDownloadDatabase.TabIndex = 12;
        btnDownloadDatabase.Text = "Download Error Database";
        btnDownloadDatabase.UseVisualStyleBackColor = true;
        btnDownloadDatabase.Click += btnDownloadDatabase_Click;
        // 
        // btnRefreshPorts
        // 
        btnRefreshPorts.Location = new Point(736, 22);
        btnRefreshPorts.Margin = new Padding(4, 5, 4, 5);
        btnRefreshPorts.Name = "btnRefreshPorts";
        btnRefreshPorts.Size = new Size(127, 38);
        btnRefreshPorts.TabIndex = 11;
        btnRefreshPorts.Text = "Refresh Ports";
        btnRefreshPorts.UseVisualStyleBackColor = true;
        btnRefreshPorts.Click += btnRefreshPorts_Click;
        // 
        // btnClearOutput
        // 
        btnClearOutput.Location = new Point(640, 423);
        btnClearOutput.Margin = new Padding(4, 5, 4, 5);
        btnClearOutput.Name = "btnClearOutput";
        btnClearOutput.Size = new Size(223, 38);
        btnClearOutput.TabIndex = 10;
        btnClearOutput.Text = "Clear Output Window";
        btnClearOutput.UseVisualStyleBackColor = true;
        btnClearOutput.Click += btnClearOutput_Click;
        // 
        // txtUARTOutput
        // 
        txtUARTOutput.Location = new Point(104, 130);
        txtUARTOutput.Margin = new Padding(4, 5, 4, 5);
        txtUARTOutput.Multiline = true;
        txtUARTOutput.Name = "txtUARTOutput";
        txtUARTOutput.ScrollBars = ScrollBars.Vertical;
        txtUARTOutput.Size = new Size(757, 281);
        txtUARTOutput.TabIndex = 9;
        // 
        // label22
        // 
        label22.AutoSize = true;
        label22.Location = new Point(9, 130);
        label22.Margin = new Padding(4, 0, 4, 0);
        label22.Name = "label22";
        label22.Size = new Size(73, 25);
        label22.TabIndex = 8;
        label22.Text = "Output:";
        // 
        // btnClearErrorCodes
        // 
        btnClearErrorCodes.Location = new Point(266, 73);
        btnClearErrorCodes.Margin = new Padding(4, 5, 4, 5);
        btnClearErrorCodes.Name = "btnClearErrorCodes";
        btnClearErrorCodes.Size = new Size(169, 38);
        btnClearErrorCodes.TabIndex = 7;
        btnClearErrorCodes.Text = "Clear Error Codes";
        btnClearErrorCodes.UseVisualStyleBackColor = true;
        btnClearErrorCodes.Click += btnClearErrorCodes_Click;
        // 
        // label21
        // 
        label21.AutoSize = true;
        label21.Location = new Point(9, 80);
        label21.Margin = new Padding(4, 0, 4, 0);
        label21.Name = "label21";
        label21.Size = new Size(80, 25);
        label21.TabIndex = 6;
        label21.Text = "Options:";
        // 
        // btnGetErrorCodes
        // 
        btnGetErrorCodes.Location = new Point(104, 73);
        btnGetErrorCodes.Margin = new Padding(4, 5, 4, 5);
        btnGetErrorCodes.Name = "btnGetErrorCodes";
        btnGetErrorCodes.Size = new Size(153, 38);
        btnGetErrorCodes.TabIndex = 5;
        btnGetErrorCodes.Text = "Get Error Codes";
        btnGetErrorCodes.UseVisualStyleBackColor = true;
        btnGetErrorCodes.Click += BtnGetErrorCodes_Click;
        // 
        // comboComPorts
        // 
        comboComPorts.FormattingEnabled = true;
        comboComPorts.Location = new Point(104, 22);
        comboComPorts.Margin = new Padding(4, 5, 4, 5);
        comboComPorts.Name = "comboComPorts";
        comboComPorts.Size = new Size(390, 33);
        comboComPorts.TabIndex = 4;
        // 
        // btnDisconnectCom
        // 
        btnDisconnectCom.Location = new Point(620, 22);
        btnDisconnectCom.Margin = new Padding(4, 5, 4, 5);
        btnDisconnectCom.Name = "btnDisconnectCom";
        btnDisconnectCom.Size = new Size(107, 38);
        btnDisconnectCom.TabIndex = 3;
        btnDisconnectCom.Text = "Disconnect";
        btnDisconnectCom.UseVisualStyleBackColor = true;
        btnDisconnectCom.Click += btnDisconnectCom_Click;
        // 
        // btnConnectCom
        // 
        btnConnectCom.Location = new Point(504, 20);
        btnConnectCom.Margin = new Padding(4, 5, 4, 5);
        btnConnectCom.Name = "btnConnectCom";
        btnConnectCom.Size = new Size(107, 38);
        btnConnectCom.TabIndex = 2;
        btnConnectCom.Text = "Connect";
        btnConnectCom.UseVisualStyleBackColor = true;
        btnConnectCom.Click += btnConnectCom_Click;
        // 
        // comPortLabel
        // 
        comPortLabel.AutoSize = true;
        comPortLabel.Location = new Point(9, 27);
        comPortLabel.Margin = new Padding(4, 0, 4, 0);
        comPortLabel.Name = "comPortLabel";
        comPortLabel.Size = new Size(91, 25);
        comPortLabel.TabIndex = 0;
        comPortLabel.Text = "Com Port:";
        // 
        // label23
        // 
        label23.AutoSize = true;
        label23.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
        label23.Location = new Point(276, 80);
        label23.Margin = new Padding(4, 0, 4, 0);
        label23.Name = "label23";
        label23.Size = new Size(324, 25);
        label23.TabIndex = 47;
        label23.Text = "and UART stuff too... BwE can SUCK IT!";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        ClientSize = new Size(1210, 928);
        Controls.Add(label23);
        Controls.Add(consoleOptionsTabControl);
        Controls.Add(sponsorLabel);
        Controls.Add(statusStrip);
        Controls.Add(donateInfoLabel);
        Controls.Add(donateImageButton);
        Controls.Add(applicationTitleLabel);
        Controls.Add(pictureBox1);
        Controls.Add(infoBoxLabel);
        Name = "MainForm";
        Text = "PS5 NOR Modifier";
        FormClosing += MainForm_FormClosing;
        Load += MainForm_Load;
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        ((System.ComponentModel.ISupportInitialize)donateImageButton).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        consoleOptionsTabControl.ResumeLayout(false);
        norModifierTabPage.ResumeLayout(false);
        norModifierTabPage.PerformLayout();
        uartCommunicationTabPage.ResumeLayout(false);
        uartCommunicationTabPage.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label infoBoxLabel;
    private PictureBox pictureBox1;
    private Label applicationTitleLabel;
    private PictureBox donateImageButton;
    private Label donateInfoLabel;
    private Label selectNorDumpLabel;
    private TextBox fileLocationBox;
    private Button browseFileButton;
    private Label dumpResultsLabel;
    private Label serialNumberLabel;
    private Label ps5ModelLabel;
    private Label fileSizeLabel;
    private Label serialNumber;
    private Label modelInfo;
    private Label fileSizeInfo;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel toolStripStatusLabel;
    private Label boardVariantLabel;
    private Label boardVariant;
    private Label label11;
    private Button convertToDigitalEditionButton;
    private ComboBox boardVariantSelectionBox;
    private Label serialNumberInputLabel;
    private Label boardVariantInputLabel;
    private TextBox serialNumberTextbox;
    private Label ps5ModelInputLabel;
    private ComboBox boardModelSelectionBox;
    private Label sponsorLabel;
    private Label wifiMacAddressLabel;
    private Label macAddressInfo;
    private Label LANMacAddressInfo;
    private Label lanMacAddress;
    private Label moboSerialInfo;
    private Label motherboardSerialLabel;
    private Label wifiMacAddressInputLabel;
    private TextBox wifiMacAddressTextbox;
    private TextBox lanMacAddressTextbox;
    private Label lanMacAddressInputLabel;
    private TabControl consoleOptionsTabControl;
    private TabPage norModifierTabPage;
    private TabPage uartCommunicationTabPage;
    private Button btnClearOutput;
    private TextBox txtUARTOutput;
    private Label label22;
    private Button btnClearErrorCodes;
    private Label label21;
    private Button btnGetErrorCodes;
    private ComboBox comboComPorts;
    private Button btnDisconnectCom;
    private Button btnConnectCom;
    private Label comPortLabel;
    private Button btnRefreshPorts;
    private Label label23;
    private Button btnDownloadDatabase;
    private CheckBox chkUseOffline;
    private Button btnSendCommand;
    private TextBox txtCustomCommand;
    private Label sendUartCommandLabel;
    private Label uartInfoLabel;
}