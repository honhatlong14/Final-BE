using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Constants;

namespace Data.Entities;

public class  Order : BaseEntity.BaseEntity
{
    // FK 
    [Required]
    public string UserId { set; get; }
    [ForeignKey("UserId")]
    public User.User User { set; get; }
    public string? StallId { set; get; }
    public string? PaymentId { set; get; }
    public StringEnum.PaymentMethod PaymentMethod { set; get; }
    public string? PaymentStatus { set; get; }
    public StringEnum.ShippingStatus? ShippingStatus { set; get; }
    public IList<OrderDetail> OrderDetails { set; get;  }
    public string? StreetNumber { set; get; }
    public string? StreetName { set; get; }
    public string? City { set; get; }
    public string? Country { set; get; }


}