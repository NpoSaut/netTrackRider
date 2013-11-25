using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using GEPlugin;
using FC.GEPlugin;
using FC.GEPluginCtrls;
using FC.GEPluginCtrls.HttpServer;

namespace Simulator.GoogleEarth
{
    public partial class FormGE : Form
    {
        //**************************//
        //      Initialization      //
        //                          //
        //**************************//
        
        private IGEPlugin ge = null;

        private bool isReady = false;

        public bool IsReady { get { return isReady; } }

        public event EventHandler OnPluginReady = null;
        public event EventHandler OnPluginStop = null;

        private GEServer server = null;


        public FormGE()
        {
            InitializeComponent();
            InitialazeGeServer();
            InitializeGoogleEarth();
        }

        private void InitialazeGeServer()
        {
            server = new GEServer(System.Net.IPAddress.Loopback, 50500, "D:\\Projects\\Simulator\\Simulator\\gewebroot");
            server.Start();
        }

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        private void InitializeGoogleEarth()
        {
            // Interface handling
            lblGEStatus.Text = "Идёт загрузка Google Earth...";

            
            // Load the html directly from the library.
            geWebBrowser.LoadEmbeddedPlugin();

            // This event is raised by the initCallBack javascript function in the holding page
            geWebBrowser.PluginReady += new EventHandler<GEEventArgs>(geWebBrowser_PluginReady);

            // This event is raised if there is a javascript error (it can also be raised manually)
            geWebBrowser.ScriptError += new EventHandler<GEEventArgs>(geWebBrowser_ScriptError);

            // This event is raised if there is an active kml event
            geWebBrowser.KmlEvent += new EventHandler<GEEventArgs>(geWebBrowser_KmlEvent);
        }
        
        /// <summary>
        /// Handles the script error event
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The error message</param>
        void geWebBrowser_ScriptError(object sender, GEEventArgs e)
        {
            // Interface handling
            if (e.Message.Contains("Plugin failed to load"))
            {
                lblGEStatus.Text = "Ошибка при загрузке Google Earth";
            }

            this.isReady = false;
            if (this.OnPluginStop != null)
            {
                this.OnPluginStop(this, new EventArgs());
            }
            MessageBox.Show(sender.ToString() + Environment.NewLine + e.Message, "Error " + e.Data);
        }

        /// <summary>
        /// Handles the plugin ready event
        /// </summary>
        /// <param name="sender">The plugin object</param>
        /// <param name="e">The API version</param>
        void geWebBrowser_PluginReady(object sender, GEEventArgs e)
        {
            /// Here we can cast the sender to the IGEPlugin interface
            /// Once this is done once can work with the plugin almost seemlessly
            ////MessageBox.Show(GEHelpers.GetTypeFromRcw(sender));
            ge = e.ApiObject as IGEPlugin;

            if (ge != null)
            {
                // Interface handling
                pnlFunctional.Enabled = true;
                lblGEStatus.Text = "Google Earth успешно загружен";
                this.isReady = true;
                if (this.OnPluginReady != null)
                {
                    this.OnPluginReady(this, new EventArgs());
                }

                //MessageBox.Show(e.Data);

                
                // Visual appearance of GEPlugin
                ge.setTermsOfUsePosition_(5000, 5000);
                ge.getNavigationControl().setVisibility(ge.VISIBILITY_SHOW);
                ge.getLayerRoot().enableLayerById(ge.LAYER_BORDERS, 1);
                ge.getOptions().setStatusBarVisibility(1);
                ge.getOptions().setUnitsFeetMiles(0);
                ge.getOptions().setScaleLegendVisibility(1);
                ge.getOptions().setFlyToSpeed(ge.SPEED_TELEPORT);



                // Events of GEPlugin
                geWebBrowser.KmlEvent += new EventHandler<GEEventArgs>(geWebBrowser_StartPlacemarkEvent);
                geWebBrowser.AddEventListener(ge.getWindow(), EventId.MouseDown);
                geWebBrowser.AddEventListener(ge.getWindow(), EventId.MouseUp);
                geWebBrowser.AddEventListener(ge.getWindow(), EventId.MouseMove);
               
                // Create some event listners using the AddEventListener wrapper
                //geWebBrowser.AddEventListener(ge.getWindow(), EventId.MouseDown);
                //geWebBrowser.AddEventListener(ge.getWindow(), EventId.MouseUp);
                //geWebBrowser.AddEventListener(ge.getWindow(), EventId.MouseMove);/**/
            }
        }

