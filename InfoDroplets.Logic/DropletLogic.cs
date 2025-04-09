using InfoDroplets.Models;
using InfoDroplets.Repository;
using InfoDroplets.Utils.Enums;
using InfoDroplets.Utils.Interfaces;
using InfoDroplets.Utils.SerialCommunication;
using System.Timers;

namespace InfoDroplets.Logic
{
    public class DropletLogic : IDropletLogic
    {
        IRepository<Droplet> repo;

        public event CommandGeneratedEventHandler CommandGenerated;
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

        public void UpdateDropletStatus(int id, IGpsPos? gnuPos = null)
        {
            Droplet droplet = Read(id);
            try
            {
                if (gnuPos != null) droplet.DistanceFromGNU = GetDistanceFromGnu(id, gnuPos);
                droplet.MovementStatus = GetMovementStatus(GetLastXEntries(droplet.Id, 5));
                droplet.SpeedKmH = GetSpeed(GetLastXEntries(droplet.Id, 5));
                droplet.LastData = GetLatestEntry(id);
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message + e.StackTrace);
            } 

            Update(droplet);
        }
        #endregion

        #region Non CRUD
        public DropletMovementStatus GetMovementStatus(List<TrackingEntry> trackingEntries)
        {
            if (trackingEntries.First().Elevation < trackingEntries.Last().Elevation)
            {
                return DropletMovementStatus.Rising;
            }
            else if (trackingEntries.First().Elevation > trackingEntries.Last().Elevation)
            {
                return DropletMovementStatus.Falling;
            }
            return DropletMovementStatus.Stationary;
        }

        public int GetSpeed(List<TrackingEntry> trackingEntries)
        {
            TrackingEntry pos1 = trackingEntries.First();
            TrackingEntry pos2 = trackingEntries.Last();
            return 69; //TODO
        }
        List<TrackingEntry> GetLastXEntries(int dropletId, int maxEntryCount, int minEntryCount = 2)
        {
            var lastMesurements = this.Read(dropletId).Measurements?.TakeLast(maxEntryCount);

            if (lastMesurements == null || lastMesurements.Count() == 0)
                throw new ArgumentNullException("Droplet has no data");
            else if (lastMesurements.Count() < minEntryCount)
                throw new ArgumentException("Droplet has not enough data");
            else return lastMesurements.ToList();
        }

        public virtual void SendCommand(int dropletId, RadioCommand commandType)
        {
            string command = GenerateCommand(dropletId, commandType);

            CommandGenerated?.Invoke(this, new CommandEventArgs(command));
        }
        public virtual void SendCommand(string input)
        {
            CommandGenerated?.Invoke(this, new CommandEventArgs(input));
        }
        string GenerateCommand(int dropletId, RadioCommand command)
        {
            switch (command)
            {
                case RadioCommand.FullReset:
                    return $"F{dropletId}";

                case RadioCommand.GpsReset:
                    return $"R{dropletId}";

                case RadioCommand.GetVersion:
                    return $"V{dropletId}";

                case RadioCommand.Ping:
                    return $"P{dropletId}";

                default:
                    throw new NotImplementedException("Command unknown");
            }
        }
        double GetDistanceFromGnu(int id, IGpsPos gnuPos)
        {
            Droplet droplet = Read(id);
            GpsPos dropletPos = new GpsPos(droplet.LastData.Latitude, droplet.LastData.Longitude, droplet.LastData.Elevation);

            double DistanceSphere = HaversineDistance(dropletPos, gnuPos);
            double AltitudeDelta = dropletPos.Elevation - gnuPos.Elevation;
            return Math.Sqrt(Math.Pow(DistanceSphere, 2) + Math.Pow(AltitudeDelta, 2));
        }
        public TrackingEntry GetLatestEntry(int dropletId)
        {
            var lastMesurement = Read(dropletId).Measurements?.LastOrDefault();
            if (lastMesurement == null)
                throw new ArgumentNullException("Droplet has no data");
            else return lastMesurement;
        }
        static double HaversineDistance(IGpsPos pos1, IGpsPos pos2)
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
        #endregion
    }
}