using Local_Event_Finder.Models;

namespace Local_Event_Finder.Services;

public interface IEventService
{
    IEnumerable<Event> GetAll();
    IEnumerable<Event> Search(string? text, string? city, string? category, DateTime? from, DateTime? to);
    Event? GetById(int id);
}

public class InMemoryEventService : IEventService
{
    private readonly List<Event> _events = new();
    public InMemoryEventService()
    {
        // Seed a few sample events (UTC times)
        _events.AddRange(new[]
        {
            new Event { Id = 1, Title = "Local Tech Meetup", Description = "Monthly developer meetup.", Category = "Tech", City = "Seattle", Venue = "Community Hall", StartUtc = DateTime.UtcNow.AddDays(2), EndUtc = DateTime.UtcNow.AddDays(2).AddHours(2), Organizer = "DevOrg", IsFree = true },
            new Event { Id = 2, Title = "Jazz Night", Description = "Live jazz music.", Category = "Music", City = "Seattle", Venue = "Downtown Club", StartUtc = DateTime.UtcNow.AddDays(5), EndUtc = DateTime.UtcNow.AddDays(5).AddHours(3), Organizer = "CityMusic", IsFree = false },
            new Event { Id = 3, Title = "Startup Pitch", Description = "Pitch session for startups.", Category = "Business", City = "Portland", Venue = "Innovation Hub", StartUtc = DateTime.UtcNow.AddDays(7), EndUtc = DateTime.UtcNow.AddDays(7).AddHours(1), Organizer = "StartupOrg", IsFree = true }
        });
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
}
