using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using InfoDroplets.Utils.Interfaces;

namespace InfoDroplets.Models
{
    public class TrackingEntry : IGpsPos
    {
        #region properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DropletId { get; private set; }

        public virtual Droplet Droplet { get; private set; }

        [Required]
        [Range(0, 32)]
        public int SatelliteCount { get; private set; }

        [Required]
        public DateTime Time { get; private set; }

        public double Elevation { get; private set; }

        public double Latitude { get; private set; }

        public double Longitude { get; private set; }
        #endregion

        public TrackingEntry(int dropletId, int satelliteCount, double longitude, double latitude, double elevation, DateTime time)
        {
            DropletId = dropletId;
            SatelliteCount = satelliteCount;
            Time = time;
        }

        public TrackingEntry(int id, int dropletId, int satelliteCount, double longitude, double latitude, double elevation, DateTime time) : this(dropletId,satelliteCount, longitude, latitude, elevation, time)
        {
            Id = id;
        }

        public TrackingEntry(string inputString)
        {
            var inputValues = inputString.Split(";");
            DropletId = int.Parse(inputValues[0]);
            SatelliteCount = int.Parse(inputValues[1]);
            Time = DateTime.ParseExact(inputValues[2], "H:m:s", null);
            Latitude = double.Parse(inputValues[3]);
            Longitude = double.Parse(inputValues[4]);
            Elevation = double.Parse(inputValues[5]);
        }
    }
}
