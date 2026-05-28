using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservation.API.Models.Requests;
using RoomReservation.API.Services;

namespace RoomReservation.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IUserService userService) : ControllerBase
{
    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);
    private bool IsAdmin => User.IsInRole("Admin");

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null,
        [FromQuery] string? firstName = null, [FromQuery] string? lastName = null,
        [FromQuery] string? role = null)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var result = await userService.GetAllAsync(page, pageSize, sortBy, sortOrder, firstName, lastName, role);
        return Ok(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var user = await userService.GetByIdAsync(CurrentUserId);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (!IsAdmin && CurrentUserId != id)
            return Forbid();

        var user = await userService.GetByIdAsync(id);
        return user is null ? NotFound(new { title = "Не е намерен", status = 404, detail = $"Потребител с id={id} не съществува." }) : Ok(user);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        if (await userService.EmailExistsAsync(request.Email))
            return Conflict(new { title = "Email вече съществува", status = 409 });

        var result = await userService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var result = await userService.UpdateAsync(id, request);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await userService.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
