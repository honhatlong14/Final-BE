using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Constants;

namespace Data.Entities.User;

public class Stall : BaseEntity.BaseEntity
{
    // FK 
    public string UserId { set; get; }
    public string StallName { set; get; }
    public StringEnum.StallStatus? StallStatus { set; get; }
    public User User { set; get; }
}