        // Common events
        void geWebBrowser_KmlEvent(object sender, GEEventArgs e)
        {
            // if it is a mouse event
            /*if (null != sender as IKmlMouseEvent)
            {
                handleKmlMouseEvents((IKmlMouseEvent)sender, e.Message);
            }
            else
            {
                MessageBox.Show(GEHelpers.GetTypeFromRcw(sender).ToString());
            }*/

            int x = 5;
        }

        private void FormGE_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.geWebBrowser.KillPlugin();
            if (this.OnPluginStop != null)
            {
                this.OnPluginStop(this, new EventArgs());
            }

            if (server != null)
            {
                server.Stop();
            }
        }


        //**************************//
        //          Medhods         //
        //                          //
        //**************************//
        #region Lists of what might be drawn

        private FullPathFeature fullPath = null;
        private RidingPathFeature ridingPath = null;
        //private List<GeoCoordinate> fullPath = null;
        //private List<> ridingPath = null;
        //private List<> trafficLights = null;
        //private List<> generators = null;
        // TODO: tls and gens — prepend or filter at runtime?

        #endregion


        #region Full Path

        public void SetFullPath(List<GeoCoordinate> fullPath, bool drawIfAllowed = true)
        {
            this.fullPath = new FullPathFeature(ge, fullPath);
            if (drawIfAllowed /*&& соответствующая галочка установлена */)
            {
                this.fullPath.DrawPath();
                SetStartPlacemark(this.fullPath.fullPath[0].Latitude, this.fullPath.fullPath[0].Longitude);
            }
        }


        Point mousedownPointFP;
        IKmlPlacemark StartPlacemark = null;

        private void SetStartPlacemark(double latitude, double longitude)
        {
            /*var doc = ge.createDocument("doc");
            ge.getFeatures().appendChild(doc);
            
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            //sb.AppendLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\">");
            //sb.AppendLine("<Document id = \"docm\"><name>Name</name>");
            sb.AppendLine("<Style><IconStyle id=\"iconstyle\"><scale>1.0</scale></IconStyle></Style>");
            //sb.AppendLine("<Placemark id=\"mountainpin1\"><name>New Zealand's Southern Alps</name><styleUrl>#style</styleUrl><Point><coordinates>170.144,-43.605,0</coordinates></Point></Placemark>");
            //sb.AppendLine("</Document></kml>");

            var k = ge.parseKml(sb.ToString());
 
            var r = ge.getElementById("docm");
            var st = ge.getElementById("iconstyle");

            string s = ((IKmlDocument)r).getKml();

            int x = 465;*/

            if (StartPlacemark == null)
            {
                var point = ge.createPoint("");
                point.setExtrude(0);
                point.setTessellate(1);
                point.setAltitudeMode(ge.ALTITUDE_CLAMP_TO_GROUND);
                point.setLatLngAlt(latitude, longitude, 0);

                var normalIcon = ge.createIcon("start_placemark_icon_n");
                normalIcon.setHref("http://maps.google.com/mapfiles/kml/pushpin/wht-pushpin.png");
                var normalStyle = ge.createStyle("start_placemark_s_n");
                normalStyle.getIconStyle().setIcon(normalIcon);
                normalStyle.getIconStyle().setScale(1.1f);
                normalStyle.getIconStyle().getHotSpot().set(20, ge.UNITS_PIXELS, 2, ge.UNITS_PIXELS);

                this.StartPlacemark = ge.createPlacemark("start_placemark");
                this.StartPlacemark.setGeometry(point);
                this.StartPlacemark.setStyleSelector(normalStyle);
                ge.getFeatures().appendChild(this.StartPlacemark);
            }
            else
            {
                ((IKmlPoint)this.StartPlacemark.getGeometry()).setLatLng(latitude, longitude);
            }
        }

        private void UnsetStartPlacemark(double latitude, double longitude)
        {

        }


