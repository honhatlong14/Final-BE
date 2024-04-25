using Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class Cart : BaseEntity.BaseEntity
{
    // PK FK.... Chỉ làm FK
    [Required]
    public string UserId { set; get; }
    [ForeignKey("UserId")]
    public User.User User { set; get; }
    // PK FK
    [Required]
    public string BookId { set; get; }
    [ForeignKey("BookId")]
    public Book Book { set; get; }
    public int Quantity { set; get; }
    public int Total { set; get; }
    public string StallId { set; get; }

    public StringEnum.SelectedItemStatus IsSelected { set; get; }
}