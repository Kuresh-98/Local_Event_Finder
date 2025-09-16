using System.ComponentModel.DataAnnotations;

namespace Local_Event_Finder.Models;

public class EventInterest
{
    public int Id { get; set; }

    [Required]
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Display(Name = "Interested At")]
    public DateTime InterestedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Status")]
    public InterestStatus Status { get; set; } = InterestStatus.Interested;
}

public enum InterestStatus
{
    Interested = 1,
    Attending = 2,
    NotAttending = 3
}
