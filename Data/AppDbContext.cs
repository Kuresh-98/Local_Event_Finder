using Local_Event_Finder.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Local_Event_Finder.Data;

public class AppDbContext : IdentityDbContext // include Identity tables
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventInterest> EventInterests => Set<EventInterest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // identity config
        modelBuilder.Entity<Event>().HasIndex(e => e.StartUtc);
        modelBuilder.Entity<Event>().HasIndex(e => new { e.City, e.Category });
        
        // Configure EventInterest relationships
        modelBuilder.Entity<EventInterest>()
            .HasOne(ei => ei.Event)
            .WithMany(e => e.EventInterests)
            .HasForeignKey(ei => ei.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ensure one interest per user per event
        modelBuilder.Entity<EventInterest>()
            .HasIndex(ei => new { ei.EventId, ei.UserId })
            .IsUnique();
    }
}
