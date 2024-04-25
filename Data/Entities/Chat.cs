using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class Chat : BaseEntity.BaseEntity
{
    [Required]
    public string Type { get; set; }
}