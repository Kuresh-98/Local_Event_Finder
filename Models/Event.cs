using System.ComponentModel.DataAnnotations;

namespace Local_Event_Finder.Models;

public class Event : IValidatableObject
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Start (UTC)")]
    [DataType(DataType.DateTime)]
    public DateTime StartUtc { get; set; }

    [Display(Name = "End (UTC)")]
    [DataType(DataType.DateTime)]
    public DateTime EndUtc { get; set; }

    [Required, StringLength(60)]
    public string Category { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string City { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Venue { get; set; } = string.Empty;

    [Display(Name = "Free Event?")]
    public bool IsFree { get; set; }

    [Required, StringLength(120)]
    public string Organizer { get; set; } = string.Empty;

    [Url]
    [Display(Name = "External URL")]
    public string? ExternalUrl { get; set; }

    // Cross-field validation
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndUtc < StartUtc)
        {
            yield return new ValidationResult("End time must be after start time", new[] { nameof(EndUtc) });
        }
    }
}
