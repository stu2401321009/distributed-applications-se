using Microsoft.AspNetCore.Mvc;
using RoomReservation.API.Models.Requests;
using RoomReservation.API.Services;

namespace RoomReservation.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (await authService.EmailExistsAsync(request.Email))
            return Conflict(new { title = "Email вече съществува", status = 409, detail = $"Потребител с email '{request.Email}' вече съществува." });

        var result = await authService.RegisterAsync(request);
        return CreatedAtAction(nameof(Register), result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await authService.LoginAsync(request);

        if (result is null)
            return Unauthorized(new { title = "Грешни данни", status = 401, detail = "Грешен email или парола." });

        return Ok(result);
    }
}
