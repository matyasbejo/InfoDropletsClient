using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoDroplets.Utils.Enums;
using InfoDroplets.Utils.Interfaces;
using System.Text.Json.Serialization;

namespace InfoDroplets.Models
{
    public class Droplet
    {
        #region Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public DropletElevationTrend? ElevationTrend { get; set; }
        public DropletDirection? Direction { get; set; }
        public DropletState? State { get; set; }
        public double? DistanceFromGNU3D { get; set; }
        public double? DistanceFromGNU2D { get; set; }
        public double? SpeedKmH { get; set; }
        public virtual ICollection<TrackingEntry> Measurements { get; set; }
        public virtual TrackingEntry? LastData { get; set; }
        public TimeOnly? LastUpdated { get; set; }

        #endregion

        public Droplet(int id)
        {
            this.Id = id;
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
