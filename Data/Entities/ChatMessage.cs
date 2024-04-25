using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ChatMessage : BaseEntity.BaseEntity
{
    public ChatMessage()
    {
        CreateAt = DateTime.UtcNow;
    }
    
    [Required]
    public string Type { get; set; }
    [Required]
    public string Message { get; set; }
    [Required]
    public string ChatId { get; set; }
    [ForeignKey("ChatId")]
    public Chat Chat { get; set; }
    [Required]
    public string FromUserId { get; set; }
}