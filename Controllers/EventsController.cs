using Microsoft.AspNetCore.Mvc;
using Local_Event_Finder.Services;

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
