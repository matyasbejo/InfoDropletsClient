using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultExporter
{
    internal class LogEntry
    {
        public LogEntry(double elevation, double latitude, double longitude)
        {
            Elevation = elevation;
            Latitude = latitude;
            Longitude = longitude;
        }

        internal double Elevation { get; private set; }
        internal double Latitude { get; private set; }
        internal double Longitude { get; private set; }
    }
}
