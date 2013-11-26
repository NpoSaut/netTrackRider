using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.IO.Ports;
using BlokFrames;
using Communications.Appi;
using Communications.Appi.Winusb;
using Communications.Can;
using Simulator.GPS;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using Simulator.GoogleEarth;

namespace Simulator
{
    public partial class FormRidepath : Form
    {
        public string databaseFile = null;
        public DatabaseParser databaseParser = null;
        public SerialPort comPort = null;
        public bool comPortFound = false;
        public IGPSProtocol gpsProtocol = null;
        public string trackobjectsFile = null;
        public List<TrackObjectRecord> trackObjects = new List<TrackObjectRecord>();
        public string targetdistancesFile = null;

        public CanPort Port { get; private set; }

        public FormRidepath()
        {
            InitializeComponent();
            InitializeRidedistanceChart();
            InitializeGpsEmulation();
            InitializeNetworkInterop();
            InitializeCan();
        }

        private void InitializeCan()
        {
            try
            {
                var dev = WinusbAppiDev.GetDevices().First(ds => ds.IsFree).OpenDevice();
                this.Port = dev.CanPorts[AppiLine.Can1];
                this.Closed += (Sender, Args) => dev.Dispose();     // Не хорошо, конечно. Но лучше так, чем никак :D
            } catch (Exception exception)
            {
                MessageBox.Show(string.Format("Не удалось водключиться к АППИ:\n{0}\n\nПопробуйте переподключить АППИ и перезапустить программу.", exception.Message),
                                "Ошибка при подключении к АППИ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeRidedistanceChart()
        {
            // Cleanup
            this.chartRidedistance.ChartAreas.Clear();
            this.chartRidedistance.Series.Clear();
            this.chartRidedistance.Annotations.Clear();
            this.chartRidedistance.Legends.Clear();
            this.chartRidedistance.Titles.Clear();


            // Chart picture settings
            chartRidedistance.AntiAliasing = AntiAliasingStyles.All;
            chartRidedistance.TextAntiAliasingQuality = TextAntiAliasingQuality.High;


            // Chart area settings
            ChartArea ChartAreaDefault = new ChartArea("ChartAreaDefault");
            this.chartRidedistance.ChartAreas.Add(ChartAreaDefault);

            // X-Axis (for chart area) settings
            this.chartRidedistance.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            this.chartRidedistance.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            this.chartRidedistance.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;
            this.chartRidedistance.ChartAreas[0].AxisX.MinorTickMark.Enabled = false;

            this.chartRidedistance.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
            this.chartRidedistance.ChartAreas[0].AxisX.IsMarginVisible = false;
            //this.chartRidedistance.ChartAreas[0].AxisX.IsStartedFromZero = true;
            this.chartRidedistance.ChartAreas[0].AxisX.Crossing = 0;

            // Y-Axis (for chart area) settings
            this.chartRidedistance.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            this.chartRidedistance.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            this.chartRidedistance.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;
            this.chartRidedistance.ChartAreas[0].AxisY.MinorTickMark.Enabled = false;

            this.chartRidedistance.ChartAreas[0].AxisY.LabelStyle.Enabled = false;
            this.chartRidedistance.ChartAreas[0].AxisY.IsMarginVisible = false;
            //this.chartRidedistance.ChartAreas[0].AxisY.IsStartedFromZero = true;
            this.chartRidedistance.ChartAreas[0].AxisY.Minimum = -5;
            this.chartRidedistance.ChartAreas[0].AxisY.Maximum = 5;
            this.chartRidedistance.ChartAreas[0].AxisY.Crossing = -5;



            // Series settings
            Series SeriesTrackLength = new Series("SeriesTrackLength");
            this.chartRidedistance.Series.Add(SeriesTrackLength);
            this.chartRidedistance.Series[0].ChartType = SeriesChartType.FastLine;

            // Series settings
            Series SeriesRide = new Series("SeriesRide");
            this.chartRidedistance.Series.Add(SeriesRide);
            this.chartRidedistance.Series[1].ChartType = SeriesChartType.FastLine;
            this.chartRidedistance.Series[1].Color = Color.Red;

            // Series settings
            Series SeriesStartMarker = new Series("SeriesStartMarker");
            this.chartRidedistance.Series.Add(SeriesStartMarker);
            this.chartRidedistance.Series["SeriesStartMarker"].ChartType = SeriesChartType.Point;
            this.chartRidedistance.Series["SeriesStartMarker"].Color = Color.FromArgb(255, 65, 140, 240);
            this.chartRidedistance.Series["SeriesStartMarker"].MarkerStyle = MarkerStyle.Square;
            this.chartRidedistance.Series["SeriesStartMarker"].MarkerSize = 5;
            //this.chartRidedistance.Series["SeriesStartMarker"].IsValueShownAsLabel = true;
            //this.chartRidedistance.Series["SeriesStartMarker"]["LabelStyle"] = "Top";

            // Series settings
            Series SeriesRidingMarker = new Series("SeriesRidingMarker");
            this.chartRidedistance.Series.Add(SeriesRidingMarker);
            this.chartRidedistance.Series["SeriesRidingMarker"].ChartType = SeriesChartType.Point;
            this.chartRidedistance.Series["SeriesRidingMarker"].Color = Color.Red;
            this.chartRidedistance.Series["SeriesRidingMarker"].MarkerStyle = MarkerStyle.Square;
            this.chartRidedistance.Series["SeriesRidingMarker"].MarkerSize = 5;
            //this.chartRidedistance.Series["SeriesRidingMarker"].Label = "#VALX";
            //this.chartRidedistance.Series["SeriesRidingMarker"].LabelFormat = "F2";   // http://msdn.microsoft.com/en-us/library/dwhawy9k.aspx
            this.chartRidedistance.Series["SeriesRidingMarker"].SmartLabelStyle.Enabled = true;
            this.chartRidedistance.Series["SeriesRidingMarker"].SmartLabelStyle.IsMarkerOverlappingAllowed = false;
            this.chartRidedistance.Series["SeriesRidingMarker"].SmartLabelStyle.MovingDirection = LabelAlignmentStyles.Top;



            // Zooming and scrolling
            this.chartRidedistance.ChartAreas[0].CursorX.IsUserEnabled = true;
            this.chartRidedistance.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            this.chartRidedistance.ChartAreas[0].CursorX.Interval = 0;

            this.chartRidedistance.ChartAreas[0].CursorY.IsUserEnabled = true;
            this.chartRidedistance.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            this.chartRidedistance.ChartAreas[0].CursorY.Interval = 0;

            this.chartRidedistance.ChartAreas[0].CursorX.AutoScroll = false;
            this.chartRidedistance.ChartAreas[0].CursorY.AutoScroll = false;

            this.chartRidedistance.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            this.chartRidedistance.ChartAreas[0].AxisY.ScaleView.Zoomable = false;

            this.chartRidedistance.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            this.chartRidedistance.ChartAreas[0].AxisY.ScrollBar.Enabled = false;

            this.chartRidedistance.ChartAreas["ChartAreaDefault"].AxisX.ScrollBar.IsPositionedInside = false;
            this.chartRidedistance.ChartAreas["ChartAreaDefault"].AxisY.ScrollBar.IsPositionedInside = false;


            // Annotation for cursor
            RectangleAnnotation annotation = new RectangleAnnotation();
            annotation.AnchorX = 9;
            annotation.AnchorY = 12;
            annotation.ForeColor = Color.Black;
            annotation.Font = this.chartRidedistance.ChartAreas[0].AxisX.LabelStyle.Font;
            annotation.LineWidth = 1;
            annotation.LineColor = Color.Gray;
            annotation.BackColor = Color.White;
            annotation.LineDashStyle = ChartDashStyle.Solid;
            this.chartRidedistance.Annotations.Add(annotation);



            /*chart1.ChartAreas["Default"].AxisX.LabelStyle.Interval = Math.PI;
            chart1.ChartAreas["Default"].AxisX.LabelStyle.Format = "##.##";
            chart1.ChartAreas["Default"].AxisX.MajorGrid.Interval = Math.PI;
            chart1.ChartAreas["Default"].AxisX.MinorGrid.Interval = Math.PI / 4;
            chart1.ChartAreas["Default"].AxisX.MinorTickMark.Interval = Math.PI / 4;
            chart1.ChartAreas["Default"].AxisX.MajorTickMark.Interval = Math.PI;
            chart1.ChartAreas["Default"].AxisY.MinorGrid.Interval = 0.25;
            chart1.ChartAreas["Default"].AxisY.MajorGrid.Interval = 0.5;
            chart1.ChartAreas["Default"].AxisY.LabelStyle.Interval = 0.5;*/

            /*foreach(Series series in chart1.Series)
            {
                // Set empty points visual appearance attributes
                series.EmptyPointStyle.Color = Color.Gray;
                series.EmptyPointStyle.BorderWidth = 2;
                series.EmptyPointStyle.BorderDashStyle = ChartDashStyle.Dash;
                series.EmptyPointStyle.MarkerSize = 7;
                series.EmptyPointStyle.MarkerStyle = MarkerStyle.Cross;
                series.EmptyPointStyle.MarkerBorderColor = Color.Black;
             }*/

            /*


            // Default Chart Area styling
            chartRidedistance.ChartAreas[0].BorderColor = Color.Black;
            chartRidedistance.ChartAreas[0].BorderWidth = 1;
            chartRidedistance.ChartAreas[0].BorderDashStyle = ChartDashStyle.Solid;*/

            // Full path plot(series) styling
        }

        private void InitializeGpsEmulation()
        {
            // Initialize GUI
            cbGpsProtocols.Items.AddRange(new object[] { "NMEA-0183", "MNP-Binary" });
            cbGpsProtocols.SelectedIndex = 0;
            
            // Initialize COM port GUI & parameters
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length != 0)
            {
                cbOutputPorts.Items.AddRange(SerialPort.GetPortNames().OrderBy(s => s).ToArray());
                cbOutputPorts.SelectedIndex = 0;

                cbPortSpeeds.Items.AddRange(new object[] { 1200, 2400, 9600, 19200, 38400, 57600, 115200 });
                cbPortSpeeds.SelectedIndex = 2;

                comPortFound = true;
            }
            else
            {
                cbOutputPorts.Items.Add("No COM found");
                cbPortSpeeds.Items.Add("No COM found");
            }
            cbListenSpeed.Checked = true;
            // Change these two on the speed property
            this.speed_ms = 0;
            UpdateCurrentSpeed();
        }

        private void btnDatabase_Click(object sender, EventArgs e)
        {
            DialogResult result = openDatabaseDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                databaseFile = openDatabaseDialog.FileName;
                lbDatabase.Text = databaseFile;
                lbDatabase.ForeColor = SystemColors.ControlText;
                btnDatabase.Text = "Изменить базу данных";
                pnlFunctional.Enabled = true;

                databaseParser = new DatabaseParser(databaseFile, true);

                
                // Full track
                this.trackLength = databaseParser.TrackLength;
                chartRidedistance.Series[0].Points.AddXY(0, 0);
                chartRidedistance.Series[0].Points.AddXY(this.trackLength, 0);
                // Иначе они в автоматическом режиме и как получить минимальное/максимальное значаение по оси — непонятно.
                this.chartRidedistance.ChartAreas[0].AxisX.Minimum = 0;
                this.chartRidedistance.ChartAreas[0].AxisX.Maximum = this.trackLength;

                SetXAxisLabels();

                // Starting point
                SetStartingDistance(0);

                // Refresh chart
                //chartRidedistance.Invalidate();

            }
        }

        


        private double GetCurrentSpeed()
        {
            return Convert.ToDouble(this.tbSpeed.Text.Replace('.', ',')) * 1000 / 3600;
        }

        private void UpdateCurrentSpeed()
        {
            double speed_kmh = this.speed_ms * 3600 / 1000;
            this.tbSpeed.Text = speed_kmh.ToString();
        }

        enum SimulationState
        {
            Off,
            Riding,
            Standing
        }

        SimulationState simulationState = SimulationState.Off;

        // Ride speed
        double trackLength = 0;
        private double speed_ms = 0;   // m/s 2000 = 100 m/s
        //private double offset = 0;  // Get distance (offset)
        //private double distance_passed = 0;  //
        private double distance_on_track = 0;  //
        int closestTrackObjectIndex = -1;

        private void StartRide()
        {
            this.simulationState = SimulationState.Riding;
            
            this.distance_on_track = GetStartingDistance();
            this.speed_ms = GetCurrentSpeed();

            chartRidedistance.Series[1].Points.Clear();
            chartRidedistance.Series[1].Points.AddXY(this.distance_on_track, 0);
            chartRidedistance.Series["SeriesRidingMarker"].Points.Clear();
            chartRidedistance.Series["SeriesRidingMarker"].Points.AddXY(this.distance_on_track, 0);
            this.chartRidedistance.Series["SeriesRidingMarker"].Label = ((this.distance_on_track - this.chartRidedistance.Series["SeriesStartMarker"].Points[0].XValue) / 1000).ToString("0.00");
            tmrSpeed.Enabled = true;
            tmrSpeedPump.Enabled = true;

            if (comPortFound)
            {
                if (comPort == null)
                {
                    try
                    {
                        comPort = new SerialPort(cbOutputPorts.SelectedItem.ToString());
                    }
                    catch
                    {
                        comPort = null;
                        comPortFound = false;
                    }
                }

                if (!comPort.IsOpen)
                {
                    try
                    {
                        comPort.BaudRate = Convert.ToInt32(cbPortSpeeds.SelectedItem);
                        comPort.DataBits = 8;
                        comPort.Parity = Parity.None;
                        comPort.StopBits = StopBits.One;
                        comPort.Handshake = Handshake.None;
                        comPort.ReadTimeout = 100;
                        comPort.WriteTimeout = 100;
                        comPort.Open();
                    }
                    catch
                    {
                        comPort = null;
                        comPortFound = false;
                    }
                }
            }

            // { "MNP-Binary", "NMEA-0183" } Слишком много new — перенести в OnStart?
            // Лучше всего создавать / менять при выборе в комбобоксеыы
            switch (cbGpsProtocols.SelectedItem.ToString())
            {
                case "NMEA-0183":
                    gpsProtocol = new NMEA0183();
                    break;
                case "MNP-Binary":
                    gpsProtocol = new MNPBinary();
                    break;
            }

            // Sending track objects
            if (this.trackObjects.Count > 0)
            {
                for (int i = 0; i < this.trackObjects.Count; i++)
                {
                    if (this.distance_on_track - this.trackObjects[i].Distance < 0)
                    {
                        this.closestTrackObjectIndex = i;
                        break;
                    }
                }
                if (this.closestTrackObjectIndex > -1)
                {
                    double distanceToClosestTrackObject = (this.trackObjects[this.closestTrackObjectIndex].Distance - this.distance_on_track)/1000;
                    this.lblClosestTrackObject.Text = "Ближайший объект: " + distanceToClosestTrackObject.ToString("0.00000").Replace(',', '.') + " км";
                    this.lblClosestTrackObject.Enabled = true;
                }
            }


            // formGE
            if (GEIsReady)
            {
                formGE.ClearGpsTrack();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartRide();
            pnlGpsCom.Enabled = false;
            this.simulationState = SimulationState.Riding;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            tmrSpeed.Enabled = false;
            tmrSpeedPump.Enabled = false;
            if (comPortFound && comPort.IsOpen)
            {
                comPort.DiscardOutBuffer();
                comPort.Close();
            }
            pnlGpsCom.Enabled = true;
            this.simulationState = SimulationState.Off;
        }



        private void tmrSpeed_Tick(object sender, EventArgs e)
        {
            double d = speed_ms * (tmrSpeed.Interval / 1000);

            this.distance_on_track += speed_ms * (tmrSpeed.Interval / 1000);
                // distance_in_tick: m = (m/s) * ((ms/1000) = s)

            if (this.distance_on_track > this.trackLength) this.distance_on_track = this.trackLength;

            //if (this.distance_on_track < this.trackLength)
            //{
            chartRidedistance.Series[1].Points.AddXY(this.distance_on_track, 0);
            chartRidedistance.Series["SeriesRidingMarker"].Points.Clear();
            chartRidedistance.Series["SeriesRidingMarker"].Points.AddXY(this.distance_on_track, 0);
            this.chartRidedistance.Series["SeriesRidingMarker"].Label =
                ((this.distance_on_track - this.chartRidedistance.Series["SeriesStartMarker"].Points[0].XValue) / 1000)
                    .ToString("0.00");


            GeoCoordinate gc = databaseParser.GetTrackCoordinate(this.distance_on_track);

            var canSet = new CanFrame[]
                         {
                             new MmAltLongFrame()
                             {
                                 Latitude = gc.Latitude,
                                 Longitude = gc.Longitude,
                                 Reliable = cbGpsIsValid.Checked
                             }
                         };

            foreach (var canFrame in canSet)
            {
                Console.WriteLine(canFrame);
            }

            Port.Send(canSet);

            if (GEIsReady)
            {
                formGE.AddPointToGpsTrack(gc);
                formGE.ShiftLookAt(gc.Latitude, gc.Longitude, -1);
            }

            gpsProtocol.CurrentSpeed = speed_ms;
            gpsProtocol.IsValid = cbGpsIsValid.Checked ? true : false;
            GPSDatum gd = new GPSDatum(gc);
            byte[] buffer = gpsProtocol.GetPacket(gd);
            if (comPortFound)
            {
                comPort.Write(buffer, 0, buffer.Length);
                comPort.BaseStream.Flush();
            }

            rtbDisplay.AppendText(gpsProtocol.GetPacketString() + "\r\n");
            rtbDisplay.ScrollToCaret();



            // Sending track objects
            if (this.trackObjects.Count > 0 && this.closestTrackObjectIndex > -1)
            {
                if (this.distance_on_track - this.trackObjects[this.closestTrackObjectIndex].Distance > 0)
                {
                    if (cbEnableTOMessages.Checked)
                    {
                        int sent =
                            udp_sending_socket.SendTo(
                                                      this.trackObjects[this.closestTrackObjectIndex]
                                                          .TReceptionDataExBytes, 32, SocketFlags.None,
                                                      this.remote_sendto_endpoint);
                    }
                    this.closestTrackObjectIndex++;
                }
                else
                {
                    double distanceToClosestTrackObject = (this.trackObjects[this.closestTrackObjectIndex].Distance
                                                           - this.distance_on_track) / 1000;
                    this.lblClosestTrackObject.Text = "Ближайший объект: "
                                                      + distanceToClosestTrackObject.ToString("0.00000")
                                                                                    .Replace(',', '.') + " км";
                }
            }


            /*}
            else
            {
                chartRidedistance.Series[1].Points.AddXY(this.trackLength, 0);
                chartRidedistance.Series["SeriesRidingMarker"].Points.Clear();
                chartRidedistance.Series["SeriesRidingMarker"].Points.AddXY(this.distance_on_track, 0);
                this.btnStop_Click(this, new EventArgs());
            }*/

            if (this.distance_on_track >= this.trackLength)
            {
                this.btnStop_Click(this, new EventArgs());
            }
        }



        private void SetStartingDistance(double xValue)
        {
            if (xValue < 0) xValue = 0;
            if (xValue > trackLength) xValue = trackLength;
            this.chartRidedistance.Series["SeriesStartMarker"].Points.Clear();
            this.chartRidedistance.Series["SeriesStartMarker"].Points.AddXY(xValue, 0);
            this.chartRidedistance.Series["SeriesStartMarker"].Label = (xValue / 1000).ToString("0.00");
        }

        private double GetStartingDistance()
        {
            return this.chartRidedistance.Series["SeriesStartMarker"].Points[0].XValue;
        }

        


        private void btnScanPorts_Click(object sender, EventArgs e)
        {
            cbOutputPorts.Items.Clear();
            cbOutputPorts.Items.AddRange(SerialPort.GetPortNames().OrderBy(s => s).ToArray());
            cbOutputPorts.SelectedIndex = 0;
        }

        private void btnSetSpeed_Click(object sender, EventArgs e)
        {
            this.speed_ms = GetCurrentSpeed();
        }

        private void cbListenSpeed_CheckedChanged(object sender, EventArgs e)
        {
            if (cbListenSpeed.Checked)
            {
                tbSpeed.Enabled = false;
                btnSetSpeed.Enabled = false;
                StartListenSpeed();
            }
            else
            {
                StopListenSpeed();
                tbSpeed.Enabled = true;
                btnSetSpeed.Enabled = true;
            }
        }




        EndPoint remote_sendto_endpoint = (EndPoint)(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2001));
        //EndPoint local_sendto_endpoint = (EndPoint)(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
        Socket udp_sending_socket = null;

        public void InitializeNetworkInterop()
        {
            try
            {
                udp_sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //udp_sending_socket.Bind(local_sendto_endpoint);
            }
            catch
            {
                MessageBox.Show("Ошибка привязки отправляющего сокета", String.Empty, MessageBoxButtons.OK);
            }
        }

        byte[] data = new byte[256];
        EndPoint remote_listen_endpoint = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
        Socket udp_listen_socket = null;

        public void StartListenSpeed()
        {
            try
            {
                // Check if it is bind
                udp_listen_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //udp_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName., new LingerOption(false, 1));
                udp_listen_socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2000));
                Array.Clear(this.data, 0, this.data.Length);
                udp_listen_socket.BeginReceiveFrom(this.data, 0, data.Length, SocketFlags.None, ref this.remote_listen_endpoint, new AsyncCallback(ReceiveData), (object)this.udp_listen_socket);
            }
            catch
            {
                MessageBox.Show("Ошибка привязки принимающего сокета", String.Empty, MessageBoxButtons.OK);
            }
        }

