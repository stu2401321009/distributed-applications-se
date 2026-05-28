using Microsoft.AspNetCore.Mvc;
using RoomReservation.Web.Models;
using RoomReservation.Web.Services;

namespace RoomReservation.Web.Controllers;

public class RoomsController(ApiService apiService) : BaseController(apiService)
{
    public async Task<IActionResult> Index(int page = 1, string? building = null, string? roomType = null, string? sortBy = null, string? sortOrder = null)
    {
        var auth = RequireAuth();
        if (auth is not null) return auth;

        var rooms = await Api.GetRoomsAsync(page, 9, building, roomType, sortBy, sortOrder);
        ViewBag.Building = building;
        ViewBag.RoomType = roomType;
        ViewBag.SortBy = sortBy;
        ViewBag.SortOrder = sortOrder;
        ViewBag.IsAdmin = IsAdmin();
        return View(rooms ?? new PagedViewModel<RoomViewModel>());
    }

    public async Task<IActionResult> Available(string? date, string? startTime, string? endTime, int? minCapacity)
    {
        var auth = RequireAuth();
        if (auth is not null) return auth;

        IEnumerable<RoomViewModel>? rooms = null;
        string? error = null;

        if (date is not null && startTime is not null && endTime is not null)
        {
            rooms = await Api.GetAvailableRoomsAsync(date, startTime, endTime, minCapacity);
            if (rooms is null) error = "Невалидни параметри или грешка при търсенето.";
        }

        ViewBag.Date = date ?? DateTime.Today.ToString("yyyy-MM-dd");
        ViewBag.StartTime = startTime ?? "09:00";
        ViewBag.EndTime = endTime ?? "11:00";
        ViewBag.MinCapacity = minCapacity;
        ViewBag.Error = error;
        return View(rooms);
    }

    public async Task<IActionResult> Details(int id)
    {
        var auth = RequireAuth();
        if (auth is not null) return auth;

        var room = await Api.GetRoomAsync(id);
        if (room is null) return NotFound();

        ViewBag.IsAdmin = IsAdmin();
        return View(room);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsAuthenticated()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin()) return Forbid();
        SetToken();
        return View(new CreateRoomFormModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoomFormModel model)
    {
        if (!IsAuthenticated()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin()) return Forbid();
        SetToken();

        var (room, error) = await Api.CreateRoomAsync(new
        {
            model.Name,
            model.Building,
            model.Floor,
            model.Capacity,
            model.RoomType,
            model.HasProjector,
            model.Description
        });

        if (room is null)
        {
            model.ErrorMessage = error ?? "Грешка при създаването.";
            return View(model);
        }

        return RedirectToAction("Details", new { id = room.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsAuthenticated()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin()) return Forbid();
        SetToken();

        var room = await Api.GetRoomAsync(id);
        if (room is null) return NotFound();

        return View(new CreateRoomFormModel
        {
            Name = room.Name,
            Building = room.Building,
            Floor = room.Floor,
            Capacity = room.Capacity,
            RoomType = room.RoomType,
            HasProjector = room.HasProjector,
            Description = room.Description
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, CreateRoomFormModel model)
    {
        if (!IsAuthenticated()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin()) return Forbid();
        SetToken();

        var (room, error) = await Api.UpdateRoomAsync(id, new
        {
            model.Name,
            model.Building,
            model.Floor,
            model.Capacity,
            model.RoomType,
            model.HasProjector,
            model.Description,
            IsActive = true
        });

        if (room is null)
        {
            model.ErrorMessage = error ?? "Грешка при редактирането.";
            return View(model);
        }

        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAuthenticated()) return RedirectToAction("Login", "Auth");
        if (!IsAdmin()) return Forbid();
        SetToken();

        await Api.DeleteRoomAsync(id);
        return RedirectToAction("Index");
    }
}
