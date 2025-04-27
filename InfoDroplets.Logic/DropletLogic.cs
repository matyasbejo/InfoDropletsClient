using CommunityToolkit.Mvvm.Messaging;
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
        IRepository<Droplet> dropletRepo;
        IMessenger messenger;

        public event CommandGeneratedEventHandler CommandGenerated;
        public DropletLogic(IMessenger messenger, IRepository<Droplet> repository)
        {
            this.messenger = messenger;
            dropletRepo = repository;
        }

        #region CRUD
        public void Create(Droplet item)
        {
            dropletRepo.Create(item);
        }

        public void Delete(Droplet item)
        {
            dropletRepo.Delete(item);
        }

        public Droplet Read(int id)
        {
            var DataEntry = dropletRepo.Read(id);
            if (DataEntry == null)
                throw new ArgumentException("Droplet doesn't exist");

            return DataEntry;
        }

        public IQueryable<int> ReadAllIds()
        {
            return dropletRepo.ReadAll().Select(d => d.Id);
        }

        public IQueryable<Droplet> ReadAll()
        {
            return dropletRepo.ReadAll();
        }

        public void Update(Droplet item)
        {
            dropletRepo.Update(item);
        }

        public void UpdateDropletStatus(int id, IGpsPos referencePos = null)
        {
            Droplet droplet = Read(id);
            try
            {
                var last5entires = GetLastXEntries(droplet.Id, 5);
                droplet.LastData = GetLatestEntry(id);
                droplet.ElevationTrend = GetElevationTrend(last5entires);
                droplet.Direction = GetDirection(last5entires);
                droplet.SpeedKmH = GetSpeedKmH(last5entires);
                droplet.LastUpdated = droplet.LastData.Time;
                if (referencePos == null)
                    referencePos = new GpsPos(46.180182, 19.010954, 112); //Baja Observatory of the University of Szeged coordinates

                IGpsPos dropletPos = droplet.Measurements.Last();
                droplet.DistanceFromGNU2D = Distance2DHaversineKm(dropletPos, referencePos);
                droplet.DistanceFromGNU3D = Distance3DKm(dropletPos, referencePos);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }

            Update(droplet);
        }

        #endregion

        #region Non CRUD
        public TrackingEntry GetLatestEntry(int dropletId)
        {
            var lastMesurement = Read(dropletId).Measurements?.LastOrDefault();
            if (lastMesurement == null)
                throw new ArgumentNullException("Droplet has no data");
            else return lastMesurement;
        }
        public DropletElevationTrend GetElevationTrend(List<TrackingEntry> trackingEntries)
        {
            if (trackingEntries.First().Elevation < trackingEntries.Last().Elevation)
            {
                return DropletElevationTrend.Rising;
            }
            else if (trackingEntries.First().Elevation > trackingEntries.Last().Elevation)
            {
                return DropletElevationTrend.Falling;
            }
            return DropletElevationTrend.Stationary;
        }  
        double GetSpeedKmH(List<TrackingEntry> trackingEntries)
        {
            TrackingEntry pos1 = trackingEntries.First();
            TrackingEntry pos2 = trackingEntries.Last();
            var DistanceMetersDelta = Distance2DHaversineKm(pos1, pos2);
            var ElapsedSeconds = (pos2.Time - pos1.Time).TotalHours;
            return Math.Round(DistanceMetersDelta / ElapsedSeconds, 2);
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

                case RadioCommand.GetFileVersion:
                    return $"V{dropletId}";

                case RadioCommand.Ping:
                    return $"P{dropletId}";

                default:
                    throw new NotImplementedException("Command unknown");
            }
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
        private DropletDirection? GetDirection(List<TrackingEntry> last5entires, double threshold = 0.0001)
        {
            TrackingEntry firstEntry = last5entires.First();
            TrackingEntry lastEntry = last5entires.Last();

            int latDir = Math.Abs(lastEntry.Latitude - firstEntry.Latitude) > threshold ? (lastEntry.Latitude > firstEntry.Latitude ? 1 : -1) : 0;
            int lonDir = Math.Abs(lastEntry.Longitude - firstEntry.Longitude) > threshold ? (lastEntry.Longitude > firstEntry.Longitude ? 1 : -1) : 0;

            return (latDir, lonDir) switch
            {
                (1, 0) => DropletDirection.North,
                (1, 1) => DropletDirection.NorthEast,
                (0, 1) => DropletDirection.East,
                (-1, 1) => DropletDirection.SouthEast,
                (-1, 0) => DropletDirection.South,
                (-1, -1) => DropletDirection.SouthWest,
                (0, -1) => DropletDirection.West,
                (1, -1) => DropletDirection.NorthWest,
                _ => DropletDirection.None
            };
        }

        public static double Distance2DHaversineKm(IGpsPos pos1, IGpsPos pos2)
        {
            double earthRadius = 6378;
            double degreesToRadians = 0.0174532925;

            var latRad = Math.Abs(pos2.Latitude - pos1.Latitude) * degreesToRadians;
            var lngRad = Math.Abs(pos2.Longitude - pos1.Longitude) * degreesToRadians;
            var h1 = Math.Sin(latRad / 2) * Math.Sin(latRad / 2) +
                          Math.Cos(pos1.Latitude * degreesToRadians) * Math.Cos(pos2.Latitude * degreesToRadians) *
                          Math.Sin(lngRad / 2) * Math.Sin(lngRad / 2);
            var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));

            return Math.Round(earthRadius * h2,4);
        }

        public static double Distance3DKm(IGpsPos pos1, IGpsPos pos2)
        {
            double distanceHaversine = Distance2DHaversineKm(pos1, pos2);
            double deltaHkm = Math.Abs(pos2.Elevation/1000 - pos1.Elevation/1000);
            return Math.Round(Math.Sqrt(distanceHaversine*distanceHaversine + deltaHkm*deltaHkm),4);
        }

        #endregion
    }
}