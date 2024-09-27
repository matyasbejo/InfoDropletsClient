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
    }

    public ClientDbContext()
    {
        this.Database.EnsureCreated();
    }
}
