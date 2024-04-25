using Common.Constants;
using Data.Entities;

namespace Data.ViewModels.Book;

public class UpdateBookPostRequest
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
        public IList<ImageDataUpload>? BookImages { set; get;  }
        public IList<Category>? Categories { set; get;  }
}


public class ImageDataUpload
{
        public string Id { set; get;  }
        public string DataImage { set; get;  }
}