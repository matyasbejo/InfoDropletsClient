using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InfoDroplets.Models
{
    public class DropletData
    {
        #region properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DropletId { get; private set; }

        [Required]
        [Range(0, 32)]
        public int SatelliteCount { get; private set; }

        [Required]
        [Range(0, 180)]
        public double Longitude { get; private set; }

        [Required]
        [Range(0, 180)]
        public double Latitude { get; private set; }

        [Required]
        [Range(0, 50000)]
        public double Elevation { get; private set; }

        [Required]
        public DateTime Time { get; private set; }
        #endregion

        public DropletData(int dropletId, int satelliteCount, double longitude, double latitude, double elevation, DateTime time)
        {
            DropletId = dropletId;
            SatelliteCount = satelliteCount;
            Longitude = longitude;
            Latitude = latitude;
            Elevation = elevation;
            Time = time;
        }

        public DropletData(string inputString)
        {
            var inputValues = inputString.Split(";");
            DropletId = int.Parse(inputValues[0]);
            SatelliteCount = int.Parse(inputValues[1]);
            Time = DateTime.ParseExact(inputValues[2], "hh:mm:ss", null);
            Latitude = double.Parse(inputValues[3]); 
            Longitude = double.Parse(inputValues[4]);
            Elevation = double.Parse(inputValues[5]);
        }
    }
}