        void geWebBrowser_StartPlacemarkEvent(object sender, GEEventArgs e)
        {
            if (fullPath == null || (e.ApiObject as IKmlMouseEvent) == null || IsRulerEnabled)
            {
                return;
            }

            IKmlMouseEvent me = e.ApiObject;

            if (me.getDidHitGlobe() == 0 /*|| me.getCtrlKey() == 0*/)
            {
                return;
            }
            
            if (e.EventId == EventId.MouseDown && me.getCtrlKey() == 1)
            {
                mousedownPointFP.X = me.getClientX();
                mousedownPointFP.Y = me.getClientY();
            }

            if (e.EventId == EventId.MouseUp)
            {
                if (mousedownPointFP.X == me.getClientX() && mousedownPointFP.Y == me.getClientY() && me.getCtrlKey() == 1)      // was no mouse move between down and up
                {
                    int i = GeoMath.FindNearest(me.getLatitude(), me.getLongitude(), fullPath.fullPath);
                    SetStartPlacemark(fullPath.fullPath[i].Latitude, fullPath.fullPath[i].Longitude);
                }                                                                                   

            }
        }

        #endregion


        #region Riding Path

        public void AddPointToGpsTrack(GeoCoordinate gc, bool drawIfAllowed = true)
        {
            if (this.ridingPath == null)
            {
                this.ridingPath = new RidingPathFeature(ge);
                this.ridingPath.geWebBrowser = this.geWebBrowser;
            }
            if (drawIfAllowed /*&& соответствующая галочка установлена */)
            {
                this.ridingPath.AddPoint(gc);
            }
        }

        public void ClearGpsTrack()
        {
            if (this.ridingPath != null)
            {
                this.ridingPath.ResetPath();
            }
        }


        #endregion

        //*********************************//
        //      Google Earth Helpers       //
        //                                 //
        //*********************************//
        #region Google Earth Helpers

        /// <summary>
        /// Look at the given coordinates
        /// </summary>
        /// <param name="ge">the plugin</param>
        /// <param name="latitude">latitude in decimal degrees</param>
        /// <param name="longitude">longitude in decimal degrees</param>
        public void LookAt(double latitude, double longitude, double range)
        {
            IKmlLookAt lookat = ge.createLookAt(String.Empty);
            lookat.set(
                latitude,
                longitude,
                0,
                ge.ALTITUDE_RELATIVE_TO_GROUND,
                0,
                0,
                range);
            ge.getView().setAbstractView(lookat);
        }

        public void ShiftLookAt(double latitude, double longitude, double range)
        {
            var lookAt = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
            lookAt.setLatitude(latitude);
            lookAt.setLongitude(longitude);
            if (range > 0)
            {
                lookAt.setRange(range);
            }
            ge.getView().setAbstractView(lookAt);
        }


        private bool uninterruptibleCamera = false;
        public void ShiftLookAt(double latitude, double longitude, double range, bool uninterruptible)
        {
            if (!uninterruptibleCamera)
            {
                uninterruptibleCamera = true;
                geWebBrowser.AddEventListener(ge, EventId.FrameEnd);
                geWebBrowser.PluginEvent += new EventHandler<GEEventArgs>(geWebBrowser_PluginEvent);

                var lookAt = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
                lookAt.setLatitude(latitude);
                lookAt.setLongitude(longitude);
                if (range > 0)
                {
                    lookAt.setRange(range);
                }
                ge.getView().setAbstractView(lookAt);
            }            
        }

        private void geWebBrowser_PluginEvent(object sender, GEEventArgs e)
        {
            if (e.Message == "frameend" && uninterruptibleCamera)
            {
                int x = 5;
            }
        }

        #endregion


        //==============================================================================
        #region Ruler
        
        private void cbEnableRuler_CheckedChanged(object sender, EventArgs e)
        {
            if (cbEnableRuler.Checked)
            {
                EnableRuler();
            }
            else
            {
                DisableRuler();
            }
        }

        private void btnClearRuler_Click(object sender, EventArgs e)
        {
            ClearRuler();
        }



        bool IsFirstCall = true;
        // I can't recreate placemarks with the same id, so I'll count it this way.
        List<IKmlPlacemark> rulerPlacemarks = new List<IKmlPlacemark>();
        IKmlPlacemark linePlacemark = null;
        int pCount = 0;
        bool IsRulerEnabled = false;
        Point mousedownPoint;        
        IKmlPlacemark draggingPM = null;



