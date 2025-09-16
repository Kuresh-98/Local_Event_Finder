using Local_Event_Finder.Models;

namespace Local_Event_Finder.ViewModels;

public class PagedEventsViewModel
{
    public IEnumerable<Event> Events { get; set; } = Enumerable.Empty<Event>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

    public string? SearchText { get; set; }
    public string? City { get; set; }
    public string? Category { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }

    // Location-based search parameters
    public double? UserLat { get; set; }
    public double? UserLon { get; set; }
    public double Radius { get; set; } = 10; // Default 10km radius

    public string Sort { get; set; } = "startAsc"; // startAsc|startDesc|titleAsc|titleDesc|distanceAsc|distanceDesc

    public string BuildQuery(int page) => $"text={Uri.EscapeDataString(SearchText ?? string.Empty)}&city={Uri.EscapeDataString(City ?? string.Empty)}&category={Uri.EscapeDataString(Category ?? string.Empty)}&from={(From?.ToString("yyyy-MM-dd") ?? string.Empty)}&to={(To?.ToString("yyyy-MM-dd") ?? string.Empty)}&userLat={UserLat}&userLon={UserLon}&radius={Radius}&sort={Sort}&page={page}";
}
