using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Local_Event_Finder.Services;

namespace Local_Event_Finder.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly IEventInterestService _interestService;

    public UserController(IEventInterestService interestService)
    {
        _interestService = interestService;
    }

    public async Task<IActionResult> MyEvents()
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        var userInterests = await _interestService.GetUserInterestsAsync(userId);
        return View(userInterests);
    }
}
