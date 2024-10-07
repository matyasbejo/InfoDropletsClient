using InfoDroplets.Models;
using InfoDroplets.Repository;

namespace InfoDroplets.Logic
{
    public class DropletLogic
    {
        IRepository<Droplet> repo;

        public DropletLogic(IRepository<Droplet> repo)
        {
            this.repo = repo;
        }

        #region CRUD
        public void Create(Droplet item)
        {
           repo.Create(item);
        }

        public void Delete(Droplet item)
        {
            repo.Delete(item);
        }

        public Droplet Read(int id)
        {
            var DataEntry = repo.Read(id);
            if (DataEntry == null)
                throw new ArgumentException("Droplet doesn't exist");

            return DataEntry;
        }

        public IQueryable<Droplet> ReadAll()
        {
            return repo.ReadAll();
        }

        public void Update(Droplet item)
        {
            repo.Update(item);
        }
        #endregion

        #region Non CRUD
        public TrackingEntry GetLastData(int dropletId)
        {
            var lastMesurement = this.Read(dropletId).Measurements?.LastOrDefault();
            if (lastMesurement == null)
                throw new ArgumentNullException("Droplet has no data");
            else return lastMesurement;
        }

        public List<TrackingEntry> GetLastDataPair(int dropletId)
        {
            var lastMesurement = this.Read(dropletId).Measurements?.TakeLast(2);
            if (lastMesurement == null)
                throw new ArgumentNullException("Droplet has no data");
            else return lastMesurement.ToList();
        }

        public GpsPos GetPosition(int dropletId)
        {
            var data = this.GetLastData(dropletId);
            return new GpsPos(data.Latitude, data.Longitude, data.Elevation);
        }

        public double GetDistanceFromGnu(GpsPos gnuPos)
        {
            var dropletPos = GetLastData(9);
            double DistanceSphere = HaversineDistance(dropletPos, gnuPos);
            double AltitudeDelta = dropletPos.Elevation - gnuPos.Elevation;
            return Math.Sqrt(Math.Pow(DistanceSphere, 2) + Math.Pow(AltitudeDelta, 2));

            double HaversineDistance(GpsPos pos1, GpsPos pos2)
            {
                double earthRadius = 6378;
                double degreesToRadians = 0.0174532925;

                var latRad = (pos2.Latitude - pos1.Latitude) * degreesToRadians;
                var lngRad = (pos2.Longitude - pos1.Longitude) * degreesToRadians;
                var h1 = Math.Sin(latRad / 2) * Math.Sin(latRad / 2) +
                              Math.Cos(pos1.Latitude * degreesToRadians) * Math.Cos(pos2.Latitude * degreesToRadians) *
                              Math.Sin(lngRad / 2) * Math.Sin(lngRad / 2);
                var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));

                return earthRadius * h2;
            }
        }
        #endregion
    }
}
