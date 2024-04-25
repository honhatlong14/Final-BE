namespace Data.ViewModels.Book;

public class CreateBookResponse
{
    public string Title { set; get; }
    public string Description { set; get; }
    public string NumPage { set; get; }
    public int AvailbleQuantity { set; get; }
    // Enum: status Book ...  
    // .... 
    public string Author { set; get; }
}