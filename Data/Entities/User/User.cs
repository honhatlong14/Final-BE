using System.ComponentModel.DataAnnotations;
using Common.Constants;
using Data.Entities.BaseEntity;

namespace Data.Entities.User;

public class User : BaseUser
{
    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    public string FullName { get; set; }
    public string Avatar { set; get; }
    public int PrestigePoint { set; get; }
    public Stall Stall { set; get; }
    public string? PhoneNumber { get; set; }
    public bool IsInternal { get; set; }
    public StringEnum.AccountStatus AccountStatus { set; get; }
    public ICollection<Comment> Comments { set; get; }
}