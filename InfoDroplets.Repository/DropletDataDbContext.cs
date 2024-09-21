using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using InfoDroplets.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoDroplets.Repository;

internal class DropletDataDbContext : DbContext
{
    internal DbSet<DropletData> DropletDatas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string conn = @"";
            optionsBuilder.UseLazyLoadingProxies()
                .UseSqlServer(conn);
        }
    }
}
