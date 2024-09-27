﻿using InfoDroplets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoDroplets.Repository
{
    public class DropletRepository : Repository<Droplet>, IRepository<Droplet>
    {
        public DropletRepository(ClientDbContext ctx) : base(ctx)
        {
        }

        public override Droplet Read(int id)
        {
            return ctx.DropletSet.FirstOrDefault(d => d.Id == id);
        }

        public override void Update(Droplet item)
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