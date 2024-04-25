using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.User;

public class WishList : BaseEntity.BaseEntity
{
    [Required]
    public string UserId { set; get; }
    [ForeignKey("UserId")]
    public User User { set; get; }
    [Required]
    public string BookId { set; get; }
    [ForeignKey("BookId")]
    public Book Book { set; get; }
    
}