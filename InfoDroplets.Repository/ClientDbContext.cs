using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using InfoDroplets.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoDroplets.Repository;

public class ClientDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<DropletData> DropletDataSet { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string conn = "DropletDb";
            optionsBuilder.UseInMemoryDatabase(conn);
        }
    }

    public ClientDbContext()
    {
        this.Database.EnsureCreated();
    }
}
