using Local_Event_Finder.Data;
using Local_Event_Finder.Models;
using Microsoft.EntityFrameworkCore;

namespace Local_Event_Finder.Services;

public class EventInterestService : IEventInterestService
{
    private readonly AppDbContext _context;

    public EventInterestService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ToggleInterestAsync(int eventId, string userId)
    {
        var eventItem = await _context.Events.FindAsync(eventId);
        if (eventItem == null) return false;

        var existingInterest = await _context.EventInterests
            .FirstOrDefaultAsync(ei => ei.EventId == eventId && ei.UserId == userId);

        if (existingInterest != null)
        {
            // Remove interest - increase available seats
            _context.EventInterests.Remove(existingInterest);
            eventItem.AvailableSeats = Math.Min(eventItem.AvailableSeats + 1, eventItem.TotalSeats ?? int.MaxValue);
            await _context.SaveChangesAsync();
            return false; // No longer interested
        }
        else
        {
            // Check if event is cancelled
            if (eventItem.IsCancelled)
            {
                return false; // Cannot register for cancelled events
            }

            // Check if seats are available
            if (eventItem.AvailableSeats <= 0)
            {
                return false; // No seats available
            }

            // Add interest - decrease available seats
            var newInterest = new EventInterest
            {
                EventId = eventId,
                UserId = userId,
                InterestedAt = DateTime.UtcNow,
                Status = InterestStatus.Interested
            };

            _context.EventInterests.Add(newInterest);
            eventItem.AvailableSeats = Math.Max(eventItem.AvailableSeats - 1, 0);
            await _context.SaveChangesAsync();
            return true; // Now interested
        }
    }

    public async Task<bool> IsUserInterestedAsync(int eventId, string userId)
    {
        return await _context.EventInterests
            .AnyAsync(ei => ei.EventId == eventId && ei.UserId == userId);
    }

    public async Task<int> GetInterestCountAsync(int eventId)
    {
        return await _context.EventInterests
            .CountAsync(ei => ei.EventId == eventId);
    }

    public async Task<List<EventInterest>> GetUserInterestsAsync(string userId)
    {
        return await _context.EventInterests
            .Include(ei => ei.Event)
            .Where(ei => ei.UserId == userId)
            .OrderByDescending(ei => ei.InterestedAt)
            .ToListAsync();
    }

    public async Task<List<EventInterest>> GetEventInterestsAsync(int eventId)
    {
        return await _context.EventInterests
            .Where(ei => ei.EventId == eventId)
            .OrderByDescending(ei => ei.InterestedAt)
            .ToListAsync();
    }

    public async Task SyncAvailableSeatsAsync(int eventId)
    {
        var eventItem = await _context.Events.FindAsync(eventId);
        if (eventItem == null || !eventItem.TotalSeats.HasValue) return;

        var reservationCount = await _context.EventInterests
            .CountAsync(ei => ei.EventId == eventId);

        eventItem.AvailableSeats = Math.Max(eventItem.TotalSeats.Value - reservationCount, 0);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CanUserRegisterAsync(int eventId)
    {
        var eventItem = await _context.Events.FindAsync(eventId);
        if (eventItem == null) return false;

        // Cannot register for cancelled events
        if (eventItem.IsCancelled) return false;

        // If no total seats set, allow unlimited registration
        if (!eventItem.TotalSeats.HasValue) return true;

        // Check if seats are available
        return eventItem.AvailableSeats > 0;
    }

    public async Task SyncAvailableSeatsAfterAdminChangeAsync(int eventId, int newTotalSeats)
    {
        var eventItem = await _context.Events.FindAsync(eventId);
        if (eventItem == null) return;

        var reservationCount = await _context.EventInterests
            .CountAsync(ei => ei.EventId == eventId);

        // Set available seats to new total minus existing reservations
        eventItem.AvailableSeats = Math.Max(newTotalSeats - reservationCount, 0);
        await _context.SaveChangesAsync();
    }
}