        public void ReceiveData(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            try
            {
                int recv = remote.EndReceiveFrom(iar, ref this.remote_listen_endpoint);

                int speed = (((int)data[1]) << 8) + (int)data[0];
                this.speed_ms = Convert.ToDouble(speed) * 1000 / 3600;
                this.tbSpeed.BeginInvoke(new MethodInvoker(delegate { this.tbSpeed.Text = speed.ToString(); }));

                Array.Clear(this.data, 0, this.data.Length);
                udp_listen_socket.BeginReceiveFrom(this.data, 0, data.Length, SocketFlags.None, ref this.remote_listen_endpoint, new AsyncCallback(ReceiveData), (object)this.udp_listen_socket);
            }
            catch { }
        }

        public void StopListenSpeed()
        {
            udp_listen_socket.Dispose();
        }



        private void btnTOFile_Click(object sender, EventArgs e)
        {
            if (openDatabaseDialog.ShowDialog() == DialogResult.OK)
            {
                this.trackobjectsFile = openDatabaseDialog.FileName;
                this.lblTOFile.Text = openDatabaseDialog.FileName;

                string line = null;
                this.trackObjects.Clear();
                StreamReader toFile = new StreamReader(this.trackobjectsFile);
                while ((line = toFile.ReadLine()) != null)
                {
                    if (String.IsNullOrEmpty(line)) continue;
                    string[] sublines = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    this.trackObjects.Add(new TrackObjectRecord(new GeoCoordinate(), Double.Parse(sublines[0].Replace(',', '.'),  CultureInfo.InvariantCulture), Convert.FromBase64String(sublines[1])));
                }
                toFile.Close();

                cbEnableTOMessages.Checked = true;
                cbEnableTOMessages.Enabled = true;
            }
        }


