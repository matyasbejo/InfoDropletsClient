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
            return this.Read(dropletId).Measurements.LastOrDefault();
        }

        public GpsPos GetPosition(int dropletId)
        {
            var data = this.GetLastData(dropletId);
            return new GpsPos(data.Latitude, data.Longitude, data.Elevation);
        }
        #endregion
    }
}
