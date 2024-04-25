using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Constants;

namespace Data.Entities;

public class Address : BaseEntity.BaseEntity
{
    public string StreetNumber { set; get; }
    public string StreetName { set; get; }
    public string City { set; get; }
    public string Country { set; get; }
    [Required]
    public string UserId { set; get; }
    [ForeignKey("UserId")] 
    public User.User User { get; set; }
    // Enum: status Address ... 
    public StringEnum.AddressStatus AddressStatus { set; get; }
}