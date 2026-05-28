using System.ComponentModel.DataAnnotations;

namespace RoomReservation.API.Models.Requests;

public class CreateReservationRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int RoomId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    [MinLength(5), MaxLength(200)]
    public string Purpose { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Броят участници трябва да е поне 1.")]
    public int AttendeeCount { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class UpdateReservationRequest
{
    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    [MinLength(5), MaxLength(200)]
    public string Purpose { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int AttendeeCount { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class UpdateReservationStatusRequest
{
    [Required]
    [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Статусът трябва да е Approved или Rejected.")]
    public string Status { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Notes { get; set; }
}
