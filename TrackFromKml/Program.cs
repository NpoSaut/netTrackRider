using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TrackFromKml
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputDocument = XDocument.Load(args[0]);
            var inputString =
                inputDocument.Root
                         .Element("Document")
                         .Elements("Placemark").First()
                         .Elements("LineString").First()
                         .Element("coordinates").Value;
            var outputString = inputString.Replace(' ', '\n').Replace(',', ' ');
            using (TextWriter tw = new StreamWriter(args[1], false))
            {
                tw.Write(outputString);
            }
            Console.WriteLine("Готово");
        }
    }
}
