using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Common.Constants;

namespace Data.Entities.BaseEntity;

public class BaseUser : BaseEntity
{
    [EmailAddress]
    public string Email { get; set; }
    public StringEnum.Roles Role { get; set; }
    public bool AcceptTerms { get; set; }
    [JsonIgnore]
    public string? PasswordHash { get; set; }
    [JsonIgnore] 
    public string? Salt { get; set; }
    [JsonIgnore] 
    public List<RefreshToken>? RefreshTokens { get; set; }
        
    public string? VerificationToken { get; set; }
    public DateTime? Verified { get; set; }
    [NotMapped]
    public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;
    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }
    public DateTime? PasswordReset { get; set; }
        
    public bool OwnsToken(string token) 
    {
        return this.RefreshTokens?.Find(x => x.Token == token) != null;
    }
        
}