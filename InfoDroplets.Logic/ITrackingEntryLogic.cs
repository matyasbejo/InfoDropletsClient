using InfoDroplets.Models;

namespace InfoDroplets.Logic
{
    public interface ITrackingEntryLogic
    {
        void Create(string data);
        void Delete(TrackingEntry item);
        TrackingEntry Read(int id);
        IQueryable<TrackingEntry> ReadAll();
        void Update(TrackingEntry item);
    }
}