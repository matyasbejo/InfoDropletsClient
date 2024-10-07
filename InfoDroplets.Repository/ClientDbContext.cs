using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using InfoDroplets.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoDroplets.Repository;

public class ClientDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<TrackingEntry> TrackingEntrySet { get; set; }
    public DbSet<Droplet> DropletSet { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    { 
        if (!optionsBuilder.IsConfigured)
        {
            string conn = "DropletDb";
            optionsBuilder.UseInMemoryDatabase(conn).UseLazyLoadingProxies();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TrackingEntry>(dropletData => dropletData
        .HasOne(dropletData => dropletData.Droplet)
        .WithMany(dropletInfo => dropletInfo.Measurements)
        .HasForeignKey(dropletData => dropletData.DropletId)
        .OnDelete(DeleteBehavior.Restrict));

        modelBuilder.Entity<Droplet>().HasData(new Droplet(9));
        modelBuilder.Entity<TrackingEntry>().HasData(new TrackingEntry(1,9, 3, 10, 10, 20, new DateTime(2002, 12, 06)));
        modelBuilder.Entity<TrackingEntry>().HasData(new TrackingEntry(2,9, 4, 20, 21, 300, new DateTime(2002, 12, 07)));
        modelBuilder.Entity<TrackingEntry>().HasData(new TrackingEntry(3,9, 4, 21, -10, 30, new DateTime(2002, 12, 08)));
    }

    public ClientDbContext()
    {
        this.Database.EnsureCreated();
    }
}
