using CommunityToolkit.Mvvm.Messaging;
using InfoDroplets.Models;
using InfoDroplets.Repository;

namespace InfoDroplets.Logic
{
    public class TrackingEntryLogic : ITrackingEntryLogic
    {
        IRepository<TrackingEntry> repo;
        IMessenger? messenger;

        public TrackingEntryLogic(IMessenger messenger, IRepository<TrackingEntry> teRepo)
        {
            this.repo = teRepo;
            this.messenger = messenger;
        }
        
        public TrackingEntryLogic(IRepository<TrackingEntry> teRepo)
        {
            this.repo = teRepo;
        }

        public void Create(string data)
        {
            string allowedCharacters = "0123456789;.:";
            data = data.Trim();
            var argumentList = data.Split(";");
            if (argumentList.Count() != 6 || data.Any(c => !allowedCharacters.Contains(c)))
                throw new ArgumentException($"Input error: {data}");

            int LogEntryDropletId = int.Parse(argumentList[0]); 
            bool isFirstDropletInCollection = !ReadAll().Any(d => d.DropletId == LogEntryDropletId);

            repo.Create(new TrackingEntry(data));
            if (isFirstDropletInCollection)
                throw new NullReferenceException($"Droplet {LogEntryDropletId} does not exist");
        }

        protected void Create(TrackingEntry item)
        {
            repo.Create(item);
        }

        public void Delete(TrackingEntry item)
        {
            repo.Delete(item);
        }

        public TrackingEntry Read(int id)
        {
            var DataEntry = repo.Read(id);
            if (DataEntry == null)
                throw new ArgumentException("TrackingEntry doesn't exist");

            return DataEntry;
        }

        public IQueryable<TrackingEntry> ReadAll()
        {
            return repo.ReadAll();
        }

        public void Update(TrackingEntry item)
        {
            repo.Update(item);
        }
    }
}
