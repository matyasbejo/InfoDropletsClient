using InfoDroplets.Models;
using InfoDroplets.Utils.Enums;
using InfoDroplets.Utils.Interfaces;
using InfoDroplets.Utils.SerialCommunication;

namespace InfoDroplets.Logic
{
    public interface IDropletLogic
    {
        event CommandGeneratedEventHandler CommandGenerated;

        void Create(Droplet item);
        void Delete(Droplet item);
        TrackingEntry GetLatestEntry(int dropletId);
        DropletElevationTrend GetElevationTrend(List<TrackingEntry> trackingEntries);
        double GetSpeedKmH(List<TrackingEntry> trackingEntries);
        Droplet Read(int id);
        IQueryable<Droplet> ReadAll();
        IQueryable<int> ReadAllIds();
        void SendCommand(int dropletId, RadioCommand commandType);
        void SendCommand(string input);
        void Update(Droplet item);
        void UpdateDropletStatus(int id, IGpsPos? gnuPos = null);
    }
}