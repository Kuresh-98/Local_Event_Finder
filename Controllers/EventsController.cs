using Microsoft.AspNetCore.Mvc;
using Local_Event_Finder.Services;
using Local_Event_Finder.Models;

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

    // GET /Events
    // GET /Events?city=Seattle
    [HttpGet("")]
    public IActionResult Index(string? text, string? city, string? category, DateTime? from, DateTime? to)
    {
        var results = _eventService.Search(text, city, category, from, to);
        ViewData["SearchText"] = text;
        ViewData["City"] = city;
        ViewData["Category"] = category;
        ViewData["From"] = from?.ToString("yyyy-MM-dd");
        ViewData["To"] = to?.ToString("yyyy-MM-dd");
        return View(results);
    }

    // GET /Events/create
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
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Event model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        _eventService.Add(model);
        // Redirect to details page
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    // GET /Events/edit/5
    [HttpGet("edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var ev = _eventService.GetById(id);
        if (ev == null) return NotFound();
        return View(ev);
    }

    // POST /Events/edit/5
    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Event model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var updated = _eventService.Update(model);
        if (updated == null) return NotFound();
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    // GET /Events/delete/5 (confirmation)
    [HttpGet("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        var ev = _eventService.GetById(id);
        if (ev == null) return NotFound();
        return View(ev);
    }

    // POST /Events/delete/5
    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public IActionResult ConfirmDelete(int id)
    {
        var ok = _eventService.Delete(id);
        if (!ok) return NotFound();
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
