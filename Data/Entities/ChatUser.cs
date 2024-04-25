using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ChatUser : BaseEntity.BaseEntity
{
    [Required]
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public User.User User { get; set; }
    [Required]
    public string ChatId { get; set; }
    [ForeignKey("ChatId")]
    public Chat Chat { get; set; }
}