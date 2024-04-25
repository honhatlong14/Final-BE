using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class OrderDetail : BaseEntity.BaseEntity
{
    // PK FK 
    [Required]
    public string OrderId { set; get; }
    [ForeignKey("OrderId")]
    public Order Order { set; get; }
    // PK FK 
    [Required]
    public string BookId { set; get; }
    [ForeignKey("BookId")]
    public Book Book { set; get; }
    
    public int Quantity { set; get; }
    public int Total { set; get; }
    public string StallId { set; get; }
    
}