        List<TargetDistance> mcoTargets = new List<TargetDistance>();
        List<TargetDistance> sautTargets = new List<TargetDistance>();

        public void ShowTargetDistances()
        {
            // Series settings
            this.chartRidedistance.Series.Add(new Series("mcoTargets"));
            this.chartRidedistance.Series["mcoTargets"].ChartType = SeriesChartType.FastLine;
            this.chartRidedistance.Series["mcoTargets"].Color = Color.Green;

            this.chartRidedistance.Series.Add(new Series("sautTargets"));
            this.chartRidedistance.Series["sautTargets"].ChartType = SeriesChartType.FastLine;
            this.chartRidedistance.Series["sautTargets"].Color = Color.Black;

            // Series loading
            chartRidedistance.Series["mcoTargets"].Points.Clear();
            chartRidedistance.Series["sautTargets"].Points.Clear();
            foreach (var v in mcoTargets)
            {
                chartRidedistance.Series["mcoTargets"].Points.AddXY(v.trackDistance, v.targetDistance);
            }
            foreach (var v in sautTargets)
            {
                chartRidedistance.Series["sautTargets"].Points.AddXY(v.trackDistance, v.targetDistance);
            }

            // Series legend
            // Chart1.Series["Series1"].IsVisibleInLegend = true;
            chartRidedistance.Legends.Add(new Legend("hack"));
            chartRidedistance.Legends["hack"].Enabled = false;

            /*chartRidedistance.Legends.Add(new Legend("DistancesToTargets"));
            chartRidedistance.Series["mcoTargets"].Legend = "DistancesToTargets";
            chartRidedistance.Series["sautTargets"].Legend = "DistancesToTargets";

            chartRidedistance.Legends["DistancesToTargets"].Title = "Расстояния до целей";
            chartRidedistance.Legends["DistancesToTargets"].TitleFont = new Font("Arial", 10, FontStyle.Regular);
            chartRidedistance.Legends["DistancesToTargets"].BorderColor = Color.LightGray;
            chartRidedistance.Legends["DistancesToTargets"].InsideChartArea = "ChartAreaDefault";*/


            chartRidedistance.Legends.Add(new Legend("DistancesToTargets"));

            chartRidedistance.Legends["DistancesToTargets"].Title = "Расстояния до целей";
            chartRidedistance.Legends["DistancesToTargets"].TitleFont = new Font("Arial", 10, FontStyle.Regular);
            chartRidedistance.Legends["DistancesToTargets"].BorderColor = Color.LightGray;
            chartRidedistance.Legends["DistancesToTargets"].InsideChartArea = "ChartAreaDefault";

            chartRidedistance.Legends["DistancesToTargets"].CustomItems.Clear();
            chartRidedistance.Legends["DistancesToTargets"].CustomItems.Add(new LegendItem("mco_state_a", Color.Green, ""));
            chartRidedistance.Legends["DistancesToTargets"].CustomItems[0].Cells.Add(new LegendCell(LegendCellType.SeriesSymbol, "", ContentAlignment.MiddleLeft));
            chartRidedistance.Legends["DistancesToTargets"].CustomItems[0].Cells.Add(new LegendCell(LegendCellType.Text, "mco_state_a", ContentAlignment.MiddleLeft));
            chartRidedistance.Legends["DistancesToTargets"].CustomItems.Add(new LegendItem("saut_state_a", Color.Black, ""));
            chartRidedistance.Legends["DistancesToTargets"].CustomItems[1].Cells.Add(new LegendCell(LegendCellType.SeriesSymbol, "", ContentAlignment.MiddleLeft));
            chartRidedistance.Legends["DistancesToTargets"].CustomItems[1].Cells.Add(new LegendCell(LegendCellType.Text, "saut_state_a", ContentAlignment.MiddleLeft));

            //chartRidedistance.Legends["Default"].CustomItems[0].Cells.Add(new LegendCell(LegendCellType.Text, "MR", ContentAlignment.TopCenter));


            /**/
            

            // Zooming and scrolling
            this.chartRidedistance.ChartAreas[0].CursorX.IsUserEnabled = true;
            this.chartRidedistance.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;

            this.chartRidedistance.ChartAreas[0].CursorY.IsUserEnabled = true;
            this.chartRidedistance.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;

            this.chartRidedistance.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            this.chartRidedistance.ChartAreas[0].AxisY.ScaleView.Zoomable = true;

            this.chartRidedistance.ChartAreas[0].CursorX.AutoScroll = true;
            this.chartRidedistance.ChartAreas[0].CursorY.AutoScroll = true;

            this.chartRidedistance.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            this.chartRidedistance.ChartAreas[0].AxisY.ScrollBar.Enabled = true;

            this.chartRidedistance.ChartAreas["ChartAreaDefault"].AxisX.ScrollBar.IsPositionedInside = false;
            this.chartRidedistance.ChartAreas["ChartAreaDefault"].AxisY.ScrollBar.IsPositionedInside = false;

            int mcoMaximum = mcoTargets.Max(m => m.targetDistance);
            int sautMaximum = sautTargets.Max(m => m.targetDistance);
            int maximum = mcoMaximum > sautMaximum ? mcoMaximum : sautMaximum;
            this.chartRidedistance.ChartAreas[0].AxisY.Minimum = 0;
            this.chartRidedistance.ChartAreas[0].AxisY.Maximum = maximum;
            this.chartRidedistance.ChartAreas[0].AxisY.Crossing = 0;


            // Ticks
            this.chartRidedistance.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            this.chartRidedistance.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            this.chartRidedistance.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            this.chartRidedistance.ChartAreas[0].AxisX.MajorTickMark.Enabled = true;
            this.chartRidedistance.ChartAreas[0].AxisX.MinorTickMark.Enabled = true;

            this.chartRidedistance.ChartAreas[0].AxisY.MajorTickMark.Enabled = true;
            this.chartRidedistance.ChartAreas[0].AxisY.LabelStyle.Enabled = true;
            
        }

