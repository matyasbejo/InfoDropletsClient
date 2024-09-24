using InfoDroplets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoDroplets.Repository
{
    public class DropletInfoRepository : Repository<DropletInfo>, IRepository<DropletInfo>
    {
        public DropletInfoRepository(ClientDbContext ctx) : base(ctx)
        {
        }

        public override DropletInfo Read(int id)
        {
            return ctx.DropletInfoSet.FirstOrDefault(d => d.Id == id);
        }

        public override void Update(DropletInfo item)
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
