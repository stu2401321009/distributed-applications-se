using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservation.API.Models.Requests;
using RoomReservation.API.Services;

namespace RoomReservation.API.Controllers;

[ApiController]
[Route("api/rooms")]
[Authorize]
public class RoomsController(IRoomService roomService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null,
        [FromQuery] string? building = null, [FromQuery] string? roomType = null)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var result = await roomService.GetAllAsync(page, pageSize, sortBy, sortOrder, building, roomType);
        return Ok(result);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable([FromQuery] AvailableRoomsRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (request.EndTime <= request.StartTime)
            return BadRequest(new { title = "Невалидно време", detail = "Крайният час трябва да е след началния." });

        var result = await roomService.GetAvailableAsync(request.Date, request.StartTime, request.EndTime, request.MinCapacity);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var room = await roomService.GetByIdAsync(id);
        return room is null ? NotFound(new { title = "Не е намерена", status = 404, detail = $"Зала с id={id} не съществува." }) : Ok(room);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateRoomRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var result = await roomService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var result = await roomService.UpdateAsync(id, request);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await roomService.DeleteAsync(id);

        if (!success && error is null) return NotFound();
        if (!success) return Conflict(new { title = "Конфликт", status = 409, detail = error });

        return NoContent();
    }
}