        private void EnableRuler()
        {
            IsRulerEnabled = true;

            if (IsFirstCall)
            {
                // Folder for ruler elements. To clear fast.
                var rulerFolder = ge.createFolder("rulerFolder");
                ge.getFeatures().appendChild(rulerFolder);

                /*geWebBrowser.AddEventListener(ge.getWindow(), EventId.MouseDown);
                geWebBrowser.AddEventListener(ge.getWindow(), EventId.MouseUp);
                geWebBrowser.AddEventListener(ge.getWindow(), EventId.MouseMove);*/

                IsFirstCall = false;
            }

            
            // Set up style for placemarks
            /*var normalIcon = ge.createIcon("icon_n");
            normalIcon.setHref("http://maps.google.com/mapfiles/kml/shapes/square.png");
            var normalStyle = ge.createStyle("s_ruler_n");
            normalStyle.getIconStyle().setIcon(normalIcon);
            normalStyle.getIconStyle().setScale(0.6f);

            var highlightIcon = ge.createIcon("icon_hl");
            highlightIcon.setHref("http://maps.google.com/mapfiles/kml/shapes/square.png");
            var highlightStyle = ge.createStyle("s_ruler_hl");
            highlightStyle.getIconStyle().setIcon(highlightIcon);
            highlightStyle.getIconStyle().setScale(0.8f);

            var styleMap = ge.createStyleMap("sm_ruler");
            styleMap.setNormalStyle(normalStyle);
            styleMap.setHighlightStyle(highlightStyle);*/


            // Sign on the events
            geWebBrowser.KmlEvent += new EventHandler<GEEventArgs>(geWebBrowser_RulerEvent);
            /**/


            linePlacemark = ge.createPlacemark("");
            ((IKmlFolder)ge.getElementById("rulerFolder")).getFeatures().appendChild(linePlacemark);
            var lineString = ge.createLineString("");
            linePlacemark.setGeometry(lineString);
            lineString.setExtrude(0);
            lineString.setTessellate(1);
            lineString.setAltitudeMode(ge.ALTITUDE_CLAMP_TO_GROUND);

            
            /*var placemark = ge.createPlacemark("pm1");
            ge.getFeatures().appendChild(placemark);
            var point = ge.createPoint("");
            point.setLatitude(37);
            point.setLongitude(-122);
            placemark.setGeometry(point);
            placemark.setStyleSelector(styleMap);


            var lookAt = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
            lookAt.setLatitude(37);
            lookAt.setLongitude(-122);
            lookAt.setRange(500);
            ge.getView().setAbstractView(lookAt);*/


            #region
            /*var a = ge.getElementById("sm_ruler");/**/



            /*var style1 = ge.createStyle("style1");
            var pm1 = ge.createPlacemark("pm1");
            var folder1 = ge.createFolder("folder1");
            folder1.getFeatures().appendChild(pm1);
            ge.getFeatures().appendChild(folder1);
            //ge.getFeatures().appendChild(pm1);
            pm1.setStyleSelector(style1);

            var a = ge.getElementById("pm1"); // object 
            var b = ge.getElementById("style1"); // object */


            /*
            // create the placemark
  var placemark = ge.createPlacemark("pm1");
  var point = ge.createPoint("");
  point.setLatitude(37);
  point.setLongitude(-122);
  placemark.setGeometry(point);

  // add the placemark to the earth DOM
  ge.getFeatures().appendChild(placemark);

  var styleMap = ge.createStyleMap("");

  // Create normal style for style map
  var normalStyle = ge.createStyle("style1");
  var normalIcon = ge.createIcon("icon1");
  normalIcon.setHref("http://maps.google.com/mapfiles/kml/shapes/triangle.png");
  normalStyle.getIconStyle().setIcon(normalIcon);

  // Create highlight style for style map
  var highlightStyle = ge.createStyle("style2");
  var highlightIcon = ge.createIcon("icon2");
  highlightIcon.setHref("http://maps.google.com/mapfiles/kml/shapes/square.png");
  highlightStyle.getIconStyle().setIcon(highlightIcon);

  styleMap.setNormalStyle(normalStyle);
  styleMap.setHighlightStyle(highlightStyle);

  // Apply stylemap to a placemark
  placemark.setStyleSelector(styleMap);


  var lookAt = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
  lookAt.setLatitude(37);
  lookAt.setLongitude(-122);
  lookAt.setRange(500);
  ge.getView().setAbstractView(lookAt);/**/
            #endregion

            int x = 5;
        }

