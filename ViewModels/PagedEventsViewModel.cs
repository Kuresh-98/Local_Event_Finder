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

    public string Sort { get; set; } = "startAsc"; // startAsc|startDesc|titleAsc|titleDesc

    public string BuildQuery(int page) => $"text={Uri.EscapeDataString(SearchText ?? string.Empty)}&city={Uri.EscapeDataString(City ?? string.Empty)}&category={Uri.EscapeDataString(Category ?? string.Empty)}&from={(From?.ToString("yyyy-MM-dd") ?? string.Empty)}&to={(To?.ToString("yyyy-MM-dd") ?? string.Empty)}&sort={Sort}&page={page}";
}
