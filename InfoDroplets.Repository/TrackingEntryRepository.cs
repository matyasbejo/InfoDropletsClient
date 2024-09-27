using InfoDroplets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoDroplets.Repository
{
    public class TrackingEntryRepository : Repository<TrackingEntry>, IRepository<TrackingEntry>
    {
        public TrackingEntryRepository(ClientDbContext ctx) : base(ctx)
        {
        }

        public override TrackingEntry Read(int id)
        {
            return ctx.TrackingEntrySet.FirstOrDefault(d => d.Id == id);
        }

        public override void Update(TrackingEntry item)
        {
            var oldData = Read(item.Id);
            foreach(var prop in oldData.GetType().GetProperties())
            {
                prop.SetValue(oldData, prop.GetValue(item));
            }
            ctx.SaveChanges();
        }
    }
}