        private void btnTDFile_Click(object sender, EventArgs e)
        {
            if (openDatabaseDialog.ShowDialog() == DialogResult.OK)
            {
                //HideTargetDistances();
                
                this.targetdistancesFile = openDatabaseDialog.FileName;
                this.lblTDFile.Text = openDatabaseDialog.FileName;
                TargetDistance.LoadFromFile(openDatabaseDialog.FileName, ref mcoTargets, ref sautTargets);
                ShowTargetDistances();
            }
        }



        // Ruler functionality on/off
        // All other — in mouse click / mouse move events (mouse hover)
        private bool RulerEnabled = false;
        private int RulerHighlightingPointIndex = -1;
        private int RulerDraggingPointIndex = -1;
        private double Distance(DataPoint p1, DataPoint p2)
        {
            double dx = p1.XValue - p2.XValue;
            double dy = p1.YValues[0] - p2.YValues[0];
            double l = Math.Sqrt(dx * dx + dy * dy);
            return l;
        }
        private int FindProximal(int x, int y, double pixelsTolerance)
        {
            // V E R Y stupid "algorithm"
            DataPoint p = new DataPoint((double)x, (double)y);
            var points = chartRidedistance.Series["SeriesRuler"].Points;
            var axisX = chartRidedistance.ChartAreas[0].AxisX;
            var axisY = chartRidedistance.ChartAreas[0].AxisY;
            List<int> pointIndexes = new List<int>();
            for (int i = 0; i < points.Count; i++)
            {
                DataPoint pi = new DataPoint(axisX.ValueToPixelPosition(points[i].XValue), axisY.ValueToPixelPosition(points[i].YValues[0]));
                if (Distance(p, pi) < pixelsTolerance)
                    pointIndexes.Add(i);
            }

            int minIndex = -1;
            if (pointIndexes.Count > 0)
            {
                DataPoint pi = new DataPoint(axisX.ValueToPixelPosition(points[0].XValue), axisY.ValueToPixelPosition(points[0].YValues[0]));
                double minDistance = Distance(p, pi);
                minIndex = pointIndexes[0];
                for (int i = 1; i < pointIndexes.Count; i++)
                {
                    pi = new DataPoint(axisX.ValueToPixelPosition(points[pointIndexes[i]].XValue), axisY.ValueToPixelPosition(points[pointIndexes[i]].YValues[0]));
                    if (Distance(p, pi) < minDistance)
                    {
                        minDistance = Distance(p, pi);
                        minIndex = pointIndexes[i];
                    }
                }
            }
            return minIndex;
        }
        private int FindProximal2(int x, int y, double pixelsTolerance)
        {
            // V E R Y stupid "algorithm"
            DataPoint p = new DataPoint((double)x, (double)y);
            var points = chartRidedistance.Series["SeriesRuler"].Points;
            var axisX = chartRidedistance.ChartAreas[0].AxisX;
            var axisY = chartRidedistance.ChartAreas[0].AxisY;
            List<int> pointIndexes = new List<int>();
            for (int i = 0; i < points.Count; i++)
            {
                DataPoint pi = new DataPoint(axisX.ValueToPixelPosition(points[i].XValue), axisY.ValueToPixelPosition(points[i].YValues[0]));
                if (Distance(p, pi) < pixelsTolerance)
                    pointIndexes.Add(i);
            }

            int minIndex = -1;
            if (pointIndexes.Count > 0)
            {
                DataPoint pi = new DataPoint(axisX.ValueToPixelPosition(points[0].XValue), axisY.ValueToPixelPosition(points[0].YValues[0]));
                double minDistance = Distance(p, pi);
                minIndex = pointIndexes[0];
                for (int i = 1; i < pointIndexes.Count; i++)
                {
                    pi = new DataPoint(axisX.ValueToPixelPosition(points[pointIndexes[i]].XValue), axisY.ValueToPixelPosition(points[pointIndexes[i]].YValues[0]));
                    if (Distance(p, pi) < minDistance)
                    {
                        minDistance = Distance(p, pi);
                        minIndex = pointIndexes[i];
                    }
                }
            }
            return minIndex;
        }

