﻿using InfoDroplets.Models;
using InfoDroplets.Repository;

namespace InfoDroplets.Logic
{
    public class TrackingEntryLogic
    {
        IRepository<TrackingEntry> repo;

        public TrackingEntryLogic(IRepository<TrackingEntry> repo)
        {
            this.repo = repo;
        }

        public void Create(string data)
        {
            string allowedCharacters = "0123456789;.:";
            data = data.Trim();
            if (data.Split(";").Count() != 6 || data.Any(c => !allowedCharacters.Contains(c)))
                throw new ArgumentException($"Input error: {data}");

            repo.Create(new TrackingEntry(data));
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
