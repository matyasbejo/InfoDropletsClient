
using System.ComponentModel.DataAnnotations;

namespace InfoDroplets.Models
{
    public class GpsPos
    {
        #region Properties
        [Required]
        [Range(0, 180)]
        public double Latitude { get; protected set; }

        [Required]
        [Range(0, 180)]
        public double Longitude { get; protected set; }

        [Required]
        [Range(0, 50000)]
        public double Elevation { get; protected set; }
        #endregion

        public GpsPos(double latitude, double longitude, double elevation)
        {
            Latitude = latitude;
            Longitude = longitude;
            Elevation = elevation;
        }

        public GpsPos()
        {
               
        }

        public override string ToString()
        {
            return $"{Latitude},{Longitude},{Elevation}";
        }
    }
}
