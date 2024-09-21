using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using InfoDroplets.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoDropletsClient.Repository;

public class DropletDataDbContext : DbContext
{
    public DbSet<DropletData> DropletDatas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string conn = @"";
            optionsBuilder.UseLazyLoadingProxies()
                .UseInMemoryDatabase(conn);
        }
    }
}
