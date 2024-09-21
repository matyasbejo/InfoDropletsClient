namespace InfoDroplets.Models
{
    public class DropletData
    {
        #region variables
        private readonly int dropletId;
        private readonly int satelliteCount;
        private readonly double longitude;
        private readonly double latitude;
        private readonly double height;
        private readonly DateTime time;
        #endregion

        public DropletData(int dropletId, int satelliteCount, double longitude, double latitude, double height, DateTime time)
        {
            this.dropletId = dropletId;
            this.satelliteCount = satelliteCount;
            this.longitude = longitude;
            this.latitude = latitude;
            this.height = height;
            this.time = time;
        }
    }
}
