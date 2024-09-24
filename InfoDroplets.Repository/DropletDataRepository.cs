using InfoDroplets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoDroplets.Repository
{
    public class DropletDataRepository : Repository<DataEntry>, IRepository<DataEntry>
    {
        public DropletDataRepository(ClientDbContext ctx) : base(ctx)
        {
        }

        public override DataEntry Read(int id)
        {
            return ctx.DropletDataSet.FirstOrDefault(d => d.Id == id);
        }

        public override void Update(DataEntry item)
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