        void geWebBrowser_RulerEvent(object sender, GEEventArgs e)
        {
            if (!IsRulerEnabled || (e.ApiObject as IKmlMouseEvent) == null)
            {
                return;
            }
            
            IKmlMouseEvent me = e.ApiObject;

            if (e.EventId == EventId.MouseDown)
            {
                mousedownPoint.X = me.getClientX();
                mousedownPoint.Y = me.getClientY();

                IKmlPlacemark pm = me.getTarget() as IKmlPlacemark;
                if (pm != null && pm.getId().StartsWith("r_pm_", StringComparison.InvariantCultureIgnoreCase))
                {
                    draggingPM = pm;
                }
            }

            if (e.EventId == EventId.MouseUp)
            {

                if (draggingPM != null)     // dragging
                {
                    /*if (me.getButton() == 2)
                    {
                        RemoveRulerPoint(rulerPlacemarks.IndexOf(draggingPM));
                    }*/
                    draggingPM = null;
                }
                else
                {
                    if (mousedownPoint.X == me.getClientX() && mousedownPoint.Y == me.getClientY())      // was no mouse move between down and up
                    {
                        AddRulerPoint(me.getLatitude(), me.getLongitude());
                        UpdateRulerLength();
                    }                                                                                    // else globe was rotated
                }
            }

            if (e.EventId == EventId.MouseMove)
            {
                if (draggingPM != null)
                {
                    me.preventDefault();
                    MoveRulerPoint(rulerPlacemarks.IndexOf(draggingPM), me.getLatitude(), me.getLongitude());
                    UpdateRulerLength();
                }
                lblCursorCoordinateValue.Text = me.getLatitude().ToString().Replace(',', '.') + " / " + me.getLongitude().ToString().Replace(',', '.');
            }
        }

        private void AddRulerPoint(double latitude, double longitude)
        {
            if (rulerPlacemarks.Count() >= 2)
            {
                ClearRuler();
            }

            // Add point placemark
            var point = ge.createPoint("");
            point.setExtrude(0);
            point.setTessellate(1);
            point.setAltitudeMode(ge.ALTITUDE_CLAMP_TO_GROUND);
            point.setLatLngAlt(latitude, longitude, 0);

            var placemark = ge.createPlacemark("r_pm_" + (++pCount).ToString());
            placemark.setGeometry(point);
            ((IKmlFolder)ge.getElementById("rulerFolder")).getFeatures().appendChild(placemark);
            rulerPlacemarks.Add(placemark);

            ((IKmlLineString)linePlacemark.getGeometry()).getCoordinates().pushLatLngAlt(latitude, longitude, 0);
        }

        private void ClearRuler()
        {
            var rulerFolderFeatures = ((IKmlFolder)ge.getElementById("rulerFolder")).getFeatures();
            foreach (var v in rulerPlacemarks)
            {
                rulerFolderFeatures.removeChild(v);
            }
            rulerPlacemarks.Clear();

            ((IKmlLineString)linePlacemark.getGeometry()).getCoordinates().clear();
        }

        private void MoveRulerPoint(int i, double latitude, double longitude)
        {
            if (i < 0 || i > rulerPlacemarks.Count - 1)
            {
                return;
            }

            ((IKmlPoint)rulerPlacemarks[i].getGeometry()).setLatLng(latitude, longitude);
            ((IKmlLineString)linePlacemark.getGeometry()).getCoordinates().setLatLngAlt(i, latitude, longitude, 0);
        }

        private void RemoveRulerPoint(int i)
        {

        }

        private void UpdateRulerLength()
        {
            if (rulerPlacemarks.Count < 2)
            {
                if (lblRulerLengthValue.Text != "—")
                {
                    lblRulerLengthValue.Text = "—";
                }
            }
            else
            {
                double lat1 = ((IKmlPoint)rulerPlacemarks[0].getGeometry()).getLatitude();
                double lon1 = ((IKmlPoint)rulerPlacemarks[0].getGeometry()).getLongitude();
                double lat2 = ((IKmlPoint)rulerPlacemarks[1].getGeometry()).getLatitude();
                double lon2 = ((IKmlPoint)rulerPlacemarks[1].getGeometry()).getLongitude();
                double l = GeoMath.DistanceBetweenCoordinates(lat1, lon1, lat2, lon2);

                lblRulerLengthValue.Text = l.ToString().Replace(',', '.');
            }
        }

