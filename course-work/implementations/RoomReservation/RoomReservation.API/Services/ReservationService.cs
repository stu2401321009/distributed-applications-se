using Microsoft.EntityFrameworkCore;
using RoomReservation.API.Data;
using RoomReservation.API.Data.Entities;
using RoomReservation.API.Data.Enums;
using RoomReservation.API.Models.Requests;
using RoomReservation.API.Models.Responses;

namespace RoomReservation.API.Services;

public interface IReservationService
{
    Task<PagedResponse<ReservationResponse>> GetAllAsync(int page, int pageSize, string? sortBy, string? sortOrder, DateTime? startDate, string? status, int? userId);
    Task<ReservationResponse?> GetByIdAsync(int id);
    Task<(ReservationResponse? result, string? error)> CreateAsync(CreateReservationRequest request, int userId);
    Task<(ReservationResponse? result, string? error)> UpdateAsync(int id, UpdateReservationRequest request, int userId, bool isAdmin);
    Task<(bool success, string? error)> DeleteAsync(int id, int userId, bool isAdmin);
    Task<(ReservationResponse? result, string? error)> UpdateStatusAsync(int id, UpdateReservationStatusRequest request);
}

public class ReservationService(ApplicationDbContext db) : IReservationService
{
    public async Task<PagedResponse<ReservationResponse>> GetAllAsync(int page, int pageSize, string? sortBy, string? sortOrder, DateTime? startDate, string? status, int? userId)
    {
        var query = db.Reservations.Include(r => r.Room).Include(r => r.User).AsQueryable();

        if (userId.HasValue)
            query = query.Where(r => r.UserId == userId.Value);
        if (startDate.HasValue)
            query = query.Where(r => r.StartTime >= startDate.Value);
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ReservationStatus>(status, out var st))
            query = query.Where(r => r.Status == st);

        query = (sortBy?.ToLower(), sortOrder?.ToLower() == "desc") switch
        {
            ("starttime", true) => query.OrderByDescending(r => r.StartTime),
            ("starttime", false) => query.OrderBy(r => r.StartTime),
            ("status", true) => query.OrderByDescending(r => r.Status),
            ("status", false) => query.OrderBy(r => r.Status),
            (_, true) => query.OrderByDescending(r => r.CreatedAt),
            _ => query.OrderBy(r => r.CreatedAt)
        };

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResponse<ReservationResponse>
        {
            Items = items.Select(MapToResponse),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ReservationResponse?> GetByIdAsync(int id)
    {
        var r = await db.Reservations.Include(x => x.Room).Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        return r is null ? null : MapToResponse(r);
    }

    public async Task<(ReservationResponse? result, string? error)> CreateAsync(CreateReservationRequest request, int userId)
    {
        if (request.StartTime <= DateTime.UtcNow)
            return (null, "Началното време трябва да е в бъдещето.");
        if (request.EndTime <= request.StartTime)
            return (null, "Крайното време трябва да е след началното.");

        var room = await db.Rooms.FindAsync(request.RoomId);
        if (room is null || !room.IsActive)
            return (null, "Залата не е намерена или е деактивирана.");
        if (request.AttendeeCount > room.Capacity)
            return (null, $"Броят участници ({request.AttendeeCount}) надвишава капацитета на залата ({room.Capacity}).");

        var conflict = await db.Reservations.AnyAsync(r =>
            r.RoomId == request.RoomId &&
            r.Status == ReservationStatus.Approved &&
            r.StartTime < request.EndTime &&
            r.EndTime > request.StartTime);

        if (conflict)
            return (null, "Залата е заета за посочения период.");

        var reservation = new Reservation
        {
            RoomId = request.RoomId,
            UserId = userId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Purpose = request.Purpose,
            AttendeeCount = request.AttendeeCount,
            Notes = request.Notes
        };

        db.Reservations.Add(reservation);
        await db.SaveChangesAsync();

        await db.Entry(reservation).Reference(r => r.Room).LoadAsync();
        await db.Entry(reservation).Reference(r => r.User).LoadAsync();

        return (MapToResponse(reservation), null);
    }

    public async Task<(ReservationResponse? result, string? error)> UpdateAsync(int id, UpdateReservationRequest request, int userId, bool isAdmin)
    {
        var reservation = await db.Reservations.Include(r => r.Room).Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);
        if (reservation is null) return (null, "not_found");
        if (!isAdmin && reservation.UserId != userId) return (null, "forbidden");
        if (reservation.Status != ReservationStatus.Pending) return (null, "Може да се редактира само резервация в статус Pending.");

        if (request.StartTime <= DateTime.UtcNow)
            return (null, "Началното време трябва да е в бъдещето.");
        if (request.EndTime <= request.StartTime)
            return (null, "Крайното време трябва да е след началното.");
        if (request.AttendeeCount > reservation.Room.Capacity)
            return (null, $"Броят участници надвишава капацитета на залата ({reservation.Room.Capacity}).");

        var conflict = await db.Reservations.AnyAsync(r =>
            r.Id != id &&
            r.RoomId == reservation.RoomId &&
            r.Status == ReservationStatus.Approved &&
            r.StartTime < request.EndTime &&
            r.EndTime > request.StartTime);

        if (conflict) return (null, "Залата е заета за новото време.");

        reservation.StartTime = request.StartTime;
        reservation.EndTime = request.EndTime;
        reservation.Purpose = request.Purpose;
        reservation.AttendeeCount = request.AttendeeCount;
        reservation.Notes = request.Notes;

        await db.SaveChangesAsync();
        return (MapToResponse(reservation), null);
    }

    public async Task<(bool success, string? error)> DeleteAsync(int id, int userId, bool isAdmin)
    {
        var reservation = await db.Reservations.FindAsync(id);
        if (reservation is null) return (false, "not_found");
        if (!isAdmin && reservation.UserId != userId) return (false, "forbidden");

        if (reservation.Status != ReservationStatus.Pending && reservation.Status != ReservationStatus.Approved)
            return (false, "Резервацията не може да се отмени (вече приключила или отхвърлена).");

        reservation.Status = ReservationStatus.Cancelled;
        await db.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(ReservationResponse? result, string? error)> UpdateStatusAsync(int id, UpdateReservationStatusRequest request)
    {
        var reservation = await db.Reservations.Include(r => r.Room).Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);
        if (reservation is null) return (null, "not_found");
        if (reservation.Status != ReservationStatus.Pending) return (null, "Може да се одобри/отхвърли само резервация в статус Pending.");

        reservation.Status = Enum.Parse<ReservationStatus>(request.Status);
        if (!string.IsNullOrWhiteSpace(request.Notes))
            reservation.Notes = request.Notes;

        await db.SaveChangesAsync();
        return (MapToResponse(reservation), null);
    }

    private static ReservationResponse MapToResponse(Reservation r) => new()
    {
        Id = r.Id,
        Room = RoomService.MapToResponse(r.Room),
        User = new UserResponse
        {
            Id = r.User.Id,
            FirstName = r.User.FirstName,
            LastName = r.User.LastName,
            Email = r.User.Email,
            Role = r.User.Role.ToString(),
            FacultyNumber = r.User.FacultyNumber,
            PhoneNumber = r.User.PhoneNumber,
            IsActive = r.User.IsActive,
            CreatedAt = r.User.CreatedAt
        },
        StartTime = r.StartTime,
        EndTime = r.EndTime,
        Purpose = r.Purpose,
        Status = r.Status.ToString(),
        Notes = r.Notes,
        AttendeeCount = r.AttendeeCount,
        CreatedAt = r.CreatedAt
    };
}
