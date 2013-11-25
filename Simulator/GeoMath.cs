using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulator
{
    public class GeoCoordinate
    {
        public double Longitude;
        public double Latitude;
        public double Altitude;
        private static System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

        public static GeoCoordinate GetFromLine(string Line)
        {
            string[] coordinates = Line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return new GeoCoordinate() { Longitude = Double.Parse(coordinates[0], ci), Latitude = Double.Parse(coordinates[1], ci) };
        }

        public bool Equals(GeoCoordinate gc)
        {
            // If parameter is null return false:
            if ((object)gc == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (this.Latitude == gc.Latitude) && (this.Longitude == gc.Longitude);
        }

        public static bool operator ==(GeoCoordinate gc1, GeoCoordinate gc2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(gc1, gc2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)gc1 == null) || ((object)gc2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return (gc1.Latitude == gc2.Latitude) && (gc1.Longitude == gc2.Longitude);
        }

        public static bool operator !=(GeoCoordinate gc1, GeoCoordinate gc2)
        {
            return !(gc1 == gc2);
        }

        public override string ToString() { return string.Format("Longitude: {0}, Latitude: {1}", Longitude, Latitude); }
    }

    public class GeoMath
    {
        #region Distances calculation

        // Distance between coordinates in kilometers
        public static double DistanceBetweenCoordinates(double lat1d, double lon1d, double lat2d, double lon2d/*, DistanceCalculationMethod CalculationMethod*/)
        {
            // Sphere Earth

            double lat1r = lat1d * Math.PI / 180;
            double lon1r = lon1d * Math.PI / 180;
            double lat2r = lat2d * Math.PI / 180;
            double lon2r = lon2d * Math.PI / 180;

            double deltalon = lon1r - lon2r;

            double num = Math.Sqrt(Math.Pow(Math.Cos(lat2r) * Math.Sin(deltalon), 2) + Math.Pow(Math.Cos(lat1r) * Math.Sin(lat2r) - Math.Sin(lat1r) * Math.Cos(lat2r) * Math.Cos(deltalon), 2));
            double denum = Math.Sin(lat1r) * Math.Sin(lat2r) + Math.Cos(lat1r) * Math.Cos(lat2r) * Math.Cos(deltalon);
            return Math.Atan(num / denum) * 6372.795;
        }

        // Distance between coordinates in kilometers
        public static double DistanceBetweenCoordinates(GeoCoordinate c1, GeoCoordinate c2)
        {
            return GeoMath.DistanceBetweenCoordinates(c1.Latitude, c1.Longitude, c2.Latitude, c2.Longitude);
        }

        // Distance between coordinates in meters
        public static double DistanceBetweenCoordinatesMeters(GeoCoordinate c1, GeoCoordinate c2)
        {
            return GeoMath.DistanceBetweenCoordinates(c1, c2) * 1000;
        }


        // f is distance from (xi. yi) to (x1,y1) divided by distance between (x1,y1) and (x2,y2)
        public static GeoCoordinate IntermediateCoordinate(GeoCoordinate c1, GeoCoordinate c2, double f)
        {
            return new GeoCoordinate() { Longitude = c1.Longitude + f * (c2.Longitude - c1.Longitude), Latitude = c1.Latitude + f * (c2.Latitude - c1.Latitude) };
        }

        // s — distance in meters to the point to be added in (c1,c2) derection after c2
        public static GeoCoordinate CoordinateSurplus(GeoCoordinate c1, GeoCoordinate c2, double s)
        {
            double distance = DistanceBetweenCoordinatesMeters(c1, c2);
            double f = (distance + s) / distance;
            return IntermediateCoordinate(c1, c2, f);
        }

        // Distance between coordinates in kilometers. For ideal sphere.
        public static double DistanceBetweenCoordinatesSphere(double lat1d, double lon1d, double lat2d, double lon2d/*, DistanceCalculationMethod CalculationMethod*/)
        {
            // Sphere Earth

            double lat1r = lat1d * Math.PI / 180;
            double lon1r = lon1d * Math.PI / 180;
            double lat2r = lat2d * Math.PI / 180;
            double lon2r = lon2d * Math.PI / 180;

            return Math.Acos(Math.Sin(lat1r) * Math.Sin(lat2r) + Math.Cos(lat1r) * Math.Cos(lat2r) * Math.Cos(lon1r - lon2r)) * 6372.795;
        }

        #endregion


        #region Shifting along the path

        // Returns in meters
        public static double GetPathLength(List<GeoCoordinate> path /*method*/)
        {
            if (path.Count < 2) return -1;

            double pathLength = 0;
            for (int i = 0; i < path.Count - 1; i++)
            {
                pathLength += GeoMath.DistanceBetweenCoordinatesMeters(path[i], path[i + 1]);
            }

            return pathLength;
        }

        private static GeoCoordinate MoveBackward(List<GeoCoordinate> geoCoordinates, int coordinateIndex, double offset_meters)
        {
            double distance = 0;
            double last_distance = 0;
            int i = coordinateIndex;

            while (distance <= offset_meters && i > 0)
            {
                last_distance = GeoMath.DistanceBetweenCoordinatesMeters(geoCoordinates[i - 1], geoCoordinates[i]);
                distance += last_distance;
                i--;
            }

            // Distance is larger than in the list
            if (i == 0 && distance < offset_meters)
            {
                return null;
            }

            return GeoMath.IntermediateCoordinate(geoCoordinates[i], geoCoordinates[i + 1], (distance - offset_meters) / last_distance);
        }

        public static GeoCoordinate MoveForward(List<GeoCoordinate> path, int coordinateIndex, double offset_meters, ref int cIndex, ref double cLength, ref double cLastLength)
        {
            /*double length = 0;
            double last_length = 0;
            int i = coordinateIndex;

            while (length <= offset_meters && i < path.Count - 1)
            {
                last_length = GeoMath.DistanceBetweenCoordinatesMeters(path[i], path[i + 1]);
                length += last_length;
                i++;
            }

            // Distance is larger than in the list
            if (i == path.Count - 1 && length < offset_meters)
            {
                return null;
            }

            cIndex = i; cLength = length; cLastLength = last_length;
            return GeoMath.IntermediateCoordinate(path[i - 1], path[i], 1 - ((length - offset_meters) / last_length));/**/


            double length = 0;
            double last_length = 0;
            int i = coordinateIndex;

            if (cIndex >= 0 && cLength > 0 && cLastLength > 0 && cLength > cLastLength)
            {
                if (offset_meters >= cLength - cLastLength)
                {
                    length = cLength;
                    last_length = cLastLength;
                    i = cIndex;
                }
            }

            while (length <= offset_meters && i < path.Count - 1)
            {
                last_length = GeoMath.DistanceBetweenCoordinatesMeters(path[i], path[i + 1]);
                length += last_length;
                i++;
            }

            // Distance is larger than in the list
            if (i == path.Count - 1 && length < offset_meters)
            {
                return new GeoCoordinate() { Latitude = Double.NaN, Longitude = Double.NaN, Altitude = Double.NaN };
                cIndex = -1; cLength = length; cLastLength = last_length;
            }

            cIndex = i; cLength = length; cLastLength = last_length;
            return GeoMath.IntermediateCoordinate(path[i - 1], path[i], 1 - ((length - offset_meters) / last_length));/**/
        }

        // Given list of coordinates and coordinate to count off from returns the coordinate that is distance_meters away. 
        // If offset_meters > 0 coordinate will be forth, else — backward.
        static public GeoCoordinate MoveAlongPath(List<GeoCoordinate> path, int coordinateIndex, double offset_meters, ref int cIndex, ref double cLength, ref double cLastLength)
        {
            if (offset_meters >= 0)
            {
                return GeoMath.MoveForward(path, coordinateIndex, offset_meters, ref cIndex, ref cLength, ref cLastLength);
            }
            else //if (offset_meters < 0)
            {
                return GeoMath.MoveBackward(path, coordinateIndex, -offset_meters);

            }
        }

        /*public static GeoCoordinate CalculateCoordinate(List<GeoCoordinate> path, double distanceFromBeginning, double pathLength)
        {
            if (pathLength == -1)
            {
                pathLength = GetPathLength(path);
            }
        }*/

        #endregion


        #region Projection on the path

        // Finding nearest point on the path to the specified one
        private static double Metrics(double xi, double yi, double xj, double yj)
        {
            //return Math.Abs(xi - xj) + Math.Abs(yi - yj);
            //return Math.Pow((xi - xj), 2) + Math.Pow((yi - yj), 2);
            //return DistanceBetweenCoordinates(xi, yi, xj, yj);
            return DistanceBetweenCoordinatesSphere(xi, yi, xj, yj);
        }

        public static int FindNearest(double pointLatitude, double pointLongitude, List<GeoCoordinate> path)
        {
            double d_mm = 0;
            int i_mm = -1;
            double d_m = Metrics(pointLatitude, pointLongitude, path[0].Latitude, path[0].Longitude);
            int i_m = 0;
            double d = 0;
            int i = -1;

            for (i = 1; i < path.Count - 1; i++)
            {
                d = Metrics(pointLatitude, pointLongitude, path[i].Latitude, path[i].Longitude);
                if (d < d_m)
                {
                    d_mm = d_m;
                    i_mm = i_m;
                    d_m = d;
                    i_m = i;
                }
            }

            return i_m;
        }

        public static int FindNearest(GeoCoordinate point, List<GeoCoordinate> path)
        {
            double d_mm = 0;
            int i_mm = -1;
            double d_m = Metrics(point.Latitude, point.Longitude, path[0].Latitude, path[0].Longitude);
            int i_m = 0;
            double d = 0;
            int i = -1;

            for (i = 1; i < path.Count - 1; i++)
            {
                d = Metrics(point.Latitude, point.Longitude, path[i].Latitude, path[i].Longitude);
                if (d < d_m)
                {
                    d_mm = d_m;
                    i_mm = i_m;
                    d_m = d;
                    i_m = i;
                }
            }

            return i_m;
        }
        #endregion
    }
}
