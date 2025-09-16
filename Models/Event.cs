namespace Local_Event_Finder.Models;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public string Category { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
    public bool IsFree { get; set; }
    public string Organizer { get; set; } = string.Empty;
    public string? ExternalUrl { get; set; }
}