        private void cbRuler_CheckedChanged(object sender, EventArgs e)
        {
            if (!RulerEnabled)
            {
                if (chartRidedistance.Series.Any(s => s.Name == "SeriesRuler"))
                {
                    chartRidedistance.Series["SeriesRuler"].Enabled = true;
                }
                else
                {
                    Series SeriesRuler = new Series("SeriesRuler");
                    chartRidedistance.Series.Add(SeriesRuler);
                    chartRidedistance.Series["SeriesRuler"].ChartType = SeriesChartType.Line;
                    chartRidedistance.Series["SeriesRuler"].Color = Color.Red;
                    chartRidedistance.Series["SeriesRuler"].MarkerStyle = MarkerStyle.Square;
                    chartRidedistance.Series["SeriesRuler"].MarkerBorderWidth = 4;
                    chartRidedistance.Series["SeriesRuler"].MarkerSize = 1;
                }
                btnClearRuler.Enabled = true;
            }
            else
            {
                chartRidedistance.Series["SeriesRuler"].Enabled = false;
                btnClearRuler.Enabled = false;
            }
            
            RulerEnabled = !RulerEnabled;
        }
        
        private void chartRidedistance_MouseDown(object sender, MouseEventArgs e)
        {
            // Begin drag
            if (RulerEnabled && Control.ModifierKeys == Keys.Shift)
            {
                int proximalPointIndex = FindProximal(e.X, e.Y, 10);
                
                /*HitTestResult result = chartRidedistance.HitTest(e.X, e.Y);
                if (result.ChartElementType == ChartElementType.DataPoint && result.Series.Name == "SeriesRuler")*/
                if (proximalPointIndex > -1)
                {
                    // Disable zoom with cursor
                    this.chartRidedistance.ChartAreas[0].CursorX.IsUserEnabled = false;
                    this.chartRidedistance.ChartAreas[0].CursorX.IsUserSelectionEnabled = false;
                    this.chartRidedistance.ChartAreas[0].CursorY.IsUserEnabled = false;
                    this.chartRidedistance.ChartAreas[0].CursorY.IsUserSelectionEnabled = false;
                    this.chartRidedistance.ChartAreas[0].AxisX.ScaleView.Zoomable = false;
                    this.chartRidedistance.ChartAreas[0].AxisY.ScaleView.Zoomable = false;

                    RulerDraggingPointIndex = proximalPointIndex;
                }
            }
        }

