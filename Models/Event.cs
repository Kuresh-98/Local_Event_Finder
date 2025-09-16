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

    [Required, StringLength(200)]
    [Display(Name = "Full Address")]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "Latitude")]
    public double? Latitude { get; set; }

    [Display(Name = "Longitude")]
    public double? Longitude { get; set; }

    [Display(Name = "Total Seats")]
    [Range(1, int.MaxValue, ErrorMessage = "Total seats must be at least 1")]
    public int? TotalSeats { get; set; }

    [Display(Name = "Available Seats")]
    public int AvailableSeats { get; set; }

    [Display(Name = "Event Cancelled")]
    public bool IsCancelled { get; set; }

    // Navigation property for user interests
    public ICollection<EventInterest> EventInterests { get; set; } = new List<EventInterest>();

    // Cross-field validation
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndUtc < StartUtc)
        {
            yield return new ValidationResult("End time must be after start time", new[] { nameof(EndUtc) });
        }

        // Validate coordinates if provided
        if (Latitude.HasValue && (Latitude < -90 || Latitude > 90))
        {
            yield return new ValidationResult("Latitude must be between -90 and 90", new[] { nameof(Latitude) });
        }

        if (Longitude.HasValue && (Longitude < -180 || Longitude > 180))
        {
            yield return new ValidationResult("Longitude must be between -180 and 180", new[] { nameof(Longitude) });
        }

        // Validate seat management
        if (TotalSeats.HasValue && TotalSeats < 1)
        {
            yield return new ValidationResult("Total seats must be at least 1", new[] { nameof(TotalSeats) });
        }

        if (TotalSeats.HasValue && AvailableSeats > TotalSeats)
        {
            yield return new ValidationResult("Available seats cannot exceed total seats", new[] { nameof(AvailableSeats) });
        }

        if (AvailableSeats < 0)
        {
            yield return new ValidationResult("Available seats cannot be negative", new[] { nameof(AvailableSeats) });
        }
    }
}
