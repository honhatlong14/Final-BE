using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data.Entities;

public class Comment : BaseEntity.BaseEntity
{
    [Required]
    public string UserId { set; get; }
    [ForeignKey("UserId")]
    public User.User? User { get; set; }

    [Required]
    public string PostId { set; get; }
    [ForeignKey("PostId")]
    public Post Post { set; get; }
    [Required]
    public string Text { set; get; }
    public int Rating { set; get; }
}