using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Simulator.GPS
{
    /*public class GCoor
    {
        public double longitude;
        public double latitude;
        public double altitude;

        public int Longitude
        {
            set
            {

            }
            get
            {

            }
        }
    }*/
    
    public class GPSDatum
    {
        public string LonLetter, LatLetter;

        public string Latitude;  // широта
        public string Longitude; // долгота
        public string Height;   //  высота 

        public double dLatitude;
        public double dLongitude;
        public double dHeight;

        public double rLatitude;
        public double rLongitude;

        public UInt32 i32Latitude;
        public UInt32 i32Longitude;
        public string nmeaLatitude;
        public string nmeaLongitude;

        // It converts to 60,000 format !!!
        public GPSDatum(GeoCoordinate gc) : this(gc.Latitude.ToString(), gc.Longitude.ToString()) { }

        public GPSDatum(double r_lat, double r_lon)
        {
            rLatitude = r_lat;
            rLongitude = r_lon;
            dHeight = 0;
            dLatitude = rLatitude * 180 / Math.PI;
            dLongitude = rLongitude * 180 / Math.PI;
            i32Latitude = (UInt32)(rLatitude * 100000000);
            i32Longitude = (UInt32)(rLongitude * 100000000);
            nmeaLatitude = getnmea(dLatitude);
            nmeaLongitude = getnmea(dLongitude);
            fillLetters();
        }

        private void fillLetters() { 
            if (rLatitude>0 ) LatLetter="N"; else LatLetter="S";
            if (rLongitude > 0) LonLetter = "E"; else LonLetter = "W";

        }

        public GPSDatum(string lat, string lon)
        {
            Latitude = lat;
            Longitude = lon;
            Height = "0";
            FillAll();
        }
        public GPSDatum(string lat, string lon, string h)
        {
            Latitude = lat;
            Longitude = lon;
            Height = h;
            FillAll();
        }

        

        private void FillAll()
        {
            dLatitude = double.Parse(Latitude.Replace(',', '.'), CultureInfo.InvariantCulture);
            dLongitude = double.Parse(Longitude.Replace(',', '.'), CultureInfo.InvariantCulture);
            dHeight = double.Parse(Height, CultureInfo.InvariantCulture);
            rLatitude = dLatitude * Math.PI / 180;
            rLongitude = dLongitude * Math.PI / 180;

            i32Latitude = (UInt32)(rLatitude * 100000000);
            i32Longitude = (UInt32)(rLongitude * 100000000);

            fillLetters();
            nmeaLatitude = getnmea(dLatitude);
            nmeaLongitude = getnmea(dLongitude);

        }


        //float -> ddmm.mmmm 
        private string getnmea(double f)
        {
            if (f < 0) f = -f;
            double int1 = Math.Truncate(f);
            int int2 = (int)int1;
            double fl = f - int1;
            double min = fl * 60;
            string s = int2.ToString("D2", CultureInfo.InvariantCulture) + String.Format(CultureInfo.InvariantCulture, "{0:00.0000}", min);
            //min.ToString("N5", CultureInfo.InvariantCulture);
            return s;
        }

		private const double  r_eq = 6378000;   //r экватор
		private const double r_pl = 6357000;    //r полярный
        public static double distanse(GPSDatum d1, GPSDatum d2)
        {
			double lon1p = d1.rLongitude;
			double lon2p = d2.rLongitude;
			double lat1p = d1.rLatitude;
			double lat2p = d2.rLatitude;
			
			double r = r_eq + (r_pl - r_eq) * (lat1p+lat2p)/Math.PI;
			double Dist = r * Math.Acos(Math.Sin(lat1p)*Math.Sin(lat2p) + Math.Cos(lat1p)*Math.Cos(lat2p)*Math.Cos(lon1p -lon2p));
			return Dist;
        }
    }
}
