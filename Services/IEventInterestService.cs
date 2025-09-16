using Local_Event_Finder.Models;

namespace Local_Event_Finder.Services;

public interface IEventInterestService
{
    Task<bool> ToggleInterestAsync(int eventId, string userId);
    Task<bool> IsUserInterestedAsync(int eventId, string userId);
    Task<int> GetInterestCountAsync(int eventId);
    Task<List<EventInterest>> GetUserInterestsAsync(string userId);
    Task<List<EventInterest>> GetEventInterestsAsync(int eventId);
    Task SyncAvailableSeatsAsync(int eventId);
    Task<bool> CanUserRegisterAsync(int eventId);
    Task SyncAvailableSeatsAfterAdminChangeAsync(int eventId, int newTotalSeats);
}
