using Local_Event_Finder.Data;
using Local_Event_Finder.Models;
using Microsoft.EntityFrameworkCore;

namespace Local_Event_Finder.Services;

public class DbEventService : IEventService
{
    private readonly AppDbContext _db;
    public DbEventService(AppDbContext db) { _db = db; }

    public IEnumerable<Event> GetAll() => _db.Events.AsNoTracking().OrderBy(e => e.StartUtc).ToList();

    public Event? GetById(int id) => _db.Events.AsNoTracking().FirstOrDefault(e => e.Id == id);

    public IEnumerable<Event> Search(string? text, string? city, string? category, DateTime? from, DateTime? to)
    {
        var query = _db.Events.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(text))
        {
            text = text.ToLower();
            query = query.Where(e => e.Title.ToLower().Contains(text) || e.Description.ToLower().Contains(text));
        }
        if (!string.IsNullOrWhiteSpace(city))
        {
            city = city.ToLower();
            query = query.Where(e => e.City.ToLower() == city);
        }
        if (!string.IsNullOrWhiteSpace(category))
        {
            category = category.ToLower();
            query = query.Where(e => e.Category.ToLower() == category);
        }
        if (from.HasValue) query = query.Where(e => e.StartUtc >= from.Value);
        if (to.HasValue) query = query.Where(e => e.StartUtc <= to.Value);
        return query.OrderBy(e => e.StartUtc).ToList();
    }

    // Admin-only operations (kept for future use)
    public Event Add(Event ev)
    {
        _db.Events.Add(ev); _db.SaveChanges(); return ev;
    }
    public Event? Update(Event ev)
    {
        var tracked = _db.Events.FirstOrDefault(e => e.Id == ev.Id);
        if (tracked == null) return null;
        tracked.Title = ev.Title; tracked.Description = ev.Description; tracked.StartUtc = ev.StartUtc; tracked.EndUtc = ev.EndUtc;
        tracked.Category = ev.Category; tracked.City = ev.City; tracked.Venue = ev.Venue; tracked.IsFree = ev.IsFree; tracked.Organizer = ev.Organizer; tracked.ExternalUrl = ev.ExternalUrl;
        _db.SaveChanges();
        return tracked;
    }
    public bool Delete(int id)
    {
        var tracked = _db.Events.FirstOrDefault(e => e.Id == id);
        if (tracked == null) return false; _db.Events.Remove(tracked); _db.SaveChanges(); return true;
    }
}
