
namespace InfoDroplets.Models
{
    public class GpsPos
    {
        #region Properties
        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        public double Elevation { get; private set; }
        #endregion

        public GpsPos(double latitude, double longitude, double elevation)
        {
            Latitude = latitude;
            Longitude = longitude;
            Elevation = elevation;
        }

        public override string ToString()
        {
            return $"{Latitude},{Longitude},{Elevation}";
        }
    }
}
