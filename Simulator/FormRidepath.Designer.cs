namespace Simulator
{
    partial class FormRidepath
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.lbDatabase = new System.Windows.Forms.Label();
            this.btnDatabase = new System.Windows.Forms.Button();
            this.openDatabaseDialog = new System.Windows.Forms.OpenFileDialog();
            this.pnlFunctional = new System.Windows.Forms.Panel();
            this.cbEnableTOMessages = new System.Windows.Forms.CheckBox();
            this.btnClearRuler = new System.Windows.Forms.Button();
            this.lblRuler = new System.Windows.Forms.Label();
            this.cbRuler = new System.Windows.Forms.CheckBox();
            this.lblTDFile = new System.Windows.Forms.Label();
            this.btnTDFile = new System.Windows.Forms.Button();
            this.lblClosestTrackObject = new System.Windows.Forms.Label();
            this.lblTOFile = new System.Windows.Forms.Label();
            this.btnTOFile = new System.Windows.Forms.Button();
            this.cbListenSpeed = new System.Windows.Forms.CheckBox();
            this.btnSetSpeed = new System.Windows.Forms.Button();
            this.gbGps = new System.Windows.Forms.GroupBox();
            this.pnlGpsCom = new System.Windows.Forms.Panel();
            this.cbPortSpeeds = new System.Windows.Forms.ComboBox();
            this.lblPortSpeed = new System.Windows.Forms.Label();
            this.btnScanPorts = new System.Windows.Forms.Button();
            this.cbOutputPorts = new System.Windows.Forms.ComboBox();
            this.lblOutputPort = new System.Windows.Forms.Label();
            this.cbGpsProtocols = new System.Windows.Forms.ComboBox();
            this.lblGpsProtocol = new System.Windows.Forms.Label();
            this.cbGpsIsValid = new System.Windows.Forms.CheckBox();
            this.rtbDisplay = new System.Windows.Forms.RichTextBox();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.tbSpeed = new System.Windows.Forms.TextBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.chartRidedistance = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnGE = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tmrSpeed = new System.Windows.Forms.Timer(this.components);
            this.pnlFunctional.SuspendLayout();
            this.gbGps.SuspendLayout();
            this.pnlGpsCom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartRidedistance)).BeginInit();
            this.SuspendLayout();
            // 
            // lbDatabase
            // 
            this.lbDatabase.AutoSize = true;
            this.lbDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbDatabase.ForeColor = System.Drawing.Color.Red;
            this.lbDatabase.Location = new System.Drawing.Point(233, 16);
            this.lbDatabase.Name = "lbDatabase";
            this.lbDatabase.Size = new System.Drawing.Size(91, 13);
            this.lbDatabase.TabIndex = 1;
            this.lbDatabase.Text = "Трэк: не выбран";
            // 
            // btnDatabase
            // 
            this.btnDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDatabase.Location = new System.Drawing.Point(12, 12);
            this.btnDatabase.Name = "btnDatabase";
            this.btnDatabase.Size = new System.Drawing.Size(206, 21);
            this.btnDatabase.TabIndex = 2;
            this.btnDatabase.Text = "Выбрать трэк";
            this.btnDatabase.UseVisualStyleBackColor = true;
            this.btnDatabase.Click += new System.EventHandler(this.btnDatabase_Click);
            // 
            // openDatabaseDialog
            // 
            this.openDatabaseDialog.FileName = "openDatabaseDialog";
            // 
            // pnlFunctional
            // 
            this.pnlFunctional.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFunctional.Controls.Add(this.cbEnableTOMessages);
            this.pnlFunctional.Controls.Add(this.btnClearRuler);
            this.pnlFunctional.Controls.Add(this.lblRuler);
            this.pnlFunctional.Controls.Add(this.cbRuler);
            this.pnlFunctional.Controls.Add(this.lblTDFile);
            this.pnlFunctional.Controls.Add(this.btnTDFile);
            this.pnlFunctional.Controls.Add(this.lblClosestTrackObject);
            this.pnlFunctional.Controls.Add(this.lblTOFile);
            this.pnlFunctional.Controls.Add(this.btnTOFile);
            this.pnlFunctional.Controls.Add(this.cbListenSpeed);
            this.pnlFunctional.Controls.Add(this.btnSetSpeed);
            this.pnlFunctional.Controls.Add(this.gbGps);
            this.pnlFunctional.Controls.Add(this.rtbDisplay);
            this.pnlFunctional.Controls.Add(this.lblSpeed);
            this.pnlFunctional.Controls.Add(this.tbSpeed);
            this.pnlFunctional.Controls.Add(this.btnStop);
            this.pnlFunctional.Controls.Add(this.btnStart);
            this.pnlFunctional.Controls.Add(this.chartRidedistance);
            this.pnlFunctional.Enabled = false;
            this.pnlFunctional.Location = new System.Drawing.Point(12, 39);
            this.pnlFunctional.Name = "pnlFunctional";
            this.pnlFunctional.Size = new System.Drawing.Size(899, 501);
            this.pnlFunctional.TabIndex = 3;
            // 
            // cbEnableTOMessages
            // 
            this.cbEnableTOMessages.AutoSize = true;
            this.cbEnableTOMessages.Checked = true;
            this.cbEnableTOMessages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbEnableTOMessages.Enabled = false;
            this.cbEnableTOMessages.Location = new System.Drawing.Point(0, 26);
            this.cbEnableTOMessages.Name = "cbEnableTOMessages";
            this.cbEnableTOMessages.Size = new System.Drawing.Size(159, 17);
            this.cbEnableTOMessages.TabIndex = 19;
            this.cbEnableTOMessages.Text = "Сообщения о генераторах";
            this.cbEnableTOMessages.UseVisualStyleBackColor = true;
            this.cbEnableTOMessages.CheckedChanged += new System.EventHandler(this.cbEnableTOMessages_CheckedChanged);
            // 
            // btnClearRuler
            // 
            this.btnClearRuler.Enabled = false;
            this.btnClearRuler.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnClearRuler.Location = new System.Drawing.Point(431, 20);
            this.btnClearRuler.Name = "btnClearRuler";
            this.btnClearRuler.Size = new System.Drawing.Size(118, 23);
            this.btnClearRuler.TabIndex = 18;
            this.btnClearRuler.Text = "Сброс";
            this.btnClearRuler.UseVisualStyleBackColor = true;
            this.btnClearRuler.Visible = false;
            this.btnClearRuler.Click += new System.EventHandler(this.btnClearRuler_Click);
            // 
            // lblRuler
            // 
            this.lblRuler.AutoSize = true;
            this.lblRuler.Location = new System.Drawing.Point(419, 26);
            this.lblRuler.Name = "lblRuler";
            this.lblRuler.Size = new System.Drawing.Size(0, 13);
            this.lblRuler.TabIndex = 17;
            this.lblRuler.Visible = false;
            // 
            // cbRuler
            // 
            this.cbRuler.AutoSize = true;
            this.cbRuler.Location = new System.Drawing.Point(343, 25);
            this.cbRuler.Name = "cbRuler";
            this.cbRuler.Size = new System.Drawing.Size(70, 17);
            this.cbRuler.TabIndex = 16;
            this.cbRuler.Text = "Линейка";
            this.cbRuler.UseVisualStyleBackColor = true;
            this.cbRuler.Visible = false;
            this.cbRuler.CheckedChanged += new System.EventHandler(this.cbRuler_CheckedChanged);
            // 
            // lblTDFile
            // 
            this.lblTDFile.AutoSize = true;
            this.lblTDFile.Location = new System.Drawing.Point(221, 34);
            this.lblTDFile.Name = "lblTDFile";
            this.lblTDFile.Size = new System.Drawing.Size(0, 13);
            this.lblTDFile.TabIndex = 15;
            // 
            // btnTDFile
            // 
            this.btnTDFile.Location = new System.Drawing.Point(343, -1);
            this.btnTDFile.Name = "btnTDFile";
            this.btnTDFile.Size = new System.Drawing.Size(206, 23);
            this.btnTDFile.TabIndex = 14;
            this.btnTDFile.Text = "Файл с расстояниями до объектов...";
            this.btnTDFile.UseVisualStyleBackColor = true;
            this.btnTDFile.Visible = false;
            this.btnTDFile.Click += new System.EventHandler(this.btnTDFile_Click);
            // 
            // lblClosestTrackObject
            // 
            this.lblClosestTrackObject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblClosestTrackObject.AutoSize = true;
            this.lblClosestTrackObject.Enabled = false;
            this.lblClosestTrackObject.Location = new System.Drawing.Point(708, 289);
            this.lblClosestTrackObject.Name = "lblClosestTrackObject";
            this.lblClosestTrackObject.Size = new System.Drawing.Size(126, 13);
            this.lblClosestTrackObject.TabIndex = 13;
            this.lblClosestTrackObject.Text = "Ближайший объект: NA";
            // 
            // lblTOFile
            // 
            this.lblTOFile.AutoSize = true;
            this.lblTOFile.Location = new System.Drawing.Point(221, 5);
            this.lblTOFile.Name = "lblTOFile";
            this.lblTOFile.Size = new System.Drawing.Size(0, 13);
            this.lblTOFile.TabIndex = 12;
            // 
            // btnTOFile
            // 
            this.btnTOFile.Location = new System.Drawing.Point(0, 0);
            this.btnTOFile.Name = "btnTOFile";
            this.btnTOFile.Size = new System.Drawing.Size(206, 23);
            this.btnTOFile.TabIndex = 11;
            this.btnTOFile.Text = "Файл объектов на трэке";
            this.btnTOFile.UseVisualStyleBackColor = true;
            this.btnTOFile.Click += new System.EventHandler(this.btnTOFile_Click);
            // 
            // cbListenSpeed
            // 
            this.cbListenSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbListenSpeed.AutoSize = true;
            this.cbListenSpeed.Location = new System.Drawing.Point(542, 288);
            this.cbListenSpeed.Name = "cbListenSpeed";
            this.cbListenSpeed.Size = new System.Drawing.Size(127, 17);
            this.cbListenSpeed.TabIndex = 10;
            this.cbListenSpeed.Text = "Скорость со стенда";
            this.cbListenSpeed.UseVisualStyleBackColor = true;
            this.cbListenSpeed.CheckedChanged += new System.EventHandler(this.cbListenSpeed_CheckedChanged);
            // 
            // btnSetSpeed
            // 
            this.btnSetSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSetSpeed.Location = new System.Drawing.Point(474, 285);
            this.btnSetSpeed.Name = "btnSetSpeed";
            this.btnSetSpeed.Size = new System.Drawing.Size(58, 23);
            this.btnSetSpeed.TabIndex = 9;
            this.btnSetSpeed.Text = "Задать";
            this.btnSetSpeed.UseVisualStyleBackColor = true;
            this.btnSetSpeed.Click += new System.EventHandler(this.btnSetSpeed_Click);
            // 
            // gbGps
            // 
            this.gbGps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gbGps.Controls.Add(this.pnlGpsCom);
            this.gbGps.Controls.Add(this.cbGpsIsValid);
            this.gbGps.Location = new System.Drawing.Point(0, 327);
            this.gbGps.Name = "gbGps";
            this.gbGps.Size = new System.Drawing.Size(899, 48);
            this.gbGps.TabIndex = 7;
            this.gbGps.TabStop = false;
            this.gbGps.Text = "GPS";
            // 
            // pnlGpsCom
            // 
            this.pnlGpsCom.Controls.Add(this.cbPortSpeeds);
            this.pnlGpsCom.Controls.Add(this.lblPortSpeed);
            this.pnlGpsCom.Controls.Add(this.btnScanPorts);
            this.pnlGpsCom.Controls.Add(this.cbOutputPorts);
            this.pnlGpsCom.Controls.Add(this.lblOutputPort);
            this.pnlGpsCom.Controls.Add(this.cbGpsProtocols);
            this.pnlGpsCom.Controls.Add(this.lblGpsProtocol);
            this.pnlGpsCom.Location = new System.Drawing.Point(122, 11);
            this.pnlGpsCom.Name = "pnlGpsCom";
            this.pnlGpsCom.Size = new System.Drawing.Size(773, 34);
            this.pnlGpsCom.TabIndex = 8;
            // 
            // cbPortSpeeds
            // 
            this.cbPortSpeeds.FormattingEnabled = true;
            this.cbPortSpeeds.Location = new System.Drawing.Point(638, 6);
            this.cbPortSpeeds.Name = "cbPortSpeeds";
            this.cbPortSpeeds.Size = new System.Drawing.Size(121, 21);
            this.cbPortSpeeds.TabIndex = 14;
            // 
            // lblPortSpeed
            // 
            this.lblPortSpeed.AutoSize = true;
            this.lblPortSpeed.Location = new System.Drawing.Point(574, 10);
            this.lblPortSpeed.Name = "lblPortSpeed";
            this.lblPortSpeed.Size = new System.Drawing.Size(58, 13);
            this.lblPortSpeed.TabIndex = 13;
            this.lblPortSpeed.Text = "Скорость:";
            // 
            // btnScanPorts
            // 
            this.btnScanPorts.Location = new System.Drawing.Point(436, 6);
            this.btnScanPorts.Name = "btnScanPorts";
            this.btnScanPorts.Size = new System.Drawing.Size(95, 23);
            this.btnScanPorts.TabIndex = 12;
            this.btnScanPorts.Text = "Сканировать";
            this.btnScanPorts.UseVisualStyleBackColor = true;
            // 
            // cbOutputPorts
            // 
            this.cbOutputPorts.FormattingEnabled = true;
            this.cbOutputPorts.Location = new System.Drawing.Point(309, 7);
            this.cbOutputPorts.Name = "cbOutputPorts";
            this.cbOutputPorts.Size = new System.Drawing.Size(121, 21);
            this.cbOutputPorts.TabIndex = 11;
            // 
            // lblOutputPort
            // 
            this.lblOutputPort.AutoSize = true;
            this.lblOutputPort.Location = new System.Drawing.Point(227, 10);
            this.lblOutputPort.Name = "lblOutputPort";
            this.lblOutputPort.Size = new System.Drawing.Size(76, 13);
            this.lblOutputPort.TabIndex = 10;
            this.lblOutputPort.Text = "Порт вывода:";
            // 
            // cbGpsProtocols
            // 
            this.cbGpsProtocols.FormattingEnabled = true;
            this.cbGpsProtocols.Location = new System.Drawing.Point(68, 8);
            this.cbGpsProtocols.Name = "cbGpsProtocols";
            this.cbGpsProtocols.Size = new System.Drawing.Size(121, 21);
            this.cbGpsProtocols.TabIndex = 9;
            // 
            // lblGpsProtocol
            // 
            this.lblGpsProtocol.AutoSize = true;
            this.lblGpsProtocol.Location = new System.Drawing.Point(3, 11);
            this.lblGpsProtocol.Name = "lblGpsProtocol";
            this.lblGpsProtocol.Size = new System.Drawing.Size(59, 13);
            this.lblGpsProtocol.TabIndex = 8;
            this.lblGpsProtocol.Text = "Протокол:";
            // 
            // cbGpsIsValid
            // 
            this.cbGpsIsValid.AutoSize = true;
            this.cbGpsIsValid.Checked = true;
            this.cbGpsIsValid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGpsIsValid.Location = new System.Drawing.Point(10, 20);
            this.cbGpsIsValid.Name = "cbGpsIsValid";
            this.cbGpsIsValid.Size = new System.Drawing.Size(105, 17);
            this.cbGpsIsValid.TabIndex = 0;
            this.cbGpsIsValid.Text = "Достоверность";
            this.cbGpsIsValid.UseVisualStyleBackColor = true;
            // 
            // rtbDisplay
            // 
            this.rtbDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rtbDisplay.Location = new System.Drawing.Point(0, 386);
            this.rtbDisplay.Name = "rtbDisplay";
            this.rtbDisplay.Size = new System.Drawing.Size(899, 113);
            this.rtbDisplay.TabIndex = 6;
            this.rtbDisplay.Text = "";
            // 
            // lblSpeed
            // 
            this.lblSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(280, 289);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(91, 13);
            this.lblSpeed.TabIndex = 5;
            this.lblSpeed.Text = "Скорость (км/ч):";
            // 
            // tbSpeed
            // 
            this.tbSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbSpeed.Location = new System.Drawing.Point(368, 287);
            this.tbSpeed.Name = "tbSpeed";
            this.tbSpeed.Size = new System.Drawing.Size(100, 20);
            this.tbSpeed.TabIndex = 4;
            this.tbSpeed.Text = "40";
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStop.Location = new System.Drawing.Point(121, 285);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(115, 23);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Стоп";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStart.Location = new System.Drawing.Point(0, 285);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(115, 23);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Старт";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // chartRidedistance
            // 
            this.chartRidedistance.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisY.Interval = 10D;
            chartArea1.AxisY.MajorGrid.Enabled = false;
            chartArea1.AxisY.MajorTickMark.Enabled = false;
            chartArea1.Name = "DefaultChartArea";
            this.chartRidedistance.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartRidedistance.Legends.Add(legend1);
            this.chartRidedistance.Location = new System.Drawing.Point(0, 50);
            this.chartRidedistance.Name = "chartRidedistance";
            series1.ChartArea = "DefaultChartArea";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Legend = "Legend1";
            series1.Name = "SeriesFullPath";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            this.chartRidedistance.Series.Add(series1);
            this.chartRidedistance.Size = new System.Drawing.Size(899, 211);
            this.chartRidedistance.TabIndex = 0;
            this.chartRidedistance.AxisViewChanged += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ViewEventArgs>(this.chartRidedistance_AxisViewChanged);
            this.chartRidedistance.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chartRidedistance_MouseDown);
            this.chartRidedistance.MouseMove += new System.Windows.Forms.MouseEventHandler(this.chartRidedistance_MouseMove);
            this.chartRidedistance.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chartRidedistance_MouseUp);
            // 
            // btnGE
            // 
            this.btnGE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGE.Location = new System.Drawing.Point(753, 10);
            this.btnGE.Name = "btnGE";
            this.btnGE.Size = new System.Drawing.Size(158, 23);
            this.btnGE.TabIndex = 20;
            this.btnGE.Text = "Google Earth";
            this.btnGE.UseVisualStyleBackColor = true;
            this.btnGE.Click += new System.EventHandler(this.btnGE_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(520, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(161, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tmrSpeed
            // 
            this.tmrSpeed.Interval = 1000;
            this.tmrSpeed.Tick += new System.EventHandler(this.tmrSpeed_Tick);
            // 
            // FormRidepath
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(923, 552);
            this.Controls.Add(this.btnGE);
            this.Controls.Add(this.pnlFunctional);
            this.Controls.Add(this.btnDatabase);
            this.Controls.Add(this.lbDatabase);
            this.Controls.Add(this.button1);
            this.Name = "FormRidepath";
            this.Text = "FormRidepath";
            this.pnlFunctional.ResumeLayout(false);
            this.pnlFunctional.PerformLayout();
            this.gbGps.ResumeLayout(false);
            this.gbGps.PerformLayout();
            this.pnlGpsCom.ResumeLayout(false);
            this.pnlGpsCom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartRidedistance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbDatabase;
        private System.Windows.Forms.Button btnDatabase;
        private System.Windows.Forms.OpenFileDialog openDatabaseDialog;
        private System.Windows.Forms.Panel pnlFunctional;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRidedistance;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.TextBox tbSpeed;
        private System.Windows.Forms.Timer tmrSpeed;
        private System.Windows.Forms.RichTextBox rtbDisplay;
        private System.Windows.Forms.GroupBox gbGps;
        private System.Windows.Forms.CheckBox cbGpsIsValid;
        private System.Windows.Forms.Button btnSetSpeed;
        private System.Windows.Forms.CheckBox cbListenSpeed;
        private System.Windows.Forms.Label lblTOFile;
        private System.Windows.Forms.Button btnTOFile;
        private System.Windows.Forms.Label lblClosestTrackObject;
        private System.Windows.Forms.Button btnTDFile;
        private System.Windows.Forms.Label lblTDFile;
        private System.Windows.Forms.CheckBox cbRuler;
        private System.Windows.Forms.Label lblRuler;
        private System.Windows.Forms.Button btnClearRuler;
        private System.Windows.Forms.CheckBox cbEnableTOMessages;
        private System.Windows.Forms.Panel pnlGpsCom;
        private System.Windows.Forms.ComboBox cbPortSpeeds;
        private System.Windows.Forms.Label lblPortSpeed;
        private System.Windows.Forms.Button btnScanPorts;
        private System.Windows.Forms.ComboBox cbOutputPorts;
        private System.Windows.Forms.Label lblOutputPort;
        private System.Windows.Forms.ComboBox cbGpsProtocols;
        private System.Windows.Forms.Label lblGpsProtocol;
        private System.Windows.Forms.Button btnGE;
    }
}