        private void DisableRuler()
        {
            // Sign on the events
            geWebBrowser.KmlEvent -= new EventHandler<GEEventArgs>(geWebBrowser_RulerEvent);
            /*geWebBrowser.RemoveEventListener(ge.getWindow(), EventId.MouseDown, true);
            geWebBrowser.RemoveEventListener(ge.getWindow(), EventId.MouseUp, true);
            geWebBrowser.RemoveEventListener(ge.getWindow(), EventId.MouseMove, true);*/

            // GE clean up
            ClearRuler();
            ((IKmlFolder)ge.getElementById("rulerFolder")).getFeatures().removeChild(linePlacemark);
            
            lblCursorCoordinateValue.Text = String.Empty;

            IsRulerEnabled = false;
        }

        #endregion
        //==============================================================================

        //==============================================================================

        //==============================================================================



        //==============================================================================
        private class Record
        {
            public string RWCoordinate;
            public double Latitude;
            public double Longitude;
            public double pathLatitude;
            public double pathLongitude;
        }

        private List<Record> records = new List<Record>();
        private static System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

        private void btnRWK_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            
            string line = String.Empty;
            StreamReader sr = new StreamReader(openFileDialog.FileName);
            while ((line = sr.ReadLine()) != null)
            {
                string[] s = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                records.Add(new Record() { Latitude = Double.Parse(s[0], ci), Longitude = Double.Parse(s[1], ci), RWCoordinate = s[2] });
            }
            sr.Close();

            if (fullPath == null)
            {
                return; // Plot as is?
            }

            for (int j = 0; j < records.Count; j++)
            {
                int nearest_index = GeoMath.FindNearest(records[j].Latitude, records[j].Longitude, fullPath.fullPath);
                records[j].pathLatitude = fullPath.fullPath[nearest_index].Latitude;
                records[j].pathLongitude = fullPath.fullPath[nearest_index].Longitude;
            }


            #region
            /*StringBuilder OGCKML = new StringBuilder();
            OGCKML.AppendLine("<Folder><name>Railway posts</name>");
            Record r_prev = null;
            int i = 0;
            foreach (var r in records)
            {
                string s = String.Empty;
                if (r_prev == null)
                {
                    r_prev = r;
                }
                else
                {
                    s = (GeoMath.DistanceBetweenCoordinatesMeters(new GeoCoordinate() { Latitude = r_prev.Latitude, Longitude = r_prev.Longitude }, new GeoCoordinate() { Latitude = r.Latitude, Longitude = r.Longitude })).ToString("00.00").Replace(',', '.');
                    r_prev = r;
                }
                OGCKML.AppendLine("<Placemark><name>" + s + "</name><Point><extrude>0</extrude><altitudeMode>relativeToGround</altitudeMode><coordinates>" + r.Longitude.ToString().Replace(',', '.') + "," + r.Latitude.ToString().Replace(',', '.') + "</coordinates></Point></Placemark>"); 
            }
            OGCKML.AppendLine("</Folder>");

            IKmlObject KmlRoot = ge.parseKml(OGCKML.ToString());
            ge.getFeatures().appendChild(KmlRoot);/**/

            
            /*IKmlFolder rw_posts = ge.createFolder("rw_posts");
            Record r_prev = null;
            int i = 0;
            foreach (var r in records)
            {
                // create a point
                KmlPointCoClass point = ge.createPoint("");
                point.setLatitude(r.Latitude);
                point.setLongitude(r.Longitude);
                // create a placemark
                KmlPlacemarkCoClass placemark = ge.createPlacemark("");
                placemark.setGeometry(point);

                string s = String.Empty;
                if (r_prev == null)
                {
                    r_prev = r;
                }
                else
                {
                    s = (GeoMath.DistanceBetweenCoordinatesMeters(new GeoCoordinate() { Latitude = r_prev.Latitude, Longitude = r_prev.Longitude }, new GeoCoordinate() { Latitude = r.Latitude, Longitude = r.Longitude })).ToString("00.00").Replace(',', '.');
                    r_prev = r;
                }

                placemark.setDescription("№" + (i++).ToString() + "\r\n" + "ЖД коорд.: " + r.RWCoordinate + "\r\n" + s);
                placemark.setName(s);
                // add the placemark to the folder
                rw_posts.getFeatures().appendChild(placemark);
            }

            // add the folder to the plugin
            ge.getFeatures().appendChild(rw_posts);/**/


