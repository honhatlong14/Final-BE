using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class BookCategory : BaseEntity.BaseEntity
{
    
    [Required]
    public string? BookId { set; get; }
    [ForeignKey("BookId")]
    public Book? Book { set; get; }
    // PK FK     
    [Required]
    public string? CategoryId { set; get; }
    [ForeignKey("CategoryId")]
    public Category? Category { set; get; }
}