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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.gvErrorCodes = new System.Windows.Forms.DataGridView();
            this.lblLastErrorCodes = new System.Windows.Forms.Label();
            this.cErrorCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDetails = new System.Windows.Forms.DataGridViewLinkColumn();
            this.cCodeDetailsLink = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gvErrorCodes)).BeginInit();
            this.SuspendLayout();
            // 
            // label25
            // 
            this.label25.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(1515, 285);
            this.label25.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(347, 210);
            this.label25.TabIndex = 18;
            this.label25.Text = resources.GetString("label25.Text");
            // 
            // btnSendCommand
            // 
            this.btnSendCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendCommand.Location = new System.Drawing.Point(1729, 233);
            this.btnSendCommand.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnSendCommand.Name = "btnSendCommand";
            this.btnSendCommand.Size = new System.Drawing.Size(129, 46);
            this.btnSendCommand.TabIndex = 17;
            this.btnSendCommand.Text = "Send";
            this.btnSendCommand.UseVisualStyleBackColor = true;
            this.btnSendCommand.Click += new System.EventHandler(this.btnSendCommand_Click);
            // 
            // txtCustomCommand
            // 
            this.txtCustomCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomCommand.Location = new System.Drawing.Point(1515, 175);
            this.txtCustomCommand.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtCustomCommand.Name = "txtCustomCommand";
            this.txtCustomCommand.Size = new System.Drawing.Size(340, 35);
            this.txtCustomCommand.TabIndex = 16;
            this.txtCustomCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCustomCommand_KeyPress);
            // 
            // label24
            // 
            this.label24.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(1520, 139);
            this.label24.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(325, 30);
            this.label24.TabIndex = 15;
            this.label24.Text = "Send custom command via UART:";
            // 
            // chkUseOffline
            // 
            this.chkUseOffline.AutoSize = true;
            this.chkUseOffline.Location = new System.Drawing.Point(879, 75);
            this.chkUseOffline.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.chkUseOffline.Name = "chkUseOffline";
            this.chkUseOffline.Size = new System.Drawing.Size(228, 34);
            this.chkUseOffline.TabIndex = 9;
            this.chkUseOffline.Text = "Use offline database";
            this.chkUseOffline.UseVisualStyleBackColor = true;
            // 
            // btnDownloadDatabase
            // 
            this.btnDownloadDatabase.Location = new System.Drawing.Point(601, 71);
            this.btnDownloadDatabase.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnDownloadDatabase.Name = "btnDownloadDatabase";
            this.btnDownloadDatabase.Size = new System.Drawing.Size(268, 46);
            this.btnDownloadDatabase.TabIndex = 8;
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
            this.btnRefreshPorts.TabIndex = 4;
            this.btnRefreshPorts.Text = "Refresh Ports";
            this.btnRefreshPorts.UseVisualStyleBackColor = true;
            this.btnRefreshPorts.Click += new System.EventHandler(this.btnRefreshPorts_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(1238, 377);
            this.button3.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(267, 46);
            this.button3.TabIndex = 12;
            this.button3.Text = "Clear Output Window";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtUARTOutput
            // 
            this.txtUARTOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUARTOutput.Location = new System.Drawing.Point(121, 139);
            this.txtUARTOutput.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtUARTOutput.Multiline = true;
            this.txtUARTOutput.Name = "txtUARTOutput";
            this.txtUARTOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtUARTOutput.Size = new System.Drawing.Size(1385, 226);
            this.txtUARTOutput.TabIndex = 11;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 139);
            this.label22.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(84, 30);
            this.label22.TabIndex = 10;
            this.label22.Text = "Output:";
            // 
            // btnClearErrorCodes
            // 
            this.btnClearErrorCodes.Location = new System.Drawing.Point(389, 71);
            this.btnClearErrorCodes.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnClearErrorCodes.Name = "btnClearErrorCodes";
            this.btnClearErrorCodes.Size = new System.Drawing.Size(202, 46);
            this.btnClearErrorCodes.TabIndex = 7;
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
            this.label21.TabIndex = 5;
            this.label21.Text = "Options:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(121, 71);
            this.button1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(258, 46);
            this.button1.TabIndex = 6;
            this.button1.Text = "Get 10 Last Error Codes";
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
            this.comboComPorts.TabIndex = 1;
            // 
            // btnDisconnectCom
            // 
            this.btnDisconnectCom.Location = new System.Drawing.Point(740, 9);
            this.btnDisconnectCom.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.btnDisconnectCom.Name = "btnDisconnectCom";
            this.btnDisconnectCom.Size = new System.Drawing.Size(129, 46);
            this.btnDisconnectCom.TabIndex = 3;
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
            this.btnConnectCom.TabIndex = 2;
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
            this.label3.TabIndex = 0;
            this.label3.Text = "Com Port:";
            // 
            // gvErrorCodes
            // 
            this.gvErrorCodes.AllowUserToAddRows = false;
            this.gvErrorCodes.AllowUserToDeleteRows = false;
            this.gvErrorCodes.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.AliceBlue;
            this.gvErrorCodes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gvErrorCodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gvErrorCodes.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.gvErrorCodes.ColumnHeadersHeight = 40;
            this.gvErrorCodes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gvErrorCodes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cErrorCode,
            this.cDescription,
            this.cDetails,
            this.cCodeDetailsLink});
            this.gvErrorCodes.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gvErrorCodes.Location = new System.Drawing.Point(120, 432);
            this.gvErrorCodes.MultiSelect = false;
            this.gvErrorCodes.Name = "gvErrorCodes";
            this.gvErrorCodes.RowHeadersWidth = 72;
            this.gvErrorCodes.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gvErrorCodes.RowTemplate.Height = 37;
            this.gvErrorCodes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gvErrorCodes.Size = new System.Drawing.Size(1385, 614);
            this.gvErrorCodes.TabIndex = 14;
            this.gvErrorCodes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvErrorCodes_CellContentClick);
            this.gvErrorCodes.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.gvErrorCodes_RowPostPaint);
            // 
            // lblLastErrorCodes
            // 
            this.lblLastErrorCodes.AutoSize = true;
            this.lblLastErrorCodes.Location = new System.Drawing.Point(6, 432);
            this.lblLastErrorCodes.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblLastErrorCodes.Name = "lblLastErrorCodes";
            this.lblLastErrorCodes.Size = new System.Drawing.Size(75, 30);
            this.lblLastErrorCodes.TabIndex = 13;
            this.lblLastErrorCodes.Text = "Codes:";
            // 
            // cErrorCode
            // 
            this.cErrorCode.DataPropertyName = "ErrorCode";
            this.cErrorCode.HeaderText = "Error Code";
            this.cErrorCode.MinimumWidth = 9;
            this.cErrorCode.Name = "cErrorCode";
            this.cErrorCode.ReadOnly = true;
            this.cErrorCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cErrorCode.Width = 200;
            // 
            // cDescription
            // 
            this.cDescription.DataPropertyName = "Description";
            this.cDescription.HeaderText = "Error Code Description";
            this.cDescription.MinimumWidth = 9;
            this.cDescription.Name = "cDescription";
            this.cDescription.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cDescription.Width = 930;
            // 
            // cDetails
            // 
            this.cDetails.DataPropertyName = "DetailsLink";
            this.cDetails.HeaderText = "Code Details";
            this.cDetails.MinimumWidth = 9;
            this.cDetails.Name = "cDetails";
            this.cDetails.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cDetails.Text = "Code Details";
            this.cDetails.TrackVisitedState = false;
            this.cDetails.UseColumnTextForLinkValue = true;
            this.cDetails.Width = 150;
            // 
            // cCodeDetailsLink
            // 
            this.cCodeDetailsLink.DataPropertyName = "DetailsLink";
            this.cCodeDetailsLink.HeaderText = "Link";
            this.cCodeDetailsLink.MinimumWidth = 9;
            this.cCodeDetailsLink.Name = "cCodeDetailsLink";
            this.cCodeDetailsLink.ReadOnly = true;
            this.cCodeDetailsLink.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cCodeDetailsLink.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cCodeDetailsLink.Visible = false;
            this.cCodeDetailsLink.Width = 175;
            // 
            // UartUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblLastErrorCodes);
            this.Controls.Add(this.gvErrorCodes);
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
            this.Size = new System.Drawing.Size(1865, 1049);
            this.Load += new System.EventHandler(this.UartUserControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvErrorCodes)).EndInit();
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
        private DataGridView gvErrorCodes;
        private Label lblLastErrorCodes;
        private DataGridViewTextBoxColumn cErrorCode;
        private DataGridViewTextBoxColumn cDescription;
        private DataGridViewLinkColumn cDetails;
        private DataGridViewTextBoxColumn cCodeDetailsLink;
    }
}
