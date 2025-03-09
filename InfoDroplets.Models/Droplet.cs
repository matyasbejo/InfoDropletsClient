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

        [Range(0,5)]
        public DropletVersion Version { get; set; }

        public DropletMovementStatus? MovementStatus { get; set; }
        public double? DistanceFromGNU { get; set; }

        public virtual TrackingEntry? LastData { get; set; }

        public virtual ICollection<TrackingEntry>? Measurements { get; set; }

        #endregion

        public Droplet(int id, DropletVersion version = DropletVersion.Mk3_1)
        {
            this.Id = id;
            this.Version = version;
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
