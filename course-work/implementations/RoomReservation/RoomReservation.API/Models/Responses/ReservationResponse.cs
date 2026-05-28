namespace RoomReservation.API.Models.Responses;

public class ReservationResponse
{
    public int Id { get; set; }
    public RoomResponse Room { get; set; } = null!;
    public UserResponse User { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int AttendeeCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
