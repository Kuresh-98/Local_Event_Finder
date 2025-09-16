using Local_Event_Finder.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Local_Event_Finder.Data;

public class AppDbContext : IdentityDbContext // include Identity tables
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Event> Events => Set<Event>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // identity config
        modelBuilder.Entity<Event>().HasIndex(e => e.StartUtc);
        modelBuilder.Entity<Event>().HasIndex(e => new { e.City, e.Category });
    }
}
