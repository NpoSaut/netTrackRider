using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Simulator.GPS
{

    class NMEA0183 : IGPSProtocol
    {
        public double CurrentSpeed
        {
            set
            {
                currentSpeed = value;
            }
        }
        public bool IsValid
        {
            set
            {
                GpsValid = value;
            }
        }
        public byte[] GetPacket(GPSDatum d)
        {
            return getPacket(d);
        }
        public string GetPacketString()
        {
           return packet;
        }



        GPSDatum prevDot = null;
        double currentSpeed = 0;
        public byte[] getPacket(GPSDatum d)
        {
            return System.Text.Encoding.ASCII.GetBytes(getPacketS(d));
        }

        private bool GpsValid = true;
        public void setValid(bool? x)
        {
            GpsValid = (bool)x;
        }


        public void setSpeed(double x)
        {
            currentSpeed = x;
        }

        public string Printable(GPSDatum d)
        {
            return packet;
        }

        string packet = "";
        string getPacketS(GPSDatum d)
        {
            packet = getRMSpacket(d);
            return packet;
        }

        private string inject_checksum(string packet)
        {
            byte[] array = System.Text.Encoding.ASCII.GetBytes(packet.Substring(1, packet.Length - 2));
            byte sum = 0;
            foreach (byte b in array)
                sum ^= b;
            return packet + sum.ToString("X") + "\r\n"; //cr lf

        }
        /*      $--RMC,hhmmss.ss,A,llll.ll,a,yyyyy.yy,a,x.x,x.x,xxxx,x.x,a*hh
                1) Time (UTC)
                2) Status, V = Navigation receiver warning
                3) Latitude
                4) N or S
                5) Longitude
                6) E or W
                7) Speed over ground, knots
                8) Track made good, degrees true
                9) Date, ddmmyy
                10) Magnetic Variation, degrees
                11) E or W
                12) Checksum */
        //$GNRMC,090056.00,A,5651.2598,N,06035.8873,E,00.000,000.0,100112,,,A*7C
        private string getRMSpacket(GPSDatum d)
        {
            double knot_speed = currentSpeed / 1000 * 3600 / 1.852;
            string s = String.Format(CultureInfo.InvariantCulture, "{0:00.000}", knot_speed);
            string s2 = getDirection(d);
            DateTime n = DateTime.UtcNow;
            string time = n.ToString("HHmmss");
            string date = n.ToString("ddMMyy");
            string val = (GpsValid) ? "A," : "V,";
            string p1 = "$GNRMC," + time + ".00," + val + d.nmeaLatitude + "," + d.LatLetter + "," + d.nmeaLongitude + "," + d.LonLetter + "," + s + "," + s2 + "," + date + ",,,A*"; //TODO: not complite NMEA string
            p1 = inject_checksum(p1);
            //$GNGGA,050631.45,5651.2595,N,06035.8363,E,0,00,0.0,254.4,M,,M,,*68
            string p2 = "$GNGGA," + time + ".00," + d.nmeaLatitude + "," + d.LatLetter + "," + d.nmeaLongitude + "," + d.LonLetter + ",1,10,2.0,254.4,M,,M,,*";
            p2 = inject_checksum(p2);
            //$GNGSA,A,1,,,,,,,,,,,,,,,*00
            string p3 = "$GNGSA," + val + "3,01,02,03,04,05,06,07,08,09,10,11,12,2.0,2.0,2.2*";
            p3 = inject_checksum(p3);
            prevDot = d;
            return p1 + p2 + p3;
        }

        private string getDirection(GPSDatum d)
        {
            double angle = 0;
            GPSDatum d0 = prevDot;
            if (d0 == null) return "000.0";
            double deltaLat = d.rLatitude - d0.rLatitude;
            double deltaLon = d.rLongitude - d0.rLongitude;
            if (deltaLon == 0)
                angle = 90 * Math.Sign(deltaLat);
            else
            {
                double c = 90;
                if (deltaLon < 0) c = 270;
                angle = c - Math.Atan(deltaLat / deltaLon) * (180 / Math.PI);
            }
            return String.Format(CultureInfo.InvariantCulture, "{0:000.0}", angle);

        }
    }

    /*public class NMEA0183 : IGPSProtocol
    {
        private GPSDatum prevDot = null;
        private double currentSpeed = 0;
        private bool gpsValid = true;
        private string packet = String.Empty;
        

        public bool IsValid
        {
            set { this.gpsValid = value; }
        }

        public double CurrentSpeed
        {
            set { this.currentSpeed = value; }
        }


        private string inject_checksum(string packet)
        {
            byte[] array = System.Text.Encoding.ASCII.GetBytes(packet.Substring(1, packet.Length - 2));
            byte sum = 0;
            foreach (byte b in array)
                sum ^= b;
            return packet + sum.ToString("X") + "\r\n"; //cr lf
        }
        
        private string getDirection(GPSDatum d)
        {
            double angle = 0;
            GPSDatum d0 = prevDot;
            if (d0 == null) return "000.0";
            double deltaLat = d.rLatitude - d0.rLatitude;
            double deltaLon = d.rLongitude - d0.rLongitude;
            if (deltaLon == 0)
                angle = 90 * Math.Sign(deltaLat);
            else
            {
                double c = 90;
                if (deltaLon < 0) c = 270;
                angle = c - Math.Atan(deltaLat / deltaLon) * (180 / Math.PI);
            }
            return String.Format(CultureInfo.InvariantCulture, "{0:000.0}", angle);
        }

        /*      $--RMC,hhmmss.ss,A,llll.ll,a,yyyyy.yy,a,x.x,x.x,xxxx,x.x,a*hh
                1) Time (UTC)
                2) Status, V = Navigation receiver warning
                3) Latitude
                4) N or S
                5) Longitude
                6) E or W
                7) Speed over ground, knots
                8) Track made good, degrees true ?! / наземный курс, в градусах
                9) Date, ddmmyy
                10) Magnetic Variation, degrees
                11) E or W
                12) режим местоопределения: А – автономный, D – дифференциальный
                13) Checksum */
        //$GNRMC,090056.00,A,5651.2598,N,06035.8873,E,00.000,000.0,100112,,,A*7C
        /*private string getGCpacket(GPSDatum d)
        {
            // 1) Time (UTC)
            DateTime dt = DateTime.UtcNow;
            string time = dt.ToString("HHmmss");

            // 2) Status, V = Navigation receiver warning
            string validity = (gpsValid) ? "A," : "V,";

            // 3) Latitude
            // 4) N or S
            // 5) Longitude
            // 6) E or W
            // —

            // 7) Speed over ground, knots
            double speed_knots = currentSpeed / 1000 * 3600 / 1.852;
            string s = String.Format(CultureInfo.InvariantCulture, "{0:00.000}", speed_knots);

            // 8) Track made good, degrees true / наземный курс, в градусах;
            string s2 = getDirection(d);

            // 9) Date, ddmmyy
            string date = dt.ToString("ddMMyy");

            string p1 = "$GNRMC," + time + ".00," + validity + d.nmeaLatitude + "," + d.LatLetter + "," + d.nmeaLongitude + "," + d.LonLetter + "," + s + "," + s2 + "," + date + ",,,A*"; //TODO: not complete NMEA string
            p1 = inject_checksum(p1);
            //$GNGGA,050631.45,5651.2595,N,06035.8363,E,0,00,0.0,254.4,M,,M,,*68
            string p2 = "$GNGGA," + time + ".00," + d.nmeaLatitude + "," + d.LatLetter + "," + d.nmeaLongitude + "," + d.LonLetter + ",1,10,2.0,254.4,M,,M,,*";
            p2 = inject_checksum(p2);
            //$GNGSA,A,1,,,,,,,,,,,,,,,*00
            string p3 = "$GNGSA," + validity + "3,01,02,03,04,05,06,07,08,09,10,11,12,2.0,2.0,2.2*";
            p3 = inject_checksum(p3);
            prevDot = d;
            return p1 + p2 + p3;
        }
        
        public string GetPacketS(GPSDatum d)
        {
            packet = getGCpacket(d);
            return packet;
        }

        public byte[] GetPacket(GPSDatum d)
        {
            return System.Text.Encoding.ASCII.GetBytes(GetPacketS(d));
        }

        public string GetPacketString()
        {
            return packet;
        }
    }*/
}
