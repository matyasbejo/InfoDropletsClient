using CommunityToolkit.Mvvm.ComponentModel;
using InfoDroplets.Utils.Enums;

namespace InfoDroplets.Models
{
    public class DropletViewDetails : ObservableObject
    {
		public GpsPos Position { get; set; }
		public int SatteliteCount { get; set; }
		public double HaversineDistance { get; set; }
		public int Speed { get; set; }
		public DropletMovementStatus MovementStatus { get; set; }
		public DropletVersion DropletVersion { get; set; }
		public DateTime Time { get; set; }
	}
}