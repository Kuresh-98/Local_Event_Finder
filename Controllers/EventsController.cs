using Microsoft.AspNetCore.Mvc;
using Local_Event_Finder.Services;

namespace Local_Event_Finder.Controllers;

public class EventsController : Controller
{
    private readonly IEventService _eventService;
    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    // /Events or /Events?text=meetup&city=Seattle
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

    public IActionResult Details(int id)
    {
        var ev = _eventService.GetById(id);
        if (ev == null) return NotFound();
        return View(ev);
    }
}
