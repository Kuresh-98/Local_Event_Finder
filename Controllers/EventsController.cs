using Microsoft.AspNetCore.Mvc;
using Local_Event_Finder.Services;
using Local_Event_Finder.Models;
using Local_Event_Finder.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Local_Event_Finder.Controllers;

// Attribute routing to allow cleaner URLs like /Events/2 or /Events/2/anything-based-on-title
[Route("Events")] // base route
public class EventsController : Controller
{
    private readonly IEventService _eventService;
    private readonly ILocationService _locationService;
    private readonly IEventInterestService _interestService;
    
    public EventsController(IEventService eventService, ILocationService locationService, IEventInterestService interestService)
    {
        _eventService = eventService;
        _locationService = locationService;
        _interestService = interestService;
    }

    private void Flash(string message, string type = "success")
    {
        TempData["Flash.Message"] = message;
        TempData["Flash.Type"] = type; // success | danger | info | warning
    }

    // GET /Events
    // GET /Events?city=Seattle&userLat=47.6062&userLon=-122.3321&radius=10
    [HttpGet("")]
    public IActionResult Index(string? text, string? city, string? category, DateTime? from, DateTime? to, 
        double? userLat, double? userLon, double radius = 10, string sort = "startAsc", int page = 1, int pageSize = 9)
    {
        var filtered = _eventService.Search(text, city, category, from, to);
        
        // Apply location-based filtering if coordinates are provided
        if (userLat.HasValue && userLon.HasValue)
        {
            var eventsWithDistance = _locationService.GetEventsWithinRadius(filtered, userLat.Value, userLon.Value, radius);
            filtered = eventsWithDistance.Select(x => x.Event);
            
            // If sorting by distance, use the calculated distances
            if (sort == "distanceAsc")
            {
                filtered = eventsWithDistance.OrderBy(x => x.DistanceKm).Select(x => x.Event);
            }
            else if (sort == "distanceDesc")
            {
                filtered = eventsWithDistance.OrderByDescending(x => x.DistanceKm).Select(x => x.Event);
            }
        }
        else
        {
            // Apply regular sorting if no location search
            filtered = sort switch
            {
                "startDesc" => filtered.OrderByDescending(e => e.StartUtc),
                "titleAsc" => filtered.OrderBy(e => e.Title),
                "titleDesc" => filtered.OrderByDescending(e => e.Title),
                _ => filtered.OrderBy(e => e.StartUtc)
            };
        }

        var total = filtered.Count();
        var items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var vm = new PagedEventsViewModel
        {
            Events = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total,
            SearchText = text,
            City = city,
            Category = category,
            From = from,
            To = to,
            Sort = sort,
            UserLat = userLat,
            UserLon = userLon,
            Radius = radius
        };
        return View(vm);
    }

    // GET /Events/create
    [Authorize(Roles = "Admin")]
    [HttpGet("create")]
    public IActionResult Create()
    {
        var model = new Event
        {
            StartUtc = DateTime.UtcNow.AddDays(1).Date.AddHours(17),
            EndUtc = DateTime.UtcNow.AddDays(1).Date.AddHours(19)
        };
        return View(model);
    }

    // POST /Events/create
    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Event model)
    {
        if (!ModelState.IsValid)
        {
            Flash("Please correct the highlighted errors.", "danger");
            return View(model);
        }

        // Ensure AvailableSeats is set correctly for new events
        if (model.TotalSeats.HasValue && model.AvailableSeats == 0)
        {
            model.AvailableSeats = model.TotalSeats.Value;
        }

        _eventService.Add(model);
        Flash("Event created successfully.");
        // Redirect to details page
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    // GET /Events/edit/5
    [Authorize(Roles = "Admin")]
    [HttpGet("edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var ev = _eventService.GetById(id);
        if (ev == null) return NotFound();
        return View(ev);
    }

    // POST /Events/edit/5
    [Authorize(Roles = "Admin")]
    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Event model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            Flash("Please correct the highlighted errors.", "danger");
            return View(model);
        }

        // Get the original event to check if TotalSeats changed
        var originalEvent = _eventService.GetById(id);
        var totalSeatsChanged = originalEvent?.TotalSeats != model.TotalSeats;

        var updated = _eventService.Update(model);
        if (updated == null) return NotFound();

        // If TotalSeats changed, sync AvailableSeats with existing reservations
        if (totalSeatsChanged && model.TotalSeats.HasValue)
        {
            await _interestService.SyncAvailableSeatsAfterAdminChangeAsync(id, model.TotalSeats.Value);
        }

        Flash("Event updated successfully.");
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    // GET /Events/delete/5 (confirmation)
    [Authorize(Roles = "Admin")]
    [HttpGet("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var ev = _eventService.GetById(id);
        if (ev == null) return NotFound();
        return View(ev);
    }

    // POST /Events/delete/5
    [Authorize(Roles = "Admin")]
    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public IActionResult ConfirmDelete(int id)
    {
        var ok = _eventService.Delete(id);
        if (!ok) return NotFound();
        Flash("Event deleted.", "info");
        return RedirectToAction(nameof(Index));
    }

    // GET /Events/2
    // GET /Events/2/optional-slug-text
    // Also still works: /Events/Details/2 (extra route below)
    [HttpGet("{id:int}")]
    [HttpGet("{id:int}/{*slug}")] // slug is ignored, for SEO-friendly titles
    [HttpGet("Details/{id:int}")] // legacy style
    public IActionResult Details(int id)
    {
        var ev = _eventService.GetById(id);
        if (ev == null) return NotFound();
        return View(ev); 
    }

    // API endpoints for interest tracking
    [HttpPost("ToggleInterest/{id:int}")]
    [Authorize]
    public async Task<IActionResult> ToggleInterest(int id)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "User not authenticated" });
        }

        // Check if user can register
        var canRegister = await _interestService.CanUserRegisterAsync(id);
        var isCurrentlyInterested = await _interestService.IsUserInterestedAsync(id, userId);

        // If not currently interested and no seats available, deny
        if (!isCurrentlyInterested && !canRegister)
        {
            return Json(new { 
                success = false, 
                message = "No seats available for this event",
                canRegister = false
            });
        }

        var isInterested = await _interestService.ToggleInterestAsync(id, userId);
        var interestCount = await _interestService.GetInterestCountAsync(id);
        var eventItem = _eventService.GetById(id);

        return Json(new 
        { 
            success = true, 
            isInterested = isInterested,
            interestCount = interestCount,
            availableSeats = eventItem?.AvailableSeats ?? 0,
            canRegister = eventItem?.AvailableSeats > 0,
            message = isInterested ? "Seat reserved successfully!" : "Seat reservation cancelled!"
        });
    }

    [HttpGet("InterestStatus/{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetInterestStatus(int id)
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "User not authenticated" });
        }

        var isInterested = await _interestService.IsUserInterestedAsync(id, userId);
        var interestCount = await _interestService.GetInterestCountAsync(id);
        var canRegister = await _interestService.CanUserRegisterAsync(id);
        var eventItem = _eventService.GetById(id);

        return Json(new 
        { 
            success = true, 
            isInterested = isInterested,
            interestCount = interestCount,
            availableSeats = eventItem?.AvailableSeats ?? 0,
            canRegister = canRegister
        });
    }
}
