using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoDroplets.ResultExporter.Models
{
    internal class LogEntry
    {
        public LogEntry(double latitude, double longitude, double elevation)
        {
            Elevation = elevation;
            Latitude = latitude;
            Longitude = longitude;
        }

        internal double Elevation { get; private set; }
        internal double Latitude { get; private set; }
        internal double Longitude { get; private set; }

        public override string ToString()
        {
            return $"[{Math.Round(Latitude,6)}, {Math.Round(Longitude, 6)}, {Math.Round(Elevation, 1)}]";
        }
    }
}
