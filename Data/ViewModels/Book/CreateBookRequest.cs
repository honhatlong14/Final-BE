using Microsoft.AspNetCore.Http;

namespace Data.ViewModels.Book;

public class CreateBookRequest
{
    public string Title { set; get; }
    public string Description { set; get; }
    public int NumPage { set; get; }
    public int AvailbleQuantity { set; get; }
    // Enum: status Book ...  
    // .... 
    // public IList<IFormFile> BookImages { get; set; }
    public IList<string> BookImages { get; set; }
    public string Author { set; get; }
    public int Price { set; get; }
}

