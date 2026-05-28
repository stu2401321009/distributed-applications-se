using Microsoft.EntityFrameworkCore;
using RoomReservation.API.Data;
using RoomReservation.API.Data.Entities;
using RoomReservation.API.Data.Enums;
using RoomReservation.API.Models.Requests;
using RoomReservation.API.Models.Responses;

namespace RoomReservation.API.Services;

public interface IUserService
{
    Task<PagedResponse<UserResponse>> GetAllAsync(int page, int pageSize, string? sortBy, string? sortOrder, string? firstName, string? lastName, string? role);
    Task<UserResponse?> GetByIdAsync(int id);
    Task<UserResponse> CreateAsync(CreateUserRequest request);
    Task<UserResponse?> UpdateAsync(int id, UpdateUserRequest request);
    Task<bool> DeleteAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
}

public class UserService(ApplicationDbContext db) : IUserService
{
    public async Task<PagedResponse<UserResponse>> GetAllAsync(int page, int pageSize, string? sortBy, string? sortOrder, string? firstName, string? lastName, string? role)
    {
        var query = db.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(firstName))
            query = query.Where(u => u.FirstName.Contains(firstName));
        if (!string.IsNullOrWhiteSpace(lastName))
            query = query.Where(u => u.LastName.Contains(lastName));
        if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<UserRole>(role, out var roleEnum))
            query = query.Where(u => u.Role == roleEnum);

        query = (sortBy?.ToLower(), sortOrder?.ToLower() == "desc") switch
        {
            ("firstname", true) => query.OrderByDescending(u => u.FirstName),
            ("firstname", false) => query.OrderBy(u => u.FirstName),
            ("lastname", true) => query.OrderByDescending(u => u.LastName),
            ("lastname", false) => query.OrderBy(u => u.LastName),
            ("email", true) => query.OrderByDescending(u => u.Email),
            ("email", false) => query.OrderBy(u => u.Email),
            (_, true) => query.OrderByDescending(u => u.CreatedAt),
            _ => query.OrderBy(u => u.CreatedAt)
        };

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResponse<UserResponse>
        {
            Items = items.Select(MapToResponse),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<UserResponse?> GetByIdAsync(int id)
    {
        var user = await db.Users.FindAsync(id);
        return user is null ? null : MapToResponse(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Enum.Parse<UserRole>(request.Role),
            FacultyNumber = request.FacultyNumber,
            PhoneNumber = request.PhoneNumber
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return MapToResponse(user);
    }

    public async Task<UserResponse?> UpdateAsync(int id, UpdateUserRequest request)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return null;

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.FacultyNumber = request.FacultyNumber;
        user.PhoneNumber = request.PhoneNumber;
        user.IsActive = request.IsActive;

        await db.SaveChangesAsync();
        return MapToResponse(user);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return false;

        user.IsActive = false;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var query = db.Users.Where(u => u.Email == email.ToLower());
        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    private static UserResponse MapToResponse(User u) => new()
    {
        Id = u.Id,
        FirstName = u.FirstName,
        LastName = u.LastName,
        Email = u.Email,
        Role = u.Role.ToString(),
        FacultyNumber = u.FacultyNumber,
        PhoneNumber = u.PhoneNumber,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt
    };
}
