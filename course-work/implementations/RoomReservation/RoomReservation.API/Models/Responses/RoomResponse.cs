namespace RoomReservation.API.Models.Responses;

public class RoomResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int Capacity { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public bool HasProjector { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
