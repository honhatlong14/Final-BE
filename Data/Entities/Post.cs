using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Constants;
using Data.Entities.User;

namespace Data.Entities;

public class Post : BaseEntity.BaseEntity
{
    // FK 
    [Required]
    public string StallId { set; get; }
    [ForeignKey("StallId")]
    public Stall Stall { set; get; }
    // FK to Book: 1 - many
    public string BookId { set; get; }
    // Change to get post by user ID and get book also get post 
    public Book Book { set; get; }

    // Enum: sell Status ... 
    public StringEnum.SellStatus SellStatus { set; get; }
}