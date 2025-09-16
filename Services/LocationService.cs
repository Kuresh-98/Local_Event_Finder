using Local_Event_Finder.Models;

namespace Local_Event_Finder.Services;

public interface ILocationService
{
    double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
    double CalculateDistance(Event event1, Event event2);
    double CalculateDistance(double userLat, double userLon, Event eventItem);
    IEnumerable<EventWithDistance> GetEventsWithinRadius(IEnumerable<Event> events, double userLat, double userLon, double radiusKm);
}

public class LocationService : ILocationService
{
    private const double EarthRadiusKm = 6371.0; // Earth's radius in kilometers

    /// <summary>
    /// Calculate distance between two points using Haversine formula
    /// </summary>
    /// <param name="lat1">Latitude of first point</param>
    /// <param name="lon1">Longitude of first point</param>
    /// <param name="lat2">Latitude of second point</param>
    /// <param name="lon2">Longitude of second point</param>
    /// <returns>Distance in kilometers</returns>
    public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Convert degrees to radians
        var lat1Rad = DegreesToRadians(lat1);
        var lon1Rad = DegreesToRadians(lon1);
        var lat2Rad = DegreesToRadians(lat2);
        var lon2Rad = DegreesToRadians(lon2);

        // Haversine formula
        var dLat = lat2Rad - lat1Rad;
        var dLon = lon2Rad - lon1Rad;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    /// <summary>
    /// Calculate distance between two events
    /// </summary>
    public double CalculateDistance(Event event1, Event event2)
    {
        if (!event1.Latitude.HasValue || !event1.Longitude.HasValue ||
            !event2.Latitude.HasValue || !event2.Longitude.HasValue)
        {
            throw new ArgumentException("Both events must have valid coordinates");
        }

        return CalculateDistance(
            event1.Latitude.Value, event1.Longitude.Value,
            event2.Latitude.Value, event2.Longitude.Value);
    }

    /// <summary>
    /// Calculate distance from user location to an event
    /// </summary>
    public double CalculateDistance(double userLat, double userLon, Event eventItem)
    {
        if (!eventItem.Latitude.HasValue || !eventItem.Longitude.HasValue)
        {
            throw new ArgumentException("Event must have valid coordinates");
        }

        return CalculateDistance(userLat, userLon, eventItem.Latitude.Value, eventItem.Longitude.Value);
    }

    /// <summary>
    /// Get events within specified radius from user location
    /// </summary>
    /// <param name="events">List of events to filter</param>
    /// <param name="userLat">User's latitude</param>
    /// <param name="userLon">User's longitude</param>
    /// <param name="radiusKm">Radius in kilometers</param>
    /// <returns>Events within radius with calculated distances</returns>
    public IEnumerable<EventWithDistance> GetEventsWithinRadius(
        IEnumerable<Event> events, 
        double userLat, 
        double userLon, 
        double radiusKm)
    {
        var eventsWithDistance = new List<EventWithDistance>();

        foreach (var eventItem in events)
        {
            if (eventItem.Latitude.HasValue && eventItem.Longitude.HasValue)
            {
                var distance = CalculateDistance(userLat, userLon, eventItem);
                
                if (distance <= radiusKm)
                {
                    eventsWithDistance.Add(new EventWithDistance
                    {
                        Event = eventItem,
                        DistanceKm = distance
                    });
                }
            }
        }

        // Sort by distance (closest first)
        return eventsWithDistance.OrderBy(x => x.DistanceKm);
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }
}

/// <summary>
/// Helper class to hold event with calculated distance
/// </summary>
public class EventWithDistance
{
    public Event Event { get; set; } = null!;
    public double DistanceKm { get; set; }
    
    public string DistanceDisplay => DistanceKm < 1 
        ? $"{Math.Round(DistanceKm * 1000)}m" 
        : $"{Math.Round(DistanceKm, 1)}km";
}
