using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Range(0,32)]
        public int SatelliteCount { get; private set; }

        [Required]
        [Range(0,180)]
        public double Longitude { get; private set; }

        [Required]
        [Range(0, 180)]
        public double Latitude { get; private set; }

        [Required]
        [Range(0,50000)]
        public double Height { get; private set; }

        [Required]
        public DateTime Time { get; private set; }
        #endregion

        public DropletData(int dropletId, int satelliteCount, double longitude, double latitude, double height, DateTime time)
        {
            DropletId = dropletId;
            SatelliteCount = satelliteCount;
            Longitude = longitude;
            Latitude = latitude;
            Height = height;
            Time = time;
        }
    }
}
