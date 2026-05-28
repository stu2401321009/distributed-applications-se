using Microsoft.AspNetCore.Mvc;
using RoomReservation.Web.Models;
using RoomReservation.Web.Services;

namespace RoomReservation.Web.Controllers;

public class AuthController(ApiService apiService) : BaseController(apiService)
{
    [HttpGet]
    public IActionResult Login()
    {
        if (IsAuthenticated()) return RedirectToAction("Index", "Rooms");
        return View(new LoginFormModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginFormModel model)
    {
        var (auth, error) = await Api.LoginAsync(model.Email, model.Password);
        if (auth is null)
        {
            model.ErrorMessage = error ?? "Грешен email или парола.";
            return View(model);
        }

        SaveSession(auth);
        return RedirectToAction("Index", "Rooms");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (IsAuthenticated()) return RedirectToAction("Index", "Rooms");
        return View(new RegisterFormModel());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterFormModel model)
    {
        var (auth, error) = await Api.RegisterAsync(new
        {
            model.FirstName,
            model.LastName,
            model.Email,
            model.Password,
            model.Role,
            model.FacultyNumber,
            model.PhoneNumber
        });

        if (auth is null)
        {
            model.ErrorMessage = error ?? "Регистрацията не успя.";
            return View(model);
        }

        SaveSession(auth);
        return RedirectToAction("Index", "Rooms");
    }

    public IActionResult Logout()
    {
        ClearSession();
        return RedirectToAction("Login");
    }
}
