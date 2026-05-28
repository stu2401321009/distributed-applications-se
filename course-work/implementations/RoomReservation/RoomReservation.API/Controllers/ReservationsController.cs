using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservation.API.Models.Requests;
using RoomReservation.API.Services;

namespace RoomReservation.API.Controllers;

[ApiController]
[Route("api/reservations")]
[Authorize]
public class ReservationsController(IReservationService reservationService) : ControllerBase
{
    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
    private bool IsAdmin => User.IsInRole("Admin");

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null,
        [FromQuery] DateTime? startDate = null, [FromQuery] string? status = null)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        int? filterUserId = IsAdmin ? null : CurrentUserId;
        var result = await reservationService.GetAllAsync(page, pageSize, sortBy, sortOrder, startDate, status, filterUserId);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMy(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null,
        [FromQuery] DateTime? startDate = null, [FromQuery] string? status = null)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var result = await reservationService.GetAllAsync(page, pageSize, sortBy, sortOrder, startDate, status, CurrentUserId);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var reservation = await reservationService.GetByIdAsync(id);
        if (reservation is null) return NotFound(new { title = "Не е намерена", status = 404 });
        if (!IsAdmin && reservation.User.Id != CurrentUserId) return Forbid();
        return Ok(reservation);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var (result, error) = await reservationService.CreateAsync(request, CurrentUserId);

        if (result is null && error == "Залата е заета за посочения период.")
            return Conflict(new { title = "Залата е заета", status = 409, detail = error });
        if (result is null)
            return BadRequest(new { title = "Невалидни данни", status = 400, detail = error });

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReservationRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var (result, error) = await reservationService.UpdateAsync(id, request, CurrentUserId, IsAdmin);

        return error switch
        {
            "not_found" => NotFound(),
            "forbidden" => Forbid(),
            null => Ok(result),
            _ when error.Contains("заета") => Conflict(new { title = "Конфликт", status = 409, detail = error }),
            _ => BadRequest(new { title = "Невалидни данни", detail = error })
        };
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await reservationService.DeleteAsync(id, CurrentUserId, IsAdmin);

        return error switch
        {
            "not_found" => NotFound(),
            "forbidden" => Forbid(),
            null when success => NoContent(),
            _ => BadRequest(new { title = "Неуспешна отмяна", status = 400, detail = error })
        };
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateReservationStatusRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var (result, error) = await reservationService.UpdateStatusAsync(id, request);

        return error switch
        {
            "not_found" => NotFound(),
            null => Ok(result),
            _ => BadRequest(new { title = "Невалидна операция", detail = error })
        };
    }
}
