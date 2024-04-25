using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.Constants;
using Microsoft.AspNetCore.Http;

namespace Data.ViewModels;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
        
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; }
    public StringEnum.Gender Gender { get; set; }
    
    [Required]
    public IFormFile AvatarFile { get; set; }
    
    public string PhoneNumber { get; set; }
    
}