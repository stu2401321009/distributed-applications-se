using System.ComponentModel.DataAnnotations;

namespace RoomReservation.API.Models.Requests;

public class RegisterRequest
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
    [RegularExpression("^(Student|Teacher)$", ErrorMessage = "Ролята трябва да е Student или Teacher.")]
    public string Role { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? FacultyNumber { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
}
