using Microsoft.AspNetCore.Mvc;
using RoomReservation.Web.Services;

namespace RoomReservation.Web.Controllers;

public class AdminController(ApiService apiService) : BaseController(apiService)
{
    public async Task<IActionResult> Users(int page = 1, string? role = null, string? firstName = null, string? lastName = null)
    {
        if (!IsAuthenticated()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin()) return Forbid();
        SetToken();

        var users = await Api.GetUsersAsync(page, 15, role, firstName, lastName);
        ViewBag.Role = role;
        ViewBag.FirstName = firstName;
        ViewBag.LastName = lastName;
        return View(users);
    }
}
