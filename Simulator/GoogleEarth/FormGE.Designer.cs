namespace Simulator.GoogleEarth
{
    partial class FormGE
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.geWebBrowser = new FC.GEPluginCtrls.GEWebBrowser();
            this.pnlFunctional = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.btnTLS = new System.Windows.Forms.Button();
            this.gbRuler = new System.Windows.Forms.GroupBox();
            this.lblCursorCoordinateValue = new System.Windows.Forms.Label();
            this.lblCursorCoordinate = new System.Windows.Forms.Label();
            this.lblRulerLengthValue = new System.Windows.Forms.Label();
            this.btnClearRuler = new System.Windows.Forms.Button();
            this.lblRulerLength = new System.Windows.Forms.Label();
            this.cbEnableRuler = new System.Windows.Forms.CheckBox();
            this.btnRWK = new System.Windows.Forms.Button();
            this.pnlGEStatus = new System.Windows.Forms.Panel();
            this.lblGEStatus = new System.Windows.Forms.Label();
            this.gbTracking = new System.Windows.Forms.GroupBox();
            this.gbModel = new System.Windows.Forms.GroupBox();
            this.cbModelSwitch = new System.Windows.Forms.CheckBox();
            this.pnlFunctional.SuspendLayout();
            this.gbRuler.SuspendLayout();
            this.pnlGEStatus.SuspendLayout();
            this.gbModel.SuspendLayout();
            this.SuspendLayout();
            // 
            // geWebBrowser
            // 
            this.geWebBrowser.AllowNavigation = false;
            this.geWebBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.geWebBrowser.ImageryBase = FC.GEPluginCtrls.ImageryBase.Earth;
            this.geWebBrowser.IsWebBrowserContextMenuEnabled = false;
            this.geWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.geWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.geWebBrowser.Name = "geWebBrowser";
            this.geWebBrowser.RedirectLinksToSystemBrowser = false;
            this.geWebBrowser.ScrollBarsEnabled = false;
            this.geWebBrowser.Size = new System.Drawing.Size(780, 351);
            this.geWebBrowser.TabIndex = 0;
            this.geWebBrowser.WebBrowserShortcutsEnabled = false;
            // 
            // pnlFunctional
            // 
            this.pnlFunctional.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFunctional.Controls.Add(this.button1);
            this.pnlFunctional.Controls.Add(this.btnTLS);
            this.pnlFunctional.Controls.Add(this.gbRuler);
            this.pnlFunctional.Controls.Add(this.btnRWK);
            this.pnlFunctional.Enabled = false;
            this.pnlFunctional.Location = new System.Drawing.Point(0, 390);
            this.pnlFunctional.Name = "pnlFunctional";
            this.pnlFunctional.Size = new System.Drawing.Size(780, 194);
            this.pnlFunctional.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(414, 47);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnTLS
            // 
            this.btnTLS.Location = new System.Drawing.Point(7, 131);
            this.btnTLS.Name = "btnTLS";
            this.btnTLS.Size = new System.Drawing.Size(177, 23);
            this.btnTLS.TabIndex = 2;
            this.btnTLS.Text = "Светофоры";
            this.btnTLS.UseVisualStyleBackColor = true;
            this.btnTLS.Click += new System.EventHandler(this.btnTLS_Click);
            // 
            // gbRuler
            // 
            this.gbRuler.Controls.Add(this.lblCursorCoordinateValue);
            this.gbRuler.Controls.Add(this.lblCursorCoordinate);
            this.gbRuler.Controls.Add(this.lblRulerLengthValue);
            this.gbRuler.Controls.Add(this.btnClearRuler);
            this.gbRuler.Controls.Add(this.lblRulerLength);
            this.gbRuler.Controls.Add(this.cbEnableRuler);
            this.gbRuler.Location = new System.Drawing.Point(7, 3);
            this.gbRuler.Name = "gbRuler";
            this.gbRuler.Size = new System.Drawing.Size(346, 93);
            this.gbRuler.TabIndex = 1;
            this.gbRuler.TabStop = false;
            this.gbRuler.Text = "Линейка";
            // 
            // lblCursorCoordinateValue
            // 
            this.lblCursorCoordinateValue.AutoSize = true;
            this.lblCursorCoordinateValue.Location = new System.Drawing.Point(117, 66);
            this.lblCursorCoordinateValue.Name = "lblCursorCoordinateValue";
            this.lblCursorCoordinateValue.Size = new System.Drawing.Size(0, 13);
            this.lblCursorCoordinateValue.TabIndex = 5;
            // 
            // lblCursorCoordinate
            // 
            this.lblCursorCoordinate.AutoSize = true;
            this.lblCursorCoordinate.Location = new System.Drawing.Point(6, 66);
            this.lblCursorCoordinate.Name = "lblCursorCoordinate";
            this.lblCursorCoordinate.Size = new System.Drawing.Size(142, 13);
            this.lblCursorCoordinate.TabIndex = 4;
            this.lblCursorCoordinate.Text = "Координата курсора (ш/д):";
            // 
            // lblRulerLengthValue
            // 
            this.lblRulerLengthValue.AutoSize = true;
            this.lblRulerLengthValue.Location = new System.Drawing.Point(62, 44);
            this.lblRulerLengthValue.Name = "lblRulerLengthValue";
            this.lblRulerLengthValue.Size = new System.Drawing.Size(13, 13);
            this.lblRulerLengthValue.TabIndex = 3;
            this.lblRulerLengthValue.Text = "—";
            // 
            // btnClearRuler
            // 
            this.btnClearRuler.Location = new System.Drawing.Point(102, 17);
            this.btnClearRuler.Name = "btnClearRuler";
            this.btnClearRuler.Size = new System.Drawing.Size(75, 19);
            this.btnClearRuler.TabIndex = 2;
            this.btnClearRuler.Text = "Очистить";
            this.btnClearRuler.UseVisualStyleBackColor = true;
            this.btnClearRuler.Click += new System.EventHandler(this.btnClearRuler_Click);
            // 
            // lblRulerLength
            // 
            this.lblRulerLength.AutoSize = true;
            this.lblRulerLength.Location = new System.Drawing.Point(5, 44);
            this.lblRulerLength.Name = "lblRulerLength";
            this.lblRulerLength.Size = new System.Drawing.Size(60, 13);
            this.lblRulerLength.TabIndex = 1;
            this.lblRulerLength.Text = "Длина (м):";
            // 
            // cbEnableRuler
            // 
            this.cbEnableRuler.AutoSize = true;
            this.cbEnableRuler.Location = new System.Drawing.Point(9, 19);
            this.cbEnableRuler.Name = "cbEnableRuler";
            this.cbEnableRuler.Size = new System.Drawing.Size(77, 17);
            this.cbEnableRuler.TabIndex = 0;
            this.cbEnableRuler.Text = "Вкл/Выкл";
            this.cbEnableRuler.UseVisualStyleBackColor = true;
            this.cbEnableRuler.CheckedChanged += new System.EventHandler(this.cbEnableRuler_CheckedChanged);
            // 
            // btnRWK
            // 
            this.btnRWK.Location = new System.Drawing.Point(7, 102);
            this.btnRWK.Name = "btnRWK";
            this.btnRWK.Size = new System.Drawing.Size(177, 23);
            this.btnRWK.TabIndex = 0;
            this.btnRWK.Text = "ЖД километры";
            this.btnRWK.UseVisualStyleBackColor = true;
            this.btnRWK.Click += new System.EventHandler(this.btnRWK_Click);
            // 
            // pnlGEStatus
            // 
            this.pnlGEStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlGEStatus.Controls.Add(this.lblGEStatus);
            this.pnlGEStatus.Location = new System.Drawing.Point(0, 357);
            this.pnlGEStatus.Name = "pnlGEStatus";
            this.pnlGEStatus.Size = new System.Drawing.Size(780, 27);
            this.pnlGEStatus.TabIndex = 2;
            // 
            // lblGEStatus
            // 
            this.lblGEStatus.AutoSize = true;
            this.lblGEStatus.Location = new System.Drawing.Point(4, 7);
            this.lblGEStatus.Name = "lblGEStatus";
            this.lblGEStatus.Size = new System.Drawing.Size(62, 13);
            this.lblGEStatus.TabIndex = 0;
            this.lblGEStatus.Text = "lblGEStatus";
            // 
            // gbTracking
            // 
            this.gbTracking.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTracking.Location = new System.Drawing.Point(786, 80);
            this.gbTracking.Name = "gbTracking";
            this.gbTracking.Size = new System.Drawing.Size(343, 165);
            this.gbTracking.TabIndex = 4;
            this.gbTracking.TabStop = false;
            this.gbTracking.Text = "Слежение";
            // 
            // gbModel
            // 
            this.gbModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbModel.Controls.Add(this.cbModelSwitch);
            this.gbModel.Location = new System.Drawing.Point(786, 0);
            this.gbModel.Name = "gbModel";
            this.gbModel.Size = new System.Drawing.Size(343, 74);
            this.gbModel.TabIndex = 5;
            this.gbModel.TabStop = false;
            this.gbModel.Text = "Модель";
            // 
            // cbModelSwitch
            // 
            this.cbModelSwitch.AutoSize = true;
            this.cbModelSwitch.Location = new System.Drawing.Point(7, 20);
            this.cbModelSwitch.Name = "cbModelSwitch";
            this.cbModelSwitch.Size = new System.Drawing.Size(65, 17);
            this.cbModelSwitch.TabIndex = 0;
            this.cbModelSwitch.Text = "Модель";
            this.cbModelSwitch.UseVisualStyleBackColor = true;
            // 
            // FormGE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1129, 586);
            this.Controls.Add(this.gbModel);
            this.Controls.Add(this.gbTracking);
            this.Controls.Add(this.pnlGEStatus);
            this.Controls.Add(this.pnlFunctional);
            this.Controls.Add(this.geWebBrowser);
            this.Name = "FormGE";
            this.Text = "FormGE";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormGE_FormClosing);
            this.pnlFunctional.ResumeLayout(false);
            this.gbRuler.ResumeLayout(false);
            this.gbRuler.PerformLayout();
            this.pnlGEStatus.ResumeLayout(false);
            this.pnlGEStatus.PerformLayout();
            this.gbModel.ResumeLayout(false);
            this.gbModel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private FC.GEPluginCtrls.GEWebBrowser geWebBrowser;
        private System.Windows.Forms.Panel pnlFunctional;
        private System.Windows.Forms.Panel pnlGEStatus;
        private System.Windows.Forms.Label lblGEStatus;
        private System.Windows.Forms.Button btnRWK;
        private System.Windows.Forms.GroupBox gbRuler;
        private System.Windows.Forms.CheckBox cbEnableRuler;
        private System.Windows.Forms.Label lblRulerLength;
        private System.Windows.Forms.Button btnClearRuler;
        private System.Windows.Forms.Label lblRulerLengthValue;
        private System.Windows.Forms.Label lblCursorCoordinate;
        private System.Windows.Forms.Label lblCursorCoordinateValue;
        private System.Windows.Forms.Button btnTLS;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox gbTracking;
        private System.Windows.Forms.GroupBox gbModel;
        private System.Windows.Forms.CheckBox cbModelSwitch;
    }
}