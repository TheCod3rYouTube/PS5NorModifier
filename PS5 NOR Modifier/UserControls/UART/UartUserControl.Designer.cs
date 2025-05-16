namespace PS5_NOR_Modifier.UserControls.UART
{
    partial class UartUserControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UartUserControl));
            this.label25 = new System.Windows.Forms.Label();
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
            this.SuspendLayout();
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(1042, 285);
            this.label25.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(347, 210);
            this.label25.TabIndex = 34;
            this.label25.Text = resources.GetString("label25.Text");
            // 
            // btnSendCommand
            // 
            this.btnSendCommand.Location = new System.Drawing.Point(1251, 233);
            this.btnSendCommand.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnSendCommand.Name = "btnSendCommand";
            this.btnSendCommand.Size = new System.Drawing.Size(129, 46);
            this.btnSendCommand.TabIndex = 33;
            this.btnSendCommand.Text = "Send";
            this.btnSendCommand.UseVisualStyleBackColor = true;
            this.btnSendCommand.Click += new System.EventHandler(this.btnSendCommand_Click);
            // 
            // txtCustomCommand
            // 
            this.txtCustomCommand.Location = new System.Drawing.Point(1042, 175);
            this.txtCustomCommand.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtCustomCommand.Name = "txtCustomCommand";
            this.txtCustomCommand.Size = new System.Drawing.Size(335, 35);
            this.txtCustomCommand.TabIndex = 32;
            this.txtCustomCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCustomCommand_KeyPress);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(1042, 139);
            this.label24.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(325, 30);
            this.label24.TabIndex = 31;
            this.label24.Text = "Send custom command via UART:";
            // 
            // chkUseOffline
            // 
            this.chkUseOffline.AutoSize = true;
            this.chkUseOffline.Location = new System.Drawing.Point(805, 77);
            this.chkUseOffline.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.chkUseOffline.Name = "chkUseOffline";
            this.chkUseOffline.Size = new System.Drawing.Size(228, 34);
            this.chkUseOffline.TabIndex = 30;
            this.chkUseOffline.Text = "Use offline database";
            this.chkUseOffline.UseVisualStyleBackColor = true;
            // 
            // btnDownloadDatabase
            // 
            this.btnDownloadDatabase.Location = new System.Drawing.Point(527, 71);
            this.btnDownloadDatabase.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnDownloadDatabase.Name = "btnDownloadDatabase";
            this.btnDownloadDatabase.Size = new System.Drawing.Size(267, 46);
            this.btnDownloadDatabase.TabIndex = 29;
            this.btnDownloadDatabase.Text = "Download Error Database";
            this.btnDownloadDatabase.UseVisualStyleBackColor = true;
            this.btnDownloadDatabase.Click += new System.EventHandler(this.btnDownloadDatabase_Click);
            // 
            // btnRefreshPorts
            // 
            this.btnRefreshPorts.Location = new System.Drawing.Point(879, 9);
            this.btnRefreshPorts.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnRefreshPorts.Name = "btnRefreshPorts";
            this.btnRefreshPorts.Size = new System.Drawing.Size(153, 46);
            this.btnRefreshPorts.TabIndex = 28;
            this.btnRefreshPorts.Text = "Refresh Ports";
            this.btnRefreshPorts.UseVisualStyleBackColor = true;
            this.btnRefreshPorts.Click += new System.EventHandler(this.btnRefreshPorts_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(764, 491);
            this.button3.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(267, 46);
            this.button3.TabIndex = 27;
            this.button3.Text = "Clear Output Window";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtUARTOutput
            // 
            this.txtUARTOutput.Location = new System.Drawing.Point(121, 139);
            this.txtUARTOutput.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtUARTOutput.Multiline = true;
            this.txtUARTOutput.Name = "txtUARTOutput";
            this.txtUARTOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtUARTOutput.Size = new System.Drawing.Size(907, 336);
            this.txtUARTOutput.TabIndex = 26;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 139);
            this.label22.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(84, 30);
            this.label22.TabIndex = 25;
            this.label22.Text = "Output:";
            // 
            // btnClearErrorCodes
            // 
            this.btnClearErrorCodes.Location = new System.Drawing.Point(315, 71);
            this.btnClearErrorCodes.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnClearErrorCodes.Name = "btnClearErrorCodes";
            this.btnClearErrorCodes.Size = new System.Drawing.Size(202, 46);
            this.btnClearErrorCodes.TabIndex = 24;
            this.btnClearErrorCodes.Text = "Clear Error Codes";
            this.btnClearErrorCodes.UseVisualStyleBackColor = true;
            this.btnClearErrorCodes.Click += new System.EventHandler(this.btnClearErrorCodes_Click);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 79);
            this.label21.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(91, 30);
            this.label21.TabIndex = 23;
            this.label21.Text = "Options:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(121, 71);
            this.button1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(183, 46);
            this.button1.TabIndex = 22;
            this.button1.Text = "Get Error Codes";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboComPorts
            // 
            this.comboComPorts.FormattingEnabled = true;
            this.comboComPorts.Location = new System.Drawing.Point(121, 9);
            this.comboComPorts.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.comboComPorts.Name = "comboComPorts";
            this.comboComPorts.Size = new System.Drawing.Size(467, 38);
            this.comboComPorts.TabIndex = 21;
            // 
            // btnDisconnectCom
            // 
            this.btnDisconnectCom.Location = new System.Drawing.Point(740, 9);
            this.btnDisconnectCom.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnDisconnectCom.Name = "btnDisconnectCom";
            this.btnDisconnectCom.Size = new System.Drawing.Size(129, 46);
            this.btnDisconnectCom.TabIndex = 20;
            this.btnDisconnectCom.Text = "Disconnect";
            this.btnDisconnectCom.UseVisualStyleBackColor = true;
            this.btnDisconnectCom.Click += new System.EventHandler(this.btnDisconnectCom_Click);
            // 
            // btnConnectCom
            // 
            this.btnConnectCom.Location = new System.Drawing.Point(601, 7);
            this.btnConnectCom.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnConnectCom.Name = "btnConnectCom";
            this.btnConnectCom.Size = new System.Drawing.Size(129, 46);
            this.btnConnectCom.TabIndex = 19;
            this.btnConnectCom.Text = "Connect";
            this.btnConnectCom.UseVisualStyleBackColor = true;
            this.btnConnectCom.Click += new System.EventHandler(this.btnConnectCom_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 15);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 30);
            this.label3.TabIndex = 18;
            this.label3.Text = "Com Port:";
            // 
            // UartUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label25);
            this.Controls.Add(this.btnSendCommand);
            this.Controls.Add(this.txtCustomCommand);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.chkUseOffline);
            this.Controls.Add(this.btnDownloadDatabase);
            this.Controls.Add(this.btnRefreshPorts);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.txtUARTOutput);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.btnClearErrorCodes);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboComPorts);
            this.Controls.Add(this.btnDisconnectCom);
            this.Controls.Add(this.btnConnectCom);
            this.Controls.Add(this.label3);
            this.Name = "UartUserControl";
            this.Size = new System.Drawing.Size(1387, 547);
            this.Load += new System.EventHandler(this.UartUserControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label25;
        private Button btnSendCommand;
        private TextBox txtCustomCommand;
        private Label label24;
        private CheckBox chkUseOffline;
        private Button btnDownloadDatabase;
        private Button btnRefreshPorts;
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
    }
}
