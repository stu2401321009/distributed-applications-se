using Microsoft.EntityFrameworkCore;
using RoomReservation.API.Data;
using RoomReservation.API.Data.Entities;
using RoomReservation.API.Data.Enums;
using RoomReservation.API.Models.Requests;
using RoomReservation.API.Models.Responses;

namespace RoomReservation.API.Services;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<bool> EmailExistsAsync(string email);
}

public class AuthService(ApplicationDbContext db, ITokenService tokenService, IConfiguration config) : IAuthService
{
    public async Task<bool> EmailExistsAsync(string email)
        => await db.Users.AnyAsync(u => u.Email == email.ToLower());

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var role = Enum.Parse<UserRole>(request.Role);

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = role,
            FacultyNumber = request.FacultyNumber,
            PhoneNumber = request.PhoneNumber
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower() && u.IsActive);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return BuildAuthResponse(user);
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var expirationHours = int.Parse(config["Jwt:ExpirationHours"] ?? "24");
        var token = tokenService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(expirationHours),
            User = new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString(),
                FacultyNumber = user.FacultyNumber,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            }
        };
    }
}
