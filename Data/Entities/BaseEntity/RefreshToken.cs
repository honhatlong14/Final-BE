using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Data.Entities.BaseEntity;

[Owned]
public class RefreshToken
{
    public RefreshToken()
    {
        Id = Guid.NewGuid().ToString();
    }
        
    [Key]
    [JsonIgnore]
    public string Id { get; set; }
    public string? Token { get; set; }
    public DateTime? Expires { get; set; }
    [NotMapped]
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime? Created { get; set; }
    public string CreatedByIp { get; set; }
    public DateTime? Revoked { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    [NotMapped]
    public bool IsActive => Revoked == null && !IsExpired;
}