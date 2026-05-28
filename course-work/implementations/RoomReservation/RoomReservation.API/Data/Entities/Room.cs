using System.ComponentModel.DataAnnotations;
using RoomReservation.API.Data.Enums;

namespace RoomReservation.API.Data.Entities;

public class Room
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Building { get; set; } = string.Empty;

    public int Floor { get; set; }

    [Required]
    public int Capacity { get; set; }

    [Required]
    public RoomType RoomType { get; set; }

    public bool HasProjector { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Reservation> Reservations { get; set; } = [];
}
