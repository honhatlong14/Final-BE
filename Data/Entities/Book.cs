using Data.Entities.User;

namespace Data.Entities;

public class Book : BaseEntity.BaseEntity
{
    
    public string Title { set; get; }
    public string Description { set; get; }
    public int NumPage { set; get; }
    public int AvailbleQuantity { set; get; }
    // Enum: status Book ...  
    // .... 
    public DateTime PublishDate { set; get; }
    public string Author { set; get; }
    public int Price { set; get; }
    public int QuantitySold { set; get; }
    public IList<Image> Images { set; get;}
    public IList<BookCategory> BookCategories  { set; get;}
    public Post Post { set; get; }
    public decimal Rating { set; get;}
}