            /*IKmlFolder rw_posts = ge.createFolder("rw_posts");
            Record r_prev = null;
            int i = 0;
            foreach (var r in records)
            {
                // create a point
                var lineString = ge.createLineString("");
                lineString.getCoordinates().pushLatLngAlt(r.Latitude, r.Longitude, 0);
                // create a placemark
                KmlPlacemarkCoClass placemark = ge.createPlacemark("");
                placemark.setGeometry(lineString);

                string s = String.Empty;
                if (r_prev == null)
                {
                    r_prev = r;
                }
                else
                {
                    s = (GeoMath.DistanceBetweenCoordinatesMeters(new GeoCoordinate() { Latitude = r_prev.Latitude, Longitude = r_prev.Longitude }, new GeoCoordinate() { Latitude = r.Latitude, Longitude = r.Longitude })).ToString("00.00").Replace(',', '.');
                    r_prev = r;
                }

                placemark.setDescription("№" + (i++).ToString() + "\r\n" + "ЖД коорд.: " + r.RWCoordinate + "\r\n" + s);
                placemark.setName(s);

                var placemarkLineStyle = placemark.getComputedStyle().getLineStyle();
                placemarkLineStyle.setWidth(25);
                placemarkLineStyle.getColor().set("ff0000ff");

                var placemarkPolyStyle = placemark.getComputedStyle().getPolyStyle();
                placemarkPolyStyle.getColor().set("ffff00ff");


                // add the placemark to the folder
                rw_posts.getFeatures().appendChild(placemark);
            }

            // add the folder to the plugin
            ge.getFeatures().appendChild(rw_posts);/**/
            #endregion

            IKmlFolder rw_posts = ge.createFolder("rw_posts");
            Record r_prev = null;
            int i = 0;
            foreach (var r in records)
            {
                // create a point
                KmlPointCoClass point = ge.createPoint("");
                point.setLatitude(r.pathLatitude);
                point.setLongitude(r.pathLongitude);
                // create a placemark
                KmlPlacemarkCoClass placemark = ge.createPlacemark("");
                placemark.setGeometry(point);

                string s = String.Empty;
                if (r_prev == null)
                {
                    r_prev = r;
                }
                else
                {
                    s = (GeoMath.DistanceBetweenCoordinatesMeters(new GeoCoordinate() { Latitude = r_prev.pathLatitude, Longitude = r_prev.pathLongitude }, new GeoCoordinate() { Latitude = r.pathLatitude, Longitude = r.pathLongitude })).ToString("00.00").Replace(',', '.');
                    r_prev = r;
                }

                placemark.setDescription("№" + (i++).ToString() + "\r\n" + "ЖД коорд.: " + r.RWCoordinate + "\r\n" + s);
                placemark.setName(s);
                // add the placemark to the folder
                rw_posts.getFeatures().appendChild(placemark);
            }

