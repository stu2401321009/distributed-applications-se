using Microsoft.AspNetCore.Mvc;
using RoomReservation.Web.Models;
using RoomReservation.Web.Services;

namespace RoomReservation.Web.Controllers;

public class ReservationsController(ApiService apiService) : BaseController(apiService)
{
    public async Task<IActionResult> Index(int page = 1, string? status = null, string? sortBy = "startTime", string? sortOrder = "asc")
    {
        var auth = RequireAuth();
        if (auth is not null) return auth;

        PagedViewModel<ReservationViewModel>? reservations;

        if (IsAdmin())
            reservations = await Api.GetReservationsAsync(page, 10, status, sortBy, sortOrder);
        else
            reservations = await Api.GetMyReservationsAsync(page, 10, status);

        ViewBag.Status = status;
        ViewBag.SortBy = sortBy;
        ViewBag.SortOrder = sortOrder;
        ViewBag.IsAdmin = IsAdmin();
        return View(reservations ?? new PagedViewModel<ReservationViewModel>());
    }

    public async Task<IActionResult> My(int page = 1, string? status = null)
    {
        var auth = RequireAuth();
        if (auth is not null) return auth;

        var reservations = await Api.GetMyReservationsAsync(page, 10, status);
        ViewBag.Status = status;
        return View(reservations ?? new PagedViewModel<ReservationViewModel>());
    }

    public async Task<IActionResult> Details(int id)
    {
        var auth = RequireAuth();
        if (auth is not null) return auth;

        var reservation = await Api.GetReservationAsync(id);
        if (reservation is null) return NotFound();

        ViewBag.IsAdmin = IsAdmin();
        ViewBag.CurrentUserId = GetUserId();
        return View(reservation);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int roomId)
    {
        var auth = RequireAuth();
        if (auth is not null) return auth;

        var room = await Api.GetRoomAsync(roomId);
        if (room is null) return NotFound();

        return View(new CreateReservationFormModel
        {
            RoomId = roomId,
            RoomName = $"{room.Name} — {room.Building}",
            StartTime = DateTime.Today.AddDays(1).AddHours(9),
            EndTime = DateTime.Today.AddDays(1).AddHours(11)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReservationFormModel model)
    {
        var auth = RequireAuth();
        if (auth is not null) return auth;

        var (reservation, error) = await Api.CreateReservationAsync(new
        {
            model.RoomId,
            StartTime = model.StartTime.ToUniversalTime(),
            EndTime = model.EndTime.ToUniversalTime(),
            model.Purpose,
            model.AttendeeCount,
            model.Notes
        });

        if (reservation is null)
        {
            model.ErrorMessage = error ?? "Грешка при създаването на резервацията.";
            return View(model);
        }

        return RedirectToAction("My");
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var auth = RequireAuth();
        if (auth is not null) return auth;

        await Api.CancelReservationAsync(id);
        return RedirectToAction(IsAdmin() ? "Index" : "My");
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        if (!IsAuthenticated()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin()) return Forbid();
        SetToken();

        await Api.UpdateReservationStatusAsync(id, "Approved");
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id, string? notes)
    {
        if (!IsAuthenticated()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin()) return Forbid();
        SetToken();

        await Api.UpdateReservationStatusAsync(id, "Rejected", notes);
        return RedirectToAction("Index");
    }
}
