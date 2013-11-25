using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Timers;

using GEPlugin;
using FC.GEPlugin;
using FC.GEPluginCtrls;
using FC.GEPluginCtrls.HttpServer;


namespace Simulator.GoogleEarth
{
    class FullPathFeature
    {
        public IGEPlugin ge = null;
        public List<GeoCoordinate> fullPath = null;

        public FullPathFeature(IGEPlugin ge, List<GeoCoordinate> fullPath)
        {
            this.ge = ge;
            this.fullPath = fullPath;
        }

        public void DrawPath(/*, PositionCameraAt positionCameraAt*/)
        {
            if (ge != null && fullPath != null)
            {
                string kmlString = GeoCoordinatesToKmlPath(this.fullPath, "fullPath", Color.FromArgb(255, 255, 255, 255), 1);
                IKmlObject KmlRoot = ge.parseKml(kmlString);
                ge.getFeatures().appendChild(KmlRoot);
            }
        }

        private string GeoCoordinatesToKmlPath(List<GeoCoordinate> geoCoordinates, string pathName, Color pathColor, float pathWidth)
        {
            /*KmlStyleCoClass*/
            // TODO: Add style to the document; whether style exists or not "ff00ffff"
            IKmlStyle style = ge.createStyle("fullPathStyle");
            style.getLineStyle().getColor().set(new KmlColor(pathColor).ToString());
            style.getLineStyle().setWidth(pathWidth);


            int i = 0;
            StringBuilder OGCKML = new StringBuilder();
            OGCKML.AppendLine("<Folder> <name>" + pathName + "</name>");
            for (int j = 0; j < geoCoordinates.Count; j++)
            {
                if (i == 0)
                {
                    OGCKML.AppendLine("<Placemark><styleUrl>#fullPathStyle</styleUrl><LineString><tessellate>1</tessellate><coordinates>");
                    OGCKML.AppendLine(geoCoordinates[j].Longitude.ToString().Replace(',', '.') + "," + geoCoordinates[j].Latitude.ToString().Replace(',', '.') + ",0");  // lon lat alt
                    i++;
                }
                else if (i < 65000)
                {
                    OGCKML.AppendLine(geoCoordinates[j].Longitude.ToString().Replace(',', '.') + "," + geoCoordinates[j].Latitude.ToString().Replace(',', '.') + ",0");  // lon lat alt
                    i++;
                }
                else
                {
                    OGCKML.AppendLine(geoCoordinates[j].Longitude.ToString().Replace(',', '.') + "," + geoCoordinates[j].Latitude.ToString().Replace(',', '.') + ",0");  // lon lat alt
                    OGCKML.AppendLine("</coordinates></LineString></Placemark>");
                    i = 0;
                }
            }
            OGCKML.AppendLine("</coordinates></LineString></Placemark>");
            OGCKML.AppendLine("</Folder>");

            return OGCKML.ToString();
        }

        //public void Show() { };
        //public void Hide() { };
    }

    class RidingModelFeature
    {
        public GEWebBrowser geWebBrowser = null;
        public IGEPlugin ge = null;
        public List<GeoCoordinate> fullPath = null;

        public RidingModelFeature() { }

        public RidingModelFeature(GEWebBrowser geWebBrowser, List<GeoCoordinate> fullPath)
        {
            this.geWebBrowser = geWebBrowser;
            this.ge = geWebBrowser.Plugin as IGEPlugin;
            this.fullPath = fullPath;

            LoadTrainModel();
        }


        private string trainModelFile = "http://localhost:50500/m12.kmz";
        private IKmlModel trainModel = null;