        private void chartRidedistance_MouseUp(object sender, MouseEventArgs e)
        {
            if (RulerEnabled)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (Control.ModifierKeys == Keys.Control && RulerDraggingPointIndex == -1)  // Adding point
                    {
                        /*double xValue = this.chartRidedistance.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                        double yValue = this.chartRidedistance.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);*/
                        chartRidedistance.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);
                        chartRidedistance.ChartAreas[0].CursorY.SetCursorPixelPosition(new Point(e.X, e.Y), true);
                        double xValue = chartRidedistance.ChartAreas[0].CursorX.Position;
                        double yValue = chartRidedistance.ChartAreas[0].CursorY.Position;

                        // TODO: Не за пределами текущего View
                        double x_min = chartRidedistance.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                        double x_max = chartRidedistance.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
                        double y_min = chartRidedistance.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                        double y_max = chartRidedistance.ChartAreas[0].AxisY.ScaleView.ViewMaximum;
                        //if (xValue >= 0 && yValue >= 0)
                        if (x_min <= xValue && xValue <= x_max && y_min <= yValue && yValue <= y_max)
                        {
                            chartRidedistance.Series["SeriesRuler"].Points.AddXY(xValue, yValue);
                        }
                    }
                    if (RulerDraggingPointIndex > -1)  // Releasing drag
                    {
                        // Enable zoom with cursor
                        this.chartRidedistance.ChartAreas[0].CursorX.IsUserEnabled = true;
                        this.chartRidedistance.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
                        this.chartRidedistance.ChartAreas[0].CursorY.IsUserEnabled = true;
                        this.chartRidedistance.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
                        this.chartRidedistance.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
                        this.chartRidedistance.ChartAreas[0].AxisY.ScaleView.Zoomable = true;

                        // Dragging end
                        RulerDraggingPointIndex = -1;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    /*HitTestResult result = chartRidedistance.HitTest(e.X, e.Y);
                    if (result.ChartElementType != ChartElementType.DataPoint) return;
                    if (result.Series.Name != "SeriesRuler") return;*/
                    int proximalPointIndex = FindProximal(e.X, e.Y, 10);
                    if (proximalPointIndex > -1)
                    {
                        chartRidedistance.Series["SeriesRuler"].Points.RemoveAt(proximalPointIndex);
                    }
                }

