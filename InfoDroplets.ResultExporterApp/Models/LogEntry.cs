namespace InfoDroplets.ResultExporter.Models
{
    public class LogEntry
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

        public override bool Equals(object? obj)
        { 
            LogEntry? le = obj as LogEntry;
            if (le?.Latitude == this.Latitude && le?.Longitude == this.Longitude && le?.Elevation == this.Elevation)
                return true;
            else return false;
        }
    }
}
