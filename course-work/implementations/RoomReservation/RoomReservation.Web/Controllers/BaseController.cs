using Microsoft.AspNetCore.Mvc;
using RoomReservation.Web.Services;

namespace RoomReservation.Web.Controllers;

public class BaseController(ApiService apiService) : Controller
{
    protected readonly ApiService Api = apiService;

    protected string? GetToken() => HttpContext.Session.GetString("jwt_token");
    protected string? GetUserRole() => HttpContext.Session.GetString("user_role");
    protected string? GetUserName() => HttpContext.Session.GetString("user_name");
    protected int GetUserId() => int.TryParse(HttpContext.Session.GetString("user_id"), out var id) ? id : 0;
    protected bool IsAdmin() => GetUserRole() == "Admin";
    protected bool IsAuthenticated() => GetToken() is not null;

    protected void SaveSession(Models.AuthViewModel auth)
    {
        HttpContext.Session.SetString("jwt_token", auth.Token);
        HttpContext.Session.SetString("user_role", auth.User.Role);
        HttpContext.Session.SetString("user_name", auth.User.FullName);
        HttpContext.Session.SetString("user_id", auth.User.Id.ToString());
    }

    protected void ClearSession() => HttpContext.Session.Clear();

    protected void SetToken()
    {
        var token = GetToken();
        if (token is not null) Api.SetToken(token);
    }

    protected IActionResult RequireAuth()
    {
        if (!IsAuthenticated()) return RedirectToAction("Login", "Auth");
        SetToken();
        return null!;
    }
}
