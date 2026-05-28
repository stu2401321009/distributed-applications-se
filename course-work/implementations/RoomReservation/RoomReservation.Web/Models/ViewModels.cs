namespace RoomReservation.Web.Models;

public class UserViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? FacultyNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}

public class RoomViewModel
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

public class ReservationViewModel
{
    public int Id { get; set; }
    public RoomViewModel Room { get; set; } = null!;
    public UserViewModel User { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int AttendeeCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PagedViewModel<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class AuthViewModel
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserViewModel User { get; set; } = null!;
}

public class LoginFormModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

public class RegisterFormModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Student";
    public string? FacultyNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ErrorMessage { get; set; }
}

public class CreateReservationFormModel
{
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; } = DateTime.Now.AddHours(1);
    public DateTime EndTime { get; set; } = DateTime.Now.AddHours(2);
    public string Purpose { get; set; } = string.Empty;
    public int AttendeeCount { get; set; } = 1;
    public string? Notes { get; set; }
    public string? ErrorMessage { get; set; }
}

public class CreateRoomFormModel
{
    public string Name { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int Capacity { get; set; } = 1;
    public string RoomType { get; set; } = "Lecture";
    public bool HasProjector { get; set; }
    public string? Description { get; set; }
    public string? ErrorMessage { get; set; }
}
