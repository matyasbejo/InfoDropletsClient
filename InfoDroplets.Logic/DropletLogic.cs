using CommunityToolkit.Mvvm.Messaging;
using InfoDroplets.Models;
using InfoDroplets.Repository;
using InfoDroplets.Utils.Enums;
using InfoDroplets.Utils.Interfaces;
using InfoDroplets.Utils.SerialCommunication;
using System.Net;
using System.Timers;

namespace InfoDroplets.Logic
{
    public class DropletLogic : IDropletLogic
    {
        IRepository<Droplet> dropletRepo;
        IMessenger? messenger;

        public event CommandGeneratedEventHandler CommandGenerated;
        public DropletLogic(IMessenger messenger, IRepository<Droplet> repository)
        {
            this.messenger = messenger;
            dropletRepo = repository;
        }

        public DropletLogic(IRepository<Droplet> repository)
        {
            dropletRepo = repository;
        }

        #region CRUD
        public void Create(Droplet item)
        {
            var d = dropletRepo.ReadAll();
            if (dropletRepo.ReadAll().FirstOrDefault(d => d.Id == item.Id) == null)
                dropletRepo.Create(item);
            else 
                throw new InvalidOperationException($"Droplet already exist with id {item.Id}.");
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

        public void UpdateDropletStatus(int id, IGpsPos? referencePos = null)
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
                    referencePos = new GpsPos(46.180182, 19.010954, 0.112); //Baja Observatory of the University of Szeged coordinates

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
        public static DropletElevationTrend GetElevationTrend(List<TrackingEntry> trackingEntries, double thresholdKm = 0.02)
        {
            double elevationDelta = trackingEntries.Last().Elevation - trackingEntries.First().Elevation;

            if(Math.Abs(elevationDelta) < thresholdKm)
                return DropletElevationTrend.Stationary;
            else if(elevationDelta > 0)
                return DropletElevationTrend.Rising;
            else 
                return DropletElevationTrend.Descending;
        }  
        public static double GetSpeedKmH(List<TrackingEntry> trackingEntries)
        {
            if (trackingEntries.Count == 1)
                return 0;

            TrackingEntry pos1 = trackingEntries.First();
            TrackingEntry pos2 = trackingEntries.Last();
            double DistanceKmDelta = Distance3DKm(pos1, pos2);
            double ElapsedTimeInHours = (pos2.Time - pos1.Time).TotalHours;
            return Math.Round(DistanceKmDelta / ElapsedTimeInHours, 2);
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
        public static string GenerateCommand(int? dropletId, RadioCommand? command)
        {
            if (dropletId == null || command == null)
                throw new ArgumentNullException("Input was null");

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
        
        public static DropletDirection? GetDirection(List<TrackingEntry> last5entires, double threshold = 0.00018)
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
                _ => DropletDirection.Stationary
            };
        }
        
        public static double Distance2DHaversineKm(IGpsPos pos1, IGpsPos pos2)
        {
            double dLat = (Math.PI / 180) * (pos2.Latitude - pos1.Latitude);
            double dLon = (Math.PI / 180) * (pos2.Longitude - pos1.Longitude);

            double lat1Radians = (Math.PI / 180) * (pos1.Latitude);
            double lat2Radians = (Math.PI / 180) * (pos2.Latitude);

            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Pow(Math.Sin(dLon / 2), 2) *
                       Math.Cos(lat1Radians) * Math.Cos(lat2Radians);

            double rad = 6371;
            double c = 2 * Math.Asin(Math.Sqrt(a));
            return Math.Round(rad * c,4);
        }

        public static double Distance3DKm(IGpsPos pos1, IGpsPos pos2)
        {
            double distanceHaversine = Distance2DHaversineKm(pos1, pos2);
            double deltaHkm = Math.Abs(pos2.Elevation - pos1.Elevation);
            var result = Math.Round(Math.Sqrt(distanceHaversine*distanceHaversine + deltaHkm*deltaHkm),4);
            return result;
        }

        #endregion
    }
}