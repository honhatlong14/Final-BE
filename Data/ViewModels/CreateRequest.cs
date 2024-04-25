using System.ComponentModel.DataAnnotations;
using Common.Constants;
using Microsoft.AspNetCore.Http;

namespace Data.ViewModels;

public class CreateRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
        
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; }
    public StringEnum.Gender Gender { get; set; }

    [Range(typeof(bool), "true", "true")]
    public bool AcceptTerms { get; set; }
    public IFormFile AvatarFile { get; set; }
    [Required] 
    public StringEnum.Roles Role { get; set; }
}