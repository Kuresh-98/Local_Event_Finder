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
    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    private void Flash(string message, string type = "success")
    {
        TempData["Flash.Message"] = message;
        TempData["Flash.Type"] = type; // success | danger | info | warning
    }

    // GET /Events
    // GET /Events?city=Seattle
    [HttpGet("")]
    public IActionResult Index(string? text, string? city, string? category, DateTime? from, DateTime? to, string sort = "startAsc", int page = 1, int pageSize = 9)
    {
        var filtered = _eventService.Search(text, city, category, from, to);
        filtered = sort switch
        {
            "startDesc" => filtered.OrderByDescending(e => e.StartUtc),
            "titleAsc" => filtered.OrderBy(e => e.Title),
            "titleDesc" => filtered.OrderByDescending(e => e.Title),
            _ => filtered.OrderBy(e => e.StartUtc)
        };

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
            Sort = sort
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
    public IActionResult Edit(int id, Event model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            Flash("Please correct the highlighted errors.", "danger");
            return View(model);
        }
        var updated = _eventService.Update(model);
        if (updated == null) return NotFound();
        Flash("Event updated.");
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
}
