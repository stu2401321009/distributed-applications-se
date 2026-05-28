using System.ComponentModel.DataAnnotations;

namespace RoomReservation.API.Models.Requests;

public class CreateUserRequest
{
    [Required]
    [MinLength(2), MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MinLength(2), MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8), MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Student|Teacher|Admin)$", ErrorMessage = "Ролята трябва да е Student, Teacher или Admin.")]
    public string Role { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? FacultyNumber { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
}

public class UpdateUserRequest
{
    [Required]
    [MinLength(2), MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MinLength(2), MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? FacultyNumber { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Required]
    public bool IsActive { get; set; }
}
