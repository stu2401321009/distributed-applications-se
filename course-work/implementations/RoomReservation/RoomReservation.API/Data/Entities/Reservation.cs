using System.ComponentModel.DataAnnotations;
using RoomReservation.API.Data.Enums;

namespace RoomReservation.API.Data.Entities;

public class Reservation
{
    public int Id { get; set; }

    [Required]
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    [MaxLength(200)]
    public string Purpose { get; set; } = string.Empty;

    [Required]
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    [MaxLength(500)]
    public string? Notes { get; set; }

    [Required]
    public int AttendeeCount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