                // Рассчитваем расстояние
                double rulerDistance = 0;
                var rulerPoints = chartRidedistance.Series["SeriesRuler"].Points;
                if (rulerPoints.Count >= 2)
                {
                    for (int i = 0; i < rulerPoints.Count - 1; i++)
                    {
                        rulerDistance += Distance(rulerPoints[i], rulerPoints[i+1]);
                    }
                    this.lblRuler.Text = rulerDistance.ToString("0.0");
                }
                else
                {
                    this.lblRuler.Text = "—";
                }
            }
            
            
            
            
            // SIMULATOR PART
            if (this.simulationState == SimulationState.Off && e.Button == MouseButtons.Right)
            {
                chartRidedistance.Series["SeriesRidingMarker"].Points.Clear();
                chartRidedistance.Series["SeriesRide"].Points.Clear();
                double xValue = this.chartRidedistance.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                SetStartingDistance(xValue);
            }
        }

        // http://stackoverflow.com/questions/5425122/showing-mouse-axis-coordinates-on-chart-control
        private void chartRidedistance_MouseMove(object sender, MouseEventArgs e)
        {
            // Crashes if goes out of Chart Control area
            // Restrict to chart area
            /*var ca = this.chartRidedistance.ChartAreas[0];
            float chartTopLeft = ca.Position.X;
            
            double xValue = this.chartRidedistance.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
            double yValue = this.chartRidedistance.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);

            this.Text = e.X.ToString() + "  " + e.Y.ToString();

            //this.Text = xValue.ToString() + "  " + yValue.ToString();*/

            chartRidedistance.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);
            chartRidedistance.ChartAreas[0].CursorY.SetCursorPixelPosition(new Point(e.X, e.Y), true);
            double xValue = chartRidedistance.ChartAreas[0].CursorX.Position;
            double yValue = chartRidedistance.ChartAreas[0].CursorY.Position;


            // Showing cursor position
           /* CustomLabel lbl = this.chartRidedistance.ChartAreas[0].AxisX.CustomLabels[0];
            if (lbl != null && lbl.Name == "CL")
            {
                //this.chartRidedistance.ChartAreas[0].AxisX.CustomLabels.SuspendUpdates();
                this.chartRidedistance.ChartAreas[0].AxisX.CustomLabels.RemoveAt(0);
                //this.chartRidedistance.ChartAreas[0].AxisX.CustomLabels.ResumeUpdates();
                this.chartRidedistance.ChartAreas[0].AxisX.CustomLabels.Invalidate();


            }*/
            /*CustomLabel lbl = new CustomLabel();
            lbl.Name = "CL";
            lbl.Text = (xValue / 1000).ToString("0.00");
            lbl.FromPosition = xValue - 100000;
            lbl.ToPosition = xValue + 100000;
            lbl.RowIndex = 0;
            lbl.GridTicks = GridTickTypes.TickMark;
            lbl.ForeColor = Color.FromArgb(255, Color.Red);
            this.chartRidedistance.ChartAreas[0].AxisX.CustomLabels[0] = lbl;*/

            //this.chartRidedistance.ChartAreas[0].CursorX.

            /*RectangleAnnotation annotation = (RectangleAnnotation)this.chartRidedistance.Annotations[0];
            annotation.AnchorX = 100*xValue/this.trackLength;
            annotation.AnchorY = -5 + 100*yValue/10;
            //annotation.AnchorDataPoint = new DataPoint(xValue, yValue);
            annotation.Text = "I am a\nRectangleAnnotation";
            this.chartRidedistance.Annotations.Invalidate();*/
            RectangleAnnotation annotation = (RectangleAnnotation)this.chartRidedistance.Annotations[0];
            annotation.Text = (xValue / 1000).ToString("0.00"); ;
            


            // Dragging point
            if (RulerDraggingPointIndex > -1)
            {
                chartRidedistance.Series["SeriesRuler"].Points[RulerDraggingPointIndex].XValue = xValue;
                chartRidedistance.Series["SeriesRuler"].Points[RulerDraggingPointIndex].YValues[0] = yValue;
            }

            // Cursor movement
            /*this.chartRidedistance.ChartAreas[0].CursorX.Position = xValue;
            this.chartRidedistance.ChartAreas[0].CursorY.Position = yValue;*/
            /*System.Windows.Forms.DataVisualization.Charting.Cursor cursorX = this.chartRidedistance.ChartAreas[0].CursorX.Position;
            System.Windows.Forms.DataVisualization.Charting.Cursor cursorY = this.chartRidedistance.ChartAreas[0].CursorY;
            cursorX.Position = this.chartRidedistance.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
            cursorY.Position = this.chartRidedistance.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);

            /*chartRidedistance.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);
            chartRidedistance.ChartAreas[0].CursorY.SetCursorPixelPosition(new Point(e.X, e.Y), true);

            double pX = chartRidedistance.ChartAreas[0].CursorX.Position;
            double pY = chartRidedistance.ChartAreas[0].CursorY.Position;*/




            // Markers highlightning
            if (RulerEnabled)
            {
                int proximalPointIndex = FindProximal(e.X, e.Y, 10);
                if (proximalPointIndex > -1)
                {
                    chartRidedistance.Series["SeriesRuler"].Points[proximalPointIndex].BorderColor = Color.Blue;
                    RulerHighlightingPointIndex = proximalPointIndex;
                }
                else
                {
                    if (RulerHighlightingPointIndex > -1)
                    {
                        if (RulerHighlightingPointIndex < chartRidedistance.Series["SeriesRuler"].Points.Count)
                        {
                            chartRidedistance.Series["SeriesRuler"].Points[RulerHighlightingPointIndex].BorderColor = Color.Red;
                        }
                        RulerHighlightingPointIndex = -1;
                    }
                }
            }
        }

        private void btnClearRuler_Click(object sender, EventArgs e)
        {
            chartRidedistance.Series["SeriesRuler"].Points.Clear();
            lblRuler.Text = "—";
        }

        private void cbEnableTOMessages_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnableTOMessages.Checked)
            {
                lblClosestTrackObject.ForeColor = SystemColors.ControlText;
            }
            else
            {
                lblClosestTrackObject.ForeColor = Color.Gray;
            }
        }



        public void SetXAxisLabels()
        {
            int count_on_view = 5;

            double xaxis_v_min = this.chartRidedistance.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
            double xaxis_v_max = this.chartRidedistance.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
            
            

            this.chartRidedistance.ChartAreas[0].AxisX.IsLabelAutoFit = false;
            this.chartRidedistance.ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont;


            // X-axis ticks and labels
            //this.chartRidedistance.ChartAreas[0].AxisX.MajorTickMark.Enabled = true;
            //this.chartRidedistance.ChartAreas[0].AxisX.MajorTickMark.Interval = this.trackLength / 10;

            /*this.chartRidedistance.ChartAreas[0].AxisX.Interval = 0;
            this.chartRidedistance.ChartAreas[0].AxisX.IsLabelAutoFit = true;
            this.chartRidedistance.ChartAreas[0].AxisX.LabelAutoFitMaxFontSize = 12;
            this.chartRidedistance.ChartAreas[0].AxisX.LabelAutoFitMinFontSize = 5;
            this.chartRidedistance.ChartAreas[0].AxisX.LabelAutoFitStyle = LabelAutoFitStyles.WordWrap;*/
            //this.chartRidedistance.ChartAreas[0].AxisX.
            this.chartRidedistance.ChartAreas[0].AxisX.LabelStyle.Enabled = true;
            //this.chartRidedistance.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Verdana", 4, FontStyle.Regular);
            //this.chartRidedistance.ChartAreas[0].AxisX.LabelStyle.Interval = this.trackLength / 10;

            this.chartRidedistance.ChartAreas[0].AxisX.CustomLabels.Clear();

            double pos = xaxis_v_min;
            double pos_step = (xaxis_v_max - xaxis_v_min) / count_on_view;
            for (int i = 0; i <= count_on_view + 1; i++)
            {
                CustomLabel lbl = new CustomLabel();
                lbl.Text = (pos / 1000).ToString("0.00");
                lbl.FromPosition = pos - pos_step / 3;
                lbl.ToPosition = pos + pos_step / 3;
                lbl.RowIndex = 0;
                lbl.GridTicks = GridTickTypes.TickMark;
                this.chartRidedistance.ChartAreas[0].AxisX.CustomLabels.Add(lbl);

                pos = xaxis_v_min + i * pos_step;
            }

            int x = 5;
        }

        private void chartRidedistance_AxisViewChanged(object sender, ViewEventArgs e)
        {
            if (e.Axis.AxisName == AxisName.X)
            {
                SetXAxisLabels();
                // Чтобы сбросить выделение
                this.chartRidedistance.ChartAreas[0].CursorY.SetSelectionPosition(0, 0);
            }
            int x = 5;
        }



        //**************************//
        //   Google Earth section   //
        //                          //
        //**************************//
        private FormGE formGE = null;

        private bool GEIsReady = false;

        private void btnGE_Click(object sender, EventArgs e)
        {
            if (formGE == null)
            {
                formGE = new FormGE();
                formGE.OnPluginReady += new EventHandler(formGEReady);
                formGE.OnPluginStop += new EventHandler(formGEStop);
                formGE.Show();
            }
            else
            {
                formGE.WindowState = FormWindowState.Normal;
            }
        }

        private void formGEReady(object sender, EventArgs e)
        {
            GEIsReady = true;

            if (this.databaseParser != null)
            {
                formGE.SetFullPath(this.databaseParser.track);
                //formGE.LookAt(this.databaseParser.track[0].Latitude, this.databaseParser.track[0].Longitude, 1000);
                GeoCoordinate gc = databaseParser.GetTrackCoordinate(this.distance_on_track);
                formGE.ShiftLookAt(gc.Latitude, gc.Longitude, 1000, true);
            }
        }

        private void formGEStop(object sender, EventArgs e)
        {
            GEIsReady = false;
            formGE = null;
        }




        private void button1_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter("D:\\Projects\\222.txt");
            double d = 0;
            double s = this.trackLength / 100;
            while (d < this.trackLength)
            {
                GeoCoordinate gc = databaseParser.GetTrackCoordinate(d);
                sw.WriteLine(gc.Longitude.ToString().Replace(',', '.') + " " + gc.Latitude.ToString().Replace(',', '.'));
                d += s;
            }
            sw.Close();
        }

        private bool ReverseRotator { get; set; }

        private void SpeedPump_Tick(object sender, EventArgs e)
        {
            var canSet = new CanFrame[]
                         {
                             new IpdEmulation()
                             {
                                 Sensor1State = new IpdEmulation.SensorState()
                                                {
                                                    Direction = (speed_ms > 0 ^ ReverseRotator) ? IpdEmulation.RorationDirection.Clockwise : IpdEmulation.RorationDirection.Counterclockwise,
                                                    Frequncy = (int)(Math.Abs(speed_ms) * 42 / (1.050 * Math.PI)),
                                                    Channel1Condition = IpdEmulation.ChannelCondition.Good,
                                                    Channel2Condition = IpdEmulation.ChannelCondition.Good
                                                },
                                 Sensor2State = new IpdEmulation.SensorState()
                                                {
                                                    Direction = IpdEmulation.RorationDirection.Clockwise,
                                                    Frequncy = 0,
                                                    Channel1Condition = IpdEmulation.ChannelCondition.Bad,
                                                    Channel2Condition = IpdEmulation.ChannelCondition.Bad
                                                }
                             }
                         };

            foreach (var canFrame in canSet)
            {
                Console.WriteLine(canFrame);
            }

            Port.Send(canSet);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ReverseRotator = ((CheckBox)sender).Checked;
        }
    }

}
