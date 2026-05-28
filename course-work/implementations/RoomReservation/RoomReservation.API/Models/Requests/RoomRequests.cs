using System.ComponentModel.DataAnnotations;

namespace RoomReservation.API.Models.Requests;

public class CreateRoomRequest
{
    [Required]
    [MinLength(1), MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MinLength(1), MaxLength(100)]
    public string Building { get; set; } = string.Empty;

    [Required]
    public int Floor { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Капацитетът трябва да е поне 1.")]
    public int Capacity { get; set; }

    [Required]
    [RegularExpression("^(Lecture|Lab|Seminar|ComputerLab)$", ErrorMessage = "Невалиден тип зала.")]
    public string RoomType { get; set; } = string.Empty;

    [Required]
    public bool HasProjector { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateRoomRequest : CreateRoomRequest
{
    [Required]
    public bool IsActive { get; set; }
}

public class AvailableRoomsRequest
{
    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    [Range(1, int.MaxValue)]
    public int? MinCapacity { get; set; }
}
