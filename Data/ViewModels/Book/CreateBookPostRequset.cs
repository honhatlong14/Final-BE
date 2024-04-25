using Common.Constants;
using Data.Entities;
using Data.Entities.User;

namespace Data.ViewModels.Book;

public class CreateBookPostRequset
{
    public string StallId { set; get; }

    public string? BookId { set; get; }
    
    public StringEnum.SellStatus SellStatus { set; get; }
    
    public string Title { set; get; }
    public string Description { set; get; }
    public int NumPage { set; get; }
    public int AvailbleQuantity { set; get; }
    // Enum: status Book ...  
    // .... 
    public DateTime PublishDate { set; get; }
    public string Author { set; get; }
    public int Price { set; get; }
    public IList<string>? BookImages { set; get;  }
    public IList<Category>? Categories { set; get;  }
}