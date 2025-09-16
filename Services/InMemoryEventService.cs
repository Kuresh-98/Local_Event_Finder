using Local_Event_Finder.Models;

namespace Local_Event_Finder.Services;

public interface IEventService
{
    IEnumerable<Event> GetAll();
    IEnumerable<Event> Search(string? text, string? city, string? category, DateTime? from, DateTime? to);
    Event? GetById(int id);
    Event Add(Event ev);
    Event? Update(Event ev);
    bool Delete(int id);
    bool ToggleCancellation(int id);
}

// This implementation kept for reference/testing without DB
public class InMemoryEventService : IEventService
{
    private readonly List<Event> _events = new();
    private int _nextId = 1;

    public InMemoryEventService()
    {
        Add(new Event { Title = "Local Tech Meetup", Description = "Monthly developer meetup.", Category = "Tech", City = "Seattle", Venue = "Community Hall", StartUtc = DateTime.UtcNow.AddDays(2), EndUtc = DateTime.UtcNow.AddDays(2).AddHours(2), Organizer = "DevOrg", IsFree = true });
        Add(new Event { Title = "Jazz Night", Description = "Live jazz music.", Category = "Music", City = "Seattle", Venue = "Downtown Club", StartUtc = DateTime.UtcNow.AddDays(5), EndUtc = DateTime.UtcNow.AddDays(5).AddHours(3), Organizer = "CityMusic", IsFree = false });
        Add(new Event { Title = "Startup Pitch", Description = "Pitch session for startups.", Category = "Business", City = "Portland", Venue = "Innovation Hub", StartUtc = DateTime.UtcNow.AddDays(7), EndUtc = DateTime.UtcNow.AddDays(7).AddHours(1), Organizer = "StartupOrg", IsFree = true });
    }

    public IEnumerable<Event> GetAll() => _events.OrderBy(e => e.StartUtc);

    public Event? GetById(int id) => _events.FirstOrDefault(e => e.Id == id);

    public IEnumerable<Event> Search(string? text, string? city, string? category, DateTime? from, DateTime? to)
    {
        var query = _events.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(text))
        {
            text = text.ToLowerInvariant();
            query = query.Where(e => e.Title.ToLower().Contains(text) || e.Description.ToLower().Contains(text));
        }
        if (!string.IsNullOrWhiteSpace(city))
        {
            city = city.ToLowerInvariant();
            query = query.Where(e => e.City.ToLower() == city);
        }
        if (!string.IsNullOrWhiteSpace(category))
        {
            category = category.ToLowerInvariant();
            query = query.Where(e => e.Category.ToLower() == category);
        }
        if (from.HasValue)
        {
            query = query.Where(e => e.StartUtc >= from.Value);
        }
        if (to.HasValue)
        {
            query = query.Where(e => e.StartUtc <= to.Value);
        }
        return query.OrderBy(e => e.StartUtc);
    }

    public Event Add(Event ev)
    {
        ev.Id = _nextId++;
        _events.Add(ev);
        return ev;
    }

    public Event? Update(Event ev)
    {
        var existing = _events.FirstOrDefault(x => x.Id == ev.Id);
        if (existing == null) return null;
        existing.Title = ev.Title;
        existing.Description = ev.Description;
        existing.StartUtc = ev.StartUtc;
        existing.EndUtc = ev.EndUtc;
        existing.Category = ev.Category;
        existing.City = ev.City;
        existing.Venue = ev.Venue;
        existing.IsFree = ev.IsFree;
        existing.Organizer = ev.Organizer;
        existing.ExternalUrl = ev.ExternalUrl;
        return existing;
    }

    public bool Delete(int id)
    {
        var existing = _events.FirstOrDefault(x => x.Id == id);
        if (existing == null) return false;
        _events.Remove(existing);
        return true;
    }

    public bool ToggleCancellation(int id)
    {
        var existing = _events.FirstOrDefault(x => x.Id == id);
        if (existing == null) return false;
        existing.IsCancelled = !existing.IsCancelled;
        return true;
    }
}