        private IKmlModel TrainModelLoaded()
        {
            IKmlObjectList gemodels = null;
            int gemodels_count = 0;
            IKmlModel model = null;

            gemodels = this.ge.getElementsByType("KmlModel");
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

        private IKmlModel LoadTrainModel()
        {
            if (this.trainModel == null)
            {
                if (this.geWebBrowser == null) return null;

                var model_file = this.geWebBrowser.FetchKmlSynchronous(/*trainModelFile*/"http://localhost:50500/m12.kmz");
                if (model_file == null)
                {
                    return null;
                }
                ge.getFeatures().appendChild(model_file);
                if ((this.trainModel = TrainModelLoaded()) == null)
                {
                    return null;
                }
            }
            this.trainModel.getLocation().setLatLngAlt(60, 60, 0);
            return this.trainModel;
        }

        private void SetTrainModel(IKmlModel model, double lat, double lon, double azimuth = 0)
        {
            if (model == null)
            {
                return;
            }

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

        private void SetTrainModel(double lat, double lon, double azimuth = 0)
        {
            SetTrainModel(this.trainModel, lat, lon, azimuth);
        }


        private bool IsStarted = false;
        /*public double StartLength { set; get; }
        public GeoCoordinate StartPoint { set; get; }
        public double Speed { set; get; }*/
        Timer timer = new Timer(25);    // 40 fps

        public void Start()
        {
            this.IsStarted = true;
            timer.Elapsed += new ElapsedEventHandler(timerTick);
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
            timer.Elapsed -= new ElapsedEventHandler(timerTick);
            this.IsStarted = false;
        }


        private double v = 40;  // km/h
        private GeoCoordinate p_gc = null;
        private double s = 0;    //
        private int cIndex = 0;
        private double cLength = 0;
        private double cLastLength = 0;

        private void timerTick(object sender, EventArgs e)
        {
            // distance in 1 tick
            // v [km/h] = v * (1/3600) [m/ms]; s [m] = v [m/ms] * t [ms]
            this.s += this.v * (1 / 3600) * this.timer.Interval;
            double d = s; // + startoffset

            GeoCoordinate gc = GeoMath.MoveAlongPath(this.fullPath, 0, d, ref this.cIndex, ref this.cLength, ref this.cLastLength);

            SetTrainModel(gc.Latitude, gc.Longitude, 0);


            this.p_gc = gc;
        }
    }


    // TODO: Singleton
    class RidingPathFeature
    {
        //private static RidingPathFeature instance = null;
        
        public IGEPlugin ge = null;
        public List<GeoCoordinate> ridingPath = new List<GeoCoordinate>();

        private int pointsInSection = 0;
        private IKmlLineString currentSection = null;

        private RidingPathFeature() { }

        public RidingPathFeature(IGEPlugin ge)
        {
            this.ge = ge;
        }

        public void AddPoint(GeoCoordinate gc)
        {
            AddPoint(gc, true, false);
        }

        IKmlStyle ridingPathStyle = null;
        public void AddPoint(GeoCoordinate gc, bool createNew, bool resetPath)
        {
            // Some constants
            string pathColor = "ff00ffff";
            float pathWidth = 2;
            
            var ridingPathFolder = (IKmlFolder)ge.getElementById("ridingPath");

            if (ridingPathFolder == null)
            {
                if (createNew)
                {
                    ridingPathFolder = ge.createFolder("ridingPath");
                    ge.getFeatures().appendChild(ridingPathFolder);

                    //
                    ridingPathStyle = ge.createStyle("ridingPathStyle");
                    ridingPathStyle.getLineStyle().getColor().set(pathColor);
                    ridingPathStyle.getLineStyle().setWidth(pathWidth);
                }
                else
                {
                    return;
                }
            }


            if (resetPath)
            {
                ResetPath();
            }


            if (pointsInSection == 0)   // Create new Placemark with new LineString
            {
                // Create the placemark
                var lineStringPlacemark = ge.createPlacemark("");

                // Create the LineString
                this.currentSection = ge.createLineString("");
                lineStringPlacemark.setGeometry(this.currentSection);
                this.currentSection.setExtrude(0);
                this.currentSection.setTessellate(1);
                this.currentSection.setAltitudeMode(ge.ALTITUDE_CLAMP_TO_GROUND);

                // Add the feature to Earth
                ridingPathFolder.getFeatures().appendChild(lineStringPlacemark);
                lineStringPlacemark.setStyleSelector(ridingPathStyle);
            }

            if (pointsInSection++ >= 65000)
            {
                pointsInSection = 0;   
            }

            this.currentSection.getCoordinates().pushLatLngAlt(gc.Latitude, gc.Longitude, 0);

            //=================//
            // Handle 3D model //
            //=================//

            if (this.ShowModel == true && this.geWebBrowser != null)
            {
                LoadTrainModel();
                double azimuth = 0;
                if (this.ridingPath.Count > 1)
                {
                    double p_lat = this.ridingPath[this.ridingPath.Count - 1].Latitude;
                    double p_lon = this.ridingPath[this.ridingPath.Count - 1].Longitude;
                    FC.GEPluginCtrls.Geo.Coordinate p_c = new FC.GEPluginCtrls.Geo.Coordinate(latitude: p_lat, longitude: p_lon);
                    FC.GEPluginCtrls.Geo.Coordinate c = new FC.GEPluginCtrls.Geo.Coordinate(latitude: gc.Latitude, longitude: gc.Longitude);
                    azimuth = FC.GEPluginCtrls.Geo.Maths.BearingInitial(p_c, c);
                }
                SetTrainModel(gc.Latitude, gc.Longitude, azimuth);
            }

            this.ridingPath.Add(gc);
        }

        public void ResetPath()
        {
            var ridingPathFolder = (IKmlFolder)ge.getElementById("ridingPath");
            if (ridingPathFolder != null)
            {
                var features = ridingPathFolder.getFeatures();
                IKmlObject firstChild = null;
                while ((firstChild = features.getFirstChild()) != null)
                {
                    features.removeChild(firstChild);
                }
            }
            this.pointsInSection = 0;
            this.currentSection = null;
        }

        //=====================================================================
        //
        //    Model handling
        //
        //=====================================================================

        public bool ShowModel = true;
        public GEWebBrowser geWebBrowser = null;

        private IKmlModel trainModel = null;

        // IKmlModel if loaded, null otherwise
        private IKmlModel TrainModelLoaded(/*string modelID*/)
        {
            IKmlObjectList gemodels = null;
            int gemodels_count = 0;
            IKmlModel model = null;

            gemodels = this.ge.getElementsByType("KmlModel");
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
            if (this.trainModel == null)
            {
                if (this.geWebBrowser == null) return null;

                var model_file = this.geWebBrowser.FetchKmlSynchronous("http://localhost:50500/m12.kmz");
                if (model_file == null)
                {
                    return null;
                }
                ge.getFeatures().appendChild(model_file);
                // (model_file as IKmlObject).getType() == "KmlFolder"
                if ((this.trainModel = TrainModelLoaded()) == null)
                {
                    return null;
                }
            }
            return this.trainModel;
        }

        private void SetTrainModel(IKmlModel model, double lat, double lon, double azimuth = 0)
        {
            if (model == null)
            {
                return;
            }

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

        private void SetTrainModel(double lat, double lon, double azimuth = 0)
        {
            SetTrainModel(this.trainModel, lat, lon, azimuth);
        }
    }
}
