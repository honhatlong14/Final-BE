using System.ComponentModel.DataAnnotations;

namespace Data.ViewModels;

public class VerifyEmailRequest
{
    [Required]
    public string Token { get; set; }
}