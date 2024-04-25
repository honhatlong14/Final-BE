using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.User;

public class Image : BaseEntity.BaseEntity
{
    
    // FK 
    [Required]
    public string BookId { set; get; }
    [ForeignKey("BookId")]
    public Book Book { set; get; }
    
    public string ImageUrl { set; get; }
}