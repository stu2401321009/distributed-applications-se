using Microsoft.EntityFrameworkCore;
using RoomReservation.API.Data;
using RoomReservation.API.Data.Entities;
using RoomReservation.API.Data.Enums;
using RoomReservation.API.Models.Requests;
using RoomReservation.API.Models.Responses;

namespace RoomReservation.API.Services;

public interface IRoomService
{
    Task<PagedResponse<RoomResponse>> GetAllAsync(int page, int pageSize, string? sortBy, string? sortOrder, string? building, string? roomType);
    Task<IEnumerable<RoomResponse>> GetAvailableAsync(DateOnly date, TimeOnly startTime, TimeOnly endTime, int? minCapacity);
    Task<RoomResponse?> GetByIdAsync(int id);
    Task<RoomResponse> CreateAsync(CreateRoomRequest request);
    Task<RoomResponse?> UpdateAsync(int id, UpdateRoomRequest request);
    Task<(bool success, string? error)> DeleteAsync(int id);
}

public class RoomService(ApplicationDbContext db) : IRoomService
{
    public async Task<PagedResponse<RoomResponse>> GetAllAsync(int page, int pageSize, string? sortBy, string? sortOrder, string? building, string? roomType)
    {
        var query = db.Rooms.Where(r => r.IsActive);

        if (!string.IsNullOrWhiteSpace(building))
            query = query.Where(r => r.Building.Contains(building));
        if (!string.IsNullOrWhiteSpace(roomType) && Enum.TryParse<RoomType>(roomType, out var rt))
            query = query.Where(r => r.RoomType == rt);

        query = (sortBy?.ToLower(), sortOrder?.ToLower() == "desc") switch
        {
            ("name", true) => query.OrderByDescending(r => r.Name),
            ("name", false) => query.OrderBy(r => r.Name),
            ("capacity", true) => query.OrderByDescending(r => r.Capacity),
            ("capacity", false) => query.OrderBy(r => r.Capacity),
            ("building", true) => query.OrderByDescending(r => r.Building),
            ("building", false) => query.OrderBy(r => r.Building),
            (_, true) => query.OrderByDescending(r => r.CreatedAt),
            _ => query.OrderBy(r => r.CreatedAt)
        };

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResponse<RoomResponse>
        {
            Items = items.Select(MapToResponse),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<RoomResponse>> GetAvailableAsync(DateOnly date, TimeOnly startTime, TimeOnly endTime, int? minCapacity)
    {
        var start = date.ToDateTime(startTime, DateTimeKind.Utc);
        var end = date.ToDateTime(endTime, DateTimeKind.Utc);

        var bookedRoomIds = await db.Reservations
            .Where(r => r.Status == ReservationStatus.Approved
                     && r.StartTime < end
                     && r.EndTime > start)
            .Select(r => r.RoomId)
            .Distinct()
            .ToListAsync();

        var query = db.Rooms.Where(r => r.IsActive && !bookedRoomIds.Contains(r.Id));

        if (minCapacity.HasValue)
            query = query.Where(r => r.Capacity >= minCapacity.Value);

        var rooms = await query.OrderBy(r => r.Name).ToListAsync();
        return rooms.Select(MapToResponse);
    }

    public async Task<RoomResponse?> GetByIdAsync(int id)
    {
        var room = await db.Rooms.FindAsync(id);
        return room is null ? null : MapToResponse(room);
    }

    public async Task<RoomResponse> CreateAsync(CreateRoomRequest request)
    {
        var room = new Room
        {
            Name = request.Name,
            Building = request.Building,
            Floor = request.Floor,
            Capacity = request.Capacity,
            RoomType = Enum.Parse<RoomType>(request.RoomType),
            HasProjector = request.HasProjector,
            Description = request.Description
        };

        db.Rooms.Add(room);
        await db.SaveChangesAsync();
        return MapToResponse(room);
    }

    public async Task<RoomResponse?> UpdateAsync(int id, UpdateRoomRequest request)
    {
        var room = await db.Rooms.FindAsync(id);
        if (room is null) return null;

        room.Name = request.Name;
        room.Building = request.Building;
        room.Floor = request.Floor;
        room.Capacity = request.Capacity;
        room.RoomType = Enum.Parse<RoomType>(request.RoomType);
        room.HasProjector = request.HasProjector;
        room.Description = request.Description;
        room.IsActive = request.IsActive;

        await db.SaveChangesAsync();
        return MapToResponse(room);
    }

    public async Task<(bool success, string? error)> DeleteAsync(int id)
    {
        var room = await db.Rooms.FindAsync(id);
        if (room is null) return (false, null);

        var hasActive = await db.Reservations.AnyAsync(r =>
            r.RoomId == id &&
            (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Approved));

        if (hasActive)
            return (false, "Залата има активни резервации и не може да бъде деактивирана.");

        room.IsActive = false;
        await db.SaveChangesAsync();
        return (true, null);
    }

    public static RoomResponse MapToResponse(Room r) => new()
    {
        Id = r.Id,
        Name = r.Name,
        Building = r.Building,
        Floor = r.Floor,
        Capacity = r.Capacity,
        RoomType = r.RoomType.ToString(),
        HasProjector = r.HasProjector,
        Description = r.Description,
        IsActive = r.IsActive,
        CreatedAt = r.CreatedAt
    };
}