            // add the folder to the plugin
            ge.getFeatures().appendChild(rw_posts);
        }


        IKmlStyleMap TlsMcoStylemap = null;

        private IKmlPlacemark AddTlsPlacemark(double latitude, double longitude, dynamic parent)
        {
            // create a point
            KmlPointCoClass point = ge.createPoint("");
            point.setLatitude(latitude);
            point.setLongitude(longitude);

            // Create style for placemarks
            if (this.TlsMcoStylemap == null)
            {
                var normalIcon = ge.createIcon("");
                normalIcon.setHref("http://maps.google.com/mapfiles/kml/shapes/triangle.png");
                var normalStyle = ge.createStyle("");
                normalStyle.getIconStyle().setIcon(normalIcon);
                normalStyle.getIconStyle().setScale(1f);

                var highlightIcon = ge.createIcon("");
                highlightIcon.setHref("http://maps.google.com/mapfiles/kml/shapes/triangle.png");
                var highlightStyle = ge.createStyle("");
                highlightStyle.getIconStyle().setIcon(highlightIcon);
                highlightStyle.getIconStyle().setScale(1.2f);

                this.TlsMcoStylemap = ge.createStyleMap("");
                this.TlsMcoStylemap.setNormalStyle(normalStyle);
                this.TlsMcoStylemap.setHighlightStyle(highlightStyle);/**/
            }

            // create a placemark
            KmlPlacemarkCoClass placemark = ge.createPlacemark("");
            placemark.setGeometry(point);
            placemark.setStyleSelector(this.TlsMcoStylemap);

            if (parent != null)
            {
                parent.getFeatures().appendChild(placemark);
            }
            else
            {
                ge.getFeatures().appendChild(placemark);
            }

            /*if (first_iteration)
            {
                var lookAt = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
                lookAt.setLatitude(gc.Latitude);
                lookAt.setLongitude(gc.Longitude);
                lookAt.setRange(1000);
                ge.getView().setAbstractView(lookAt);

                first_iteration = false;
            }*/
            return placemark;
        }

        private void btnTLS_Click(object sender, EventArgs e)
        {
            if (ge == null) return;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            IKmlFolder tls_mco_folder = ge.createFolder("tls_mco_folder");

            bool first_iteration = true;
            string line = null;
            StreamReader file_tls_mco = new StreamReader(openFileDialog.FileName);
            while ((line = file_tls_mco.ReadLine()) != null)
            {
                string[] s = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                double latitude = Double.Parse(s[0], ci);
                double longitude = Double.Parse(s[1], ci);

                AddTlsPlacemark(latitude, longitude, tls_mco_folder);
            }

            // add the folder to the plugin
            ge.getFeatures().appendChild(tls_mco_folder);
        }


        //=====================================================================
        //
        //    Model handling
        //
        //=====================================================================

        // IKmlModel if loaded, null otherwise
        private IKmlModel TrainModelLoaded(/*string modelID*/)
        {
            IKmlObjectList gemodels = null;
            int gemodels_count = 0;
            IKmlModel model = null;

            gemodels = ge.getElementsByType("KmlModel");
            if (gemodels == null)
            {
                return null;
            }
            gemodels_count = gemodels.getLength();
            if (gemodels_count == 0)
            {
                return null;
            }
            else
            {
                // Take the first one. GetElementById("train") DOESN'T WORK! Iterate through collection.
                model = (gemodels.item(0) as IKmlModel);
                return model;
            }
        }

        private IKmlModel LoadTrainModel(/*string modelID, string filePath*/)
        {
            IKmlModel model = null;
            if ((model = TrainModelLoaded()) == null)
            {
                var model_file = this.geWebBrowser.FetchKmlSynchronous(server.BaseUrl.AbsoluteUri + "m12.kmz");
                if (model_file == null)
                {
                    return null;
                }
                ge.getFeatures().appendChild(model_file);
                // (model_file as IKmlObject).getType() == "KmlFolder"
                if ((model = TrainModelLoaded()) == null)
                {
                    return null;
                }
            }
            return model;
        }

        private void SetTrainModel(IKmlModel model, double lat, double lon, double azimuth = 0)
        {
            model.getLocation().setLatLngAlt(lat, lon, 0);
            model.setAltitudeMode(ge.ALTITUDE_RELATIVE_TO_GROUND);
            // Model is rotated on 180°
            if (0 <= azimuth && azimuth < 180)
            {
                azimuth += 180;
            }
            else if (180 <= azimuth && azimuth < 360)
            {
                azimuth -= 180;
            }
            model.getOrientation().setHeading(azimuth);
        }


        RidingModelFeature ridingModelFeature = null;
        private void button1_Click(object sender, EventArgs e)
        {
            /*var model = LoadTrainModel();

            var lookAt = ge.getView().copyAsLookAt(ge.ALTITUDE_RELATIVE_TO_GROUND);
            var lat = lookAt.getLatitude();
            var lon = lookAt.getLongitude();
            lookAt.setRange(50);
            ge.getView().setAbstractView(lookAt);

            SetTrainModel(model, lat, lon, 350);*/


            this.ridingModelFeature = new RidingModelFeature(this.geWebBrowser, this.fullPath.fullPath);
            this.ridingModelFeature.Start();
        }


    }
}
