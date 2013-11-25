using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Simulator
{
    public class DatabaseParser
    {
        private struct LastRequestCache
        {
            public double RequestedDistance;
            public double PassedDisntance;
            public int TrackPointIndex;
        };  

        public List<GeoCoordinate> track = new List<GeoCoordinate>();
        private double trackLength = 0;
        private string databaseFile = null;
        private LastRequestCache lastRequestCache = new LastRequestCache() { RequestedDistance = 0, PassedDisntance = 0, TrackPointIndex = 0 };

        public double TrackLength
        {
            get { return this.trackLength; }
        }

        //public DatabaseParser() { }
        public DatabaseParser(string databaseFile, bool load = false)   // Сослаться на предыдущий конструктор
        {
            this.databaseFile = databaseFile;

            if (load)
            {
                StreamReader streamReader = new StreamReader(databaseFile);
                string line = null;
                while ((line = streamReader.ReadLine()) != null)
                {
                    this.track.Add(GeoCoordinate.GetFromLine(line));
                }
                streamReader.Close();
            }

            this.trackLength = GetTrackLength();
        }


        // Track length in meters
        private double GetTrackLength()
        {
            double distance = 0;
            for (int i = 0; i < this.track.Count - 1; i++)
            {
                distance += GeoMath.DistanceBetweenCoordinatesMeters(this.track[i], this.track[i+1]);
            }
            return distance;
        }

        public GeoCoordinate GetTrackCoordinate(double distanceFromBeginning)
        {
            if (distanceFromBeginning == 0) return this.track[0];
            if (distanceFromBeginning >= this.trackLength) return this.track[this.track.Count - 1];
            //return this.track[0];
            double last_distance = 0;
            double passed_distance = 0;
            int i = -1;

            //if (lastRequestCache.RequestedDistance < distanceFromBeginning)
            //{
            //    passed_distance = lastRequestCache.PassedDisntance;
            //    i = lastRequestCache.TrackPointIndex+1;
            //}

            while (i < this.track.Count - 2 && passed_distance < distanceFromBeginning)
            {
                i++;
                last_distance = GeoMath.DistanceBetweenCoordinatesMeters(this.track[i], this.track[i + 1]);
                passed_distance += last_distance;
            }
            // Which exit condition fired?
            //if (distance < distanceFromBeginning) return

            lastRequestCache.RequestedDistance = distanceFromBeginning;
            lastRequestCache.PassedDisntance = passed_distance;
            lastRequestCache.TrackPointIndex = i;

            ////if (distanceFromBeginning > this.trackLength) distanceFromBeginning = this.trackLength;
            double f = (distanceFromBeginning - (passed_distance - last_distance)) / last_distance;

            //bool ff = f >= 0.999 ? true : false;
            //GeoCoordinate gcc = GeoMath.IntermediateCoordinate(this.track[i], this.track[i + 1], f);
            //bool inf = Double.IsInfinity(gcc.Latitude);

            //return gcc;
            
            return GeoMath.IntermediateCoordinate(this.track[i], this.track[i + 1], f);/**/
        }

        // Track length in meters
        public static double GetTrackLength(string databaseFile)
        {
            StreamReader streamReader = new StreamReader(databaseFile);
            string line = null;
            GeoCoordinate gc = new GeoCoordinate(); // To avoid anussigned variable issue
            double distance = 0;

            // "Zero" iteration
            if ((line = streamReader.ReadLine()) != null) gc = GeoCoordinate.GetFromLine(line);

            // Iterating further
            while ((line = streamReader.ReadLine()) != null)
            {
                distance += GeoMath.DistanceBetweenCoordinatesMeters(gc, (gc = GeoCoordinate.GetFromLine(line)));
            }

            streamReader.Close();
            return distance;
        }
    }
}
