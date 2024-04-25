using System.ComponentModel.DataAnnotations;

namespace Data.Dtos.GoogleAuthDtos;

public class GoogleLoginRequest
{
    [Required]
    public string TokenId { get; set; }
}