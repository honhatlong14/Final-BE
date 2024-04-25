using System.ComponentModel.DataAnnotations;

namespace Data.ViewModels.User;

public class AuthenticateRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}